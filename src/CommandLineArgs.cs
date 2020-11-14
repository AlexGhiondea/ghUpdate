using CommandLine.Attributes;

public class CommandLineArgs
{
    [OptionalArgument(null, "token", "The GitHub authentication token.")]
    public string Token { get; set; }

    [RequiredArgument(0, "issuesFile", "The file containing the list of issues, one per line")]
    public string IssuesFile { get; set; }

    [RequiredArgument(1, "actionsFile", "The file containing the actions to take on each issue in the list of issues, one per line.")]
    public string ActionsFile { get; set; }
}