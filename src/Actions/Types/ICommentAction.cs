using Octokit;

public interface ICommentAction
{
    string GetComment(Issue issue);
}
