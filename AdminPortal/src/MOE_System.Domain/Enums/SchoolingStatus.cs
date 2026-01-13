using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Domain.Enums
{
    public enum SchoolingStatus
    {
        NotInSchool = 0 ,
        InSchool = 1,
    }

    public static class SchoolingStatusExtensions
    {
        public static string ToFriendlyString(this SchoolingStatus status)
        {
            return status switch
            {
                SchoolingStatus.NotInSchool => "Not in School",
                SchoolingStatus.InSchool => "In School",
                _ => "Unknown",
            };
        }
    }
}
