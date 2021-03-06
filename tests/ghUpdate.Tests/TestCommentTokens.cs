using NUnit.Framework;
using NUnit.Framework.Constraints;
using Octokit;
using System;

namespace ghUpdate.Tests
{
    public class TestCommentTransform
    {
        [Category("Comment transform")]
        [TestCase("#issue.title", "Title")]
        [TestCase("#issue.body", "body")]
        [TestCase("#issue.number", "1")]
        [TestCase("#issue.url", "htmlUrl")]
        [TestCase("#issue.repository.name#", "testRepo")]
        [TestCase("#issue.repository.org#", "test")]
        [TestCase("#env.newline.encoded#", "%0A")]
        [TestCase("#env.newline", "\n\r")]
        public void CheckMapping(string toTransform, string expected)
        {
            Issue issue = ObjectCreators.GetMockIssue("Title", "body", "htmlUrl", 1, ItemState.Open, "test", "testRepo");

            string commentTransform = ((CommentAction)IssueAction.Create(OperationTypeEnum.add, AttributeTypeEnum.comment, "#issue.title#")).GetComment(issue);

            Assert.AreEqual("Title", commentTransform);
        }
    }
}