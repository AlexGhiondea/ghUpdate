using Octokit;

public interface IIssueAttributeAction
{
    void ApplyTo(IssueUpdate issue);
}
