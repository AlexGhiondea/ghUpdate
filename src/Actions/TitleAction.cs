using Octokit;
using System;

public class TitleAction : IssueAction, IIssueAttributeAction
{
    public string Title => AdditionalData[0];

    public void ApplyTo(IssueUpdate issue)
    {
        switch (Operation)
        {
            case OperationTypeEnum.set:
                issue.Title = Title;
                break;
            default:
                throw new NotSupportedException($"{nameof(TitleAction)} only supports 'set' operation.");
        }
    }
}
