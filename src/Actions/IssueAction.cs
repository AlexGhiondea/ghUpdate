using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class IssueAction : IAction
{
    public OperationTypeEnum Operation { get; set; }
    public AttributeTypeEnum Attribute { get; set; }

    public List<string> AdditionalData { get; set; }

    public override string ToString()
    {
        return $"{Operation} {Attribute} ({string.Join(',', AdditionalData)})";
    }

    public static IssueAction Parse(string configLine)
    {
        string[] entries = configLine.Split(',');

        OperationTypeEnum operation = Enum.Parse<OperationTypeEnum>(entries[0]);
        AttributeTypeEnum attribute = Enum.Parse<AttributeTypeEnum>(entries[1]);
        List<string> additionalData = entries.Skip(2).ToList();

        switch (attribute)
        {
            case AttributeTypeEnum.label:
                {
                    return new LabelAction()
                    {
                        Operation = operation,
                        Attribute = attribute,
                        AdditionalData = additionalData
                    };
                }
            case AttributeTypeEnum.assignee:
                {
                    return new AssigneeAction()
                    {
                        Operation = operation,
                        Attribute = attribute,
                        AdditionalData = additionalData
                    };
                }
        }

        throw new InvalidOperationException("Cannot parse configuration line");
    }

    public abstract void ApplyTo(IssueUpdate issue);
}
