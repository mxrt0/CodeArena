using CodeArena.Services.DTOs.Admin.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace CodeArena.Web.Areas.Admin.Models;

public class AdminDashboardViewModel
{
    AdminDashboardDto DashboardData { get; set; } = null!;
} 
