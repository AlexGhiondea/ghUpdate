using Octokit;
using System.ComponentModel;

namespace ghUpdate.Tests
{
    class ObjectCreators
    {

        public static Issue GetMockIssue(string title, string body, string htmlUrl, int number, ItemState state, string org, string repoName)
        {
            return new Issue(default, htmlUrl, default, default, number, state,
                                   title, body, default, default, default, default, default, default,
                                   default, default, default, default, default, default, default, default,
                                   GetMockRepo(org, repoName), default);
        }

        public static Repository GetMockRepo(string org, string repoName)
        {
            // using reflection to set the properties.

            User user = new User();
            typeof(User).GetProperty("Login").SetValue(user, org);

            Repository repo = new Repository();
            typeof(Repository).GetProperty("Name").SetValue(repo, repoName);
            typeof(Repository).GetProperty("Owner").SetValue(repo, user);

            return repo;
        }

        public static T CreateAction<T>(OperationTypeEnum operation, AttributeTypeEnum attribute, string additionalData) where T : IssueAction, new()
        {
            T ca = new T();
            ca.Attribute = attribute;
            ca.Operation = operation;
            ca.AdditionalData = new System.Collections.Generic.List<string>() { additionalData };
            return ca;
        }
    }
}
