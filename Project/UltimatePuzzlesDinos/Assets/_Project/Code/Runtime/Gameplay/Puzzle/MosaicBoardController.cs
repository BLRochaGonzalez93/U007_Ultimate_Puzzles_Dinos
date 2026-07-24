using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.Audio;
using VRMGames.UltimatePuzzlesDinos.Content;
using VRMGames.UltimatePuzzlesDinos.Haptics;

namespace VRMGames.UltimatePuzzlesDinos.Gameplay.Puzzle
{
    [DisallowMultipleComponent]
    public sealed class MosaicBoardController : MonoBehaviour
    {
        [SerializeField] private PuzzleBoardController layoutSource;

        public event Action<PuzzleCompletionResult> PuzzleCompleted;

        private readonly List<Sprite> runtimePieceSprites = new();
        private readonly List<MosaicTileView> tiles = new();

        private RectTransform blankCell;
        private MosaicTileView[] positionContents;
        private Sprite sourcePreviewSprite;
        private int columns;
        private int rows;
        private int pieceCount;
        private int blankPosition;
        private int moveCount;
        private float startedAt;
        private bool completed;

        public void Initialize(PuzzleBoardController source)
        {
            layoutSource = source;
        }

        private void Start()
        {
            if (PuzzleSession.SelectedMode == PuzzleMode.Mosaic)
            {
                BuildBoard();
            }
        }

        private void OnDestroy()
        {
            DestroyRuntimeSprites();
        }

        public void RestartBoard()
        {
            ClearGeneratedContent();
            BuildBoard();
        }

        private void BuildBoard()
        {
            if (layoutSource == null)
            {
                Debug.LogError("[Mosaic] Missing layout source.", this);
                return;
            }

            moveCount = 0;
            completed = false;
            startedAt = Time.unscaledTime;
            layoutSource.CompletionPanel?.SetActive(false);

            PuzzleDifficultyInfo difficulty =
                PuzzleDifficultyCatalog.Get(PuzzleSession.SelectedDifficulty);

            columns = difficulty.Columns;
            rows = difficulty.Rows;
            pieceCount = columns * rows;
            blankPosition = pieceCount - 1;
            positionContents = new MosaicTileView[pieceCount];

            PuzzleDefinition definition = layoutSource.PuzzleCatalog != null
                ? layoutSource.PuzzleCatalog.GetByLevelNumber(
                    PuzzleSession.SelectedLevelId)
                : null;

            sourcePreviewSprite = definition != null ? definition.Image : null;

            Color fallbackBase = definition != null
                ? definition.FallbackColor
                : new Color(0.25f, 0.62f, 0.28f, 1f);

            Sprite[] slicedSprites =
                CreatePieceSprites(sourcePreviewSprite, columns, rows);

            ConfigureBoardGrid();
            CreateTiles(slicedSprites, fallbackBase);
            CreateBlankCell();
            CreateInstructions();
            ShuffleBoard();
            RefreshMovableHighlights();
            UpdateStatus();
        }

        private void ConfigureBoardGrid()
        {
            RectTransform boardRoot = layoutSource.BoardRoot;
            GridLayoutGroup grid = boardRoot.GetComponent<GridLayoutGroup>();

            if (grid == null)
            {
                grid = boardRoot.gameObject.AddComponent<GridLayoutGroup>();
            }

            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.spacing = new Vector2(4f, 4f);
            grid.childAlignment = TextAnchor.MiddleCenter;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;

            float width = boardRoot.rect.width > 0f
                ? boardRoot.rect.width
                : 690f;

            float height = boardRoot.rect.height > 0f
                ? boardRoot.rect.height
                : 690f;

            float cellSize = Mathf.Floor(Mathf.Min(
                (width - grid.spacing.x * (columns - 1)) / columns,
                (height - grid.spacing.y * (rows - 1)) / rows));

            grid.cellSize = new Vector2(cellSize, cellSize);
        }

