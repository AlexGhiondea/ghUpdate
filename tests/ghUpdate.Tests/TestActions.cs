using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using NUnit.Framework;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ghUpdate.Tests
{
    public class TestActions
    {
        [Category("Actions")]
        [TestCase(OperationTypeEnum.add, AttributeTypeEnum.assignee, "testUser", new string[] { "assignedUser", "testUser" })]
        [TestCase(OperationTypeEnum.add, AttributeTypeEnum.label, "testLabel", new string[] { "defaultLabel", "testLabel" })]

        [TestCase(OperationTypeEnum.set, AttributeTypeEnum.body, "updatedBody", "updatedBody")]
        [TestCase(OperationTypeEnum.set, AttributeTypeEnum.title, "updatedTitle", "updatedTitle")]
        [TestCase(OperationTypeEnum.set, AttributeTypeEnum.state, "Closed", ItemState.Closed)]

        [TestCase(OperationTypeEnum.remove, AttributeTypeEnum.assignee, "assignedUser", new string[] { })]
        [TestCase(OperationTypeEnum.remove, AttributeTypeEnum.label, "defaultLabel", new string[] { })]
        public void TestOperations(OperationTypeEnum operation, AttributeTypeEnum attribute, string additionalData, object expectedResult)
        {
            IssueAction action = IssueAction.Create(operation, attribute, new string[] { additionalData });

            Issue issue = ObjectCreators.GetMockIssue("Title", "body", "htmlUrl", 1, ItemState.Open, "test", "testRepo");

            IssueUpdate updatedIssue = issue.ToUpdate();

            (action as IIssueAttributeAction).ApplyTo(updatedIssue);

            Assert.AreEqual(expectedResult, GetValueOfAttribute(updatedIssue, attribute));
        }

        [Category("Invalid operation on Action")]

        [TestCase(OperationTypeEnum.add, AttributeTypeEnum.state, "testLabel")]
        [TestCase(OperationTypeEnum.add, AttributeTypeEnum.title, "updatedTitle")]
        [TestCase(OperationTypeEnum.add, AttributeTypeEnum.body, "updatedBody")]

        [TestCase(OperationTypeEnum.set, AttributeTypeEnum.label, "testLabel")]
        [TestCase(OperationTypeEnum.set, AttributeTypeEnum.assignee, "testUser")]

        [TestCase(OperationTypeEnum.remove, AttributeTypeEnum.state, "testLabel")]
        [TestCase(OperationTypeEnum.remove, AttributeTypeEnum.title, "updatedTitle")]
        [TestCase(OperationTypeEnum.remove, AttributeTypeEnum.body, "updatedBody")]
        public void TestInvalidOperations(OperationTypeEnum operation, AttributeTypeEnum attribute, string additionalData)
        {
            IssueAction action = IssueAction.Create(operation, attribute, new string[] { additionalData });

            Issue issue = ObjectCreators.GetMockIssue("Title", "body", "htmlUrl", 1, ItemState.Open, "test", "testRepo");

            IssueUpdate updatedIssue = issue.ToUpdate();

            Assert.Throws<NotSupportedException>(() => (action as IIssueAttributeAction).ApplyTo(updatedIssue));
        }

        private static object GetValueOfAttribute(IssueUpdate issue, AttributeTypeEnum attribute)
        {
            switch (attribute)
            {
                case AttributeTypeEnum.label:
                    return issue.Labels.ToArray();
                case AttributeTypeEnum.assignee:
                    return issue.Assignees.ToArray();
                case AttributeTypeEnum.state:
                    return issue.State;
                case AttributeTypeEnum.title:
                    return issue.Title;
                case AttributeTypeEnum.body:
                    return issue.Body;
                default:
                    throw new NotSupportedException();
            }
        }

    }
}