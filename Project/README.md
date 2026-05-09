# U007_Ultimate_Puzzles_Dinos

[English](README.en.md) | [Español](README.md)

## Resumen

Juego de puzzles de piezas desarrollado en Unity con C#, con temática de dinosaurios y público objetivo infantil/casual. El juego está construido sobre una vista 2D desde arriba y una experiencia principalmente basada en UI.

Cada puzzle funciona como un nivel independiente. El jugador selecciona, arrastra y coloca piezas para completar la imagen del dinosaurio o escena correspondiente. El juego cuenta con más de 50 niveles y está preparado para evolucionar hacia publicación en Google Play Store con Ads, IAP y Analytics.

## Tecnologías

- Unity
- C#
- Sistema de UI de Unity
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

## Características principales

- Puzzles de piezas.
- Vista 2D desde arriba.
- UI como base principal del gameplay.
- Temática de dinosaurios.
- Público objetivo infantil/casual.
- Selección de piezas.
- Arrastrar y soltar.
- Validación de solución.
- Sistema de niveles.
- Más de 50 niveles.
- Desbloqueo de niveles.
- Sistema de puntuación.
- Feedback de acierto/error.
- UI principal.
- Menú principal.
- Menú de niveles.
- Sonido y música.
- Preparación para Ads.
- Preparación para IAP.
- Preparación para Analytics.
- Build Android.
- Versión Windows prevista próximamente.

## Capturas

> Pendiente de añadir capturas finales.

Ruta prevista:

![Gameplay](./Media/screenshots/gameplay-01.png)

## Arquitectura

La lógica principal se organiza dentro de `Assets/Puzzles/Scripts`:

- `Board` — estructura base del tablero.
- `Board_Mosaic` — representación visual del tablero.
- `Board_Puzzle` — comportamiento principal del puzzle.
- `Board_PuzzleLogic` — validación de la solución y lógica interna.
- `Cell` — unidad base del puzzle.
- `CellDraw` — dibujo o representación de celdas.
- `CellGroup` — agrupación de celdas o piezas.
- `Content` — contenido asociado a puzzles.
- `Game` — control general de juego.
- `GameType` — tipo o modo de juego.
- `LvlManager` — gestión de niveles.
- `SoundManager` — sonido y música.
- `AdsManager` — gestión de anuncios.
- `EventDispatcher` — eventos globales.
- `Lib` — utilidades.
- `TextureScale` — escalado o procesamiento de texturas.

## Código recomendado para revisar

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

La build está disponible en GitHub Releases.

[`Releases/Download.md`](../Releases/Download.md)

[Descargar build U007-v1.0.0](https://github.com/BLRochaGonzalez93/U007_Ultimate_Puzzles_Dinos/releases/tag/U007-v1.0.0)

## Estado

**Juego desarrollado pendiente de publicación.**

El proyecto incluye más de 50 niveles de puzzles de piezas, sistema de niveles, selección de piezas, drag and drop, validación de solución, feedback de acierto/error, UI, menú principal, menú de niveles, sonido y música.

Pendiente de posibles mejoras:

- Añadir guardado de progreso.
- Añadir desbloqueo de niveles.
- Añadir sistema de puntuación.
- Añadir temporizador.
- Mejorar feedback visual.
- Mejorar feedback sonoro.
- Añadir Ads.
- Añadir IAP.
- Añadir Analytics.

## Aprendizajes

Este proyecto me permitió trabajar diseño de puzzles basados en UI, con selección de piezas, drag and drop y validación de soluciones.

También me ayudó a practicar gestión de niveles, feedback de acierto/error y flujo de menús.

Además, el proyecto me permitió organizar datos de dinosaurios y contenido de niveles mediante una arquitectura basada en tablero, celdas, grupos y lógica de puzzle.

Por último, el proyecto me sirvió para dar un paso hacia desarrollo móvil orientado a producto, con preparación para Ads, IAP, Analytics y publicación en Google Play Store.
