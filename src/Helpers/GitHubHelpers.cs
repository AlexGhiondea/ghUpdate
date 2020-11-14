using Octokit;

namespace ghUpdate.Helpers
{
    public static class GitHubHelpers
    {
        /// <summary>
        /// Create a new issue with a new Repository object
        /// </summary>
        public static Issue WithRepository(this Issue issue, Repository repository)
        {
            return new Issue(issue.Url, issue.HtmlUrl, issue.CommentsUrl, issue.EventsUrl, issue.Number, issue.State.Value,
                                   issue.Title, issue.Body, issue.ClosedBy, issue.User, issue.Labels, issue.Assignee, issue.Assignees, issue.Milestone,
                                   issue.Comments, issue.PullRequest, issue.ClosedAt, issue.CreatedAt, issue.UpdatedAt, issue.Id, issue.NodeId, issue.Locked,
                                   repository, issue.Reactions);
        }
    }
}
