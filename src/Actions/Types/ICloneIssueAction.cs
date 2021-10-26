using Octokit;

public interface ICloneIssueAction
{
    NewIssue CloneIssue(Issue issue);

    string GetNewRepo();
    string GetNewOrg();
}
