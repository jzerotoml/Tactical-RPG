# Tactical-RPG — Software Architecture

**Version:** 0.1.0  
**Status:** Active Development  
**Architecture Stage:** F04-T04 and F04-T05 complete; F04-T06 onward pending

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

Gameplay functionality is distributed across MonoBehaviour components, with communication through direct references and public methods.

The current implementation prioritizes simplicity, readability, and incremental development.

### Component Relationships

```text
Player Input
     |
     v
GridCursor <------> ActionMenu
     |
     v
GridManager
     |
     v
    Unit ------> UnitData
```

This diagram represents the primary gameplay dependencies, not the Unity scene hierarchy.

## 3. Core Components

### 3.1 GridManager

**File:** `GridManager.cs`

**Responsibility:** Battlefield generation, unit registration, tile occupancy, movement validation, and movement visualization.

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

#### Current Methods

| Method | Responsibility |
|---|---|
| `Start()` | Initialize occupancy, generate the grid, and spawn configured units |
| `GenerateGrid()` | Instantiate battlefield tiles |
| `SpawnConfiguredUnits()` | Instantiate configured units and register their initial occupancy |
| `TryBuildPrefabMap()` | Validate and map unit classes to reusable prefabs |
| `ValidateSpawnDefinitions()` | Validate class, team, bounds, and position overlap before spawning |
| `IsTileOccupied()` | Check tile occupancy |
| `GetUnitAt()` | Retrieve a unit from a grid position |
| `MoveUnit()` | Recalculate and authorize a reachable route, then reserve its destination |
| `GetReachablePaths()` | Calculate reachable destinations and routes using BFS |
| `ReconstructPath()` | Rebuild a route from BFS predecessor data |
| `HandleMovementCompleted()` | Release the origin tile and finalize destination occupancy |
| `ShowMovementRange()` | Display reachable, unoccupied movement destinations |
| `ClearMovementRange()` | Remove movement indicators |

#### Current Limitations

- Movement uses unit-cost tiles; variable terrain costs and dedicated terrain obstacles are not implemented.
- Directional path preview is not implemented.
- Battlefield generation, occupancy management, and visualization share the same component.

### 3.2 Unit

**File:** `Unit.cs`

**Responsibility:** Individual unit position and visual movement.

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
| `MovementCompleted` | Event raised after route execution |

#### Current Methods

| Method | Responsibility |
|---|---|
| `Awake()` | Initialize the target position and current health |
| `Update()` | Move along the authorized route one tile at a time |
| `SetPosition()` | Update logical coordinates and visual destination |
| `SetTeam()` | Assign team identity and update its outline color |
| `MoveAlongPath()` | Execute a route authorized by GridManager |

#### Current Limitations

- Combat execution and health changes are not implemented.

### 3.3 UnitData

**File:** `UnitData.cs`

**Responsibility:** Shared, Inspector-configurable base statistics for a unit class.

UnitData stores the class, maximum health, attack, armor, movement range, attack range, and initiative speed. Runtime health is stored on each Unit instance and initialized from `MaxHealth`; it is not written back to the shared asset.

The Knight, Wizard, and Archer assets are stored under `Assets/Data/Units/`. Unit prefabs must reference the corresponding asset in the Inspector.

### 3.4 GridCursor

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
| `Start()` | Initialize cursor and menu references |
| `Update()` | Process player input |
| `HandleConfirm()` | Process confirmation according to the current state |
| `HandleCancel()` | Process cancellation |
| `EnterMovementMode()` | Activate movement selection |
| `CancelMovementSelection()` | Clear movement overlays and restore the selected unit and action menu |
| `HandleCursorMovement()` | Process directional keyboard input |
| `MoveCursor()` | Update cursor position while clamping it to battlefield boundaries |

#### Current Limitations

- Unit selection does not distinguish between teams.
- Attack selection does not execute combat logic.

### 3.4 ActionMenu

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
| `ConfirmSelection()` | Execute the selected action |
| `UpdateSelection()` | Update visual button selection |

#### Current Limitations

- Actions are associated with button array indices.
- Attack and Wait are placeholders.
- The component has a bidirectional dependency with GridCursor.

## 4. Interaction State Machine

Player interaction is controlled through `CursorState`.

| State | Responsibility |
|---|---|
| `Exploration` | Navigate the battlefield and select units |
| `ActionMenu` | Select an available action |
| `Movement` | Select a movement destination |
| `Attack` | Reserved for attack target selection |
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

Cancelling the action menu returns the player to Exploration.

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

### Future Features

- Combat resolution.
- Health and damage management.
- Turn management.
- Initiative calculation.
- Action availability management.
- Victory and defeat detection.
- Local multiplayer.
- Network multiplayer.

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