# Tactical-RPG — Software Architecture

**Version:** 0.1.0  
**Status:** Active Development  
**Architecture Stage:** F06 Alpha 0.1.0 core manually validated (line of sight pending); F07 Alpha 0.1.0 core manually validated; F09 Alpha 0.1.0 KO/winner core manually validated

## 1. Technology Stack

| Technology | Purpose |
|---|---|
| Unity 6 | Game engine |
| C# | Gameplay programming |
| Unity Input System | Player input |
| Unity UI | User interface |
| Git | Version control |
| GitHub | Repository hosting and project management |
| Git LFS | Large asset version control |
| GitHub Copilot | AI-assisted development |
| Visual Studio Code | Code editing |

## 2. Architectural Overview

The current architecture follows Unity's component-based development model.

Gameplay functionality is distributed across MonoBehaviour components and
plain C# services, with communication through direct references and events.

The current implementation prioritizes simplicity, readability, and incremental development.

### Component Relationships

```text
Player Input
     |
     v
GridCursor <------> ActionMenu
     |
     v
GridManager ------> TurnManager
     | \
     |  +-------------> BattleManager
     v
CombatSystem -------> Unit ------> UnitData
```

This diagram represents the primary gameplay dependencies, not the Unity scene hierarchy.

GridManager composes BattleManager, TurnManager, and CombatSystem. Unit reports
defeat to GridManager, which clears occupancy and notifies BattleManager before
removing the unit from turn order. BattleManager's `BattleEnded` event causes GridManager to stop turns and clean
overlays; GridCursor closes ActionMenu. GridManager exposes BattleManager's
read-only result properties and event for future presentation components.

## 3. Core Components

### 3.1 GridManager

**File:** `GridManager.cs`

**Responsibility:** Battlefield generation, unit registration, tile occupancy, movement validation, and spatial queries.

#### Current Data

| Field | Purpose |
|---|---|
| `width` | Battlefield width |
| `height` | Battlefield height |
| `tilePrefab` | Battlefield tile prefab |
| `unitPrefabs` | Class-to-prefab mappings for reusable unit prefabs |
| `unitSpawns` | Configurable team, class, and starting grid position per unit |
| `unitSpawnHeight` | Visual Y coordinate used when spawning units |
| `occupiedTiles` | Two-dimensional unit occupancy matrix |
| `movementOverlayPrefab` | Movement indicator prefab |
| `movementOverlays` | Active movement indicators |
| `attackOverlayPrefab` | Shared source for attack-range overlays and the active-unit indicator |
| Active unit indicator | Single persistent yellow instance of `AttackOverlay` |
| `defeatedUnits` | Units whose KO has already been processed |
| `Combat` | CombatSystem service initialized in `Awake()` |
| `Turns` | TurnManager service initialized in `Awake()` |
| `Battle` | BattleManager service initialized in `Awake()` |

#### Current Methods

| Method | Responsibility |
|---|---|
| `Awake()` | Create gameplay services and subscribe to battle and active-unit events |
| `Start()` | Initialize occupancy, generate the grid, spawn units, initialize BattleManager, then TurnManager |
| `GenerateGrid()` | Instantiate battlefield tiles |
| `SpawnConfiguredUnits()` | Instantiate units, subscribe to defeat events, and register occupancy |
| `TryBuildPrefabMap()` | Validate and map unit classes to reusable prefabs |
| `ValidateSpawnDefinitions()` | Validate class, team, bounds, and position overlap before spawning |
| `IsTileOccupied()` | Check tile occupancy |
| `GetUnitAt()` | Retrieve a unit from a grid position |
| `GetRegisteredUnits()` | Return units currently registered in the occupancy grid |
| `IsWithinBoard()` | Check whether grid coordinates are inside the battlefield |
| `MoveUnit()` | Reject defeated units or a finished battle, then authorize and reserve a reachable route |
| `GetReachablePaths()` | Calculate reachable destinations and routes using BFS |
| `ReconstructPath()` | Rebuild a route from BFS predecessor data |
| `HandleMovementCompleted()` | Finalize occupancy and reposition the indicator after movement |
| `HandleUnitDefeated()` | Process each KO once, clear occupancy, report it, and destroy the unit |
| `ClearUnitOccupancy()` | Release every occupied tile reference to a unit |
| `HandleBattleEnded()` | Stop turns and clear action and active-unit overlays |
| `HandleActiveUnitChanged()` | Show or reposition the active-unit indicator |
| `UpdateActiveUnitIndicator()` | Keep the indicator on the active unit's current tile |
| `ShowMovementRange()` | Display reachable, unoccupied movement destinations |
| `ClearMovementRange()` | Remove movement indicators |

