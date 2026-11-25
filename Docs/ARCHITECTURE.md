# Tactical-RPG — Software Architecture

**Version:** 0.1.0  
**Status:** Active Development  
**Architecture Stage:** F04 implementation in progress; T03 complete

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
| `unitPrefab` | Prototype unit prefab |
| `occupiedTiles` | Two-dimensional unit occupancy matrix |
| `movementOverlayPrefab` | Movement indicator prefab |
| `movementOverlays` | Active movement indicators |

#### Current Methods

| Method | Responsibility |
|---|---|
| `Start()` | Initialize occupancy, generate the grid, and spawn the prototype unit |
| `GenerateGrid()` | Instantiate battlefield tiles |
| `SpawnUnit()` | Instantiate and register the prototype unit |
| `IsTileOccupied()` | Check tile occupancy |
| `GetUnitAt()` | Retrieve a unit from a grid position |
| `MoveUnit()` | Validate and execute a movement request |
| `ShowMovementRange()` | Display valid movement destinations |
| `ClearMovementRange()` | Remove movement indicators |

#### Current Limitations

- Unit spawning uses hardcoded coordinates and a single prefab.
- Movement validation does not consider intermediate tiles.
- Movement validation is partially duplicated across components.
- Battlefield generation, occupancy management, and visualization share the same component.

### 3.2 Unit

**File:** `Unit.cs`

**Responsibility:** Individual unit position and visual movement.

#### Current Data

| Field | Purpose |
|---|---|
| `unitData` | Shared class statistics |
| `CurrentHealth` | Individual current health initialized from UnitData |
| `gridX` | Logical X coordinate |
| `gridZ` | Logical Z coordinate |
| Movement range | Read from UnitData |
| `movementSpeed` | Visual movement speed |
| `targetPosition` | Visual movement destination |
| `IsMoving` | Movement animation state |

#### Current Methods

| Method | Responsibility |
|---|---|
| `Awake()` | Initialize the target position and current health |
| `Update()` | Execute visual movement |
| `SetPosition()` | Update logical coordinates and visual destination |
| `CanMoveTo()` | Validate UnitData movement range using Manhattan distance |

#### Current Limitations

- Unit class and base statistics are configured through UnitData, but team identity is not implemented.
- Combat execution and health changes are not implemented.
- Movement animation follows a direct line toward the destination.
- Movement does not follow individual grid tiles.
- Logical coordinates are updated before visual movement finishes.

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
| `HandleCursorMovement()` | Process directional keyboard input |
| `MoveCursor()` | Update cursor position and validate navigation |

#### Current Limitations

- Unit selection does not distinguish between teams.
- Movement selection contains duplicated movement validation.
- Occupied tiles restrict cursor navigation during movement selection.
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

### 6.1 Distance Calculation

The current implementation uses Manhattan distance.

```text
Distance = |targetX - gridX| + |targetZ - gridZ|
```

A destination is within movement range when its Manhattan distance does not exceed the unit's movementRange.

This calculation does not account for obstacles or blocked paths.

### 6.2 Movement Execution

The current movement sequence is:

1. GridCursor requests movement through GridManager.
2. GridManager validates the destination.
3. GridManager clears the previous occupancy entry.
4. Unit updates its logical coordinates and target position.
5. GridManager registers the destination occupancy.
6. GridCursor enters the Locked state.
7. Unit performs visual movement using Vector3.MoveTowards.
8. Unit reports movement completion through IsMoving.
9. GridCursor returns to ActionMenu.

### 6.3 Movement Visualization

GridManager iterates through battlefield tiles and displays overlays for destinations that:

- Are not the selected unit's current position.
- Are not occupied.
- Are within the unit's movement range.

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
| TD-01 | Movement validation duplicated across components | Feature 04 |
| TD-02 | Hardcoded prototype unit spawning | Feature 04 |
| TD-03 | Missing obstacle-aware pathfinding | Feature 04 |
| TD-04 | Direct-line movement animation | Feature 04 |
| TD-05 | Occupied tiles restrict movement cursor navigation | Feature 04 |
| TD-06 | Bidirectional GridCursor and ActionMenu dependency | Future evaluation |
| TD-07 | Limited encapsulation of public fields | Incremental evaluation |

Technical debt does not automatically authorize refactoring.

Changes must remain within the approved feature scope.

## 10. Planned Architectural Evolution

The following changes represent architectural objectives, not completed functionality.

### Feature 04 — Units and Movement

- Support multiple units.
- Introduce unit classes and team identities.
- Define unit statistics.
- Support configurable unit spawning.
- Centralize movement validation.
- Introduce obstacle-aware pathfinding.
- Execute movement along valid grid paths.
- Maintain consistent movement overlays and destination validation.

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