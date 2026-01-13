using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MOE_System.Application.DTOs.AccountHolder.Request
{
    public class CreateAccountHolderRequest : IValidatableObject
    {
        [Required(ErrorMessage = "NRIC is required.")]
        public required string NRIC { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 100 characters.")]
        public required string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        public required DateTime DateOfBirth { get; set; } = default;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(256, ErrorMessage = "Email must not exceed 256 characters.")]
        public required string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public required string ContactNumber { get; set; }

        public string EducationLevel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Registered address is required.")]
        public required string RegisteredAddress { get; set; }

        public string MailingAddress { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(NRIC))
            {
                var nricPattern = @"^[STM]\d{7}[A-Z]$";
                if (!Regex.IsMatch(NRIC.ToUpper(), nricPattern))
                {
                    yield return new ValidationResult(
                        "NRIC must be in valid Singapore format (e.g., S1234567A).",
                        new[] { nameof(NRIC) });
                }

                if(NRIC.Length != 9)
                {
                    yield return new ValidationResult(
                        "NRIC must be exactly 9 characters long.",
                        new[] { nameof(NRIC) });
                }

                if (DateOfBirth.Year < 2000 && (NRIC.ToUpper().StartsWith("T") || NRIC.ToUpper().StartsWith("M")))
                {
                    yield return new ValidationResult(
                        "NRIC starting with 'T' or 'M' is only for individuals born in or after the year 2000.",
                        new[] { nameof(NRIC), nameof(DateOfBirth) });
                }

                if (DateOfBirth.Year >= 2000 && NRIC.ToUpper().StartsWith("S"))
                {
                    yield return new ValidationResult(
                        "NRIC starting with 'S' is only for individuals born before the year 2000.",
                        new[] { nameof(NRIC), nameof(DateOfBirth) });
                }

                string lastTwoDigitsOfBirthYear = (DateOfBirth.Year % 100).ToString("D2");
                var match = Regex.Match(NRIC, @"\d{2}");

                if (!match.Success)
                {
                    yield return new ValidationResult(
                        "NRIC format is invalid.",
                        new[] { nameof(NRIC) });
                }
            }

            // Validate Date of Birth (must be in the past and person must be at least 1 year old)
            if (DateOfBirth >= DateTime.Today)
            {
                yield return new ValidationResult(
                    "Date of birth must be in the past.",
                    new[] { nameof(DateOfBirth) });
            }

            if (DateOfBirth > DateTime.Today.AddYears(-1))
            {
                yield return new ValidationResult(
                    "Account holder must be at least 1 year old.",
                    new[] { nameof(DateOfBirth) });
            }

            // Validate that person is not unreasonably old (e.g., > 150 years)
            if (DateOfBirth < DateTime.Today.AddYears(-150))
            {
                yield return new ValidationResult(
                    "Date of birth is invalid.",
                    new[] { nameof(DateOfBirth) });
            }

            // Validate ContactNumber format (Singapore format: 8 digits starting with 6, 8, or 9)
            if (!string.IsNullOrWhiteSpace(ContactNumber))
            {
                var cleanedNumber = Regex.Replace(ContactNumber, @"[\s\-\(\)\+]", "");
                
                // Singapore mobile: 8 digits starting with 8 or 9
                // Singapore landline: 8 digits starting with 6
                // International format: starts with country code
                if (!Regex.IsMatch(cleanedNumber, @"^([689]\d{7}|65[689]\d{7}|\+65[689]\d{7})$"))
                {
                    yield return new ValidationResult(
                        "Contact number must be a valid Singapore number (e.g., 81234567, 91234567, 61234567, +6581234567).",
                        new[] { nameof(ContactNumber) });
                }
            }
        }
    }
}
