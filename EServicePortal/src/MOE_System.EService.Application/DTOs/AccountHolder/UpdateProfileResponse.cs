using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.EService.Application.DTOs.AccountHolder
{
    public class UpdateProfileResponse
    {
        public string AccountHolderId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string RegisteredAddress { get; set; } = string.Empty;
        public string MailingAddress { get; set; } = string.Empty;
    }
}
