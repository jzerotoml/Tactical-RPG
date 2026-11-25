# Tactical-RPG — Game Design Document

**Version:** 0.2.0  
**Status:** Active Development  
**Genre:** Turn-Based Tactical RPG

## 1. Game Overview

Tactical-RPG is a grid-based, turn-based strategy game focused on unit composition, battlefield positioning, resource management, and tactical decision-making.

Players prepare their armies before entering battle and control individual units according to their movement capabilities, combat statistics, and turn order.

The initial playable game mode will be Player vs AI.

Player vs Player and network multiplayer are planned for future development.

## 2. Design Pillars

- Strategic freedom.
- Meaningful unit positioning.
- Distinct unit roles.
- Readable battlefield information.
- Predictable gameplay rules.
- Accessible keyboard controls.
- Configurable and extensible game mechanics.

Players should win through tactical decisions rather than unnecessary mechanical complexity.

## 3. Battlefield

The battlefield consists of a rectangular grid.

### Initial Configuration

| Property | Value |
|---|---|
| Width | 16 tiles |
| Height | 16 tiles |
| Total tiles | 256 |
| Movement directions | Orthogonal |
| Diagonal movement | Disabled |
| Maximum units per tile | 1 |
| Units per team | 6 |
| Total battle units | 12 |

Battlefield dimensions must remain configurable.

Future maps may introduce different layouts, obstacles, terrain types, and environmental effects.

### 3.1 Battlefield Visibility

The game does not use a Fog of War system.

Once the battle begins:

- The entire battlefield is visible to both teams.
- All allied and enemy units are visible.
- Unit visibility is independent of distance and position.
- Terrain and obstacles do not conceal units.
- No exploration or vision radius mechanics are required.

Blind Deployment applies exclusively to the preparation phase.

Camera limitations do not affect gameplay visibility.

Attack line-of-sight rules are independent of battlefield visibility and will be defined during combat system development.

## 4. Teams

The game supports two opposing teams.

| Team | Unit Color | Deployment Location |
|---|---|---|
| Blue | Blue | Bottom-left corner |
| Red | Red | Top-right corner |

Each team consists of exactly six units at the beginning of a standard battle.

Players may freely choose their army composition.

There are no restrictions on the number of units belonging to a particular class, provided the total team size is respected.

For example, a team may consist entirely of Knights, Wizards, or Archers.

## 5. Unit Classes

### 5.1 Knight

**Role:** Frontline tank.

The Knight specializes in durability, close-range combat, and protecting vulnerable allied units.

Characteristics:

- High health.
- High armor.
- Limited movement range.
- Melee attacks.
- Low initiative speed.

Initial visual representation: square-based geometry.

### 5.2 Wizard

**Role:** High-damage ranged attacker.

The Wizard specializes in dealing significant damage while relying on positioning and allied protection.

Characteristics:

- Low health.
- Low armor.
- High attack damage.
- Medium attack range.
- Medium initiative speed.

Initial visual representation: circular geometry.

### 5.3 Archer

**Role:** Mobile long-range attacker.

The Archer specializes in attacking from a distance and maintaining favorable positioning.

Characteristics:

- Moderate health.
- Long attack range.
- High initiative speed.
- Good mobility.
- Cannot attack adjacent units.

Initial visual representation: triangular geometry.

## 6. Unit Statistics

### 6.1 Initial Balance

| Attribute | Knight | Wizard | Archer |
|---|---:|---:|---:|
| Max Health | 120 | 70 | 80 |
| Attack | 30 | 45 | 35 |
| Armor (%) | 20 | 5 | 10 |
| Movement Range | 3 | 4 | 4 |
| Minimum Attack Range | 1 | 1 | 2 |
| Maximum Attack Range | 1 | 3 | 5 |
| Speed | 3 | 5 | 7 |

These values represent the initial balance configuration and may change following gameplay testing.

### 6.2 Attribute Definitions

| Attribute | Description |
|---|---|
| Max Health | Maximum health capacity |
| Current Health | Remaining individual health |
| Attack | Base offensive damage |
| Armor | Percentage of incoming damage reduced |
| Movement Range | Available movement points |
| Minimum Attack Range | Minimum permitted attack distance |
| Maximum Attack Range | Maximum permitted attack distance |
| Speed | Attribute used for future turn initiative |

