using CodeArena.Services.DTOs.Admin.Submission;

namespace CodeArena.Web.Areas.Admin.Models;

public class SubmissionsIndexViewModel
{
    public IEnumerable<SubmissionDisplayDto> Submissions {  get; set; } 
        = new List<SubmissionDisplayDto>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
