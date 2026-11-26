# F04 — Units and Movement

**Version:** 1.0.0  
**Status:** Approved  
**Feature ID:** F04  
**Branch:** `feature/f04-units-and-movement`  
**Dependencies:** Existing battlefield, cursor, action menu, and prototype movement system.

## 1. Objective

Replace the single-unit movement prototype with a reusable, data-driven, multi-unit architecture supporting team identities, configurable unit statistics, cost-aware pathfinding, movement visualization, and tile-by-tile movement animation.

This feature establishes the foundation for the deployment, combat, turn management, and battle AI systems.

## 2. Reference Documents

Before implementation, review:

- `AGENTS.md`
- `.github/copilot-instructions.md`
- `Docs/GAME_DESIGN.md`
- `Docs/ARCHITECTURE.md`
- `Docs/DEVELOPMENT_RULES.md`

Inspect the existing implementation before modifying any files.

Report inconsistencies between the specification, reference documents, and existing code before proceeding.

## 3. Current Implementation

The existing prototype includes:

- A configurable battlefield, currently configured as 16×16.
- A single unit spawned at a hardcoded position.
- A two-dimensional unit occupancy matrix.
- Keyboard-based cursor navigation.
- An action menu containing Move, Attack, and Wait.
- UnitClass and UnitData definitions with Knight, Wizard, and Archer base-stat assets.
- Per-unit current health initialized from UnitData.
- The prototype unit prefab is configured with KnightData.
- Manhattan-distance movement validation.
- Blue movement range overlays.
- Direct-line movement animation.
- Input locking during movement animation.
Current limitations include:

- Team identity is not implemented.
- Combat execution and health changes are not implemented.
- No multi-unit spawning.
- No obstacle-aware pathfinding.
- Duplicated movement validation.
- Restricted cursor navigation during movement selection.
- Logical position updates before animation completion.

## 4. Feature Scope

### 4.1 Included

- Unit class definitions.
- Team identities.
- ScriptableObject-based unit statistics.
- Individual runtime unit state.
- Three reusable unit prefabs.
- Team-based visual identification.
- Configurable temporary unit spawning.
- Support for twelve simultaneous units.
- Multi-unit occupancy management.
- Dijkstra pathfinding.
- Extensible movement cost calculation.
- Movement range visualization.
- Unrestricted cursor navigation.
- Movement path preview.
- Tile-by-tile movement animation.
- Movement completion and occupancy synchronization.
- Technical and functional validation.

### 4.2 Excluded

- Deployment scene.
- Player-controlled army composition interface.
- Blind deployment implementation.
- AI formation selection.
- Combat execution.
- Damage calculation.
- Health reduction and unit elimination.
- Turn management.
- Initiative calculation.
- Action availability restrictions.
- Battle AI.
- Special terrain implementation.
- Victory and defeat conditions.
- Multiplayer.
- Camera observation mode.

Excluded functionality must not be implemented as part of this feature.

## 5. Functional Requirements

### FR-01 — Unit Classes

The system must support three unit classes:

- Knight.
- Wizard.
- Archer.

Unit classes must be represented using an enumeration named `UnitClass`.

Units must share a common implementation rather than requiring separate behavior classes for each unit type.

### FR-02 — Team Identity

The system must support two teams:

- Blue.
- Red.

Team identity must be represented using an enumeration named `Team`.

Multiple units of the same class may belong to the same team.

No class composition restrictions shall be enforced.

### FR-03 — Unit Statistics

Base unit statistics must use ScriptableObjects.

The initial statistics are:

| Attribute | Knight | Wizard | Archer |
|---|---:|---:|---:|
| Max Health | 120 | 70 | 80 |
| Attack | 30 | 45 | 35 |
| Armor (%) | 20 | 5 | 10 |
| Movement Range | 3 | 4 | 4 |
| Min Attack Range | 1 | 1 | 2 |
| Max Attack Range | 1 | 3 | 5 |
| Speed | 3 | 5 | 7 |

