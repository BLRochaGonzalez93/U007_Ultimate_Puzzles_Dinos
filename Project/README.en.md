# U007_Ultimate_Puzzles_Dinos

[English](README.en.md) | [Español](README.md)

## Summary

Piece-based puzzle game developed in Unity with C#, featuring a dinosaur theme and a child-friendly/casual target audience. The game is built around a 2D top-down view and an experience mainly based on UI.

Each puzzle works as an independent level. The player selects, drags and places pieces to complete the corresponding dinosaur image or scene. The game includes more than 50 levels and is prepared to evolve toward publication on Google Play Store with Ads, IAP and Analytics.

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

- Piece-based puzzles.
- 2D top-down view.
- UI as the main gameplay foundation.
- Dinosaur theme.
- Child-friendly/casual target audience.
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
- Android build.
- Windows version planned soon.

## Screenshots

> Final screenshots pending.

Planned path:

![Gameplay](./Media/screenshots/gameplay-01.png)

## Architecture

The main logic is organized inside `Assets/Puzzles/Scripts`:

- `Board` — base board structure.
- `Board_Mosaic` — board visual representation.
- `Board_Puzzle` — main puzzle behavior.
- `Board_PuzzleLogic` — solution validation and internal logic.
- `Cell` — base puzzle unit.
- `CellDraw` — cell drawing or representation.
- `CellGroup` — grouping of cells or pieces.
- `Content` — puzzle-related content.
- `Game` — general game control.
- `GameType` — game type or mode.
- `LvlManager` — level management.
- `SoundManager` — sound and music.
- `AdsManager` — ad management.
- `EventDispatcher` — global events.
- `Lib` — utilities.
- `TextureScale` — texture scaling or processing.

More information:

[`Docs/Architecture.md`](./Docs/Architecture.md)

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

[`Releases/Download.md`](./Releases/Download.md)

[Download build U007-v1.0.0](https://github.com/BLRochaGonzalez93/U007_Ultimate_Puzzles_Dinos/releases/tag/U007-v1.0.0)

## Status

**Developed game pending publication.**

The project includes more than 50 piece-based puzzle levels, level system, piece selection, drag and drop, solution validation, correct/incorrect feedback, UI, main menu, level selection menu, sound and music.

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

This project allowed me to work on UI-based puzzle design, with piece selection, drag and drop and solution validation.

It also helped me practice level management, correct/incorrect feedback and menu flow.

In addition, the project allowed me to organize dinosaur data and level content through an architecture based on board, cells, groups and puzzle logic.

Finally, the project helped me take a step toward product-oriented mobile development, preparing Ads, IAP, Analytics and Google Play Store publication.
