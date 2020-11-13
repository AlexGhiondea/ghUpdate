using Octokit;
using System;
using System.Threading;

public class CommentAction : IssueAction, ICommentAction
{
    public string Comment => AdditionalData[0].Trim();

    public string GetComment()
    {
        if (Operation != OperationTypeEnum.add)
        {
            throw new InvalidOperationException($"{nameof(CommentAction)} only supports 'add' operation");
        }

        return Comment;
    }
}