These values are provisional and subject to future balance adjustments.

Speed represents future turn initiative and must remain independent of visual movement speed.

### FR-04 — Individual Unit State

Each unit must maintain independent runtime state, including:

- Team.
- Current health.
- Grid coordinates.
- Visual movement speed.
- Movement animation state.

Current health must be initialized from the corresponding maximum health value.

Shared ScriptableObject assets must not store or receive individual runtime state.

Turn-specific state is deferred to the turn management feature.

### FR-05 — Unit Prefabs

The system must support three reusable prefabs:

- Knight.
- Wizard.
- Archer.

Initial visual representations:

| Class | Shape |
|---|---|
| Knight | Square-based geometry |
| Wizard | Circular geometry |
| Archer | Triangular geometry |

Team identity must determine unit color.

Blue and Red variants must reuse the same class prefab.

Each prefab must reference its corresponding UnitData asset.

The initial visual movement speed must be 5 Unity world units per second.

### FR-06 — Temporary Unit Spawning

Until the deployment system is implemented, the battlefield must support configurable test formations.

The spawning system must:

- Support multiple units.
- Support repeated unit classes.
- Support both teams.
- Support six units per team.
- Accept configurable starting positions.
- Validate battlefield boundaries.
- Prevent overlapping unit placement.
- Validate required prefab components and data references.
- Report invalid configurations.

Temporary formations must not introduce permanent class composition restrictions.

The future deployment system will replace temporary configuration as the primary source of starting formations.

### FR-07 — Tile Occupancy

GridManager must remain responsible for battlefield occupancy.

Each tile may contain at most one logically registered unit.

The occupancy matrix must remain consistent with unit grid coordinates.

Invalid coordinates must not cause array indexing exceptions.

### FR-08 — Movement Rules

Movement must follow orthogonal grid directions.

Diagonal movement is not permitted.

| Tile Type | Traversable | Valid Destination |
|---|---|---|
| Empty | Yes | Yes |
| Allied unit | Yes | No |
| Enemy unit | No | No |
| Impassable obstacle | No | No |
| Outside battlefield | No | No |

The unit's current tile must not be treated as a valid movement destination.

Impassable terrain support is an architectural requirement, but terrain objects and terrain gameplay are outside this feature's scope.

### FR-09 — Pathfinding

Movement calculations must use Dijkstra's algorithm.

The algorithm must:

- Calculate minimum accumulated movement costs.
- Respect the unit's movement budget.
- Respect battlefield boundaries.
- Respect traversal restrictions.
- Identify all reachable destinations.
- Record sufficient information to reconstruct valid paths.
- Support variable movement costs.
- Avoid unnecessary recalculation during cursor navigation.

All traversable tiles must initially have a movement cost of 1.

### FR-10 — Movement Range Visualization

Movement overlays must display valid destinations calculated by the pathfinding system.

Valid destinations must use the existing blue semitransparent overlay.

Movement range overlays must not appear on:

- Allied units.
- Enemy units.
- Impassable obstacles.
- Unreachable tiles.
- The selected unit's current tile.

Traversable allied tiles may be included in a valid path without displaying a blue overlay.

### FR-11 — Cursor Navigation

During movement selection, the cursor must navigate freely within battlefield boundaries.

Cursor navigation must not be restricted by:

- Movement range.
- Allied units.
- Enemy units.
- Impassable obstacles.

Movement confirmation must remain restricted to valid destinations.

The cursor must use a white semitransparent tile indicator.

Invalid destinations must not introduce a red cursor or red movement overlay.

Red is reserved for future attack visualization.

### FR-12 — Movement Path Preview

The system must display the expected movement route toward the current cursor position when that position is a valid destination.

The preview must:

- Follow the calculated orthogonal path.
- Update when the cursor changes position.
- Disappear when the cursor selects an invalid destination.
- Disappear when movement is cancelled or confirmed.
- Remain consistent with the path used during execution.

