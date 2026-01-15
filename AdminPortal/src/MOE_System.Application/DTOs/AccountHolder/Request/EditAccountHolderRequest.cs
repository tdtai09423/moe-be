using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text;
using System.Text.RegularExpressions;

namespace MOE_System.Application.DTOs.AccountHolder.Request
{
    public class EditAccountHolderRequest : IValidatableObject
    {
        [Required(ErrorMessage = "Account Holder ID is required.")]
        public required string AccountHolderId { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Registered address is required.")]
        public required string RegisteredAddress { get; set; } 

        [Required(ErrorMessage = "Mailing address is required.")]
        public required string MailingAddress { get; set; }

        [Required(ErrorMessage = "Education level is required.")]
        public required string EducationLevel { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(PhoneNumber))
            {
                var cleanedNumber = Regex.Replace(PhoneNumber, @"[\s\-\(\)\+]", "");

                // Singapore mobile: 8 digits starting with 8 or 9
                // Singapore landline: 8 digits starting with 6
                // International format: starts with country code
                if (!Regex.IsMatch(cleanedNumber, @"^([689]\d{7}|65[689]\d{7}|\+65[689]\d{7})$"))
                {
                    yield return new ValidationResult(
                        "Contact number must be a valid Singapore number (e.g., 81234567, 91234567, 61234567, +6581234567).",
                        new[] { nameof(PhoneNumber) });
                }
            }
        }
    }
}
