using CodeArena.Data.Common.Enums;
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

public class XpTransactionRepository : IXpTransactionRepository
{
    private readonly ApplicationDbContext _context;

    public XpTransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(XpTransaction transaction)
    {
        await _context.XpTransactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<XpTransaction, bool>> predicate)
    {
        return await _context.XpTransactions.AnyAsync(predicate);
    }

    public IQueryable<XpTransaction> GetAll() => _context.XpTransactions.AsNoTracking();

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
