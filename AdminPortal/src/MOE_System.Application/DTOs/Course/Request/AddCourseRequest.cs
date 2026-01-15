using System.ComponentModel.DataAnnotations;

namespace MOE_System.Application.DTOs.Course.Request
{
    public class AddCourseRequest : IValidatableObject
    {
        [Required(ErrorMessage = "Course name is required.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Course name must be between 1 and 200 characters.")]
        public required string CourseName { get; set; }

        [Required(ErrorMessage = "Provider ID is required.")]
        public required string ProviderId { get; set; }

        [Required(ErrorMessage = "Mode of training is required.")]
        [StringLength(100, ErrorMessage = "Mode of training must not exceed 100 characters.")]
        public required string ModeOfTraining { get; set; }

        [Required(ErrorMessage = "Course start date is required.")]
        public required DateTime CourseStartDate { get; set; }

        [Required(ErrorMessage = "Course end date is required.")]
        public required DateTime CourseEndDate { get; set; }

        [Required(ErrorMessage = "Payment option is required.")]
        [RegularExpression("^(One-time|Recurring)$", ErrorMessage = "Payment option must be either 'One-time' or 'Recurring'.")]
        public required string PaymentOption { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Total fee must be greater than 0.")]
        public decimal? TotalFee { get; set; }

        [RegularExpression("^(Monthly|Quarterly|Biannually|Yearly)$", ErrorMessage = "Billing cycle must be one of: Monthly, Quarterly, Biannually, Yearly.")]
        public string? BillingCycle { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Fee per cycle must be greater than 0.")]
        public decimal? FeePerCycle { get; set; }

        // Optional fields
        public string? CourseCode { get; set; }
        public string? TermName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validate that Course End Date is after Course Start Date
            if (CourseEndDate <= CourseStartDate)
            {
                yield return new ValidationResult(
                    "Course end date must be after course start date.",
                    new[] { nameof(CourseEndDate) });
            }

            // Validate One-time payment option
            if (PaymentOption == "One-time")
            {
                if (!TotalFee.HasValue || TotalFee <= 0)
                {
                    yield return new ValidationResult(
                        "Total fee is required and must be greater than 0 for One-time payment option.",
                        new[] { nameof(TotalFee) });
                }

                // Clear recurring fields if One-time is selected
                if (BillingCycle != null || FeePerCycle.HasValue)
                {
                    yield return new ValidationResult(
                        "Billing cycle and fee per cycle should not be provided for One-time payment option.",
                        new[] { nameof(BillingCycle), nameof(FeePerCycle) });
                }
            }

            // Validate Recurring payment option
            if (PaymentOption == "Recurring")
            {
                if (string.IsNullOrWhiteSpace(BillingCycle))
                {
                    yield return new ValidationResult(
                        "Billing cycle is required for Recurring payment option.",
                        new[] { nameof(BillingCycle) });
                }

                // At least one of FeePerCycle or TotalFee must be provided
                if (!FeePerCycle.HasValue && !TotalFee.HasValue)
                {
                    yield return new ValidationResult(
                        "Either Fee per Cycle or Total Fee must be provided for Recurring payment option.",
                        new[] { nameof(FeePerCycle), nameof(TotalFee) });
                }

                // Both cannot be provided at the same time
                if (FeePerCycle.HasValue && TotalFee.HasValue)
                {
                    yield return new ValidationResult(
                        "Only one of Fee per Cycle or Total Fee should be provided, not both.",
                        new[] { nameof(FeePerCycle), nameof(TotalFee) });
                }
            }

            // Validate that Course Start Date is not in the past
            if (CourseStartDate.Date < DateTime.UtcNow.Date)
            {
                yield return new ValidationResult(
                    "Course start date cannot be in the past.",
                    new[] { nameof(CourseStartDate) });
            }
        }
    }
}
