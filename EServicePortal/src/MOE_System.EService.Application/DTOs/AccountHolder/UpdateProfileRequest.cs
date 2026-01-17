using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace MOE_System.EService.Application.DTOs.AccountHolder
{
    public class UpdateProfileRequest : IValidatableObject
    {
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string ContactNumber { get; set; } = string.Empty;

        public string RegisteredAddress { get; set; } = string.Empty;

        public string MailingAddress { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
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
