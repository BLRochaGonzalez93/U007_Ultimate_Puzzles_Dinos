using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.Audio;
using VRMGames.UltimatePuzzlesDinos.Haptics;
using VRMGames.UltimatePuzzlesDinos.Content;

namespace VRMGames.UltimatePuzzlesDinos.Gameplay.Puzzle
{
    [DisallowMultipleComponent]
    public sealed class PuzzleBoardController : MonoBehaviour
    {
        [Header("Content")]
        [SerializeField] private PuzzleCatalog puzzleCatalog;

        [Header("Scene references")]
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private RectTransform boardRoot;
        [SerializeField] private RectTransform trayRoot;
        [SerializeField] private Text statusLabel;
        [SerializeField] private GameObject completionPanel;
        [SerializeField] private Button completionRestartButton;

        public event Action<PuzzleCompletionResult> PuzzleCompleted;

        public Canvas RootCanvas => rootCanvas;
        public RectTransform BoardRoot => boardRoot;
        public RectTransform TrayRoot => trayRoot;
        public Text StatusLabel => statusLabel;
        public GameObject CompletionPanel => completionPanel;
        public PuzzleCatalog PuzzleCatalog => puzzleCatalog;

        private readonly List<PuzzlePieceView> pieces = new();
        private readonly List<Sprite> runtimePieceSprites = new();
        private int placedCount;
        private int pieceCount;
        private int moveCount;
        private float startedAt;

        private void OnEnable()
        {
            completionRestartButton?.onClick.AddListener(RestartBoard);
        }

        private void OnDisable()
        {
            completionRestartButton?.onClick.RemoveListener(RestartBoard);
        }

        private void OnDestroy()
        {
            DestroyRuntimeSprites();
        }

        private void Start()
        {
            BuildBoard();
        }

        public void RestartBoard()
        {
            ClearGeneratedContent();
            BuildBoard();
        }

        private void BuildBoard()
        {
            completionPanel?.SetActive(false);
            placedCount = 0;
            moveCount = 0;
            startedAt = Time.unscaledTime;

            PuzzleDifficultyInfo difficulty = PuzzleDifficultyCatalog.Get(PuzzleSession.SelectedDifficulty);
            int columns = difficulty.Columns;
            int rows = difficulty.Rows;
            pieceCount = columns * rows;

            PuzzleDefinition definition = puzzleCatalog != null
                ? puzzleCatalog.GetByLevelNumber(PuzzleSession.SelectedLevelId)
                : null;
            Sprite sourceSprite = definition != null ? definition.Image : null;
            Color fallbackBase = definition != null ? definition.FallbackColor : new Color(0.72f, 0.34f, 0.12f, 1f);
            Sprite[] slicedSprites = CreatePieceSprites(sourceSprite, columns, rows);

            GridLayoutGroup boardGrid = ConfigureGrid(boardRoot, columns, 4f);
            GridLayoutGroup trayGrid = ConfigureGrid(trayRoot, columns, 6f);

            float boardWidth = boardRoot.rect.width > 0f ? boardRoot.rect.width : 690f;
            float boardHeight = boardRoot.rect.height > 0f ? boardRoot.rect.height : 690f;
            float cellSize = Mathf.Floor(Mathf.Min(
                (boardWidth - boardGrid.spacing.x * (columns - 1)) / columns,
                (boardHeight - boardGrid.spacing.y * (rows - 1)) / rows));
            boardGrid.cellSize = new Vector2(cellSize, cellSize);

            float trayWidth = trayRoot.rect.width > 0f ? trayRoot.rect.width : 690f;
            float trayHeight = trayRoot.rect.height > 0f ? trayRoot.rect.height : 620f;
            float trayCellSize = Mathf.Floor(Mathf.Min(
                (trayWidth - trayGrid.spacing.x * (columns - 1)) / columns,
                (trayHeight - trayGrid.spacing.y * (rows - 1)) / rows));
            trayGrid.cellSize = new Vector2(trayCellSize, trayCellSize);

            List<int> order = CreateShuffledOrder(pieceCount);
            RectTransform[] targets = new RectTransform[pieceCount];

            for (int index = 0; index < pieceCount; index++)
            {
                targets[index] = CreateTarget(index);
            }

            for (int trayIndex = 0; trayIndex < pieceCount; trayIndex++)
            {
                int pieceIndex = order[trayIndex];
                RectTransform homeSlot = CreateHomeSlot(trayIndex);
                PuzzlePieceView piece = CreatePiece(pieceIndex, columns);
                pieces.Add(piece);
                piece.Initialize(
                    pieceIndex,
                    rootCanvas,
                    homeSlot,
                    targets[pieceIndex],
                    slicedSprites != null ? slicedSprites[pieceIndex] : null,
                    GetFallbackPieceColor(fallbackBase, pieceIndex, columns, rows),
                    OnPiecePlaced,
                    OnMoveCompleted);
            }

            UpdateStatus();
        }

