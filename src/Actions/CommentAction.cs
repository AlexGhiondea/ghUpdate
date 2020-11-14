using Octokit;
using System;
using System.Collections.Generic;
using System.Net;

public class CommentAction : IssueAction, ICommentAction
{
    public string Comment => AdditionalData[0].Trim();

    internal static Dictionary<string, Func<Issue, string>> s_tokenMap = new Dictionary<string, Func<Issue, string>>()
    {
        { "#issue.number#", issue => issue.Number.ToString() },
        { "#issue.url#", issue => issue.HtmlUrl.ToString() },
        { "#issue.title#", issue => issue.Title },
        { "#issue.body#", issue => issue.Body },

        // These contain information about the repository
        { "#issue.repository.name#", issue => issue.Repository.Name },
        { "#issue.repository.org#", issue => issue.Repository.Owner.Login},

        // some properties can appear in the link so they need to be encoded
        { "#issue.title.encoded#", issue => WebUtility.UrlEncode(issue.Title) },
        { "#issue.body.encoded#", issue => WebUtility.UrlEncode(issue.Body) },

        // some are not related to the issue at all
        { "#env.newline#", issue => Environment.NewLine },
    };

    public string GetComment(Issue issue)
    {
        if (Operation != OperationTypeEnum.add)
        {
            throw new InvalidOperationException($"{nameof(CommentAction)} only supports 'add' operation");
        }

        string commentBuilder = Comment;
        foreach (string key in s_tokenMap.Keys)
        {
            commentBuilder = commentBuilder.Replace(key, s_tokenMap[key](issue), StringComparison.OrdinalIgnoreCase);
        }

        return commentBuilder;
    }
}
