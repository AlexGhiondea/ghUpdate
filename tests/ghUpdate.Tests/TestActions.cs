using NUnit.Framework;
using NUnit.Framework.Constraints;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ghUpdate.Tests
{
    public class TestActions
    {
        [Category("Aplied action")]
        [Test]
        public void ApplyLabel()
        {
            IssueAction action = ObjectCreators.CreateAction<LabelAction>(OperationTypeEnum.add, AttributeTypeEnum.label, "testLabel");

            Issue issue = ObjectCreators.GetMockIssue("Title", "body", "htmlUrl", 1, ItemState.Open, "test", "testRepo");

            IssueUpdate updatedIssue = issue.ToUpdate();

            (action as IIssueAttributeAction).ApplyTo(updatedIssue);

            Assert.AreEqual("testLabel", updatedIssue.Labels.First());
        }

        [Category("Aplied assignee")]
        [Test]
        public void ApplyAssignee()
        {
            IssueAction action = ObjectCreators.CreateAction<AssigneeAction>(OperationTypeEnum.add, AttributeTypeEnum.assignee, "testUser");

            Issue issue = ObjectCreators.GetMockIssue("Title", "body", "htmlUrl", 1, ItemState.Open, "test", "testRepo");

            IssueUpdate updatedIssue = issue.ToUpdate();

            (action as IIssueAttributeAction).ApplyTo(updatedIssue);

            Assert.AreEqual("testUser", updatedIssue.Assignees.First());
        }
    }
}