Speed must remain independent of visual movement speed.

Base statistics are shared between units of the same class, while individual gameplay state remains independent.

## 7. Game Flow

The standard game flow is:

```text
Game Mode Selection
        |
        v
Deployment Phase
        |
        v
Battle Initialization
        |
        v
Battle Phase
        |
        v
Battle Resolution
```

Game mode selection and battle resolution will be implemented in future features.

## 8. Deployment Phase

**Implementation Status:** Planned — F05.

The deployment phase allows players to configure their armies before entering the battlefield.

Deployment takes place in a dedicated preparation scene rather than directly within the battle scene.

### 8.1 Deployment Grid

Each team has access to a local 4×4 deployment grid.

| Property | Value |
|---|---|
| Width | 4 tiles |
| Height | 4 tiles |
| Available positions | 16 |
| Required units | 6 |

Deployment dimensions and required unit count should remain configurable.

Valid deployment tiles are highlighted in green.

### 8.2 Preparation Menu

The preparation menu contains three primary actions:

| Action | Description |
|---|---|
| Deploy Unit | Select and position a new unit |
| Rearrange Units | Move or remove previously deployed units |
| Start Battle | Confirm the completed formation |

The interface must display the number of deployed units.

Example:

`Deployed Units: 4/6`

Start Battle must remain unavailable until exactly six valid units have been deployed.

Deploy Unit must not allow the team size limit to be exceeded.

### 8.3 Unit Selection

The unit selection menu must provide access to:

- Knight.
- Wizard.
- Archer.

Highlighting a class must display its statistics, including:

- Health.
- Attack.
- Armor.
- Movement range.
- Attack range.
- Speed.

Players may select the same class multiple times.

### 8.4 Unit Placement

After selecting a unit class, the player enters placement mode.

The selected unit may be positioned on any valid, unoccupied deployment tile.

Controls:

- Directional keys move the placement cursor.
- Enter confirms placement.
- Escape or Backspace cancels placement.

Cancelling placement removes the unconfirmed unit preview.

Confirming placement registers the unit and returns the player to the preparation menu.

### 8.5 Unit Rearrangement

Players may select previously deployed units and:

- Move them to another valid deployment tile.
- Remove them from the formation.

Removing a unit releases its occupied tile and decreases the deployed unit count.

Rearrangement does not consume movement points.

Players may replace removed units with any available class.

### 8.6 Blind Deployment

Enemy formations must remain hidden throughout preparation.

Players must not receive information about:

- Enemy unit composition.
- Enemy starting positions.
- Enemy formation selection.

Both formations are revealed when the battle begins.

### 8.7 Battlefield Deployment Locations

For the initial 16×16 battlefield:

| Team | X Coordinates | Z Coordinates |
|---|---|---|
| Blue | 0–3 | 0–3 |
| Red | 12–15 | 12–15 |

```text
............RRRR
............RRRR
............RRRR
............RRRR
................
................
................
................
................
................
................
................
BBBB............
BBBB............
BBBB............
BBBB............
```

B represents the Blue deployment region.

R represents the Red deployment region.

These regions define starting positions, not permanent movement restrictions.

Local deployment coordinates must be converted into the corresponding battlefield coordinates during battle initialization.

### 8.8 Battle Initialization

The battle scene receives both completed formations.

Each formation contains the selected unit classes and their starting positions.

The battlefield generates the corresponding units using their class configurations and team identities.

The preparation interface must not remain active during battle.

## 9. AI Formations

**Implementation Status:** Planned — F05.

The initial Player vs AI mode supports three predefined enemy formations.

| Formation | Knight | Wizard | Archer | Total |
|---|---:|---:|---:|---:|
| Defensive | 3 | 1 | 2 | 6 |
| Offensive | 2 | 3 | 1 | 6 |
| Balanced | 2 | 2 | 2 | 6 |

### 9.1 Defensive Formation

Prioritizes frontline durability and ranged support.

### 9.2 Offensive Formation

Prioritizes high damage output while maintaining a smaller defensive frontline.

### 9.3 Balanced Formation

Provides an equal distribution of defensive, offensive, and ranged capabilities.

### 9.4 Formation Selection

The AI selects one of the three predefined formations independently of the player's composition.

The initial selection method is random, with equal probability for each formation.

The AI must not inspect the player's formation to select a counter-composition.

