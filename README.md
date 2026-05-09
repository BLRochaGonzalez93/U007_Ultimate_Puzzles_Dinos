# U007_Ultimate_Puzzles_Dinos

[English](README.en.md) | [Español](README.md)

## Resumen

**Ultimate Puzzles Dinos** es un juego de puzzles de piezas desarrollado en Unity con C#, con temática de dinosaurios y enfoque casual/infantil. Cada puzzle funciona como un nivel independiente, presentado con vista 2D desde arriba y una estructura basada principalmente en UI.

El jugador selecciona, arrastra y coloca piezas para completar cada puzzle. El objetivo es resolver la imagen correctamente para avanzar por la colección de niveles. El proyecto cuenta con más de 50 niveles y está en proceso de preparación para publicación en Google Play Store, incorporando sistemas de monetización y análisis como Ads, IAP y Analytics.

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

- Juego de puzzles de piezas.
- Temática de dinosaurios.
- Enfoque casual e infantil.
- Vista 2D desde arriba.
- Gameplay basado principalmente en UI.
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
- Objetivo de publicación en Google Play Store.
- Build Android publicada mediante GitHub Releases.
- Versión Windows prevista para futuras builds.

## Visuales

> Pendiente de añadir capturas e imágenes finales.

Nombres previstos para el pack visual:

- `ultimatepuzzlesdinos-logo.png`
- `ultimatepuzzlesdinos-cover.png`
- `ultimatepuzzlesdinos-banner.png`
- `ultimatepuzzlesdinos-thumbnail-01-puzzle-selection.png`
- `ultimatepuzzlesdinos-thumbnail-02-dino-puzzle.png`
- `ultimatepuzzlesdinos-thumbnail-03-solution-validation.png`
- `ultimatepuzzlesdinos-thumbnail-04-level-progression.png`

## Arquitectura

La lógica principal se organiza dentro de `Assets/Puzzles/Scripts` y se divide en varias áreas:

- **Board / Puzzle Core** — tablero, celdas, grupos de celdas, contenido, lógica de puzzle y validación.
- **Level Management** — carga, selección y progresión de niveles.
- **Sound** — control de sonido y música.
- **Ads** — integración y gestión de anuncios.
- **Base** — utilidades, eventos y herramientas de soporte.

Scripts destacados del proyecto:

- `Game` — control general del juego.
- `LvlManager` — gestión de niveles y progresión.
- `Board` — estructura base del tablero.
- `Board_Puzzle` — comportamiento específico de puzzle.
- `Board_PuzzleLogic` — validación y lógica del puzzle.
- `Board_Mosaic` — comportamiento visual del tablero/mosaico.
- `Cell` — unidad base del puzzle.
- `CellDraw` — representación o dibujo de celda.
- `CellGroup` — agrupación de piezas o celdas.
- `Content` — contenido asociado al puzzle.
- `GameType` — definición o tipo de modo de juego.
- `SoundManager` — sonido y música.
- `AdsManager` — gestión de anuncios.
- `EventDispatcher` — sistema de eventos.
- `Lib` — utilidades generales.
- `TextureScale` — procesamiento o escalado de texturas.

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

[Descargar build U007-v1.0.0](https://github.com/BLRochaGonzalez93/U007_Ultimate_Puzzles_Dinos/releases/tag/U007-v1.0.0)

## Estado

**Juego desarrollado pendiente de publicación.**

El proyecto cuenta con un juego de puzzles funcional con más de 50 niveles, sistema de niveles, selección y colocación de piezas, validación de solución, feedback de acierto/error, UI principal, menú principal, menú de niveles, sonido y música.

Actualmente está en proceso de preparación para publicación en Google Play Store mediante la integración de Ads, IAP y Analytics.

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

Este proyecto me permitió trabajar el diseño de puzzles basados en UI, con interacción mediante selección, arrastrar y soltar, y validación de soluciones.

También me sirvió para practicar gestión de niveles, progresión, feedback de acierto/error y organización de contenido por temática.

Además, el proyecto me ayudó a estructurar datos de dinosaurios y niveles, utilizando ScriptableObjects y una arquitectura basada en tablero, celdas, grupos y lógica de validación.

Por último, el proyecto me permitió avanzar hacia un enfoque más cercano a producto móvil, preparando integración de Ads, IAP, Analytics y una futura publicación en Google Play Store.
