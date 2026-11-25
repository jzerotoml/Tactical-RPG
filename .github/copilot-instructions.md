# GitHub Copilot — Repository Instructions

## Project Context

This repository contains Tactical-RPG, a turn-based tactical role-playing game developed with Unity and C#.

The project follows a specification-driven development methodology.

## Repository Instructions

Read and follow the instructions defined in `/AGENTS.md`.

Consult the relevant documentation under `Docs/` before implementing changes.

Approved feature specifications define the scope, requirements, and acceptance criteria of each development task.

## Agent Mode

When working in Agent Mode:

- Inspect the existing codebase before making changes.
- Identify relevant files and component dependencies.
- Present an implementation plan before editing files.
- Wait for approval unless implementation has already been explicitly authorized.
- Implement only the assigned task.
- Prefer targeted changes over complete file rewrites.
- Preserve existing functionality and serialized Unity references.
- Do not automatically implement subsequent tasks.

## Context Management

- Read only the files relevant to the assigned task.
- Expand the investigation when additional dependencies are identified.
- Avoid unnecessary repository-wide analysis.
- Do not inspect generated directories unless explicitly required.

Exclude the following directories from routine investigation:

- `Library/`
- `Temp/`
- `Logs/`
- `Obj/`
- `UserSettings/`

## Validation

Use available tools to validate implementation changes.

Do not assume Unity compilation or runtime behavior has been verified unless the corresponding tests were executed.

Provide manual Unity testing instructions when necessary.

## Response Format

Keep implementation reports concise and structured.

Include:

- Task ID and status.
- Summary of changes.
- Modified or created files.
- Relevant technical decisions.
- Validation results.
- Manual testing requirements.
- Known limitations or blockers.

Explain significant implementation decisions to support the project owner's learning process.

Do not commit or push changes without explicit authorization.