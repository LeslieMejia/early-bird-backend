using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using EarlyBirdAPI.Model.Entities;
using EarlyBird.Model.Repositories;

namespace EarlyBird.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/user
        [HttpGet]
        public ActionResult<List<User>> GetAllUsers()
        {
            return Ok(_userRepository.GetUsers());
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(int id)
        {
            var user = _userRepository.GetUserById(id);
            return user == null ? NotFound() : Ok(user);
        }

        // POST: api/user (Signup)
        [HttpPost]
        public ActionResult CreateUser([FromBody] User user)
        {
            Console.WriteLine($"👤 Received user: {user.Name}, {user.Email}, {user.Role}");

            if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.PasswordHash))
                return BadRequest("Name and password are required.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            var success = _userRepository.InsertUser(user);
            return success
                ? CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user)
                : BadRequest("Failed to create user.");


        }

        // POST: api/user/login
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var user = _userRepository.GetUserByEmail(loginRequest.Email);

            if (user == null || string.IsNullOrWhiteSpace(user.PasswordHash))
                return Unauthorized("Invalid email or password");

            bool isValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash);
            if (!isValid)
                return Unauthorized("Invalid email or password");

            return Ok(new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                role = user.Role
            });
        }


        // PUT: api/user/5
        [HttpPut("{id}")]
        public ActionResult UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id) return BadRequest("ID mismatch.");

            var existing = _userRepository.GetUserById(id);
            if (existing == null) return NotFound();

            var success = _userRepository.UpdateUser(user);
            return success ? NoContent() : StatusCode(500, "Update failed.");
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            var existing = _userRepository.GetUserById(id);
            if (existing == null) return NotFound();

            var success = _userRepository.DeleteUser(id);
            return success ? NoContent() : StatusCode(500, "Delete failed.");
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
