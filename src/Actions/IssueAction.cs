using ghUpdate.Models;
using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

public abstract class IssueAction
{
    public OperationTypeEnum Operation { get; set; }
    public AttributeTypeEnum Attribute { get; set; }
    public List<string> AdditionalData { get; set; }

    // Create mapping of actions to labels.
    private static readonly Dictionary<AttributeTypeEnum, Type> s_mapOfActions = InitializeActionMap();

    public override string ToString()
    {
        return $"{Operation} {Attribute} ({string.Join(',', AdditionalData).Trim()})";
    }

    public static IssueAction Parse(string configLine)
    {
        string[] entries = configLine.Split(',');

        OperationTypeEnum operation = Enum.Parse<OperationTypeEnum>(entries[0]);
        AttributeTypeEnum attribute = Enum.Parse<AttributeTypeEnum>(entries[1]);
        List<string> additionalData = entries[2..].ToList();

        // this can throw, and that's ok!
        Type typeToCreate = s_mapOfActions[attribute];

        // create the object and assign the properties
        var instantiatedType = (IssueAction)Activator.CreateInstance(typeToCreate);
        instantiatedType.Operation = operation;
        instantiatedType.Attribute = attribute;
        instantiatedType.AdditionalData = additionalData;

        return instantiatedType;
    }

    public static IssueAction Create(OperationTypeEnum operation, AttributeTypeEnum attribute, params string[] additionalData)
    {
        // this can throw, and that's ok!
        Type typeToCreate = s_mapOfActions[attribute];

        // create the object and assign the properties
        var instantiatedType = (IssueAction)Activator.CreateInstance(typeToCreate);
        instantiatedType.Operation = operation;
        instantiatedType.Attribute = attribute;
        instantiatedType.AdditionalData = new List<string>(additionalData);

        return instantiatedType;
    }

    private static Dictionary<AttributeTypeEnum, Type> InitializeActionMap()
    {
        Dictionary<AttributeTypeEnum, Type> mapOfActions = new Dictionary<AttributeTypeEnum, Type>();
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.BaseType == typeof(IssueAction))
            {
                // get the custom attribute
                AppliesToAttribute customAttribute = type.GetCustomAttribute<AppliesToAttribute>();
                if (customAttribute == null)
                {
                    throw new ArgumentException($"The type {type.Name} should have an {nameof(AppliesToAttribute)} attribute");
                }

                if (mapOfActions.ContainsKey(customAttribute.AppliesTo))
                {
                    throw new ArgumentException($"The key {customAttribute.AppliesTo} is duplicated!");
                }

                mapOfActions[customAttribute.AppliesTo] = type;
            }
        }

        return mapOfActions;
    }
}
