using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Services.DTOs.Admin.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Admin;

public class AdminDashboardService : IAdminDashboardService
{
    public Task<AdminDashboardDto> GetDashboardDataAsync()
    {
        throw new NotImplementedException();
    }
}