#### Current Limitations

- Movement uses unit-cost tiles; variable terrain costs and dedicated terrain obstacles are not implemented.
- Directional path preview is not implemented.
- Battlefield generation, occupancy management, and visualization share the same component.

### 3.2 CombatSystem

**File:** `CombatSystem.cs`

**Responsibility:** Attack range calculation, target validation, immediate damage resolution, and AttackOverlay lifecycle.

CombatSystem is a plain C# service created by GridManager. It reads unit
occupancy and board bounds through GridManager spatial queries, while Unit
remains responsible for changing its own current health.

#### Current Methods

| Method | Responsibility |
|---|---|
| `TryAttackUnit()` | Validate and resolve an attack against an in-range enemy |
| `IsWithinAttackRange()` | Check a tile against the attacker's configured Manhattan range |
| `ShowAttackRange()` | Display tiles within the unit's configured Manhattan range |
| `ClearAttackRange()` | Remove active attack overlays |

#### Current Limitations

- F06 remains partially implemented; line of sight is deferred.

### 3.3 TurnManager

**File:** `TurnManager.cs`

**Responsibility:** Build turn order, track the active unit and available
actions, remove defeated units, and advance rounds.

TurnManager is a plain C# service initialized by GridManager after unit
spawning. It receives the registered units; turn rules remain in this service,
not in GridManager. It sorts by Initiative Speed and alternates teams within
equal-speed groups, starting each round with the current priority team. The
initial priority is random and switches at each new round.

TurnManager publishes `ActiveUnitChanged` when a unit becomes active. GridManager
uses the event to update the persistent board indicator, and GridCursor uses it
to position the cursor at the start of the battle and on each new turn.

#### Current Methods

| Method | Responsibility |
|---|---|
| `BeginBattle()` | Randomize initial team priority and create the first round |
| `CanMove()` / `CanAttack()` | Check whether the active unit may use an action |
| `CompleteMove()` / `CompleteAttack()` | Consume a valid action and advance if both are spent |
| `EndTurn()` | End the active unit's turn when Wait is selected |
| `RemoveUnit()` | Remove a defeated unit from the current and future turn orders |
| `StopBattle()` | Clear the active unit and prevent further turn progression |
| `ActiveUnitChanged` | Notify presentation components when the active unit changes |

#### Current Limitations

- Turn animations are not implemented.

### 3.4 BattleManager

**File:** `BattleManager.cs`

**Responsibility:** Track living units, determine battle completion, and expose
the winning team.

BattleManager is a plain C# service composed by GridManager. It tracks the
initial living roster and removes each defeated unit from that set once. When
a team has no living units, it stores `IsFinished` and `Winner` (null until a
result exists), logs the winner, and raises `BattleEnded`. GridManager handles
this event before the turn order can advance.

#### Current Methods

| Method | Responsibility |
|---|---|
| `BeginBattle()` | Initialize the living roster and detect a missing team |
| `RegisterUnitDefeated()` | Remove a defeated unit and evaluate victory |
| `FinishBattle()` | Store and log the winner, then notify listeners |

#### Current Limitations

- Draw handling, result interfaces, menus, and battle restart are not implemented.

### 3.5 Unit

**File:** `Unit.cs`

**Responsibility:** Individual health, position, defeat notification, and visual movement.

#### Current Data