        private static GridLayoutGroup ConfigureGrid(RectTransform root, int columns, float spacing)
        {
            GridLayoutGroup grid = root.GetComponent<GridLayoutGroup>();
            if (grid == null) grid = root.gameObject.AddComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.spacing = new Vector2(spacing, spacing);
            grid.childAlignment = TextAnchor.MiddleCenter;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            return grid;
        }

        private RectTransform CreateTarget(int index)
        {
            GameObject targetObject = new($"Target_{index:00}", typeof(RectTransform), typeof(Image));
            RectTransform targetRect = targetObject.GetComponent<RectTransform>();
            targetRect.SetParent(boardRoot, false);
            Image image = targetObject.GetComponent<Image>();
            image.color = new Color(0.08f, 0.09f, 0.07f, 0.9f);
            image.raycastTarget = false;
            return targetRect;
        }

        private RectTransform CreateHomeSlot(int index)
        {
            GameObject slotObject = new($"Home_{index:00}", typeof(RectTransform), typeof(Image));
            RectTransform slotRect = slotObject.GetComponent<RectTransform>();
            slotRect.SetParent(trayRoot, false);
            Image image = slotObject.GetComponent<Image>();
            image.color = new Color(0.10f, 0.09f, 0.07f, 0.55f);
            image.raycastTarget = false;
            return slotRect;
        }

        private PuzzlePieceView CreatePiece(int index, int columns)
        {
            GameObject pieceObject = new($"Piece_{index:00}", typeof(RectTransform), typeof(Image), typeof(CanvasGroup), typeof(PuzzlePieceView));
            RectTransform rect = pieceObject.GetComponent<RectTransform>();
            rect.SetParent(trayRoot, false);

            Text label = CreateLabel(pieceObject.transform, GetPieceLabel(index, columns));
            label.raycastTarget = false;
            return pieceObject.GetComponent<PuzzlePieceView>();
        }

        private static Text CreateLabel(Transform parent, string value)
        {
            GameObject labelObject = new("Label", typeof(RectTransform), typeof(Text));
            RectTransform rect = labelObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Text label = labelObject.GetComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = value;
            label.fontSize = 22;
            label.fontStyle = FontStyle.Bold;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            return label;
        }

        private Sprite[] CreatePieceSprites(Sprite source, int columns, int rows)
        {
            if (source == null || source.texture == null) return null;

            Rect sourceRect = source.textureRect;
            float pieceWidth = sourceRect.width / columns;
            float pieceHeight = sourceRect.height / rows;
            Sprite[] result = new Sprite[columns * rows];

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    int index = row * columns + column;
                    float x = sourceRect.x + column * pieceWidth;
                    float y = sourceRect.y + (rows - 1 - row) * pieceHeight;
                    Rect pieceRect = new(x, y, pieceWidth, pieceHeight);
                    Sprite pieceSprite = Sprite.Create(
                        source.texture,
                        pieceRect,
                        new Vector2(0.5f, 0.5f),
                        source.pixelsPerUnit,
                        0,
                        SpriteMeshType.FullRect,
                        Vector4.zero,
                        false);
                    pieceSprite.name = $"{source.name}_Piece_{index:00}";
                    result[index] = pieceSprite;
                    runtimePieceSprites.Add(pieceSprite);
                }
            }

