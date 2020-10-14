using Octokit;

public class AssigneeAction : IssueAction
{
    public string Assignee
    {
        get
        {
            return AdditionalData[0];
        }
    }

    public override void ApplyTo(IssueUpdate issue)
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
                break;
        }
    }
}
