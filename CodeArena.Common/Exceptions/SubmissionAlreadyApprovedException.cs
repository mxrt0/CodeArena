using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeArena.Common.OutputMessages;

namespace CodeArena.Common.Exceptions;

public class SubmissionAlreadyApprovedException : Exception
{
    public SubmissionAlreadyApprovedException(int id)
        : base(string.Format(SubmissionAlreadyApprovedMessage, id))
    {        
    }
}
