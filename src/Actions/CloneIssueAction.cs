using ghUpdate.Models;
using Octokit;
using System;

[AppliesTo(AttributeTypeEnum.issue)]
public class CloneIssueAction : IssueAction, ICloneIssueAction
{
    public NewIssue CloneIssue(Issue issue)
    {
        switch (Operation)
        {
            case OperationTypeEnum.clone:
                return new NewIssue(issue.Title);
            default:
                throw new NotSupportedException($"{nameof(TitleAction)} only supports 'clone' operation.");
        }
    }

    public string GetNewOrg() => AdditionalData[0].Trim();
    
    public string GetNewRepo() => AdditionalData[1].Trim();
}
