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
    IQueryable<Challenge> GetAll();
    Task<Challenge?> GetByIdAsync(int id);
    Task AddAsync(Challenge challenge);
    Task UpdateAsync(Challenge challenge);
    Task DeleteAsync(int id);
    Task<int> CountAsync(Expression<Func<Challenge, bool>>? predicate = null);
}
