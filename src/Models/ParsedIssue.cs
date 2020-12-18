using System;

public class ParsedIssue
{
    public string Org { get; set; }
    public string Repo { get; set; }
    public string Type { get; set; }
    public string Id { get; set; }

    public override string ToString()
    {
        return $"https://github.com/{Org}/{Repo}/{Type}/{Id}";
    }

    public static ParsedIssue Parse(string url)
    {
        //https://github.com/Azure/azure-sdk-for-ruby/issues/957
        Uri uri;
        try
        {
            uri = new Uri(url);
        }
        catch (Exception e)
        {
            throw new ArgumentException($"Expected a valid url in {url}", e);
        }

        //get the segments and validate that the last segment is a number.

        int segmentCount = uri.Segments.Length;
        if (segmentCount < 4)
        {
            throw new ArgumentException($"Expected url to contain 'org'/'repo'/issues/'id' in {uri}");
        }

        ParsedIssue i = new ParsedIssue
        {
            Org = StripTrailingSlash(uri.Segments[segmentCount - 4]),
            Repo = StripTrailingSlash(uri.Segments[segmentCount - 3]),
            Type = StripTrailingSlash(uri.Segments[segmentCount - 2]),
            Id = StripTrailingSlash(uri.Segments[segmentCount - 1])
        };

        return i;
    }

    private static string StripTrailingSlash(string input)
    {
        if (!string.IsNullOrEmpty(input) && input[input.Length - 1] == '/' && input.Length > 1)
            return input.Substring(0, input.Length - 1);

        return input;
    }
}