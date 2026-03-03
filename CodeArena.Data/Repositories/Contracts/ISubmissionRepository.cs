using CodeArena.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Repositories.Contracts;

public interface ISubmissionRepository
{
    Task AddAsync(Submission submission);
    Task RemoveAsync(Submission submission);
}
