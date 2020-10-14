using Octokit;

public class LabelAction : IssueAction
{
    public string Label
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
                issue.AddLabel(Label);
                break;
            case OperationTypeEnum.remove:
                issue.RemoveLabel(Label);
                break;
            default:
                break;
        }
    }
}
