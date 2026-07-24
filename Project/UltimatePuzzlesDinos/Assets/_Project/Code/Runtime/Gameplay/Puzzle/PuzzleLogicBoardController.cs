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
    public sealed class PuzzleLogicBoardController : MonoBehaviour
    {
        [SerializeField] private PuzzleBoardController layoutSource;

        public event Action<PuzzleCompletionResult> PuzzleCompleted;

        private readonly List<Sprite> runtimePieceSprites = new();
        private readonly List<int> pieceOrder = new();
        private RectTransform[] targets;
        private Sprite[] slicedSprites;
        private Sprite sourcePreviewSprite;
        private PuzzlePieceView activePiece;
        private RectTransform activeHomeSlot;
        private Color fallbackBase;
        private int columns;
        private int rows;
        private int pieceCount;
        private int currentOrderIndex;
        private int placedCount;
        private int moveCount;
        private float startedAt;

        public void Initialize(PuzzleBoardController source)
        {
            layoutSource = source;
        }

        private void Start()
        {
            if (PuzzleSession.SelectedMode == PuzzleMode.Logic)
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
                Debug.LogError("[Puzzle Logic] Missing layout source.", this);
                return;
            }

            placedCount = 0;
            moveCount = 0;
            currentOrderIndex = 0;
            startedAt = Time.unscaledTime;
            layoutSource.CompletionPanel?.SetActive(false);

            PuzzleDifficultyInfo difficulty =
                PuzzleDifficultyCatalog.Get(PuzzleSession.SelectedDifficulty);
            columns = difficulty.Columns;
            rows = difficulty.Rows;
            pieceCount = columns * rows;

            PuzzleDefinition definition = layoutSource.PuzzleCatalog != null
                ? layoutSource.PuzzleCatalog.GetByLevelNumber(PuzzleSession.SelectedLevelId)
                : null;

            sourcePreviewSprite = definition != null ? definition.Image : null;
            fallbackBase = definition != null
                ? definition.FallbackColor
                : new Color(0.72f, 0.34f, 0.12f, 1f);

            slicedSprites = CreatePieceSprites(sourcePreviewSprite, columns, rows);
            ConfigureBoardGrid();
            CreateTargets();
            CreatePieceOrder();
            CreateTrayLayout();
            SpawnNextPiece();
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

            float width = boardRoot.rect.width > 0f ? boardRoot.rect.width : 690f;
            float height = boardRoot.rect.height > 0f ? boardRoot.rect.height : 690f;
            float cellSize = Mathf.Floor(Mathf.Min(
                (width - grid.spacing.x * (columns - 1)) / columns,
                (height - grid.spacing.y * (rows - 1)) / rows));
            grid.cellSize = new Vector2(cellSize, cellSize);
        }

        private void CreateTargets()
        {
            targets = new RectTransform[pieceCount];
            for (int index = 0; index < pieceCount; index++)
            {
                GameObject targetObject = new(
                    $"LogicTarget_{index:00}",
                    typeof(RectTransform),
                    typeof(Image));

                RectTransform target = targetObject.GetComponent<RectTransform>();
                target.SetParent(layoutSource.BoardRoot, false);

                Image image = targetObject.GetComponent<Image>();
                image.color = new Color(0.08f, 0.09f, 0.07f, 0.92f);
                image.raycastTarget = false;
                targets[index] = target;
            }
        }

        private void CreatePieceOrder()
        {
            pieceOrder.Clear();
            for (int index = 0; index < pieceCount; index++)
            {
                pieceOrder.Add(index);
            }

            for (int index = pieceOrder.Count - 1; index > 0; index--)
            {
                int swapIndex = UnityEngine.Random.Range(0, index + 1);
                (pieceOrder[index], pieceOrder[swapIndex]) =
                    (pieceOrder[swapIndex], pieceOrder[index]);
            }
        }

        private void CreateTrayLayout()
        {
            RectTransform trayRoot = layoutSource.TrayRoot;

            GameObject instructionObject = new(
                "LogicInstruction",
                typeof(RectTransform),
                typeof(Text));

            RectTransform instructionRect =
                instructionObject.GetComponent<RectTransform>();
            instructionRect.SetParent(trayRoot, false);
            instructionRect.anchorMin = new Vector2(0.5f, 1f);
            instructionRect.anchorMax = new Vector2(0.5f, 1f);
            instructionRect.pivot = new Vector2(0.5f, 1f);
            instructionRect.anchoredPosition = new Vector2(0f, -24f);
            instructionRect.sizeDelta = new Vector2(620f, 90f);

            Text instruction = instructionObject.GetComponent<Text>();
            instruction.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            instruction.text = "COLOCA LA PIEZA ACTIVA\nDONDE CORRESPONDA";
            instruction.fontSize = 24;
            instruction.fontStyle = FontStyle.Bold;
            instruction.alignment = TextAnchor.MiddleCenter;
            instruction.color = new Color(1f, 0.78f, 0.12f, 1f);
            instruction.raycastTarget = false;

            CreatePreview(trayRoot);
            CreateActiveHomeSlot(trayRoot);
        }

        private void CreatePreview(RectTransform trayRoot)
        {
            GameObject previewObject = new(
                "LogicPreview",
                typeof(RectTransform),
                typeof(Image));

            RectTransform previewRect = previewObject.GetComponent<RectTransform>();
            previewRect.SetParent(trayRoot, false);
            previewRect.anchorMin = new Vector2(0.5f, 1f);
            previewRect.anchorMax = new Vector2(0.5f, 1f);
            previewRect.pivot = new Vector2(0.5f, 1f);
            previewRect.anchoredPosition = new Vector2(0f, -128f);
            previewRect.sizeDelta = new Vector2(250f, 250f);

            Image preview = previewObject.GetComponent<Image>();
            preview.sprite = sourcePreviewSprite;
            preview.preserveAspect = true;
            preview.raycastTarget = false;
            preview.color = sourcePreviewSprite != null
                ? new Color(1f, 1f, 1f, 0.80f)
                : new Color(0.20f, 0.20f, 0.20f, 0.82f);

            GameObject previewLabelObject = new(
                "LogicPreviewLabel",
                typeof(RectTransform),
                typeof(Text));

            RectTransform previewLabelRect =
                previewLabelObject.GetComponent<RectTransform>();
            previewLabelRect.SetParent(trayRoot, false);
            previewLabelRect.anchorMin = new Vector2(0.5f, 1f);
            previewLabelRect.anchorMax = new Vector2(0.5f, 1f);
            previewLabelRect.pivot = new Vector2(0.5f, 1f);
            previewLabelRect.anchoredPosition = new Vector2(0f, -390f);
            previewLabelRect.sizeDelta = new Vector2(360f, 40f);

            Text previewLabel = previewLabelObject.GetComponent<Text>();
            previewLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            previewLabel.text = "IMAGEN OBJETIVO";
            previewLabel.fontSize = 20;
            previewLabel.fontStyle = FontStyle.Bold;
            previewLabel.alignment = TextAnchor.MiddleCenter;
            previewLabel.color = new Color(1f, 1f, 1f, 0.92f);
            previewLabel.raycastTarget = false;
        }

        private void CreateActiveHomeSlot(RectTransform trayRoot)
        {
            GameObject slotObject = new(
                "LogicActivePieceSlot",
                typeof(RectTransform),
                typeof(Image));

            activeHomeSlot = slotObject.GetComponent<RectTransform>();
            activeHomeSlot.SetParent(trayRoot, false);
            activeHomeSlot.anchorMin = new Vector2(0.5f, 0f);
            activeHomeSlot.anchorMax = new Vector2(0.5f, 0f);
            activeHomeSlot.pivot = new Vector2(0.5f, 0f);

            GridLayoutGroup boardGrid = layoutSource.BoardRoot.GetComponent<GridLayoutGroup>();
            float pieceSize = boardGrid != null
                ? Mathf.Clamp(boardGrid.cellSize.x * 1.30f, 96f, 220f)
                : 190f;

            activeHomeSlot.sizeDelta = new Vector2(pieceSize, pieceSize);
            activeHomeSlot.anchoredPosition = new Vector2(0f, 28f);

            Image slotImage = slotObject.GetComponent<Image>();
            slotImage.color = new Color(0.10f, 0.09f, 0.07f, 0.78f);
            slotImage.raycastTarget = false;
        }

        private void SpawnNextPiece()
        {
            if (currentOrderIndex >= pieceOrder.Count)
            {
                CompletePuzzle();
                return;
            }

            int pieceIndex = pieceOrder[currentOrderIndex];
            GameObject pieceObject = new(
                $"LogicPiece_{pieceIndex:00}",
                typeof(RectTransform),
                typeof(Image),
                typeof(CanvasGroup),
                typeof(PuzzlePieceView));

            RectTransform pieceRect = pieceObject.GetComponent<RectTransform>();
            pieceRect.SetParent(activeHomeSlot, false);

            Text label = CreateLabel(pieceObject.transform, GetPieceLabel(pieceIndex, columns));
            label.raycastTarget = false;

            activePiece = pieceObject.GetComponent<PuzzlePieceView>();
            activePiece.Initialize(
                pieceIndex,
                layoutSource.RootCanvas,
                activeHomeSlot,
                targets[pieceIndex],
                slicedSprites != null ? slicedSprites[pieceIndex] : null,
                GetFallbackPieceColor(fallbackBase, pieceIndex, columns, rows),
                OnPiecePlaced,
                OnMoveCompleted);
        }

        private void OnPiecePlaced(PuzzlePieceView piece)
        {
            placedCount++;
            currentOrderIndex++;
            activePiece = null;
            UpdateStatus();

            if (placedCount >= pieceCount)
            {
                CompletePuzzle();
                return;
            }

            SpawnNextPiece();
        }

        private void OnMoveCompleted(bool correctPlacement)
        {
            moveCount++;
            UpdateStatus();
        }

        private void CompletePuzzle()
        {
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
                    $"LOGICA COMPLETADA · {BuildStarsText(bestStars)}";
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

            int remaining = Mathf.Max(0, pieceCount - placedCount);
            layoutSource.StatusLabel.text =
                $"COLOCADAS: {placedCount} / {pieceCount}" +
                $"   ·   RESTANTES: {remaining}" +
                $"   ·   MOVIMIENTOS: {moveCount}";
        }

        private Sprite[] CreatePieceSprites(Sprite source, int targetColumns, int targetRows)
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
                    float y = sourceRect.y + (targetRows - 1 - row) * pieceHeight;

                    Sprite pieceSprite = Sprite.Create(
                        source.texture,
                        new Rect(x, y, pieceWidth, pieceHeight),
                        new Vector2(0.5f, 0.5f),
                        source.pixelsPerUnit,
                        0,
                        SpriteMeshType.FullRect,
                        Vector4.zero,
                        false);

                    pieceSprite.name = $"{source.name}_LogicPiece_{index:00}";
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

            RectTransform rect = labelObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Text label = labelObject.GetComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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

            Color.RGBToHSV(baseColor, out float hue, out float saturation, out float value);
            hue = Mathf.Repeat(
                hue +
                column / Mathf.Max(1f, targetColumns) * 0.16f +
                row / Mathf.Max(1f, targetRows) * 0.05f,
                1f);
            saturation = Mathf.Clamp01(saturation * (0.85f + 0.15f * (row % 2)));
            value = Mathf.Clamp01(value * (0.86f + 0.14f * (column % 2)));
            return Color.HSVToRGB(hue, saturation, value);
        }

        private static string BuildStarsText(int stars)
        {
            int value = Mathf.Clamp(stars, 0, 4);
            return new string('★', value) + new string('☆', 4 - value);
        }

        private void ClearGeneratedContent()
        {
            activePiece = null;
            activeHomeSlot = null;
            targets = null;
            slicedSprites = null;
            pieceOrder.Clear();
            DestroyRuntimeSprites();
            DestroyChildren(layoutSource != null ? layoutSource.BoardRoot : null);
            DestroyChildren(layoutSource != null ? layoutSource.TrayRoot : null);
        }

        private void DestroyRuntimeSprites()
        {
            for (int index = runtimePieceSprites.Count - 1; index >= 0; index--)
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