Exact starting positions for each AI formation will be defined in the deployment feature specification.

AI formation selection is separate from AI combat behavior.

## 10. Movement System

**Implementation Status:** Prototype implemented; advanced movement specified in F04.

### 10.1 Movement Rules

Units move horizontally or vertically across the battlefield.

Diagonal movement is not permitted.

| Tile Type | Traversable | Valid Destination |
|---|---|---|
| Empty | Yes | Yes |
| Allied unit | Yes | No |
| Enemy unit | No | No |
| Impassable obstacle | No | No |
| Outside battlefield | No | No |

Units may move through allied units but cannot finish their movement on occupied tiles.

Enemy units block movement paths.

### 10.2 Movement Points

Each unit has a movement budget determined by its Movement Range attribute.

Initially, moving into a traversable tile consumes one movement point.

A movement destination is valid only when the total movement cost of an available route does not exceed the unit's movement budget.

### 10.3 Pathfinding

Movement must use cost-aware pathfinding.

The approved algorithm is Dijkstra.

The system must calculate reachable destinations and minimum-cost routes while respecting battlefield restrictions.

### 10.4 Future Terrain Mechanics

The movement system must support future variable terrain costs.

Potential terrain mechanics include:

- Roads.
- Forests.
- Sand.
- Swamps.
- Impassable terrain.
- Class-specific terrain movement modifiers.

Specific terrain costs and class interactions remain undefined.

Special terrain gameplay is outside the scope of F04.

### 10.5 Movement Selection

During movement selection, the player may navigate the cursor freely across the battlefield.

The cursor may inspect occupied and unreachable tiles.

Only valid movement destinations may be confirmed.

### 10.6 Movement Visualization

| Element | Visual Representation |
|---|---|
| Valid movement destination | Blue semitransparent overlay |
| Occupied tile | No movement range overlay |
| Unreachable tile | No movement range overlay |
| Impassable obstacle | No movement range overlay |
| Cursor position | White semitransparent overlay |
| Expected movement path | White or green directional arrow |

The movement overlay represents valid destinations rather than every traversable tile.

An allied tile may be part of a valid movement path without displaying a blue overlay.

The expected path must disappear when the cursor is positioned over an invalid destination.

The final path indicator color will be selected through visual testing.

Red overlays are reserved for attack visualization.

### 10.7 Movement Execution

Units follow their calculated movement paths one tile at a time.

Player interaction remains locked during movement animation.

Logical grid coordinates and battlefield occupancy are updated after successful animation completion.

The destination remains reserved throughout movement execution.

## 11. Combat System

**Implementation Status:** Planned — F06.

### 11.1 Attack Rules

Units may attack enemy units within their permitted attack range.

Friendly fire is not supported in the initial combat design.

Counterattacks are not supported.

Archers cannot attack adjacent units because their minimum attack range is two.

Attack distance calculation and line-of-sight rules remain pending.

### 11.2 Damage Calculation

The initial damage formula is:

`Final Damage = Attack × (1 - Armor / 100)`

Final damage must be represented as an integer.

Fractional damage is rounded to the nearest integer, with halfway values rounded upward.

Example:

A Wizard attacks a Knight.

- Attack: 45.
- Knight armor: 20%.
- Final damage: 36.

### 11.3 Health

Damage reduces the target's current health.

Current health must not fall below zero.

Unit elimination behavior will be defined during combat system development.

Armor limits and minimum damage rules remain pending.

## 12. Turn System

**Implementation Status:** Planned — F07.

The game uses individual unit turns.

The Speed attribute influences turn initiative.

The exact initiative calculation remains pending.

### 12.1 Available Actions

- Move.
- Attack.
- Wait.

### 12.2 Action Rules

A unit may move and then attack during its turn.

Attacking ends the unit's turn automatically.

Moving after attacking is not permitted.

Waiting voluntarily ends the unit's turn.

If Wait is the only remaining available action, the turn ends automatically.

Counterattacks are not supported.

### 12.3 Development Exception

Until turn management is implemented:

- Units from either team may be selected.
- Consecutive movement actions are permitted.
- Action availability is not restricted by turn state.

These exceptions exist exclusively to facilitate development and testing.

## 13. Player Controls

The game initially uses keyboard-based interaction.

