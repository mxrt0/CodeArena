using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.DTOs.Admin.Dashboard;

public sealed record AdminDashboardDto(
    int TotalUsers,
    int TotalChallenges,
    int TotalSubmissions,
    int PendingSubmissions,
    int ApprovedSubmissions,
    int RejectedSubmissions
    );