The route must be represented using a readable directional arrow or connected directional indicators.

The preview color must be configurable.

White and green must be evaluated during manual testing before selecting the final default color.

### FR-13 — Movement Animation

Units must follow the approved path one tile at a time.

The existing Vector3.MoveTowards approach may be reused.

Units must not move directly across diagonal paths or through blocked enemy tiles.

The cursor must remain locked throughout movement execution.

Movement animation must preserve the configured visual elevation of the unit.

### FR-14 — Movement Completion

Logical grid coordinates and occupancy must be updated after the movement animation finishes.

During movement:

- The original tile remains logically occupied.
- The destination is reserved.
- Additional movement requests are rejected.
- Player actions remain locked.

After successful movement:

- The original tile is released.
- Logical unit coordinates are updated.
- The destination is registered as occupied.
- The destination reservation is released.
- Movement state is cleared.
- Player interaction resumes.
- The action menu reopens.

Movement completion must not leave duplicate occupancy entries.

### FR-15 — Movement Cancellation

Cancelling movement selection before confirmation must:

- Remove movement range overlays.
- Remove the path preview.
- Return the cursor to the selected unit.
- Reopen the action menu.
- Preserve the original unit position and occupancy.

Movement cancellation during animation is not supported.

### FR-16 — Development Exception

Consecutive movement actions must remain available for testing.

Units from either team may be selected and moved during this development stage.

These exceptions will be removed when the turn management system is implemented.

## 6. Technical Design

### 6.1 Component Responsibilities

| Component | Responsibility |
|---|---|
| UnitData | Shared base statistics |
| UnitClass | Unit class enumeration |
| Team | Team enumeration |
| Unit | Individual state and visual movement |
| GridManager | Battlefield, spawning, occupancy, and movement authorization |
| GridPathfinder | Cost-aware pathfinding and route reconstruction |
| GridCursor | Input, navigation, selection, and interaction state |
| ActionMenu | Action selection |

New components must follow the existing repository organization.

Do not restructure unrelated directories.

### 6.2 UnitData

UnitData must inherit from ScriptableObject.

It must expose configurable base statistics through the Unity Inspector.

Expected data fields:

- Unit class.
- Maximum health.
- Attack.
- Armor.
- Movement range.
- Minimum attack range.
- Maximum attack range.
- Initiative speed.

Runtime systems must treat these values as shared configuration rather than individual mutable state.

Create three corresponding assets:

- KnightData.
- WizardData.
- ArcherData.

### 6.3 Unit

Unit must remain a MonoBehaviour.

Its responsibilities include:

- Referencing UnitData.
- Maintaining individual runtime state.
- Maintaining team identity.
- Maintaining logical grid coordinates.
- Executing an authorized movement path.
- Reporting movement progress and completion.

Unit must not independently authorize battlefield movement.

Movement range must be obtained from UnitData rather than maintained as a separate conflicting value.

### 6.4 GridManager

GridManager must remain responsible for:

- Battlefield generation.
- Unit spawning and registration.
- Tile occupancy.
- Battlefield boundary validation.
- Movement authorization.
- Movement cost access.
- Movement range visualization.
- Movement path preview coordination.
- Movement completion synchronization.

Existing functionality should be reused where appropriate.

Additional abstractions require a demonstrated need.

### 6.5 GridPathfinder

GridPathfinder must be a conventional C# class.

It must not require:

- A GameObject.
- MonoBehaviour inheritance.
- Unity lifecycle methods.
- Per-frame execution.

The algorithm must operate on grid coordinates and supplied battlefield traversal information.

Recommended data structures:

- A priority queue or equivalent minimum-cost frontier.
- A distance collection.
- A predecessor collection.

The implementation must verify compatibility with the project's Unity and C# runtime.

If the standard PriorityQueue type is unavailable, propose a compatible alternative before implementation.

External dependencies must not be introduced without approval.

### 6.6 Movement Cost Abstraction

