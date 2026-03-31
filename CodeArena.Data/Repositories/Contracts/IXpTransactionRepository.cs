using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Repositories.Contracts;

public interface IXpTransactionRepository
{
    IQueryable<XpTransaction> GetAll();
    Task AddAsync(XpTransaction transaction);

    Task<bool> AnyAsync(Expression<Func<XpTransaction, bool>> predicate);

    Task SaveChangesAsync();
}
