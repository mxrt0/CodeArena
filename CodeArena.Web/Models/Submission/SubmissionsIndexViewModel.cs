using CodeArena.Services.DTOs.Submission;

namespace CodeArena.Web.Models.Submission;

public class SubmissionsIndexViewModel
{
    public IEnumerable<SubmissionDisplayDto> Submissions { get; set; }
                = new List<SubmissionDisplayDto>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
