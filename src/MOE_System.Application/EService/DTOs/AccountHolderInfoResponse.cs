using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.EService.DTOs
{
    public class AccountHolderInfoResponse
    {
        public string AccountHolderId { get; set; }
        public string FullName { get; set; }
        public string NRIC {  get; set; }
        public bool IsActive { get; set; }
        public string SchoolingStatus { get; set; }
        public DateTime DOB { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
    }
}
