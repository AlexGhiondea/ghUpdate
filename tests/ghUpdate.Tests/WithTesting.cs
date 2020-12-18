using ghUpdate.Helpers;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using NUnit.Framework;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ghUpdate.Tests
{
    public class WithTesting
    {
        [Category("'With' testing")]
        [Test]
        public void IssueWithRepository()
        {
            Issue issue = ObjectCreators.GetMockIssue("Title", "body", "htmlUrl", 1, ItemState.Open, "test", "testRepo");

            Issue issueUpdated = issue.WithRepository(ObjectCreators.GetMockRepo("Foo", "Bar"));

            Assert.AreEqual("Bar", issueUpdated.Repository.Name);
        }
    }
}