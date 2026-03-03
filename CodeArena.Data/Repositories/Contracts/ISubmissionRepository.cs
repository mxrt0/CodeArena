using CodeArena.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Repositories.Contracts;

public interface ISubmissionRepository
{
    Task AddAsync(Submission submission);
    Task RemoveAsync(Submission submission);
    Task<bool> AnyAsync(Expression<Func<Submission, bool>> predicate);
    Task<Submission?> FirstOrDefaultAsync(Expression<Func<Submission, bool>> predicate);
}
