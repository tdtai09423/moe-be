using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Domain.Enums
{
    public enum ResidentialStatus
    {
        SingaporeCitizen = 0,
        PermanentResident = 1,
        NonResident = 2
    }

    public static class ResidentialExtensions
    {
        public static string ToFriendlyString(this ResidentialStatus status)
        {
            return status switch
            {
                ResidentialStatus.SingaporeCitizen => "Singapore Citizen",
                ResidentialStatus.PermanentResident => "Permanent Resident",
                ResidentialStatus.NonResident => "Non-Resident",
                _ => "Unknown",
            };
        }
    }
}
