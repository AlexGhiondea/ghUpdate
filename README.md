# ghUpdate
Tool to bulk update GitHub issues

Usage:
```
Usage:
 ghUpdate.exe issuesFile actionsFile [-token value]
  - issuesFile  : The file containing the list of issues, one per line (string, required)
  - actionsFile : The file containing the actions to take on each issue in the list of issues, one per line. (string, required)
  - token       : The GitHub authentication token. (string, default=)

When using the Comment action, the following text replacements are available
 > #issue.number#
 > #issue.url#
 > #issue.title#
 > #issue.body#
 > #issue.repository.name#
 > #issue.repository.org#
 > #issue.title.encoded#
 > #issue.body.encoded#
 > #env.newline#
```

# Data file format

The following section contains information about how the input files should look like

## issuesFile

This file contains the list of issues that we want to apply updates to. The structure of the file is very simple: on each line there is a link to the issue that
you want to update. 

Here is an example:
```
https://github.com/alexghiondea/ghUpdate/issues/1
```

You can have as many issues as you want from as many repositories as you want.

## actionsFile

This file describes the set of actions that you want to apply on the issues. Each action is on a single line in the file.

Here is an example:
```
add, label, reopened
remove, asignee, userName
set, state, open
add, comment, We are closing this issue
```

Each line starts with an action to take. Not all actions are supported for every attribute.
The set of actions you can take are:
 - add
 - remove
 - set

The second part of the data represents the attribute of the issue that you are updating.
The set of attributes you can modify are:
 - label
 - asignee
 - state
 - comment

The third part of the line contains any additional data that you need for a specific attribute. When adding/removing labels, the additional information contains the name of the label.

### Comment replacement token
Comments are special because they allow transformations of the data based on the issue attributes. Special tokens are specified in the line that adds the comment and those tokens are replaced with the actual value from the issue when the issue is processed.

These are the tokens that are currently supported:
 > #issue.number#
 > #issue.url#
 > #issue.title#
 > #issue.body#
 > #issue.repository.name#
 > #issue.repository.org#
 > #issue.title.encoded#
 > #issue.body.encoded#
 > #env.newline#

Here is an example:
```
add, comment, Closing issue #issue.number#. You can create a new issue with the same title by going to: https://github.com/#issue.repository.org#/#issue.repository.name#/issues/new?title=#issue.title.encoded#
```

In the example above, the following replacements will happen:
- #issue.number# will be replaced with the issue number for each issue that is being updated
- #issue.repository.org# and #issue.repository.name# will be updated to the repository information for the issue that is being updated
- #issue.title.encoded# will url encode the title of the issue. This is useful when the title needs to be used in a link (for instance when the issues are created based on existing issue )

