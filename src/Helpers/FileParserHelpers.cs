using System;
using System.Collections.Generic;
using System.IO;

public static class FileParserHelpers
{
    public static List<TContent> ParseContent<TContent>(string file, Func<string, TContent> parseFunction)
    {
        List<TContent> parsedContent = new List<TContent>();
        using (StreamReader sr = new StreamReader(file))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                parsedContent.Add(parseFunction(line));
            }
        }
        return parsedContent;
    }
}
