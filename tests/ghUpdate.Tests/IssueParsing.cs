using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using NUnit.Framework;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ghUpdate.Tests
{
    [Category("Issue Parsing")]
    public class IssueParsing
    {
        [Test]
        [TestCaseSource(nameof(GetValidUrls))]
        public void ParseIssue(TestCaseData data)
        {
            ParsedIssue pi = ParsedIssue.Parse(data.Url);

            Assert.AreEqual(data.Issue.Id, pi.Id);
            Assert.AreEqual(data.Issue.Org, pi.Org);
            Assert.AreEqual(data.Issue.Repo, pi.Repo);
            Assert.AreEqual(data.Issue.Type, pi.Type);
        }

        [TestCase("")]
        [TestCase("foo/bar")]
        [TestCase("https://github.com/Azure/azure-sdk-for-ruby/")]
        public void ParseInvalidUrl(string url)
        {
            Assert.Throws<ArgumentException>(() => ParsedIssue.Parse(url));
        }

        public static List<TestCaseData> GetValidUrls()
        {
            return new List<TestCaseData>()
            {
                new TestCaseData()
                {
                    Url = "https://github.com/Azure/azure-sdk-for-ruby/issues/957",
                    Issue = new ParsedIssue()
                    {
                        Repo= "azure-sdk-for-ruby",
                        Org = "Azure",
                        Type= "issues",
                        Id = "957"
                    }
                }
                ,new TestCaseData()
                {
                    Url = "https://github.com/Azure/azure-sdk-for-net/pulls/1234",
                    Issue = new ParsedIssue()
                    {
                        Repo= "azure-sdk-for-net",
                        Org = "Azure",
                        Type= "pulls",
                        Id = "1234"
                    }
                } ,new TestCaseData()
                {
                    Url = "https://github.com/Azure/azure-sdk-for-net/pulls/1234/",
                    Issue = new ParsedIssue()
                    {
                        Repo= "azure-sdk-for-net",
                        Org = "Azure",
                        Type= "pulls",
                        Id = "1234"
                    }
                }
            };
        }

        public class TestCaseData
        {
            public string Url;
            public ParsedIssue Issue;
            public override string ToString()
            {
                return Url;
            }
        }
    }
}