namespace MOE_System.EService.Application.DTOs
{
    public class OutstandingFeeResponse
    {
        public string EducationAccountId { get; set; } = string.Empty;
        public string AccountHolderId { get; set; } = string.Empty;
        public decimal TotalOutstandingFee { get; set; }
        public List<OutstandingInvoiceInfo> OutstandingInvoices { get; set; } = new();
    }

    public class OutstandingInvoiceInfo
    {
        public string InvoiceId { get; set; } = string.Empty;
        public string EnrollmentId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
