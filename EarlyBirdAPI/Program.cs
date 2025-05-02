//using EarlyBirdAPI;
//using EarlyBirdAPI.Model.Repositories;
using EarlyBird.Model.Repositories;
using EarlyBirdAPI.Model;
using EarlyBirdAPI.Model.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure the DbContext with PostgreSQL
builder.Services.AddDbContext<EarlyBirdDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("EBDatabase")));

//This will make the UserRepository available for dependency injection in your controllers.
builder.Services.AddScoped<UserRepository, UserRepository>();
builder.Services.AddScoped<ResumeRepository, ResumeRepository>();
builder.Services.AddScoped<JobRepository, JobRepository>();
builder.Services.AddScoped<JobApplicationRepository, JobApplicationRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection(); //PROMPTED TO REMOVE THIS LINE

//CORS must be enabled in backend for frontend to reach it
app.UseCors(policy =>
    policy.AllowAnyHeader()
          .AllowAnyMethod()
          .AllowAnyOrigin());


app.UseAuthorization();

app.MapControllers();

app.Run();
