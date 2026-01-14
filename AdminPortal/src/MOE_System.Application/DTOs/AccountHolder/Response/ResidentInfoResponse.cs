using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.DTOs.AccountHolder.Response
{
    public class ResidentInfoResponse
    {
        public string FullName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string RegisteredAddress { get; set; } = string.Empty;
        public string ResidentialStatus { get; set; } = string.Empty;
    }
}
