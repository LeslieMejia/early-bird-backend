using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EarlyBirdAPI.Model.Entities;
using EarlyBird.Model.Repositories;
using EarlyBirdAPI.Model.Repositories;

namespace EarlyBird.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {
        protected JobApplicationRepository Repository { get; }

        public JobApplicationController(JobApplicationRepository repository)
        {
            Repository = repository;
        }

        // GET: api/JobApplication
        [HttpGet]
        public ActionResult<IEnumerable<JobApplication>> GetJobApplications()
        {
            return Ok(Repository.GetJobApplications());
        }

        // ✅ NEW: GET applications by JobseekerId
        // GET: api/JobApplication/jobseeker/{jobseekerId}
      // GET: api/JobApplication/jobseeker/{jobseekerId}
[HttpGet("jobseeker/{jobseekerId}")]
public ActionResult<List<JobApplication>> GetApplicationsByJobseekerId(int jobseekerId)
{
    var apps = Repository.GetJobApplicationsByJobseekerId(jobseekerId);
    if (apps == null || apps.Count == 0)
    {
        return NotFound();
    }
    return Ok(apps);
}

        // GET: api/JobApplication/{id}
        [HttpGet("{id}")]
        public ActionResult<JobApplication> GetJobApplication(int id)
        {
            JobApplication? application = Repository.GetJobApplicationById(id);
            if (application == null)
                return NotFound();

            return Ok(application);
        }

        // POST: api/JobApplication
        [HttpPost]
        public ActionResult Post([FromBody] JobApplication application)
        {
            if (application == null)
                return BadRequest("Job application data is not correct");

            bool status = Repository.InsertJobApplication(application);
            return status ? Ok() : BadRequest("Failed to insert job application");
        }

        // PUT: api/JobApplication
        [HttpPut]
        public ActionResult UpdateJobApplication([FromBody] JobApplication application)
        {
            if (application == null)
                return BadRequest("Job application data is not correct");

            JobApplication? existing = Repository.GetJobApplicationById(application.Id);
            if (existing == null)
                return NotFound($"Job application with id {application.Id} not found");

            bool status = Repository.UpdateJobApplication(application);
            return status ? Ok() : BadRequest("Something went wrong while updating");
        }

        // DELETE: api/JobApplication/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteJobApplication(int id)
        {
            JobApplication? existing = Repository.GetJobApplicationById(id);
            if (existing == null)
                return NotFound($"Job application with id {id} not found");

            bool status = Repository.DeleteJobApplication(id);
            return status ? NoContent() : BadRequest($"Unable to delete job application with id {id}");
        }
    }
}

