using Octokit;
using System;

public class LabelAction : IssueAction, IIssueAttributeAction
{
    public string Label => AdditionalData[0];

    public void ApplyTo(IssueUpdate issue)
    {
        switch (Operation)
        {
            case OperationTypeEnum.add:
                issue.AddLabel(Label);
                break;
            case OperationTypeEnum.remove:
                issue.RemoveLabel(Label);
                break;
            default:
                throw new NotSupportedException($"{nameof(LabelAction)} does not support operation {Operation}");
        }
    }
}
