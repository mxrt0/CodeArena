using CodeArena.Data.Models;
using CodeArena.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ApplicationUser user)
    {
        await _context.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<ApplicationUser, bool>> predicate)
    {
        return await _context.Users.AnyAsync(predicate);
    }

    public Task<int> CountAsync(Expression<Func<ApplicationUser, bool>>? predicate = null)
    {
        return predicate is null
        ? _context.Users.CountAsync()
        : _context.Users.CountAsync(predicate);
    }

    public async Task<ApplicationUser?> FirstOrDefaultAsync(Expression<Func<ApplicationUser, bool>> predicate)
    {
        return await _context.Users.FirstOrDefaultAsync(predicate);
    }

    public IQueryable<ApplicationUser> GetAll() => _context.Users.AsNoTracking();

    public async Task RemoveAsync(ApplicationUser user)
    {
        _context.Remove(user);
        await _context.SaveChangesAsync();
    }
}