        private void CreateTiles(Sprite[] slicedSprites, Color fallbackBase)
        {
            tiles.Clear();

            for (int tileIndex = 0; tileIndex < pieceCount - 1; tileIndex++)
            {
                GameObject tileObject = new(
                    $"MosaicTile_{tileIndex:00}",
                    typeof(RectTransform),
                    typeof(Image),
                    typeof(MosaicTileView));

                RectTransform tileRect =
                    tileObject.GetComponent<RectTransform>();

                tileRect.SetParent(layoutSource.BoardRoot, false);

                Text label = CreateLabel(
                    tileObject.transform,
                    GetPieceLabel(tileIndex, columns));

                label.raycastTarget = false;

                MosaicTileView tile =
                    tileObject.GetComponent<MosaicTileView>();

                tile.Initialize(
                    tileIndex,
                    slicedSprites != null ? slicedSprites[tileIndex] : null,
                    GetFallbackPieceColor(
                        fallbackBase,
                        tileIndex,
                        columns,
                        rows),
                    GetPieceLabel(tileIndex, columns),
                    HandleTileClicked);

                tiles.Add(tile);
                positionContents[tileIndex] = tile;
            }
        }

        private void CreateBlankCell()
        {
            GameObject blankObject = new(
                "MosaicBlankCell",
                typeof(RectTransform),
                typeof(Image));

            blankCell = blankObject.GetComponent<RectTransform>();
            blankCell.SetParent(layoutSource.BoardRoot, false);

            Image image = blankObject.GetComponent<Image>();
            image.color = new Color(0.04f, 0.05f, 0.04f, 0.92f);
            image.raycastTarget = false;

            GameObject markerObject = new(
                "BlankMarker",
                typeof(RectTransform),
                typeof(Text));

            RectTransform markerRect =
                markerObject.GetComponent<RectTransform>();

            markerRect.SetParent(blankCell, false);
            markerRect.anchorMin = Vector2.zero;
            markerRect.anchorMax = Vector2.one;
            markerRect.offsetMin = Vector2.zero;
            markerRect.offsetMax = Vector2.zero;

            Text marker = markerObject.GetComponent<Text>();
            marker.font =
                Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            marker.text = "HUECO";
            marker.resizeTextForBestFit = true;
            marker.resizeTextMinSize = 10;
            marker.resizeTextMaxSize = 24;
            marker.alignment = TextAnchor.MiddleCenter;
            marker.color = new Color(1f, 1f, 1f, 0.22f);
            marker.raycastTarget = false;
        }

        private void CreateInstructions()
        {
            RectTransform trayRoot = layoutSource.TrayRoot;

            GameObject titleObject = new(
                "MosaicInstructions",
                typeof(RectTransform),
                typeof(Text));

            RectTransform titleRect =
                titleObject.GetComponent<RectTransform>();

            titleRect.SetParent(trayRoot, false);
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0f, -24f);
            titleRect.sizeDelta = new Vector2(620f, 130f);

            Text title = titleObject.GetComponent<Text>();
            title.font =
                Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            title.text =
                "TOCA UNA PIEZA ILUMINADA\n" +
                "PARA MOVERLA AL HUECO";
            title.fontSize = 27;
            title.fontStyle = FontStyle.Bold;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = new Color(0.56f, 0.92f, 0.32f, 1f);
            title.raycastTarget = false;

            GameObject previewObject = new(
                "MosaicPreview",
                typeof(RectTransform),
                typeof(Image));

            RectTransform previewRect =
                previewObject.GetComponent<RectTransform>();

            previewRect.SetParent(trayRoot, false);
            previewRect.anchorMin = new Vector2(0.5f, 0.5f);
            previewRect.anchorMax = new Vector2(0.5f, 0.5f);
            previewRect.pivot = new Vector2(0.5f, 0.5f);
            previewRect.anchoredPosition = new Vector2(0f, -35f);
            previewRect.sizeDelta = new Vector2(300f, 300f);

