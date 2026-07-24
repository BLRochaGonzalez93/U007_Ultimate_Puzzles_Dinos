using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VRMGames.UltimatePuzzlesDinos.UI.Responsive
{
    [DisallowMultipleComponent]
    public sealed class ResponsiveSceneLayout : MonoBehaviour
    {
        [SerializeField] private RectTransform safeAreaRoot;
        [SerializeField] private float phoneSideMargin = 28f;
        [SerializeField] private float tabletSideMargin = 72f;

        private int lastWidth;
        private int lastHeight;

        private void Awake() => Apply();
        private void OnEnable() => Apply();

        private void Update()
        {
            if (lastWidth != Screen.width || lastHeight != Screen.height)
            {
                Apply();
            }
        }

        public void Apply()
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;

            float aspect = Screen.height > 0
                ? (float)Screen.width / Screen.height
                : 1f;
            bool portrait = Screen.height >= Screen.width;
            bool tablet = portrait ? aspect > 0.62f : aspect < 1.55f;
            float margin = tablet ? tabletSideMargin : phoneSideMargin;

            RectTransform root = safeAreaRoot != null
                ? safeAreaRoot
                : transform as RectTransform;

            if (root != null)
            {
                root.offsetMin = new Vector2(margin, root.offsetMin.y);
                root.offsetMax = new Vector2(-margin, root.offsetMax.y);
            }

            ApplyMainMenu(root, portrait, tablet);
            ApplyLevelSelection(root, portrait, tablet);
            ApplyDifficulty(root, portrait, tablet);
            ApplyGameplay(root, portrait, tablet);
            ApplySettings(root, portrait, tablet);
        }

        private static void ApplyMainMenu(Transform root, bool portrait, bool tablet)
        {
            RectTransform logo = Find(root, "LogoArea");
            if (logo != null)
            {
                logo.anchorMin = new Vector2(0.5f, 1f);
                logo.anchorMax = new Vector2(0.5f, 1f);
                logo.pivot = new Vector2(0.5f, 1f);
                logo.anchoredPosition = new Vector2(0f, portrait ? -70f : -34f);
                logo.sizeDelta = portrait
                    ? new Vector2(tablet ? 860f : 760f, tablet ? 420f : 360f)
                    : new Vector2(720f, 300f);
            }

            RectTransform modeSelection = Find(root, "ModeSelection");
            if (modeSelection != null)
            {
                modeSelection.anchorMin = new Vector2(0.5f, 0.5f);
                modeSelection.anchorMax = new Vector2(0.5f, 0.5f);
                modeSelection.pivot = new Vector2(0.5f, 0.5f);
                modeSelection.anchoredPosition = new Vector2(0f, portrait ? -170f : -40f);
                modeSelection.sizeDelta = portrait
                    ? new Vector2(tablet ? 820f : 720f, 560f)
                    : new Vector2(980f, 360f);
            }
        }

        private static void ApplyLevelSelection(Transform root, bool portrait, bool tablet)
        {
            GridLayoutGroup grid = FindComponent<GridLayoutGroup>(root);
            if (grid == null)
            {
                return;
            }

            RectTransform gridRect = grid.transform as RectTransform;
            float width = gridRect != null && gridRect.rect.width > 0f
                ? gridRect.rect.width
                : (tablet ? 900f : 720f);

            int columns = portrait ? (tablet ? 4 : 3) : (tablet ? 6 : 5);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.spacing = new Vector2(tablet ? 20f : 14f, tablet ? 20f : 14f);
            grid.padding = new RectOffset(12, 12, 12, 18);

            float usableWidth = width - grid.padding.left - grid.padding.right -
                grid.spacing.x * (columns - 1);
            float size = Mathf.Floor(usableWidth / columns);
            grid.cellSize = new Vector2(size, size);
        }

        private static void ApplyDifficulty(Transform root, bool portrait, bool tablet)
        {
            GridLayoutGroup grid = Find(root, "DifficultyCards")?.GetComponent<GridLayoutGroup>();
            if (grid == null)
            {
                grid = FindComponent<GridLayoutGroup>(root);
            }

            if (grid == null)
            {
                return;
            }

            int columns = portrait ? 2 : 4;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.spacing = new Vector2(tablet ? 30f : 20f, tablet ? 30f : 20f);
            grid.cellSize = portrait
                ? new Vector2(tablet ? 330f : 290f, tablet ? 260f : 230f)
                : new Vector2(320f, 250f);
        }

        private static void ApplyGameplay(Transform root, bool portrait, bool tablet)
        {
            RectTransform content = Find(root, "GameplayContent");
            RectTransform board = Find(root, "BoardFrame");
            RectTransform tray = Find(root, "TrayFrame");

            if (content == null || board == null || tray == null)
            {
                return;
            }

            if (portrait)
            {
                content.anchorMin = new Vector2(0.5f, 0.5f);
                content.anchorMax = new Vector2(0.5f, 0.5f);
                content.pivot = new Vector2(0.5f, 0.5f);
                content.sizeDelta = new Vector2(tablet ? 920f : 760f, tablet ? 1320f : 1180f);
                content.anchoredPosition = new Vector2(0f, -40f);

                float boardSize = tablet ? 780f : 650f;
                board.anchorMin = new Vector2(0.5f, 1f);
                board.anchorMax = new Vector2(0.5f, 1f);
                board.pivot = new Vector2(0.5f, 1f);
                board.anchoredPosition = new Vector2(0f, 0f);
                board.sizeDelta = new Vector2(boardSize, boardSize);

                tray.anchorMin = new Vector2(0.5f, 0f);
                tray.anchorMax = new Vector2(0.5f, 0f);
                tray.pivot = new Vector2(0.5f, 0f);
                tray.anchoredPosition = new Vector2(0f, 0f);
                tray.sizeDelta = new Vector2(boardSize, tablet ? 460f : 410f);
            }
            else
            {
                content.anchorMin = new Vector2(0.5f, 0.5f);
                content.anchorMax = new Vector2(0.5f, 0.5f);
                content.pivot = new Vector2(0.5f, 0.5f);
                content.sizeDelta = new Vector2(tablet ? 1500f : 1640f, tablet ? 900f : 850f);
                content.anchoredPosition = new Vector2(0f, -20f);

                float boardSize = tablet ? 800f : 760f;
                board.anchorMin = new Vector2(0f, 0.5f);
                board.anchorMax = new Vector2(0f, 0.5f);
                board.pivot = new Vector2(0f, 0.5f);
                board.anchoredPosition = new Vector2(0f, 0f);
                board.sizeDelta = new Vector2(boardSize, boardSize);

                tray.anchorMin = new Vector2(1f, 0.5f);
                tray.anchorMax = new Vector2(1f, 0.5f);
                tray.pivot = new Vector2(1f, 0.5f);
                tray.anchoredPosition = new Vector2(0f, 0f);
                tray.sizeDelta = new Vector2(tablet ? 620f : 700f, boardSize);
            }
        }

        private static void ApplySettings(Transform root, bool portrait, bool tablet)
        {
            RectTransform panel = Find(root, "SettingsPanel");
            if (panel == null)
            {
                return;
            }

            panel.anchorMin = new Vector2(0.5f, 0.5f);
            panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.sizeDelta = portrait
                ? new Vector2(tablet ? 820f : 720f, tablet ? 680f : 620f)
                : new Vector2(820f, 600f);
            panel.anchoredPosition = Vector2.zero;
        }

        private static RectTransform Find(Transform root, string name)
        {
            if (root == null)
            {
                return null;
            }

            if (root.name == name)
            {
                return root as RectTransform;
            }

            foreach (Transform child in root)
            {
                RectTransform result = Find(child, name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static T FindComponent<T>(Transform root) where T : Component
        {
            if (root == null)
            {
                return null;
            }

            T component = root.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            foreach (Transform child in root)
            {
                component = FindComponent<T>(child);
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }
    }
}
