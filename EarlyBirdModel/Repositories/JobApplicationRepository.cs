using EarlyBirdAPI.Model.Entities;
using EarlyBirdAPI.Model.Repositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;

namespace EarlyBirdAPI.Model.Repositories
{
    public class JobApplicationRepository : BaseRepository
    {
        public JobApplicationRepository(IConfiguration configuration) : base(configuration) { }

        // R - Get a job application by ID
        public JobApplication? GetJobApplicationById(int id)
        {
            NpgsqlConnection? dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM public.jobapplication WHERE id = @id";
                cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

                var data = GetData(dbConn, cmd);
                if (data != null && data.Read())
                {
                    return new JobApplication
                    {
                        Id = Convert.ToInt32(data["id"]),
                        JobId = Convert.ToInt32(data["jobid"]),
                        JobSeekerId = Convert.ToInt32(data["jobseekerid"]),
                        ResumeId = data["resumeid"] is DBNull ? null : (int?)Convert.ToInt32(data["resumeid"]),
                        CoverLetter = data["coverletter"] as string,
                        Status = Enum.Parse<ApplicationStatus>(data["status"].ToString()!)
                    };
                }
                return null;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        // R - Get all job applications
        public List<JobApplication> GetJobApplications()
        {
            NpgsqlConnection? dbConn = null;
            var applications = new List<JobApplication>();
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();

                // Extended join includes resume content, job title, and jobseeker name
                cmd.CommandText = @"
                SELECT 
                     ja.id,
                     ja.jobid,
                     ja.jobseekerid,
                     ja.resumeid,
                     ja.coverletter,
                     ja.status,
                     r.content AS resume_content,
                     j.title AS job_title,
                     u.name AS jobseeker_name
                    FROM jobapplication ja
                    LEFT JOIN resume r ON ja.resumeid = r.id
                    LEFT JOIN job j ON ja.jobid = j.id
                    LEFT JOIN users u ON ja.jobseekerid = u.id;";

                var data = GetData(dbConn, cmd);
                if (data != null)
                {
                    while (data.Read())
                    {
                        var app = new JobApplication
                        {
                            Id = Convert.ToInt32(data["id"]),
                            JobId = Convert.ToInt32(data["jobid"]),
                            JobSeekerId = Convert.ToInt32(data["jobseekerid"]),
                            ResumeId = data["resumeid"] is DBNull ? null : (int?)Convert.ToInt32(data["resumeid"]),
                            CoverLetter = data["coverletter"] as string,
                            Status = Enum.Parse<ApplicationStatus>(data["status"].ToString()!),
                            ResumeContent = data["resume_content"]?.ToString() ?? string.Empty,
                            JobTitle = data["job_title"]?.ToString() ?? string.Empty,
                            JobSeekerName = data["jobseeker_name"]?.ToString() ?? string.Empty
                        };
                        applications.Add(app);
                    }
                }
                return applications;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        // R - Get job applications by JobseekerId
        public List<JobApplication> GetJobApplicationsByJobseekerId(int jobseekerId)
        {
            var dbConn = new NpgsqlConnection(ConnectionString);
            var applications = new List<JobApplication>();

            try
            {
                dbConn.Open();
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
            SELECT 
                ja.id,
                ja.jobid,
                ja.jobseekerid,
                ja.resumeid,
                ja.coverletter,
                ja.status,
                r.content AS resume_content,
                j.title AS job_title,
                u.name AS jobseeker_name
            FROM jobapplication ja
            LEFT JOIN resume r ON ja.resumeid = r.id
            LEFT JOIN job j ON ja.jobid = j.id
            LEFT JOIN users u ON ja.jobseekerid = u.id
            WHERE ja.jobseekerid = @jobseekerid;";

                cmd.Parameters.AddWithValue("@jobseekerid", NpgsqlDbType.Integer, jobseekerId);

                var data = GetData(dbConn, cmd);
                while (data != null && data.Read())
                {
                    applications.Add(new JobApplication
                    {
                        Id = Convert.ToInt32(data["id"]),
                        JobId = Convert.ToInt32(data["jobid"]),
                        JobSeekerId = Convert.ToInt32(data["jobseekerid"]),
                        ResumeId = data["resumeid"] is DBNull ? null : (int?)Convert.ToInt32(data["resumeid"]),
                        CoverLetter = data["coverletter"] as string,
                        Status = Enum.Parse<ApplicationStatus>(data["status"].ToString()!),
                        ResumeContent = data["resume_content"]?.ToString() ?? string.Empty,
                        JobTitle = data["job_title"]?.ToString() ?? string.Empty,
                        JobSeekerName = data["jobseeker_name"]?.ToString() ?? string.Empty
                    });
                }

                return applications;
            }
            finally
            {
                dbConn.Close();
            }
        }

        // C - Insert a new job application
        public bool InsertJobApplication(JobApplication app)
        {
            NpgsqlConnection? dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
            INSERT INTO public.jobapplication
            (jobid, jobseekerid, resumeid, coverletter, status)
            VALUES
            (@jobid, @jobseekerid, @resumeid, @coverletter, @status::applicationstatus);
        ";

                cmd.Parameters.AddWithValue("@jobid", NpgsqlDbType.Integer, app.JobId);
                cmd.Parameters.AddWithValue("@jobseekerid", NpgsqlDbType.Integer, app.JobSeekerId);
                cmd.Parameters.AddWithValue("@resumeid", app.ResumeId.HasValue ? (object)app.ResumeId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@coverletter", NpgsqlDbType.Text, app.CoverLetter ?? string.Empty);
                cmd.Parameters.AddWithValue("@status", app.Status.ToString());

                return InsertData(dbConn, cmd);
            }
            finally
            {
                dbConn?.Close();
            }
        }

        // U - Update an existing job application
        public bool UpdateJobApplication(JobApplication app)
        {
            var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
        UPDATE public.jobapplication SET
            jobid = @jobid,
            jobseekerid = @jobseekerid,
            resumeid = @resumeid,
            coverletter = @coverletter,
            status = @status
        WHERE id = @id;
    ";

            cmd.Parameters.AddWithValue("@jobid", NpgsqlDbType.Integer, app.JobId);
            cmd.Parameters.AddWithValue("@jobseekerid", NpgsqlDbType.Integer, app.JobSeekerId);
            cmd.Parameters.AddWithValue("@resumeid", app.ResumeId.HasValue ? (object)app.ResumeId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@coverletter", NpgsqlDbType.Text, app.CoverLetter ?? string.Empty);
            cmd.Parameters.AddWithValue("@status", NpgsqlDbType.Text, app.Status.ToString());
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, app.Id);

            return UpdateData(dbConn, cmd);
        }

        // D - Delete a job application
        public bool DeleteJobApplication(int id)
        {
            var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "DELETE FROM public.jobapplication WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

            return DeleteData(dbConn, cmd);
        }
    }
}