| Field | Purpose |
|---|---|
| `unitData` | Shared class statistics |
| `team` | Individual team identity |
| `CurrentHealth` | Individual current health initialized from UnitData |
| `gridX` | Logical X coordinate |
| `gridZ` | Logical Z coordinate |
| Movement range | Read from UnitData |
| `movementSpeed` | Visual movement speed |
| `targetPosition` | Visual movement destination |
| `movementPath` | Remaining tile-by-tile route |
| `movementDestination` | Authorized final grid position |
| `IsMoving` | Movement animation state |
| `IsDefeated` | Whether current health has reached zero |
| `MovementCompleted` | Event raised after route execution |
| `Defeated` | One-time event raised when current health reaches zero |

#### Current Methods

| Method | Responsibility |
|---|---|
| `Awake()` | Initialize the target position and current health |
| `Update()` | Move along the authorized route one tile at a time |
| `SetPosition()` | Update logical coordinates and visual destination |
| `SetTeam()` | Assign team identity and update its outline color |
| `MoveAlongPath()` | Execute a route authorized by GridManager |
| `ApplyDamage()` | Reduce this unit's health and report defeat once without modifying UnitData |

#### Current Limitations

- Board removal is delegated to GridManager after the `Defeated` event.

### 3.6 UnitData

**File:** `UnitData.cs`

**Responsibility:** Shared, Inspector-configurable base statistics for a unit class.

UnitData stores the class, maximum health, attack, armor, movement range, attack range, and initiative speed. Runtime health is stored on each Unit instance and initialized from `MaxHealth`; it is not written back to the shared asset.

The Knight, Wizard, and Archer assets are stored under `Assets/Data/Units/`. Unit prefabs must reference the corresponding asset in the Inspector.

### 3.7 GridCursor

**File:** `GridCursor.cs`

**Responsibility:** Player input, battlefield navigation, unit selection, and action state management.

#### Current Data

| Field | Purpose |
|---|---|
| `x`, `z` | Cursor grid coordinates |
| `currentState` | Current interaction state |
| `selectedUnit` | Currently selected unit |
| `initialDelay` | Initial input repetition delay |
| `repeatInterval` | Continuous input repetition interval |
| `previousDirection` | Previous movement input |
| `nextMoveTime` | Next permitted cursor movement time |

#### Current Methods

| Method | Responsibility |
|---|---|
| `Start()` | Initialize cursor and menu references, then synchronize to the active unit |
| `HandleActiveUnitChanged()` | Move the cursor to the active unit |
| `HandleBattleEnded()` | Clear selection and close ActionMenu |
| `PositionCursorAtUnit()` | Synchronize logical and visual cursor coordinates |
| `Update()` | Process player input |
| `HandleConfirm()` | Process confirmation according to the current state |
| `HandleCancel()` | Process cancellation |
| `EnterMovementMode()` | Activate movement selection |
| `EnterAttackMode()` | Activate attack target selection through CombatSystem |
| `CancelMovementSelection()` | Clear movement overlays and restore the selected unit and action menu |
| `WaitActiveUnit()` | End the active unit's turn |
| `HandleCursorMovement()` | Process directional keyboard input |
| `MoveCursor()` | Update cursor position while clamping it to battlefield boundaries |

#### Current Limitations

- Unit selection does not distinguish between teams.

### 3.8 ActionMenu

**File:** `ActionMenu.cs`

**Responsibility:** Action menu navigation and action selection.

#### Available Actions

- Move
- Attack
- Wait

#### Current Methods

| Method | Responsibility |
|---|---|
| `Awake()` | Initialize the button collection |
| `OnEnable()` | Reset the selected action |
| `Update()` | Process menu navigation input |
| `SelectNext()` | Select the next action |
| `SelectPrevious()` | Select the previous action |
| `SetActionAvailability()` | Disable actions already consumed this turn |
| `ConfirmSelection()` | Execute the selected action |
| `UpdateSelection()` | Update visual button selection |

#### Current Limitations

- Actions are associated with button array indices.
- The component has a bidirectional dependency with GridCursor.

## 4. Interaction State Machine

Player interaction is controlled through `CursorState`.

| State | Responsibility |
|---|---|
| `Exploration` | Navigate the battlefield and select units |
| `ActionMenu` | Select an available action |
| `Movement` | Select a movement destination |
| `Attack` | Navigate freely and select an enemy within attack range |
| `Locked` | Prevent interaction during movement animation |