| Input | Action |
|---|---|
| W / Up Arrow | Navigate upward |
| S / Down Arrow | Navigate downward |
| A / Left Arrow | Navigate left |
| D / Right Arrow | Navigate right |
| Enter | Confirm |
| Numpad Enter | Confirm |
| Space | Confirm |
| Backspace | Cancel |
| Escape | Cancel or return |
| Observation key | Temporary battlefield overview — planned |

Mouse interaction is not required for the initial implementation.

Context-specific controls must remain consistent between menus and battlefield interaction.

## 14. Camera System

The battlefield uses an orthographic isometric camera.

The camera should provide sufficient visibility of the battlefield and selected units.

### 14.1 Observation Mode

A temporary top-down observation mode is planned.

Holding the observation key will display the battlefield from a 90-degree overhead perspective.

Releasing the key will restore the standard camera view.

The final observation key remains pending.

Zoom functionality may be evaluated in future development.

## 15. User Interface

The interface must prioritize readability and clear gameplay feedback.

### 15.1 Visual Language

| Color | Meaning |
|---|---|
| Blue | Movement range and Blue team identity |
| Red | Attack range and Red team identity |
| Green | Valid deployment positions |
| White | Cursor and neutral selection |

Movement path indicators may use white or green depending on visual testing.

### 15.2 Battle Interface

Planned battle information includes:

- Selected unit.
- Current health.
- Unit statistics.
- Available actions.
- Active turn indicator.
- Turn order.
- Movement range.
- Attack range.

### 15.3 Action Menu

The battle action menu contains:

- Move.
- Attack.
- Wait.

Action availability will depend on the unit's current turn state once turn management is implemented.

## 16. Game Modes

### 16.1 Player vs AI

The initial playable game mode.

The player prepares a custom six-unit army and faces an AI-controlled six-unit army.

The AI uses one of the predefined formations.

AI combat decision-making will be implemented separately.

### 16.2 Local Player vs Player

Planned for future development.

Both players prepare their formations independently.

Neither player may inspect the opposing formation before battle initialization.

### 16.3 Network Multiplayer

Planned for future development.

Network multiplayer must preserve blind deployment and independent team preparation.

Networking architecture and synchronization rules remain undefined.

## 17. Battle Resolution

**Implementation Status:** Planned — F09.

The game must eventually determine victory, defeat, and battle completion.

Exact victory conditions, draw conditions, and result presentation remain pending.

## 18. Balance Considerations

The initial statistics are experimental.

Future playtesting must evaluate:

- Knight durability against concentrated attacks.
- Wizard damage output.
- Archer mobility and long-range advantage.
- Archer kiting potential.
- Initiative advantages.
- Single-class army compositions.
- Defensive and offensive formation effectiveness.
- Deployment region size.
- Battlefield dimensions.
- Average battle duration.

Class composition restrictions must not be introduced without an approved design decision.

## 19. Development Roadmap

| Feature | Name | Status |
|---|---|---|
| F04 | Units and Movement | Specified |
| F05 | Deployment System | Planned |
| F06 | Combat System | Planned |
| F07 | Turn System | Planned |
| F08 | Battle AI | Planned |
| F09 | Battle Resolution | Planned |

Additional features:

- Camera observation mode.
- Special terrain.
- Expanded maps.
- Additional unit classes.
- Local Player vs Player.
- Network multiplayer.

Roadmap identifiers and priorities may evolve through approved planning decisions.

## 20. Pending Design Decisions

- Final movement path indicator color.
- Exact AI formation positions.
- Red team deployment coordinate orientation.
- Attack distance calculation.
- Line-of-sight rules.
- Initiative calculation.
- Armor limits.
- Minimum damage rules.
- Unit elimination behavior.
- Victory and draw conditions.
- Enemy unit inspection.
- Observation mode key.
- Terrain movement costs.
- Multiplayer implementation.

## 21. Documentation Maintenance

This document defines gameplay requirements and approved design decisions.

Implementation details belong in ARCHITECTURE.md and the corresponding feature specifications.

Planned functionality must not be represented as implemented functionality.

Changes to gameplay rules require project owner approval.

## 22. Change Log

| Version | Description |
|---|---|
| 0.1.0 | Initial game design document |
| 0.2.0 | Introduced six-unit teams, free army composition, blind deployment, AI formations, cost-aware pathfinding, movement visualization, and updated development roadmap |