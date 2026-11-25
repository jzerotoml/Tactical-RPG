# Tactical-RPG — Development Rules

**Version:** 0.1.0  
**Status:** Active

## 1. Development Methodology

The project follows Specification-Driven Development (SDD) with AI-assisted implementation.

Each feature follows this lifecycle:

1. Current implementation analysis.
2. Requirements definition.
3. Technical design.
4. Task decomposition.
5. Specification approval.
6. Implementation.
7. Code review.
8. Testing and validation.
9. Documentation update.
10. Commit and integration.

Implementation must not begin before the corresponding requirements and technical design are approved.

## 2. Feature Management

Each feature must have a unique identifier.

Examples:

- F04 — Units and Movement
- F04-T01 — Architecture Analysis
- F04-T02 — Unit Attributes

Feature specifications must be stored under:

`Docs/Features/`

Each feature specification should contain:

- Objective.
- Current implementation.
- Functional requirements.
- Technical design.
- Scope.
- Out-of-scope functionality.
- Implementation tasks.
- Acceptance criteria.
- Testing scenarios.
- Known limitations.
- Pending decisions.

GitHub Issues and Projects should be used to track development tasks, bugs, improvements, and future features.

## 3. Git Branching Strategy

The `main` branch represents the stable development baseline.

New features should be developed in dedicated branches.

### Branch Naming

| Type | Convention |
|---|---|
| Feature | `feature/f04-units-and-movement` |
| Bug fix | `fix/movement-validation` |
| Documentation | `docs/update-architecture` |
| Refactoring | `refactor/grid-management` |
| Testing | `test/movement-validation` |

A separate branch is not required for every individual task within a feature.

Feature branches should remain focused on their approved scope.

Changes should be integrated into `main` after successful validation and review.

## 4. Commit Conventions

Use Conventional Commits.

### Format

`type(scope): short description`

### Supported Types

| Type | Purpose |
|---|---|
| `feat` | New functionality |
| `fix` | Bug correction |
| `docs` | Documentation changes |
| `refactor` | Internal code restructuring |
| `test` | Testing changes |
| `chore` | Maintenance and configuration |
| `perf` | Performance improvements |

### Examples

- `feat(units): add unit class and team identities`
- `feat(movement): implement grid pathfinding`
- `fix(cursor): prevent invalid movement selection`
- `docs(architecture): update component responsibilities`
- `docs(project): establish agent development workflow`

Commits must represent coherent, reviewable changes.

Avoid combining unrelated modifications into a single commit.

Commit messages must be written in English.

## 5. C# Coding Conventions

### 5.1 Naming

| Element | Convention | Example |
|---|---|---|
| Classes | PascalCase | `GridManager` |
| Enums | PascalCase | `UnitClass` |
| Enum values | PascalCase | `Knight` |
| Methods | PascalCase | `MoveUnit()` |
| Properties | PascalCase | `IsMoving` |
| Private fields | camelCase | `movementSpeed` |
| Local variables | camelCase | `targetPosition` |
| Parameters | camelCase | `targetX` |
| Constants | PascalCase | `MaxTeamSize` |
| Interfaces | PascalCase with I prefix | `IMovable` |

Existing naming conventions should be preserved unless a change is necessary.

Do not rename existing serialized fields without considering Unity serialization compatibility.

### 5.2 Formatting

- Use four spaces for indentation.
- Use consistent brace placement.
- Place opening braces on a new line.
- Use descriptive identifiers.
- Avoid unnecessary abbreviations.
- Remove unused imports and variables when relevant to the assigned task.
- Keep formatting consistent within each file.

### 5.3 Encapsulation

- Prefer private fields when external access is unnecessary.
- Use `[SerializeField]` for private fields that require Inspector configuration.
- Expose public methods and properties only when required.
- Avoid unrestricted modification of gameplay state.
- Protect internal data structures from unintended external changes.

### 5.4 Comments

- Write comments in English.
- Explain intent, constraints, and non-obvious decisions.
- Avoid comments that merely repeat the code.
- Update comments when implementation behavior changes.
- Use TODO comments only for clearly identified pending work.

## 6. Architecture Principles