Movement cost calculation must be centralized.

Conceptual interface:

`GetMovementCost(Unit unit, Vector2Int position)`

The initial implementation must return 1 for traversable tiles.

Future implementations may consider:

- Terrain type.
- Unit class.
- Movement modifiers.
- Environmental effects.

Movement costs must be positive.

Impassable tiles must be handled through traversal restrictions rather than arbitrary movement cost values.

### 6.7 Pathfinding Results

Pathfinding results must provide:

- Origin coordinates.
- Minimum accumulated movement costs.
- Reachable destinations.
- Path reconstruction information.

A movement search must be associated with the selected unit and current battlefield state.

Results may be reused while the cursor navigates.

Results must be invalidated when relevant occupancy, movement costs, unit selection, or movement budget changes.

Movement confirmation must not rely on stale pathfinding results.

### 6.8 Movement Execution

The movement system must distinguish between:

- Logical grid position.
- Visual world position.
- Authorized destination.
- Active movement path.
- Movement completion.

GridManager must authorize the movement before Unit begins animation.

Unit must report successful completion to GridManager through an appropriate completion mechanism.

The exact implementation mechanism must be included in the agent's technical plan.

### 6.9 Interaction State

The existing CursorState architecture must be preserved where possible.

Expected movement flow:

```text
Exploration
    |
    v
ActionMenu
    |
    v
Movement
    |
    v
Locked
    |
    v
ActionMenu
```

Cancelling movement selection returns directly to ActionMenu.

Attack and Wait must remain placeholders until their corresponding features are implemented.

## 7. Implementation Tasks

### F04-T01 — Current Architecture Analysis

**Status:** Completed.

Review the existing gameplay scripts and identify limitations, dependencies, and required architectural changes.

### F04-T02 — Unit Statistics and Technical Design

**Status:** Completed.

Define initial unit statistics, movement rules, class identities, and the approved technical architecture.

### F04-T03 — Data-Driven Unit Classes

**Status:** Completed.

**Dependencies:** F04-T02.

Independent runtime health validation across multiple spawned units is deferred to F04-T05.

Implement:

- UnitClass enumeration.
- UnitData ScriptableObject.
- Base statistics configuration.
- UnitData integration into Unit.
- Independent current health initialization.

Acceptance:

- Three unit classes can be configured.
- Shared statistics do not contain individual runtime state.
- Existing movement remains functional.

### F04-T04 — Team Identity and Unit Prefabs

**Status:** Completed.

**Dependencies:** F04-T03.

Implement:

- Team enumeration.
- Individual team assignment.
- Team-based visual identification.
- Support for three reusable class prefabs.

Acceptance:

- Both teams can use the same class prefab.
- Unit class and team are independently identifiable.
- Team colors are correctly assigned.

Required Unity asset creation or prefab configuration must be reported for manual setup or explicitly authorized before modification.

### F04-T05 — Multi-Unit Spawning

**Status:** Completed.

**Dependencies:** F04-T04.

Implement:

- Configurable temporary spawn definitions.
- Multiple unit instantiation.
- Support for repeated unit classes.
- Spawn validation.
- Registration of spawned units.

Acceptance:

- Twelve units can be spawned simultaneously.
- Six units can belong to each team.
- Repeated classes are supported.
- Invalid spawn configurations are rejected safely.

**Manual verification reported by the project owner:**

- Twelve units spawned on the 16×16 battlefield with configurable teams and positions.
- The formation used two Knights, two Wizards, and two Archers per team.
- Both team outlines displayed correctly.
- Reachable movement overlays and unrestricted cursor navigation behaved as expected.
- Confirming an invalid movement destination returned to the ActionMenu.
- Movement was verified for Knight (3), Wizard (2), and Archer (2).
- No compilation or console errors were observed.

### F04-T06 — Occupancy and Movement Authorization

**Status:** Pending.

**Dependencies:** F04-T05.

Implement:

