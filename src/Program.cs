using ghUpdate.Helpers;
using Octokit;
using OutputColorizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    private static GitHubClient s_gitHub;
    private static CommandLineArgs s_cmdLine;
    static void Main(string[] args)
    {
        using (ColorizerFileLog s_writer = new ColorizerFileLog("fileLog.txt"))
        {
            Colorizer.SetupWriter(s_writer);

            if (!CommandLine.Parser.TryParse(args, out s_cmdLine))
            {
                Colorizer.WriteLine("When using the Comment action, the following text replacements are available");

                // also print the encoders that are supported by the Comment.
                foreach (var item in CommentAction.s_tokenMap.Keys)
                {
                    Colorizer.WriteLine(" > " + item);
                }

                return;
            }

            if (s_cmdLine == null || !IsDataValid(s_cmdLine))
            {
                return;
            }

            InternalMainAsync(args).GetAwaiter().GetResult();
        }
    }

    static async Task InternalMainAsync(string[] args)
    {
        s_gitHub = GetGitHubClientWithToken(s_cmdLine.Token);

        string dataFileToUse = s_cmdLine.IssuesFile;
        // Does it have a 'progress.dat' file?

        if (File.Exists("progress.dat"))
        {
            Colorizer.WriteLine("Found existing progress file. Use that (y/n)?");
            string input = Console.ReadLine();
            if (input.Trim().ToLowerInvariant() == "y")
            {
                dataFileToUse = "progress.dat";
            }
        }

        List<IssueAction> actionsToTake = ReadActionsFromFile();

        List<ParsedIssue> issues = ReadIssuesFromFile(dataFileToUse);

        // Colorizer.WriteLine("Ready to proceed with updating [Cyan!{0}] issues? (y/n)", issues.Count);
        // string proceed = Console.ReadLine();
        // if (proceed.Trim().ToLowerInvariant() != "y")
        // {
        //     return;
        // }

        while (issues.Count > 0)
        {
            ParsedIssue issue = null;
            try
            {
                // the current issue is the issue at position 0.
                issue = issues[0];

                Colorizer.Write("Updating [Yellow!{0}]...", issue);
                var ghIssue = await s_gitHub.Issue.Get(issue.Org, issue.Repo, int.Parse(issue.Id));
                await Task.Delay(1000);

                // there will be 2 modes of operation;
                // 1. We update an existing issue
                // 2. We clone an existing issue


                if (actionsToTake.OfType<ICloneIssueAction>().Any())
                {
                    // We are cloning an issue. 
                    // TODO: validate that we only have clone actions in this case!
                    foreach (ICloneIssueAction action in actionsToTake.OfType<ICloneIssueAction>())
                    {
                        NewIssue ni = new NewIssue(ghIssue.Title);
                        // apply the CLONE operation, if it exists.
                        ni.Body = ghIssue.Body;

                        foreach (User assignee in ghIssue.Assignees)
                        {
                            ni.Assignees.Add(assignee.Login);
                        }
                        foreach (Label label in ghIssue.Labels)
                        {
                            ni.Labels.Add(label.Name);
                        }
                        // do not clone the milestone as they rely on a milestone number which will be different across the repos

                        string newRepo = action.GetNewRepo();
                        string newOrg = action.GetNewOrg();
                        Issue createIssue = await s_gitHub.Issue.Create(newOrg,newRepo, ni);
                        await Task.Delay(500);

                        // clone the comments
                        foreach(IssueComment comment in await s_gitHub.Issue.Comment.GetAllForIssue(issue.Org, issue.Repo, ghIssue.Number))
                        {
                            string commentText = $"Created by: {comment.User.Login}{Environment.NewLine}{comment.Body}";
                            await s_gitHub.Issue.Comment.Create(newOrg,newRepo, createIssue.Number, commentText);
                            await Task.Delay(500);
                        }
                    }

                }
                else{
                    // ensure there is a repository specified for the issue.
                    Repository ghRepository = await s_gitHub.Repository.Get(issue.Org, issue.Repo);
                    await Task.Delay(1000);

                    // we need to set the repository on the issue
                    ghIssue = ghIssue.WithRepository(ghRepository);

                    // apply comments to the issue
                    foreach (ICommentAction action in actionsToTake.OfType<ICommentAction>())
                    {
                        string commentData = action.GetComment(ghIssue);
                        await s_gitHub.Issue.Comment.Create(issue.Org, issue.Repo, int.Parse(issue.Id), commentData);
                        await Task.Delay(1000);
                    }
                
                    // apply the modifications to the issue
                    IssueUpdate updatedIssue = ghIssue.ToUpdate();

                    // this will filter to just the issues that change the attributes of an issue
                    foreach (IIssueAttributeAction action in actionsToTake.OfType<IIssueAttributeAction>())
                    {
                        action.ApplyTo(updatedIssue);
                    }

                    await s_gitHub.Issue.Update(issue.Org, issue.Repo, ghIssue.Number, updatedIssue);
                    await Task.Delay(1000);
                }

                Colorizer.WriteLine("[Green!done].");
            }
            catch
            {
                // if we end up hitting the rate limit, do something here.
                Colorizer.WriteLine("[Red!failed].");

                if (issue != null)
                {
                    File.AppendAllText("error.log", $"{issue}{Environment.NewLine}");
                }
            }
            finally
            {
                // Let's write the remaining entries to the progress.dat file.
                issues.Remove(issue);
                WriteProgressFile(issues);

                Colorizer.WriteLine("Remaining issues [Cyan!{0}]", issues.Count);

                // Wait 1 seconds before moving to the next one.
                await Task.Delay(1000);
            }
        }

        //remove the progress.dat file.
        Colorizer.WriteLine("Removing the progress.dat file");
        File.Delete("progress.dat");
    }

    private static List<ParsedIssue> ReadIssuesFromFile(string dataFileToUse)
    {
        Colorizer.WriteLine("Reading data from [Yellow!{0}]", dataFileToUse);
        List<ParsedIssue> issues = FileParserHelpers.ParseContent(dataFileToUse, ParsedIssue.Parse);
        Colorizer.WriteLine("Found [Yellow!{0}] issues.", issues.Count);
        return issues;
    }

    private static List<IssueAction> ReadActionsFromFile()
    {
        Colorizer.WriteLine("Reading actions to take on issues from [Yellow!{0}]", s_cmdLine.ActionsFile);
        List<IssueAction> actionsToTake = FileParserHelpers.ParseContent(s_cmdLine.ActionsFile, IssueAction.Parse);
        Colorizer.WriteLine("Found [Yellow!{0}] actions to take.", actionsToTake.Count);
        foreach (var action in actionsToTake)
        {
            Console.WriteLine(" > " + action.ToString());
        }

        return actionsToTake;
    }

    private static bool IsDataValid(CommandLineArgs s_cmdLine)
    {
        // if we have specified a token, use that.
        if (s_cmdLine.Token == null)
        {
            Colorizer.WriteLine("[Red!Error] Please specify a GitHub access token!");
            return false;
        }

        if (!File.Exists(s_cmdLine.IssuesFile))
        {
            Colorizer.WriteLine("[Red!Error] Please specify an existing data file!");
            return false;
        }

        if (!File.Exists(s_cmdLine.ActionsFile))
        {
            Colorizer.WriteLine("[Red!Error] Please specify an existing actions file!");
            return false;
        }

        return true;
    }

    private static void WriteProgressFile(List<ParsedIssue> issues)
    {
        StringBuilder sb = new StringBuilder();
        foreach (ParsedIssue issue in issues)
        {
            string line = issue.ToString();
            sb.AppendLine(line);
        }

        File.WriteAllText("progress.dat", sb.ToString());
    }

    public static GitHubClient GetGitHubClientWithToken(string token)
    {
        GitHubClient ghClient = new GitHubClient(new ProductHeaderValue("GitHubSync"));
        ghClient.Credentials = new Credentials(token);
        return ghClient;
    }
}
