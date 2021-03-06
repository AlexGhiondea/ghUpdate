﻿using ghUpdate.Models;
using Octokit;
using System;

[AppliesTo(AttributeTypeEnum.state)]
public class IssueStateAction : IssueAction, IIssueAttributeAction
{
    public ItemState State => Enum.Parse<ItemState>(AdditionalData[0].Trim(), true);

    public void ApplyTo(IssueUpdate issue)
    {
        switch (Operation)
        {
            case OperationTypeEnum.set:
                issue.State = State;
                break;
            default:
                throw new NotSupportedException($"{nameof(IssueStateAction)} only supports 'set' operation.");
        }
    }
}