            Image preview = previewObject.GetComponent<Image>();
            preview.sprite = sourcePreviewSprite;
            preview.preserveAspect = true;
            preview.raycastTarget = false;
            preview.color = sourcePreviewSprite != null
                ? new Color(1f, 1f, 1f, 0.72f)
                : new Color(0.22f, 0.32f, 0.18f, 0.85f);

            GameObject previewLabelObject = new(
                "PreviewLabel",
                typeof(RectTransform),
                typeof(Text));

            RectTransform previewLabelRect =
                previewLabelObject.GetComponent<RectTransform>();

            previewLabelRect.SetParent(trayRoot, false);
            previewLabelRect.anchorMin = new Vector2(0.5f, 0f);
            previewLabelRect.anchorMax = new Vector2(0.5f, 0f);
            previewLabelRect.pivot = new Vector2(0.5f, 0f);
            previewLabelRect.anchoredPosition = new Vector2(0f, 28f);
            previewLabelRect.sizeDelta = new Vector2(500f, 54f);

            Text previewLabel = previewLabelObject.GetComponent<Text>();
            previewLabel.font =
                Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            previewLabel.text = "IMAGEN OBJETIVO";
            previewLabel.fontSize = 22;
            previewLabel.fontStyle = FontStyle.Bold;
            previewLabel.alignment = TextAnchor.MiddleCenter;
            previewLabel.color = new Color(1f, 0.78f, 0.12f, 1f);
            previewLabel.raycastTarget = false;
        }

        private void ShuffleBoard()
        {
            int shuffleMoves = GetShuffleMoveCount();
            int previousBlank = -1;

            for (int iteration = 0; iteration < shuffleMoves; iteration++)
            {
                List<int> neighbours =
                    GetAdjacentPositions(blankPosition);

                if (neighbours.Count > 1)
                {
                    neighbours.Remove(previousBlank);
                }

                int selected =
                    neighbours[UnityEngine.Random.Range(0, neighbours.Count)];

                previousBlank = blankPosition;
                MovePositionIntoBlank(selected, false);
            }

            if (IsSolved())
            {
                List<int> neighbours =
                    GetAdjacentPositions(blankPosition);

                MovePositionIntoBlank(neighbours[0], false);
            }

            RefreshSiblingOrder();
        }

        private int GetShuffleMoveCount()
        {
            return PuzzleSession.SelectedDifficulty switch
            {
                PuzzleDifficulty.Easy => 12,
                PuzzleDifficulty.Normal => 28,
                PuzzleDifficulty.Hard => 55,
                PuzzleDifficulty.Expert => 90,
                _ => 12
            };
        }

        private void HandleTileClicked(MosaicTileView tile)
        {
            if (completed || tile == null)
            {
                return;
            }

            int tilePosition = FindTilePosition(tile);
            if (tilePosition < 0)
            {
                return;
            }

            if (!AreAdjacent(tilePosition, blankPosition))
            {
                AudioService.PlaySfx(AudioCue.PieceIncorrect);
                HapticService.Play(HapticCue.Error);
                return;
            }

            MovePositionIntoBlank(tilePosition, true);
        }

        private void MovePositionIntoBlank(int tilePosition, bool countMove)
        {
            MosaicTileView tile = positionContents[tilePosition];
            if (tile == null)
            {
                return;
            }

            int oldBlankPosition = blankPosition;
            positionContents[oldBlankPosition] = tile;
            positionContents[tilePosition] = null;
            blankPosition = tilePosition;

            if (countMove)
            {
                moveCount++;
                AudioService.PlaySfx(AudioCue.PieceCorrect);
                HapticService.Play(HapticCue.Selection);
            }

            RefreshSiblingOrder();
            RefreshMovableHighlights();
            UpdateStatus();

            if (countMove && IsSolved())
            {
                CompletePuzzle();
            }
        }