- Follow Unity's component-based architecture.
- Give components clear responsibilities.
- Prefer composition over unnecessary inheritance.
- Centralize gameplay rules and validation.
- Avoid duplicating logic across components.
- Separate logical gameplay state from visual behavior.
- Prefer readable implementations over unnecessarily complex solutions.
- Avoid premature optimization.
- Introduce design patterns only when they solve an identified problem.
- Avoid creating managers or abstractions without a defined responsibility.

Architectural changes must be justified by project requirements.

## 7. Unity Development Rules

- Preserve existing serialized references.
- Preserve asset GUIDs and `.meta` files.
- Include relevant `.meta` files when adding Unity assets.
- Use prefabs for reusable GameObjects.
- Expose configurable gameplay values through appropriate Unity mechanisms.
- Avoid hardcoding values that should be configurable.
- Respect Unity lifecycle methods.
- Use the existing Unity Input System.
- Do not introduce additional packages without approval.
- Document required Inspector configuration.
- Do not commit generated Unity directories.

Changes to scenes, prefabs, or project settings require explicit authorization.

## 8. Testing Strategy

Testing must be performed incrementally.

### 8.1 Technical Validation

When available:

- Check compilation.
- Execute automated tests.
- Validate component dependencies.
- Review modified code.
- Identify potential regressions.

### 8.2 Functional Testing

Verify gameplay behavior against the approved requirements.

Manual testing scenarios should include:

- Initial conditions.
- Player actions.
- Expected results.
- Actual results.
- Pass or fail status.

### 8.3 Edge Cases

Consider relevant edge cases, including:

- Battlefield boundaries.
- Invalid coordinates.
- Occupied tiles.
- Missing component references.
- Invalid player actions.
- Unexpected gameplay states.

### 8.4 Regression Testing

Existing functionality must remain operational after implementing new features.

A feature must not be considered fully validated until its required Unity tests have been completed.

## 9. Code Review

Every significant implementation must be reviewed before integration.

Review criteria:

- Requirement compliance.
- Correctness.
- Readability.
- Maintainability.
- Architectural consistency.
- Unnecessary complexity.
- Potential regressions.
- Unity compatibility.
- Test coverage.

AI-generated code is subject to the same review requirements as manually written code.

The project owner retains final approval over implementation decisions.

## 10. Documentation Standards

Technical documentation must be written in English.

Documentation should be updated when relevant functionality or architectural decisions change.

### Documentation Responsibilities

| Document | Responsibility |
|---|---|
| `README.md` | Project overview and setup |
| `AGENTS.md` | General agent instructions |
| `.github/copilot-instructions.md` | Copilot-specific instructions |
| `GAME_DESIGN.md` | Game mechanics and design decisions |
| `ARCHITECTURE.md` | Implemented software architecture |
| `Docs/DEVELOPMENT_RULES.md` | Development conventions |
| `Docs/Features/` | Feature specifications |

Clearly distinguish between:

- Implemented functionality.
- Approved requirements.
- Planned functionality.
- Experimental behavior.
- Known limitations.

Avoid duplicating information unnecessarily across documents.

## 11. AI-Assisted Development

AI agents are implementation assistants, not autonomous project owners.

Agents must:

- Follow approved specifications.
- Inspect existing implementations.
- Respect task boundaries.
- Explain significant technical decisions.
- Report validation results accurately.
- Identify blockers and specification conflicts.
- Wait for approval before expanding the scope.

Agents must not independently change game mechanics, introduce dependencies, or integrate unapproved changes.

## 12. Definition of Done

A development task is complete when:

- All assigned requirements are implemented.
- Acceptance criteria are satisfied.
- Relevant technical validation is completed.
- Required functional tests are completed.
- Existing functionality remains operational.
- Relevant documentation is updated.
- Known limitations are documented.
- The implementation has been reviewed.
- The project owner has approved the result.

A feature is complete when all required tasks satisfy the Definition of Done and the feature has been successfully integrated into the stable development baseline.

## 13. Continuous Improvement

Development rules may evolve as the project grows.

New conventions should address demonstrated needs rather than hypothetical complexity.

Changes to these rules require project owner approval.