using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Common;

public static class EntityValidation
{
    public static class Challenge
    {
        public const int TitleMaxLength = 100;

        public const int DescriptionMaxLength = 2000;

        public const int TagsMaxLength = 200;
    }

    public static class Submission
    {
        public const int SolutionCodeMaxLength = 10000;

        public const int FeedbackMaxLength = 1000;
    }

    public static class ApplicationUser
    {
        public const int DisplayNameMaxLength = 50;
    }
}