            return result;
        }

        private static string GetPieceLabel(int index, int columns)
        {
            int row = index / columns;
            int column = index % columns;
            return $"{(char)('A' + row)}{column + 1}";
        }

        private static Color GetFallbackPieceColor(Color baseColor, int index, int columns, int rows)
        {
            int row = index / columns;
            int column = index % columns;
            Color.RGBToHSV(baseColor, out float hue, out float saturation, out float value);
            hue = Mathf.Repeat(hue + column / Mathf.Max(1f, columns) * 0.16f + row / Mathf.Max(1f, rows) * 0.05f, 1f);
            saturation = Mathf.Clamp01(saturation * (0.85f + 0.15f * (row % 2)));
            value = Mathf.Clamp01(value * (0.86f + 0.14f * (column % 2)));
            return Color.HSVToRGB(hue, saturation, value);
        }

        private static List<int> CreateShuffledOrder(int count)
        {
            List<int> order = new(count);
            for (int index = 0; index < count; index++) order.Add(index);
            for (int index = count - 1; index > 0; index--)
            {
                int swapIndex = UnityEngine.Random.Range(0, index + 1);
                (order[index], order[swapIndex]) = (order[swapIndex], order[index]);
            }
            return order;
        }

        private void OnPiecePlaced(PuzzlePieceView piece)
        {
            placedCount++;
            UpdateStatus();
            if (placedCount >= pieceCount)
            {
                int previousBestStars = ProgressService.GetStars(
                    PuzzleSession.SelectedMode,
                    PuzzleSession.SelectedLevelId);
                int earnedStars = Mathf.Clamp((int)PuzzleSession.SelectedDifficulty + 1, 1, 4);
                bool improvedBest = ProgressService.RecordCompletion(
                    PuzzleSession.SelectedMode,
                    PuzzleSession.SelectedLevelId,
                    PuzzleSession.SelectedDifficulty);
                int bestStars = ProgressService.GetStars(
                    PuzzleSession.SelectedMode,
                    PuzzleSession.SelectedLevelId);

                if (statusLabel != null)
                {
                    statusLabel.text = $"COMPLETADO · {BuildStarsText(bestStars)}";
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
        }


        private void OnMoveCompleted(bool correctPlacement)
        {
            moveCount++;
            UpdateStatus();
        }

        private static string BuildStarsText(int stars)
        {
            int normalizedStars = Mathf.Clamp(stars, 0, 4);
            return new string('★', normalizedStars) + new string('☆', 4 - normalizedStars);
        }

        private void UpdateStatus()
        {
            if (statusLabel != null)
            {
                statusLabel.text = $"COLOCADAS: {placedCount} / {pieceCount}   ·   MOVIMIENTOS: {moveCount}";
            }
        }

        private void ClearGeneratedContent()
        {
            pieces.Clear();
            DestroyRuntimeSprites();
            DestroyChildren(boardRoot);
            DestroyChildren(trayRoot);
        }

        private void DestroyRuntimeSprites()
        {
            for (int index = runtimePieceSprites.Count - 1; index >= 0; index--)
            {
                if (runtimePieceSprites[index] != null) Destroy(runtimePieceSprites[index]);
            }
            runtimePieceSprites.Clear();
        }

        private static void DestroyChildren(RectTransform root)
        {
            if (root == null) return;
            for (int index = root.childCount - 1; index >= 0; index--)
            {
                Destroy(root.GetChild(index).gameObject);
            }
        }
    }
}
