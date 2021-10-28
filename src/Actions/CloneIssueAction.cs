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
                {
                    NewIssue ni = new NewIssue(issue.Title);

                    ni.Body = issue.Body;
                    foreach (User assignee in issue.Assignees)
                    {
                        ni.Assignees.Add(assignee.Login);
                    }
                    foreach (Label label in issue.Labels)
                    {
                        ni.Labels.Add(label.Name);
                    }
                    // do not clone the milestone as they rely on a milestone number which will be different across the repos

                    return ni;
                }
            default:
                throw new NotSupportedException($"{nameof(TitleAction)} only supports 'clone' operation.");
        }
    }

    public string GetNewOrg(){
        return AdditionalData[0];
    } 

    public string GetNewRepo() {
        return AdditionalData[1];
    }
}
