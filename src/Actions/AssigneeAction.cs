using Octokit;
using System;

public class AssigneeAction : IssueAction, IIssueAttributeAction
{
    public string Assignee => AdditionalData[0];

    public void ApplyTo(IssueUpdate issue)
    {
        switch (Operation)
        {
            case OperationTypeEnum.add:
                issue.AddAssignee(Assignee);
                break;
            case OperationTypeEnum.remove:
                issue.RemoveAssignee(Assignee);
                break;
            default:
                throw new NotSupportedException($"{nameof(AssigneeAction)} does not support operation {Operation}");
        }
    }
}
