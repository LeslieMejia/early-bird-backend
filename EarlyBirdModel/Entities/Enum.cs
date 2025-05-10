namespace EarlyBirdAPI.Model.Entities
{
    public enum UserRole
    {
        employer,
        jobseeker
    }

    public enum JobStatus
    {
        active,
        expired,
        closed
    }

    public enum ApplicationStatus
    {
        applied,
        reviewed,
        interview,
        rejected
    }
}
