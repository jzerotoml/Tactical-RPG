# Tactical-RPG

A turn-based tactical RPG developed with Unity 6 and C#, inspired by classic tactical strategy games.

## Overview

Tactical-RPG is a personal game development project focused on grid-based combat, strategic positioning, and character classes with distinct combat roles.

The project is currently in its early development stage.

## Current Prototype

- The battle scene is configured with a 16×16 grid.
- One prototype unit can be selected and moved using keyboard controls.
- Movement uses Manhattan-distance validation, blue range overlays, and direct-line animation.
- Unit class statistics are defined by ScriptableObject assets, and the prototype prefab uses KnightData.
- Team identities, combat execution, and turn management are not implemented yet.

## Approved Initial Gameplay

- A 16×16 tactical battlefield.
- Two opposing teams with six units each.
- Three unit classes: Knight, Wizard, and Archer.
- Player vs AI as the initial game mode.
- Class-specific movement, attack range, damage, and health.
- Individual turns influenced by unit speed.
- Units can move and then attack during their turn.

## Planned Features

- Army deployment and formation selection.
- Basic enemy combat AI.
- Health bars and combat feedback.
- Victory and defeat conditions.

## Technologies

- **Game Engine:** Unity 6
- **Programming Language:** C#
- **Rendering:** Universal Render Pipeline (URP)
- **Version Control:** Git & GitHub

## Development Status

**Version:** 0.1.0 — Initial Development

The current prototype provides a grid-based battlefield. The approved initial game target is a 16×16 battlefield; planned gameplay systems are not yet implemented.

## License

No license has been selected yet.
