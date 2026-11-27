# Tactical-RPG

A turn-based tactical RPG developed with Unity 6 and C#, inspired by classic tactical strategy games.

# Tactical-RPG

A turn-based tactical RPG developed with Unity 6 and C#, inspired by classic tactical strategy games.

[![Play Demo](https://img.shields.io/badge/Play_Demo-Alpha_0.1.0-2EA44F?style=for-the-badge&logo=unity&logoColor=white)](https://jzerotoml.github.io/Tactical-RPG/)

![Unity](https://img.shields.io/badge/Unity-6-000000?style=flat-square&logo=unity)
![CSharp](https://img.shields.io/badge/C%23-512BD4?style=flat-square&logo=dotnet)
![Platform](https://img.shields.io/badge/Platform-WebGL-blue?style=flat-square)
![Status](https://img.shields.io/badge/Status-Active_Development-orange?style=flat-square)

## Overview

## Overview

Tactical-RPG is a personal game development project focused on grid-based combat, strategic positioning, and character classes with distinct combat roles.

The project is currently in its early development stage.

## Alpha 0.1.0 Battle Core

- The battle scene uses a configurable 16×16 grid and supports configured units for both teams.
- Unit class statistics are stored in `UnitData`; current health belongs to each `Unit` instance.
- The cursor can explore the board freely, while only the active unit can act.
- A unit may move once and attack once per turn, in either order. Wait ends the turn.
- Movement and attack ranges are displayed with overlays. A persistent yellow indicator marks the active unit.
- Attacks use configured Manhattan ranges and resolve damage immediately.
- Units reaching zero HP are removed from the board. The battle ends when one team has no living units, and further actions are blocked.
- Damage, remaining HP, and the winning team are reported through `Debug.Log`.

The project owner reports successful manual validation of combat ranges and damage, turn and action restrictions, the active-unit indicator, unit defeat, and battle end when only one team remains. No compilation or automated test results are claimed here.

## Approved Initial Gameplay

- A 16×16 tactical battlefield.
- Two opposing teams with six units each.
- Three unit classes: Knight, Wizard, and Archer.
- Player vs AI as the initial game mode.
- Class-specific movement, attack range, damage, and health.
- Individual turns influenced by unit speed.
- Units can move and attack once each during their turn, in either order; Wait ends the turn.

## Pending Features

- Army deployment and formation selection.
- Basic enemy combat AI.
- Victory and defeat presentation, main menu, and battle restart.
- Health bars and unit information.
- Attack and unit-defeat animations.
- Line-of-sight rules, visual improvements, and camera enhancements.

## Technologies

- **Game Engine:** Unity 6
- **Programming Language:** C#
- **Rendering:** Universal Render Pipeline (URP)
- **Version Control:** Git & GitHub

## Development Status

**Version:** 0.1.0 — Alpha battle core implemented

The Alpha 0.1.0 battle core includes movement, combat, individual turns, unit defeat, and winner detection. Deployment, AI, and the presentation features listed above remain pending.

## License

No license has been selected yet.
