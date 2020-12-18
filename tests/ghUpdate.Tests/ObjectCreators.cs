using Octokit;
using System.Collections.Generic;
using System.ComponentModel;

namespace ghUpdate.Tests
{
    class ObjectCreators
    {

        public static Issue GetMockIssue(string title, string body, string htmlUrl, int number, ItemState state, string org, string repoName)
        {
            return new Issue(default, htmlUrl, default, default, number, state,
                                   title, body, default, default, new List<Label>() { GetMockLabel("defaultLabel") }, default, new List<User>() { GetMockUser("assignedUser") }, default,
                                   default, default, default, default, default, default, default, default,
                                   GetMockRepo(org, repoName), default);
        }

        public static Label GetMockLabel(string name)
        {
            // using reflection to set the properties.

            Label label = new Label();
            typeof(Label).GetProperty("Name").SetValue(label, name);

            return label;
        }

        public static Repository GetMockRepo(string org, string repoName)
        {
            // using reflection to set the properties.

            User user = GetMockUser("userName");

            Repository repo = new Repository();
            typeof(Repository).GetProperty("Name").SetValue(repo, repoName);
            typeof(Repository).GetProperty("Owner").SetValue(repo, user);

            return repo;
        }

        private static User GetMockUser(string name)
        {
            User user = new User();
            typeof(User).GetProperty("Login").SetValue(user, name);
            return user;
        }
    }
}
