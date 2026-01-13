namespace MOE_System.Application.DTOs.AccountHolder.Response
{
    public class AccountHolderResponse
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string NRIC { get; set; } = string.Empty;
        public int Age { get; set; }
        public decimal Balance { get; set; }
        public string EducationLevel { get; set; } = string.Empty;
        //public string ResidentialStatus { get; set; } = string.Empty; implement later
        public DateOnly CreatedDate { get; set; }
        public int CourseCount { get; set; }

    }
}
