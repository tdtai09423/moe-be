using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace MOE_System.Application.DTOs.AccountHolder
{
    public class CreateAccountHolderRequest : IValidatableObject
    {
        [Required(ErrorMessage = "NRIC is required.")]
        [StringLength(50, MinimumLength = 9, ErrorMessage = "NRIC must be between 9 and 50 characters.")]
        public string NRIC { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 100 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 100 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(256, ErrorMessage = "Email must not exceed 256 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string ContactNumber { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(NRIC))
            {
                var nricPattern = @"^[STFGM]\d{7}[A-Z]$";
                if (!Regex.IsMatch(NRIC.ToUpper(), nricPattern))
                {
                    yield return new ValidationResult(
                        "NRIC must be in valid Singapore format (e.g., S1234567A).",
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

            // Validate FirstName and LastName don't contain numbers or special characters
            if (!string.IsNullOrWhiteSpace(FirstName))
            {
                if (!Regex.IsMatch(FirstName, @"^[a-zA-Z\s\-']+$"))
                {
                    yield return new ValidationResult(
                        "First name can only contain letters, spaces, hyphens, and apostrophes.",
                        new[] { nameof(FirstName) });
                }
            }

            if (!string.IsNullOrWhiteSpace(LastName))
            {
                if (!Regex.IsMatch(LastName, @"^[a-zA-Z\s\-']+$"))
                {
                    yield return new ValidationResult(
                        "Last name can only contain letters, spaces, hyphens, and apostrophes.",
                        new[] { nameof(LastName) });
                }
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
