using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Common;

public static class OutputMessages
{
    public const string NoFeedbackMessage = "No feedback available.";

    public const string LanguageNotSelectedMessage = "Please select a language.";
    public const string SolutionCodeRequiredMessage = "Solution code is required.";
    public const string InvalidSolutionCodeLengthMessage = "Solution code must be between 5 and 10000 characters.";

    public const string InvalidChallengeTitleLengthMessage = "Title must be between 5 and 100 characters long.";
    public const string InvalidChallengeDescriptionLengthMessage = "Description must be between 10 and 2000 characters long.";

    public const string MissingAdminCredentialsMessage = "Admin email or password not set in configuration.";

    public const string SubmissionNotFoundMessage = "Submission not found.";
    public const string UnauthenticatedUserSubmissionAttemptMessage = "User must be authenticated to create a submission.";
    public const string UserAlreadyHasPendingSubmissionMessage = "User already has a pending submission for this challenge.";

    public const string ChallengeCreatedMessage = "Challenge has been created.";
    public const string ChallengeUpdatedMessage = "Challenge has been updated.";
    public const string ChallengeDeletedMessage = "Challenge has been deleted.";
    public const string ChallengeRestoredMessage = "Challenge has been restored.";

    public const string SubmissionCreatedMessage = "Your submission has been submitted.";
    public const string SubmissionCancelledMessage = "Your submission has been cancelled.";
    public const string SubmissionApprovedMessage = "Submission approved.";
    public const string SubmissionRejectedMessage = "Submission rejected.";

    public const string ChallengeNotFoundExceptionMessage = "Challenge with Id {0} was not found.";
    public const string SubmissionNotFoundExceptionMessage = "Submission with Id {0} was not found.";
    public const string SubmissionAlreadyPendingExceptionMessage = "User '{0}' already has a pending submission for challenge {1}.";
    public const string UnauthorizedActionExceptionMessage = "You are not authorized to perform this action.";
}