- Centralized occupancy validation.
- Team-aware traversal rules.
- Destination validation.
- Movement reservation support.
- Movement cost abstraction.

Acceptance:

- Allied tiles are traversable but cannot be destinations.
- Enemy tiles are not traversable.
- Invalid coordinates are handled safely.
- Occupancy rules are centralized.

### F04-T07 — Dijkstra Pathfinding

**Status:** Pending.

**Dependencies:** F04-T06.

Implement:

- GridPathfinder.
- Minimum-cost frontier processing.
- Movement budget enforcement.
- Reachable destination calculation.
- Path reconstruction.
- Variable movement cost support.

Acceptance:

- Minimum-cost routes are calculated correctly.
- Enemy units block traversal.
- Allied units permit traversal.
- Movement budgets are respected.
- Variable movement costs can be tested without implementing terrain gameplay.

### F04-T08 — Movement Visualization and Cursor Integration

**Status:** Pending.

**Dependencies:** F04-T07.

Implement:

- Pathfinding-based movement overlays.
- Unrestricted cursor navigation.
- White cursor indicator.
- Directional path preview.
- Invalid destination handling.
- Overlay and preview cleanup.

Acceptance:

- Blue overlays match valid destinations.
- Occupied tiles do not display movement range overlays.
- The cursor can navigate freely.
- Path previews match calculated routes.
- Invalid destinations cannot be confirmed.

### F04-T09 — Path-Based Movement Execution

**Status:** Pending.

**Dependencies:** F04-T08.

Implement:

- Tile-by-tile movement animation.
- Movement input locking.
- Destination reservation.
- Completion notification.
- Logical position synchronization.
- Final occupancy updates.
- Action menu restoration.

Acceptance:

- Units follow the approved route.
- Movement cannot be interrupted by player input.
- Occupancy updates after animation completion.
- The action menu reopens correctly.
- Consecutive movement remains available for testing.

### F04-T10 — Integration, Testing, and Documentation

**Status:** Pending.

**Dependencies:** F04-T03 through F04-T09.

Perform:

- Technical validation.
- Functional testing.
- Regression testing.
- Code review.
- Architecture documentation updates.
- Feature status updates.
- Known limitation reporting.
- Preparation for commit and integration.

Commit and integration require explicit project owner approval.

## 8. Acceptance Criteria

| ID | Scenario | Expected Result |
|---|---|---|
| AC-01 | Initialize Knight | Correct base statistics |
| AC-02 | Initialize Wizard | Correct base statistics |
| AC-03 | Initialize Archer | Correct base statistics |
| AC-04 | Create repeated classes | Independent runtime state |
| AC-05 | Modify base statistics | New instances use updated configuration |
| AC-06 | Spawn twelve units | All valid units registered |
| AC-07 | Spawn overlapping units | Invalid spawn rejected |
| AC-08 | Spawn outside battlefield | Invalid spawn rejected |
| AC-09 | Traverse allied tile | Valid route |
| AC-10 | Finish on allied tile | Movement rejected |
| AC-11 | Traverse enemy tile | Route rejected |
| AC-12 | Route around enemy | Valid when within budget |
| AC-13 | Exceed movement budget | Destination rejected |
| AC-14 | Test variable movement costs | Minimum-cost route selected |
| AC-15 | Navigate outside movement range | Cursor remains unrestricted |
| AC-16 | Navigate over occupied tile | Cursor remains unrestricted |
| AC-17 | Display reachable destinations | Blue overlays match valid destinations |
| AC-18 | Inspect occupied tiles | No blue movement overlay |
| AC-19 | Select valid destination | Directional path preview displayed |
| AC-20 | Select invalid destination | Path preview removed |
| AC-21 | Cancel movement | Original position preserved |
| AC-22 | Confirm movement | Unit follows approved path |
| AC-23 | Provide input during movement | Input does not interrupt movement |
| AC-24 | Inspect occupancy during movement | Origin occupied and destination reserved |
| AC-25 | Complete movement | Occupancy and coordinates synchronized |
| AC-26 | Complete movement | Action menu reopens |
| AC-27 | Move consecutively | Allowed during development |
| AC-28 | Navigate battlefield boundaries | No indexing errors |
| AC-29 | Confirm current unit tile | No movement initiated |
| AC-30 | Compare preview and execution | Identical approved route |

