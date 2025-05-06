using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EarlyBirdAPI.Model.Entities;

    public class JobApplication
    {
        public int Id { get; set; }

        public int JobId { get; set; }

        public int JobSeekerId { get; set; }

        public int? ResumeId { get; set; }

        public string? CoverLetter { get; set; }

        public ApplicationStatus Status { get; set; }

     [NotMapped]
        public string? ResumeContent { get; set; } //for frontend use not an actual column

    [NotMapped]
        public string? JobTitle { get; set; } //for frontend use not an actual column

}

