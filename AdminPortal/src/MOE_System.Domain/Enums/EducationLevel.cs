using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Domain.Enums
{
    public enum EducationLevel
    {
        Primary = 0,
        Secondary = 1,
        PostSecondary = 2,
        Tertiary = 3,
        PostGraduate = 4
    }

    public static class EducationLevelExtensions
    {
        public static string ToFriendlyString(this EducationLevel level)
        {
            return level switch
            {
                EducationLevel.Primary => "Primary",
                EducationLevel.Secondary => "Secondary",
                EducationLevel.PostSecondary => "Post-Secondary",
                EducationLevel.Tertiary => "Tertiary",
                EducationLevel.PostGraduate => "Post-Graduate",
                _ => "Unknown",
            };
        }
    }
}
