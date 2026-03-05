using CodeArena.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Repositories.Contracts;

public interface IChallengeRepository
{
    IQueryable<Challenge> GetAll(bool includeDeleted = false);
    Task<Challenge?> GetByIdAsync(int id, bool includeDeleted = false);
    Task AddAsync(Challenge challenge);
    Task UpdateAsync(Challenge challenge);
    Task DeleteAsync(Challenge challenge);
    Task<int> CountAsync(Expression<Func<Challenge, bool>>? predicate = null);
}
