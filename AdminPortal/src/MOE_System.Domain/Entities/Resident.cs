using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Domain.Entities
{
    public class Resident
    {
        public string NRIC { get; set; } = string.Empty;
        public string PrincipalName { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty; 
        public string SecondaryRace { get; set; } = string.Empty;
        public string Dialect { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; } 
        public string ResidentialStatus { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string RegisteredAddress { get; set; } = string.Empty;
    }
}