        private void RefreshSiblingOrder()
        {
            for (int position = 0; position < positionContents.Length; position++)
            {
                MosaicTileView tile = positionContents[position];

                if (tile != null)
                {
                    tile.transform.SetSiblingIndex(position);
                }
                else if (blankCell != null)
                {
                    blankCell.SetSiblingIndex(position);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(
                layoutSource.BoardRoot);
        }

        private void RefreshMovableHighlights()
        {
            HashSet<int> movablePositions =
                new(GetAdjacentPositions(blankPosition));

            for (int position = 0; position < positionContents.Length; position++)
            {
                MosaicTileView tile = positionContents[position];
                if (tile == null)
                {
                    continue;
                }

                Image image = tile.GetComponent<Image>();
                if (image == null)
                {
                    continue;
                }

                bool movable = movablePositions.Contains(position);
                image.color = movable
                    ? new Color(1f, 1f, 1f, 1f)
                    : new Color(0.72f, 0.72f, 0.72f, 1f);

                tile.transform.localScale = movable
                    ? Vector3.one * 0.96f
                    : Vector3.one;
            }
        }

        private bool IsSolved()
        {
            for (int position = 0; position < pieceCount - 1; position++)
            {
                MosaicTileView tile = positionContents[position];

                if (tile == null || tile.TileIndex != position)
                {
                    return false;
                }
            }

            return blankPosition == pieceCount - 1;
        }

        private void CompletePuzzle()
        {
            completed = true;

            int previousBestStars = ProgressService.GetStars(
                PuzzleSession.SelectedMode,
                PuzzleSession.SelectedLevelId);

            int earnedStars = Mathf.Clamp(
                (int)PuzzleSession.SelectedDifficulty + 1,
                1,
                4);

            bool improvedBest = ProgressService.RecordCompletion(
                PuzzleSession.SelectedMode,
                PuzzleSession.SelectedLevelId,
                PuzzleSession.SelectedDifficulty);

            int bestStars = ProgressService.GetStars(
                PuzzleSession.SelectedMode,
                PuzzleSession.SelectedLevelId);

            if (layoutSource.StatusLabel != null)
            {
                layoutSource.StatusLabel.text =
                    $"MOSAICO COMPLETADO · {BuildStarsText(bestStars)}";
            }

            AudioService.PlaySfx(AudioCue.PuzzleCompleted);
            HapticService.Play(HapticCue.Completion);

            PuzzleCompleted?.Invoke(new PuzzleCompletionResult(
                PuzzleSession.SelectedMode,
                PuzzleSession.SelectedLevelId,
                PuzzleSession.SelectedDifficulty,
                earnedStars,
                previousBestStars,
                bestStars,
                improvedBest,
                moveCount,
                Mathf.Max(0f, Time.unscaledTime - startedAt)));
        }

        private void UpdateStatus()
        {
            if (layoutSource == null || layoutSource.StatusLabel == null)
            {
                return;
            }

            layoutSource.StatusLabel.text =
                $"MOSAICO {columns} × {rows}" +
                $"   ·   MOVIMIENTOS: {moveCount}";
        }

        private int FindTilePosition(MosaicTileView tile)
        {
            for (int position = 0; position < positionContents.Length; position++)
            {
                if (positionContents[position] == tile)
                {
                    return position;
                }
            }

            return -1;
        }

        private bool AreAdjacent(int first, int second)
        {
            int firstRow = first / columns;
            int firstColumn = first % columns;
            int secondRow = second / columns;
            int secondColumn = second % columns;

            return Mathf.Abs(firstRow - secondRow) +
                Mathf.Abs(firstColumn - secondColumn) == 1;
        }

        private List<int> GetAdjacentPositions(int position)
        {
            List<int> positions = new();
            int row = position / columns;
            int column = position % columns;

            if (row > 0)
            {
                positions.Add(position - columns);
            }

            if (row < rows - 1)
            {
                positions.Add(position + columns);
            }

            if (column > 0)
            {
                positions.Add(position - 1);
            }

            if (column < columns - 1)
            {
                positions.Add(position + 1);
            }

            return positions;
        }

        private Sprite[] CreatePieceSprites(
            Sprite source,
            int targetColumns,
            int targetRows)
        {
            if (source == null || source.texture == null)
            {
                return null;
            }

            Rect sourceRect = source.textureRect;
            float pieceWidth = sourceRect.width / targetColumns;
            float pieceHeight = sourceRect.height / targetRows;
            Sprite[] result = new Sprite[targetColumns * targetRows];

            for (int row = 0; row < targetRows; row++)
            {
                for (int column = 0; column < targetColumns; column++)
                {
                    int index = row * targetColumns + column;
                    float x = sourceRect.x + column * pieceWidth;
                    float y = sourceRect.y +
                        (targetRows - 1 - row) * pieceHeight;

                    Sprite pieceSprite = Sprite.Create(
                        source.texture,
                        new Rect(x, y, pieceWidth, pieceHeight),
                        new Vector2(0.5f, 0.5f),
                        source.pixelsPerUnit,
                        0,
                        SpriteMeshType.FullRect,
                        Vector4.zero,
                        false);

                    pieceSprite.name =
                        $"{source.name}_MosaicTile_{index:00}";

                    result[index] = pieceSprite;
                    runtimePieceSprites.Add(pieceSprite);
                }
            }

            return result;
        }

        private static Text CreateLabel(Transform parent, string value)
        {
            GameObject labelObject = new(
                "Label",
                typeof(RectTransform),
                typeof(Text));

            RectTransform rect =
                labelObject.GetComponent<RectTransform>();

            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Text label = labelObject.GetComponent<Text>();
            label.font =
                Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = value;
            label.fontSize = 24;
            label.fontStyle = FontStyle.Bold;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            return label;
        }

        private static string GetPieceLabel(int index, int targetColumns)
        {
            int row = index / targetColumns;
            int column = index % targetColumns;
            return $"{(char)('A' + row)}{column + 1}";
        }

        private static Color GetFallbackPieceColor(
            Color baseColor,
            int index,
            int targetColumns,
            int targetRows)
        {
            int row = index / targetColumns;
            int column = index % targetColumns;

            Color.RGBToHSV(
                baseColor,
                out float hue,
                out float saturation,
                out float value);

            hue = Mathf.Repeat(
                hue +
                column / Mathf.Max(1f, targetColumns) * 0.13f +
                row / Mathf.Max(1f, targetRows) * 0.07f,
                1f);

            saturation = Mathf.Clamp01(
                saturation * (0.88f + 0.12f * (row % 2)));

            value = Mathf.Clamp01(
                value * (0.88f + 0.12f * (column % 2)));

            return Color.HSVToRGB(hue, saturation, value);
        }

        private static string BuildStarsText(int stars)
        {
            int value = Mathf.Clamp(stars, 0, 4);
            return new string('★', value) +
                new string('☆', 4 - value);
        }

        private void ClearGeneratedContent()
        {
            completed = false;
            blankCell = null;
            positionContents = null;
            tiles.Clear();

            DestroyRuntimeSprites();

            DestroyChildren(
                layoutSource != null ? layoutSource.BoardRoot : null);

            DestroyChildren(
                layoutSource != null ? layoutSource.TrayRoot : null);
        }

        private void DestroyRuntimeSprites()
        {
            for (int index = runtimePieceSprites.Count - 1;
                 index >= 0;
                 index--)
            {
                if (runtimePieceSprites[index] != null)
                {
                    Destroy(runtimePieceSprites[index]);
                }
            }

            runtimePieceSprites.Clear();
        }

        private static void DestroyChildren(RectTransform root)
        {
            if (root == null)
            {
                return;
            }

            for (int index = root.childCount - 1; index >= 0; index--)
            {
                Destroy(root.GetChild(index).gameObject);
            }
        }
    }
}
