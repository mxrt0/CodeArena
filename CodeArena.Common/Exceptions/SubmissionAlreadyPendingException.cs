using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeArena.Common.OutputMessages;

namespace CodeArena.Common.Exceptions;

public class SubmissionAlreadyPendingException : Exception
{
    public SubmissionAlreadyPendingException(int challengeId, string userName)
        : base(string.Format(SubmissionAlreadyPendingExceptionMessage, userName, challengeId)) 
    {
    }
}
