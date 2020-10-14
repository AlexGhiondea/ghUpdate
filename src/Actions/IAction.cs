using Octokit;

public interface IAction
{
    void ApplyTo(IssueUpdate issue);
}
