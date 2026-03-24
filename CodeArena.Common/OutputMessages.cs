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

    public const string ChallengeNotFoundMessage = "Challenge with identifier '{0}' was not found.";
    public const string SubmissionNotFoundMessage = "Submission with Id {0} was not found.";
    public const string SubmissionAlreadyPendingMessage = "User '{0}' already has a pending submission for challenge {1}.";
    public const string UnauthorizedActionMessage = "User '{0}' attempted an unauthorized action.";
    public const string UnauthenticatedActionMessage = "Unauthenticated user attempted an action which requires authentication.";

    // Logging
    public const string ChallengeAlreadyDeletedMessage = "Admin attempted to alter state of already deleted challenge with ID: {0}.";
    public const string ChallengeAlreadyActiveMessage = "Admin attempted to restore already active challenge with ID: {0}.";

    public const string SubmissionAlreadyApprovedMessage = "Admin attempted to alter status of already approved submission with ID: {0}.";
    public const string SubmissionAlreadyRejectedMessage = "Admin attempted to alter status of already rejected submission with ID: {0}.";

    // User-targeted
    public const string Admin_SubmissionAlreadyApprovedMessage = "This submission is already marked as approved.";
    public const string Admin_SubmissionAlreadyRejectedMessage = "This submission is already marked as rejected.";

    public const string Admin_ChallengeAlreadyDeletedMessage = "This challenge is already marked deleted.";
    public const string Admin_ChallengeAlreadyActiveMessage = "This challenge is already active.";
}
