using Octokit;
using OutputColorizer;
using System;
using System.Collections.Generic;
using System.IO;
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
            InternalMainAsync(args).GetAwaiter().GetResult();
        }
    }

    static async Task InternalMainAsync(string[] args)
    {
        if (!CommandLine.Parser.TryParse(args, out s_cmdLine) &&
            s_cmdLine != null &&
            !IsDataValid(s_cmdLine))
        {
            return;
        }

        s_gitHub = GetGitHubClientWithToken(s_cmdLine.Token);

        string dataFileToUse = s_cmdLine.DataFile;
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

        Colorizer.WriteLine("Reading actions to take on issues from [Yellow!{0}]", s_cmdLine.ActionsFile);
        List<IssueAction> actionsToTake = FileParserHelpers.ParseContent(s_cmdLine.ActionsFile, IssueAction.Parse);
        Colorizer.WriteLine("Found [Yellow!{0}] actions to take.", actionsToTake.Count);

        Colorizer.WriteLine("Reading data from [Yellow!{0}]", dataFileToUse);
        List<ParsedIssue> issues = FileParserHelpers.ParseContent(dataFileToUse, ParsedIssue.Parse);
        Colorizer.WriteLine("Found [Yellow!{0}] issues.", issues.Count);

        Colorizer.WriteLine("Ready to proceed with updating [Cyan!{0}] issues? (y/n)", issues.Count);
        string proceed = Console.ReadLine();
        if (proceed.Trim().ToLowerInvariant() != "y")
        {
            return;
        }

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

                IssueUpdate updatedIssue = ghIssue.ToUpdate();

                // apply the modifications to the issue.
                foreach (IAction action in actionsToTake)
                {
                    action.ApplyTo(updatedIssue);
                }

                await s_gitHub.Issue.Update(issue.Org, issue.Repo, ghIssue.Number, updatedIssue);

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

    private static bool IsDataValid(CommandLineArgs s_cmdLine)
    {
        // if we have specified a token, use that.
        if (s_cmdLine.Token == null)
        {
            Colorizer.WriteLine("[Red!Error] Please specify a GitHub access token!");
            return false;
        }

        if (!File.Exists(s_cmdLine.DataFile))
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
