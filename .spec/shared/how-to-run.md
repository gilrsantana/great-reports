# How to Run

This file contains standard instructions applicable to any change specification in the project.

## Execution Rules

1. Read the `Objective` and `Technical Context` of the change spec before starting.
2. Execute tasks in the order they appear within each group.
3. Upon completing a task, check the box and add evidence directly below it.
4. Never remove a task.
5. If a task cannot be executed, register the reason in the evidence.
6. Consider the spec complete only when all checkboxes are checked.
7. When a task specifies a preferred tool or automation (skill, script, generator), use it as the primary implementation.
8. If the specified tool does not cover the entire case, apply it as far as reasonable and log the deviation in the evidence.

## Evidence Format

Always use this pattern:

```markdown
- [x] Task description
>  ✅ YYYY-MM-DD HH:MM - Action taken, decisions made, plan deviations
```

## Purpose

These rules exist to keep execution traceable without turning specifications into heavy documentation.
