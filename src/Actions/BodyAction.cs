using Octokit;
using System;

public class BodyAction : IssueAction, IIssueAttributeAction
{
    public string Body => AdditionalData[0];

    public void ApplyTo(IssueUpdate issue)
    {
        switch (Operation)
        {
            case OperationTypeEnum.set:
                issue.Body = Body;
                break;
            default:
                throw new NotSupportedException($"{nameof(BodyAction)} only supports 'set' operation.");
        }
    }
}