### Current Movement Flow

```text
Exploration
     |
     | Select unit
     v
ActionMenu
     |
     | Select Move
     v
Movement
     |
     | Confirm valid destination
     v
Locked
     |
     | Movement animation completed
     v
ActionMenu
```

Movement cancellation returns the cursor to the selected unit's position and reopens the action menu.

Only the active unit can open the action menu. Cursor navigation remains free
throughout the battlefield.

Attack selection displays the configured Manhattan range through CombatSystem.
A valid enemy target receives immediate damage; invalid confirmation or
Backspace returns to the action menu and clears the attack indicators.

### Current Attack Flow

```text
ActionMenu
     |
     | Select Attack
     v
Attack
     |
     | Confirm valid enemy
     v
CombatSystem validates and resolves the attack
     |
     v
ActionMenu
```

### Manual Combat Validation (Project Owner Report)

The project owner reports these manual checks passed:

- Attack ranges were correct.
- Damage accumulated independently per Unit instance.
- Current health was limited to zero.
- Allied units were rejected as targets.
- Combat worked for both teams.

After the CombatSystem refactor, the project owner confirmed that these same
manual combat checks remained successful.

These checks validate the implemented Alpha 0.1.0 slice only; F06 remains
partially complete.

### Current Turn Flow

TurnManager owns the active unit and action availability. A valid Move and
Attack may occur in either order; Wait ends the turn, and using both actions
advances automatically. At the end of a round, the team priority switches.

The active-unit event keeps one yellow indicator over the active unit's tile.
GridCursor positions itself on the active unit at startup and when turns
change; after Move or Attack, it returns to the current active unit. Navigation
remains free in Exploration, Movement, and Attack.

When a unit reaches zero HP, Unit raises `Defeated`. GridManager clears its
occupancy, BattleManager evaluates victory, and TurnManager removes the unit
from its order. If that KO ends the battle, BattleManager's event stops
TurnManager before any next-turn advancement, closes ActionMenu, and clears
movement, attack, and active-unit overlays.

### Manual Turn and Indicator Validation (Project Owner Report)

The project owner reports that F07 manual tests passed and that turn order and
action restrictions work correctly. The owner also reports that active-unit
indicator and automatic cursor-positioning tests passed.

### Manual Battle Resolution Validation

The project owner reports that the final manual checks passed:

- Units disappear when they reach zero HP.
- Unit defeat works for both teams.
- The battle ends when only one team has living units, and further actions stop.

These are project-owner-reported Unity results; no compilation or tests were
run by the documentation update.

## 5. Battlefield Representation

The battlefield uses a two-dimensional coordinate system mapped onto Unity's XZ plane.

- X represents horizontal grid position.
- Z represents vertical grid position.
- Y represents visual elevation.
- Grid coordinates use integer values.
- World positions use Unity's Vector3 type.

### Battlefield Configuration

| Property | Value |
|---|---|
| Current configured width | 16 |
| Current configured height | 16 |
| Script default width | 8 |
| Script default height | 8 |
| Current tile count | 256 |

Battlefield dimensions are configurable through the Unity Inspector.

The current scene configuration overrides the script's default dimensions.

### Tile Occupancy

The battlefield stores unit occupancy using:

```csharp
private Unit[,] occupiedTiles;
```

Each matrix element contains either:

- A reference to a Unit occupying the tile.
- Null, representing an empty tile.

GridManager maintains the occupancy matrix.

## 6. Current Movement Architecture

### 6.1 Reachability Calculation

Every movement step currently costs one point. GridManager uses breadth-first
search (BFS) to calculate minimum step counts and reconstruct routes.

```text
Movement cost = number of orthogonal tiles in the route
```

The search respects the unit's Movement Range and battlefield bounds. Allied
units may be traversed but are not valid destinations. Enemy units block
traversal. Only reachable, empty tiles other than the origin are valid
destinations.

BFS provides minimum-cost routes while every step has equal cost. Variable
terrain costs and dedicated terrain obstacles are not implemented; weighted
movement will require the planned Dijkstra pathfinder.

