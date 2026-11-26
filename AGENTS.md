# Tactical-RPG — Agent Instructions

## 1. Project Overview

Tactical-RPG is a turn-based tactical role-playing game developed with Unity and C#.

The project aims to deliver a playable tactical combat experience while maintaining a readable, maintainable, and extensible codebase. It also serves as a learning project for software engineering and game development.

## 2. Source of Truth

Before implementing any task, consult the relevant project documentation:

- `Docs/GAME_DESIGN.md` — Approved game mechanics and rules.
- `Docs/ARCHITECTURE.md` — Technical architecture and component responsibilities.
- `Docs/DEVELOPMENT_RULES.md` — Development standards and conventions.
- `Docs/Features/` — Feature-specific requirements and acceptance criteria.

Do not invent requirements or modify approved game mechanics without authorization.

If requirements are missing, ambiguous, or contradictory, report the issue before proceeding with the affected implementation.

## 3. Development Workflow

Follow a specification-driven development approach:

1. Read the relevant feature specification.
2. Inspect the existing implementation.
3. Identify affected files and dependencies.
4. Present a concise implementation plan.
5. Implement only the approved task.
6. Perform a static code review and provide manual validation instructions.
7. Report changes, static review findings, and any human-reported test results.
8. Wait for review before proceeding to another task.

Do not automatically proceed to subsequent tasks.

## 4. Implementation Rules

- Prefer simple, readable, and maintainable solutions.
- Reuse existing components whenever appropriate.
- Avoid unnecessary abstractions and premature optimization.
- Maintain clear separation of responsibilities.
- Avoid duplicating game rules across multiple components.
- Preserve existing functionality unless explicitly instructed otherwise.
- Do not perform unrelated refactoring.
- Do not introduce new dependencies without approval.
- Do not implement features outside the approved scope.
- Use English for code identifiers, comments, and technical documentation.

## 5. Unity-Specific Rules

- Respect existing Unity component relationships and serialized references.
- Preserve existing `.meta` files and asset GUIDs.
- Do not manually modify generated Unity directories.
- Do not modify scenes, prefabs, or project settings without authorization.
- Report any required Inspector configuration.
- Use the project's existing input system.
- Avoid hardcoding values that should be configurable.
- Consider Unity lifecycle methods when designing component behavior.

## 6. Testing and Validation

Compilation, execution, and functional validation in Unity are the exclusive responsibility of the human developer.

- Agents must not run builds, tests, application execution, or Unity.
- Agent validation is limited to static code review, including inspection for likely syntax issues, regressions, and consistency with acceptance criteria.
- Provide clear Unity Inspector configuration and manual testing instructions.
- Report edge cases and known limitations.
- Do not claim compilation, execution, or functional tests passed; report human-provided results only.

## 7. Version Control

- Respect the existing Git branching strategy.
- Keep changes focused on the assigned task.
- Do not overwrite or discard unrelated local changes.
- Do not create commits, push changes, merge branches, or delete branches without authorization.
- Provide a suggested commit message when requested.

## 8. Communication and Learning

The project owner is actively learning software engineering and game development.

When reporting implementation results:

- Explain important architectural decisions.
- Identify newly introduced algorithms or design patterns.
- Explain why significant changes were necessary.
- Avoid unnecessary complexity.
- Clearly distinguish implemented functionality from future recommendations.

## 9. Task Completion Report

After completing an assigned task, provide:

- Task ID and completion status.
- Summary of changes.
- Files created or modified.
- Important implementation decisions.
- Static review findings and any human-reported compilation, execution, or functional validation results.
- Required Unity Inspector configuration.
- Manual testing instructions.
- Known limitations, blockers, or deviations from the specification.

Do not proceed to another task until the project owner approves the current implementation.

### Context and Token Efficiency

- Treat project owner confirmations as authoritative.
- Do not independently verify confirmed manual actions unless requested.
- Read only files necessary for the assigned task.
- Avoid redundant repository inspections.
- Update documentation only when explicitly required.
- Keep plans and implementation reports concise.
- Do not expand the scope of simple administrative tasks.