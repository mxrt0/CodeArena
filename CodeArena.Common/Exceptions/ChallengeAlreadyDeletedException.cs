using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeArena.Common.OutputMessages;

namespace CodeArena.Common.Exceptions;

public class ChallengeAlreadyDeletedException : Exception
{
    public ChallengeAlreadyDeletedException(int id)
    : base(string.Format(ChallengeAlreadyDeletedMessage, id))
    {       
    }
}