### 6.2 Movement Execution

The current movement sequence is:

1. GridCursor requests movement through GridManager.
2. GridManager recalculates reachable routes and rejects destinations not in the result.
3. GridManager reserves the destination while retaining occupancy of the origin.
4. GridCursor enters the Locked state, and Unit follows the authorized route one tile at a time using Vector3.MoveTowards.
5. Unit updates its logical coordinates and raises MovementCompleted after reaching the destination.
6. GridManager releases the origin tile and finalizes destination occupancy.
7. GridCursor returns to ActionMenu after movement finishes.

### 6.3 Movement Visualization

GridManager displays overlays for the valid destinations returned by the same
reachable-route calculation used for movement authorization. Overlays are
omitted for:

- The selected unit's current position.
- Allied or enemy occupied tiles.
- Destinations outside the movement budget or blocked by enemies.

Movement overlays are instantiated from a prefab and tracked in a list for subsequent removal.

### 6.4 Development Exception

Consecutive movement actions are intentionally permitted to support development and testing.

Action restrictions will be introduced with the turn system.

## 7. Camera Architecture

The current camera uses a parent-child transform structure.

```text
CameraPivot
    |
    +-- MainCamera
```

The camera provides an orthographic, isometric view of the battlefield.

CameraPivot supports horizontal orientation, while MainCamera provides the viewing angle.

A temporary top-down observation mode is planned but is not currently implemented.

## 8. User Interface Architecture

The current action interface uses Unity UI components.

```text
BattleCanvas
    |
    +-- ActionMenu
            |
            +-- Move Button
            +-- Attack Button
            +-- Wait Button
```

GridCursor controls action menu visibility and confirmation.

ActionMenu manages button selection and navigation.

## 9. Known Technical Debt

| ID | Description | Planned Resolution |
|---|---|---|
| TD-03 | Variable terrain costs and dedicated terrain obstacles are not implemented | Future terrain/pathfinding work |
| TD-06 | Bidirectional GridCursor and ActionMenu dependency | Future evaluation |
| TD-07 | Limited encapsulation of public fields | Incremental evaluation |
| TD-08 | Directional movement path preview is not implemented | Feature 04 |

Technical debt does not automatically authorize refactoring.

Changes must remain within the approved feature scope.

## 10. Planned Architectural Evolution

The following changes represent architectural objectives, not completed functionality.

### Feature 04 — Units and Movement

The current implementation provides unit classes and team identities, shared
unit statistics, configurable multi-unit spawning, BFS-based reachability for
unit-cost movement, unrestricted cursor navigation within battlefield bounds,
reachable-destination overlays, and tile-by-tile movement execution with
occupancy synchronization.

Remaining work includes the approved path preview, variable terrain movement
costs, dedicated terrain obstacles, and completion of the remaining F04 tasks.

### Remaining Features

- Deployment and battle AI.
- Line of sight; counterattacks are not part of the current combat rules.
- Draw handling, victory/defeat presentation, main menu, and battle restart.
- Health bars and unit-statistics UI.
- Attack and unit-defeat animations, plus visual and camera improvements.
- Local multiplayer and network multiplayer.

Specific components, algorithms, and data structures must be approved through feature specifications before implementation.

## 11. Architectural Principles

- Maintain a single source of truth for gameplay rules.
- Separate player interaction from gameplay validation.
- Separate logical gameplay state from visual representation.
- Prefer component composition over unnecessary inheritance.
- Keep implementations appropriate to the current project scale.
- Preserve configurable gameplay values.
- Introduce new abstractions only when justified.
- Avoid duplicating logic across components.
- Preserve existing functionality during incremental development.

## 12. Documentation Maintenance

This document must reflect the actual implemented architecture.

Update it when an approved implementation introduces significant changes to:

- Component responsibilities.
- Component dependencies.
- Data structures.
- Gameplay state management.
- Architectural patterns.
- Technical limitations.

Game mechanics must remain documented in GAME_DESIGN.md.

Feature-specific implementation requirements must remain documented under docs/features/.

Do not document planned functionality as implemented functionality.