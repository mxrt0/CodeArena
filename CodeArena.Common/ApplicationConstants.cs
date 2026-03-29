namespace CodeArena.Common;

public static class ApplicationConstants
{
    public const string DefaultDateFormat = "dd/MM/yyyy HH:mm";

    public const string ErrorTempDataKey = "ErrorMessage";
    public const string WarningTempDataKey = "WarningMessage";
    public const string InfoTempDataKey = "InfoMessage";
    public const string SuccessTempDataKey = "SuccessMessage";

    public const string CacheKey_ChallengesAll = "Cache_Challenges_All";
    public const string CacheKey_ChallengeBySlug = "Cache_Challenge_{0}";

    public const string CacheKey_SubmissionsAll = "Cache_Submissions_All";
    public const string CacheKey_SubmissionsByUserId = "Cache_Submissions_User_{0}";
    public const string CacheKey_PendingSubmissions = "Cache_Submissions_Pending";
    public const string CacheKey_User_SubmissionById = "Cache_User_Submission_{0}";
    public const string CacheKey_Admin_SubmissionById = "Cache_Admin_Submission_{0}";

    public const int CacheDuration_ChallengesAll_Minutes = 60;
    public const int CacheDuration_ChallengeBySlug_Minutes = 60;
    public const int CacheDuration_SubmissionsAll_Minutes = 2;
    public const int CacheDuration_SubmissionsByUserId_Minutes = 5;
    public const int CacheDuration_PendingSubmissions_Minutes = 1;
    public const int CacheDuration_SubmissionById_Minutes = 5;
}
