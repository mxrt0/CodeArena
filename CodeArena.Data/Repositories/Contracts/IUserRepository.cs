using CodeArena.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Repositories.Contracts;

public interface IUserRepository
{
    IQueryable<ApplicationUser> GetAll();
    Task AddAsync(ApplicationUser user);
    Task RemoveAsync(ApplicationUser user);
    Task<bool> AnyAsync(Expression<Func<ApplicationUser, bool>> predicate);
    Task<ApplicationUser?> FirstOrDefaultAsync(Expression<Func<ApplicationUser, bool>> predicate);
    Task<int> CountAsync(Expression<Func<ApplicationUser, bool>>? predicate = null);
}