## 9. Testing Strategy

### 9.1 Pathfinding Tests

Validate the algorithm independently where practical.

Required scenarios:

- Empty battlefield.
- Allied traversal.
- Enemy blocking.
- Alternative routes.
- Unreachable destinations.
- Battlefield boundaries.
- Insufficient movement budget.
- Variable movement costs.
- Multiple routes with equal cost.
- Origin excluded from valid destinations.

### 9.2 Unity Functional Tests

Manual testing must verify:

- Unit appearance.
- Team colors.
- Inspector configuration.
- Unit spawning.
- Cursor navigation.
- Movement overlays.
- Path preview readability.
- Tile-by-tile animation.
- Input locking.
- Occupancy synchronization.
- Menu behavior.

### 9.3 Regression Tests

Confirm that existing functionality remains operational:

- Battlefield generation.
- Keyboard navigation.
- Input repetition.
- Unit selection.
- Action menu navigation.
- Confirmation and cancellation.
- Camera configuration.

### 9.4 Validation Reporting

Each test result must be classified as:

- PASS.
- FAIL.
- NOT RUN.
- BLOCKED.

An agent must not report successful compilation or Unity execution unless the corresponding validation was actually performed.

## 10. Implementation Constraints

- Preserve existing serialized references.
- Preserve asset GUIDs and relevant meta files.
- Use the existing Unity Input System.
- Do not introduce external dependencies without approval.
- Do not modify unrelated gameplay functionality.
- Do not implement future features prematurely.
- Do not perform unrelated refactoring.
- Do not modify scenes, prefabs, or project settings without explicit authorization.
- Do not commit, push, or merge without explicit authorization.
- Implement one approved task at a time.

## 11. Future Integration

### F05 — Deployment System

The future deployment system will provide:

- An independent preparation scene.
- A 4×4 deployment grid per team.
- Six units per team.
- Unrestricted class composition.
- Unit statistics preview.
- Unit placement.
- Unit rearrangement and removal.
- Blind deployment.
- Formation transfer into the battle scene.

Blue units will deploy in the bottom-left battlefield corner.

Red units will deploy in the top-right battlefield corner.

Initial AI formations:

| Formation | Knight | Wizard | Archer |
|---|---:|---:|---:|
| Defensive | 3 | 1 | 2 |
| Offensive | 2 | 3 | 1 |
| Balanced | 2 | 2 | 2 |

The AI formation must be selected independently of the player's composition.

Future player-versus-player deployment must keep both formations hidden until preparation is complete.

### Additional Future Features

- F06 — Combat System.
- F07 — Turn System.
- F08 — Battle AI.
- F09 — Battle Resolution.
- Special terrain and movement modifiers.
- Multiplayer.

These references establish integration expectations and do not authorize implementation.

## 12. Pending Decisions

- Final movement path preview color.
- Compatible priority queue implementation.
- Exact temporary test formation coordinates.
- Final prefab geometry and visual dimensions.
- Movement completion notification mechanism.
- Exact implementation of the directional path indicator.

Implementation-specific decisions must be proposed during the corresponding task planning stage.

## 13. Definition of Done

Feature 04 is complete when:

- All implementation tasks are completed.
- All required acceptance criteria are satisfied.
- Required Unity tests are performed.
- Existing functionality remains operational.
- Code has been reviewed.
- Relevant documentation reflects the implementation.
- Known limitations are documented.
- The project owner approves the feature.
- Changes are integrated into the stable development baseline.

## 14. Change Log

| Version | Description |
|---|---|
| 1.0.0 | Initial feature specification containing approved functional and architectural requirements. |