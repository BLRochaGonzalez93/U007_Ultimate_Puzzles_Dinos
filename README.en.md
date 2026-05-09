# U007_Ultimate_Puzzles_Dinos

[English](README.en.md) | [Español](README.md)

## Summary

**Ultimate Puzzles Dinos** is a piece-based puzzle game developed in Unity with C#, featuring a dinosaur theme and a casual/child-friendly focus. Each puzzle works as an independent level, presented through a 2D top-down view and a structure mainly based on UI.

The player selects, drags and places pieces to complete each puzzle. The objective is to solve the image correctly and progress through the level collection. The project includes more than 50 levels and is being prepared for publication on Google Play Store, incorporating monetization and analytics systems such as Ads, IAP and Analytics.

## Technologies

- Unity
- C#
- Unity UI system
- Canvas
- EventSystem
- Drag & Drop
- ScriptableObjects
- Animator
- Particle System
- AudioSource
- Google Mobile Ads
- Unity Purchasing / IAP
- Git LFS
- GitHub Releases

## Main features

- Piece-based puzzle game.
- Dinosaur theme.
- Casual and child-friendly focus.
- 2D top-down view.
- Gameplay mainly based on UI.
- Piece selection.
- Drag and drop.
- Solution validation.
- Level system.
- More than 50 levels.
- Level unlocking.
- Scoring system.
- Correct/incorrect feedback.
- Main UI.
- Main menu.
- Level selection menu.
- Sound and music.
- Ads preparation.
- IAP preparation.
- Analytics preparation.
- Google Play Store publication target.
- Android build published through GitHub Releases.
- Windows version planned for future builds.

## Visuals

> Final screenshots and images pending.

Planned visual pack names:

- `ultimatepuzzlesdinos-logo.png`
- `ultimatepuzzlesdinos-cover.png`
- `ultimatepuzzlesdinos-banner.png`
- `ultimatepuzzlesdinos-thumbnail-01-puzzle-selection.png`
- `ultimatepuzzlesdinos-thumbnail-02-dino-puzzle.png`
- `ultimatepuzzlesdinos-thumbnail-03-solution-validation.png`
- `ultimatepuzzlesdinos-thumbnail-04-level-progression.png`

## Architecture

The main logic is organized inside `Assets/Puzzles/Scripts` and divided into several areas:

- **Board / Puzzle Core** — board, cells, cell groups, content, puzzle logic and validation.
- **Level Management** — level loading, selection and progression.
- **Sound** — sound and music control.
- **Ads** — ad integration and management.
- **Base** — utilities, events and support tools.

Highlighted project scripts:

- `Game` — general game control.
- `LvlManager` — level and progression management.
- `Board` — base board structure.
- `Board_Puzzle` — puzzle-specific behavior.
- `Board_PuzzleLogic` — puzzle validation and logic.
- `Board_Mosaic` — board/mosaic visual behavior.
- `Cell` — base puzzle unit.
- `CellDraw` — cell rendering or drawing.
- `CellGroup` — grouping of pieces or cells.
- `Content` — puzzle-related content.
- `GameType` — game mode/type definition.
- `SoundManager` — sound and music.
- `AdsManager` — ad management.
- `EventDispatcher` — event system.
- `Lib` — general utilities.
- `TextureScale` — texture processing or scaling.

## Recommended code to review

- [`Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/Game.cs`](./Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/Game.cs)
- [`Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/LvlManager.cs`](./Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/LvlManager.cs)
- [`Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/Board.cs`](./Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/Board.cs)
- [`Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/Board_Puzzle.cs`](./Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/Board_Puzzle.cs)
- [`Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/Board_PuzzleLogic.cs`](./Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/Board_PuzzleLogic.cs)
- [`Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/Cell.cs`](./Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/Cell.cs)
- [`Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/CellGroup.cs`](./Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/CellGroup.cs)
- [`Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/SoundManager.cs`](./Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/SoundManager.cs)
- [`Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/Ads/AdsManager.cs`](./Project/PRJ_UltimatePuzzles/Assets/Puzzles/Scripts/Ads/AdsManager.cs)

## Build

The build is available through GitHub Releases.

[Download build U007-v1.0.0](https://github.com/BLRochaGonzalez93/U007_Ultimate_Puzzles_Dinos/releases/tag/U007-v1.0.0)

## Status

**Developed game pending publication.**

The project includes a functional puzzle game with more than 50 levels, level system, piece selection and placement, solution validation, correct/incorrect feedback, main UI, main menu, level selection menu, sound and music.

It is currently being prepared for publication on Google Play Store through the integration of Ads, IAP and Analytics.

Possible pending improvements:

- Add progress saving.
- Add level unlocking.
- Add a scoring system.
- Add a timer.
- Improve visual feedback.
- Improve audio feedback.
- Add Ads.
- Add IAP.
- Add Analytics.

## Learnings

This project allowed me to work on UI-based puzzle design, with interaction through selection, drag and drop, and solution validation.

It also helped me practice level management, progression, correct/incorrect feedback and content organization by theme.

In addition, the project helped me structure dinosaur and level data, using ScriptableObjects and an architecture based on board, cells, groups and validation logic.

Finally, the project allowed me to move closer to a mobile product approach, preparing Ads, IAP, Analytics and a future Google Play Store release.
