#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment024CardLockCoverSync
    {
        private const string LevelSelectionScenePath =
            "Assets/_Project/Scenes/LevelSelection.unity";

        public static void Run()
        {
            if (!File.Exists(LevelSelectionScenePath))
            {
                Debug.LogError(
                    $"[Increment 024] No existe {LevelSelectionScenePath}.");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(
                LevelSelectionScenePath,
                OpenSceneMode.Single);

            Transform sceneRoot = GetSceneRoot(scene);
            Transform templateCard = Find(sceneRoot, "LevelCard_01");
            Transform templateCover = templateCard != null
                ? Find(templateCard, "LockCover")
                : null;

            if (templateCard == null || templateCover == null)
            {
                Debug.LogError(
                    "[Increment 024] LevelCard_01 no contiene LockCover. " +
                    "Se cancela para no alterar la configuración manual.");
                return;
            }

            RectTransform templateRect =
                templateCover.GetComponent<RectTransform>();
            Image templateImage = templateCover.GetComponent<Image>();

            if (templateRect == null || templateImage == null)
            {
                Debug.LogError(
                    "[Increment 024] El LockCover de LevelCard_01 debe " +
                    "contener RectTransform e Image.");
                return;
            }

            int updatedCards = 0;
            int removedIcons = 0;

            for (int level = 1; level <= 60; level++)
            {
                Transform card = Find(sceneRoot, $"LevelCard_{level:00}");
                if (card == null)
                {
                    Debug.LogWarning(
                        $"[Increment 024] No se encontró LevelCard_{level:00}.");
                    continue;
                }

                removedIcons += RemoveChildrenNamed(card, "LockIcon");

                Transform lockIndicator = Find(card, "LockIndicator");
                Transform lockCover = Find(card, "LockCover");

                if (lockIndicator == null)
                {
                    Debug.LogWarning(
                        $"[Increment 024] {card.name} no contiene LockIndicator.");
                    continue;
                }

                Text lockText = lockIndicator.GetComponent<Text>();
                if (lockText != null)
                {
                    lockText.text = string.Empty;
                    lockText.enabled = false;
                    lockText.raycastTarget = false;
                    EditorUtility.SetDirty(lockText);
                }

                if (lockCover == null)
                {
                    GameObject coverObject = new(
                        "LockCover",
                        typeof(RectTransform),
                        typeof(Image));

                    lockCover = coverObject.transform;
                    lockCover.SetParent(lockIndicator, false);
                }
                else if (lockCover.parent != lockIndicator)
                {
                    lockCover.SetParent(lockIndicator, false);
                }

                ApplyRectTransform(
                    templateRect,
                    lockCover.GetComponent<RectTransform>());

                ApplyImage(
                    templateImage,
                    lockCover.GetComponent<Image>());

                lockCover.name = "LockCover";
                lockCover.gameObject.SetActive(
                    templateCover.gameObject.activeSelf);
                lockCover.SetSiblingIndex(
                    Mathf.Min(
                        templateCover.GetSiblingIndex(),
                        lockIndicator.childCount - 1));

                EditorUtility.SetDirty(lockCover);
                updatedCards++;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                $"[Increment 024] Configuración sincronizada en " +
                $"{updatedCards} Cards. LockIcon eliminados: {removedIcons}. " +
                "La escena LevelSelection es compartida por Puzzle, " +
                "Puzzle Logic y Mosaic.");

            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                $"Increment 024 completado.\n\n" +
                $"Cards configuradas: {updatedCards}\n" +
                $"LockIcon eliminados: {removedIcons}\n\n" +
                "El cambio se aplica a los tres modos de juego.",
                "OK");
        }

        public static void Validate()
        {
            if (!File.Exists(LevelSelectionScenePath))
            {
                Debug.LogError(
                    $"[Increment 024] No existe {LevelSelectionScenePath}.");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(
                LevelSelectionScenePath,
                OpenSceneMode.Single);

            Transform sceneRoot = GetSceneRoot(scene);
            Transform templateCard = Find(sceneRoot, "LevelCard_01");
            Transform templateCover = templateCard != null
                ? Find(templateCard, "LockCover")
                : null;

            List<string> problems = new();

            if (templateCover == null)
            {
                problems.Add("LevelCard_01 no contiene LockCover.");
            }
            else
            {
                RectTransform templateRect =
                    templateCover.GetComponent<RectTransform>();
                Image templateImage = templateCover.GetComponent<Image>();

                for (int level = 1; level <= 60; level++)
                {
                    Transform card =
                        Find(sceneRoot, $"LevelCard_{level:00}");

                    if (card == null)
                    {
                        problems.Add($"Falta LevelCard_{level:00}.");
                        continue;
                    }

                    if (Find(card, "LockIcon") != null)
                    {
                        problems.Add(
                            $"{card.name} todavía contiene LockIcon.");
                    }

                    Transform cover = Find(card, "LockCover");
                    if (cover == null)
                    {
                        problems.Add(
                            $"{card.name} no contiene LockCover.");
                        continue;
                    }

                    RectTransform rect =
                        cover.GetComponent<RectTransform>();
                    Image image = cover.GetComponent<Image>();

                    if (!RectMatches(templateRect, rect))
                    {
                        problems.Add(
                            $"{card.name}/LockCover no coincide " +
                            "con LevelCard_01.");
                    }

                    if (!ImageMatches(templateImage, image))
                    {
                        problems.Add(
                            $"{card.name}/LockCover tiene una " +
                            "configuración Image distinta.");
                    }
                }
            }

            if (problems.Count == 0)
            {
                Debug.Log(
                    "[Increment 024] Validación correcta: las 60 Cards " +
                    "comparten la configuración de LockCover y no existe " +
                    "ningún LockIcon.");

                EditorUtility.DisplayDialog(
                    "Validación correcta",
                    "Las 60 Cards están sincronizadas con LevelCard_01.",
                    "OK");
                return;
            }

            Debug.LogError(
                "[Increment 024] Validación fallida:\n- " +
                string.Join("\n- ", problems));

            EditorUtility.DisplayDialog(
                "Validación fallida",
                $"Se encontraron {problems.Count} incidencias. " +
                "Consulta la consola.",
                "OK");
        }

        private static void ApplyRectTransform(
            RectTransform source,
            RectTransform target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.anchorMin = source.anchorMin;
            target.anchorMax = source.anchorMax;
            target.pivot = source.pivot;
            target.anchoredPosition = source.anchoredPosition;
            target.sizeDelta = source.sizeDelta;
            target.offsetMin = source.offsetMin;
            target.offsetMax = source.offsetMax;
            target.localScale = source.localScale;
            target.localRotation = source.localRotation;
        }

        private static void ApplyImage(Image source, Image target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.sprite = source.sprite;
            target.overrideSprite = source.overrideSprite;
            target.type = source.type;
            target.preserveAspect = source.preserveAspect;
            target.fillCenter = source.fillCenter;
            target.fillMethod = source.fillMethod;
            target.fillAmount = source.fillAmount;
            target.fillClockwise = source.fillClockwise;
            target.fillOrigin = source.fillOrigin;
            target.color = source.color;
            target.material = source.material;
            target.raycastTarget = source.raycastTarget;
            target.maskable = source.maskable;
            target.useSpriteMesh = source.useSpriteMesh;
            target.pixelsPerUnitMultiplier =
                source.pixelsPerUnitMultiplier;
            EditorUtility.SetDirty(target);
        }

        private static bool RectMatches(
            RectTransform expected,
            RectTransform actual)
        {
            if (expected == null || actual == null)
            {
                return false;
            }

            return Approximately(expected.anchorMin, actual.anchorMin) &&
                Approximately(expected.anchorMax, actual.anchorMax) &&
                Approximately(expected.pivot, actual.pivot) &&
                Approximately(
                    expected.anchoredPosition,
                    actual.anchoredPosition) &&
                Approximately(expected.sizeDelta, actual.sizeDelta) &&
                Approximately(expected.offsetMin, actual.offsetMin) &&
                Approximately(expected.offsetMax, actual.offsetMax) &&
                Approximately(expected.localScale, actual.localScale) &&
                Quaternion.Angle(
                    expected.localRotation,
                    actual.localRotation) < 0.01f;
        }

        private static bool ImageMatches(Image expected, Image actual)
        {
            if (expected == null || actual == null)
            {
                return false;
            }

            return expected.sprite == actual.sprite &&
                expected.overrideSprite == actual.overrideSprite &&
                expected.type == actual.type &&
                expected.preserveAspect == actual.preserveAspect &&
                expected.fillCenter == actual.fillCenter &&
                expected.fillMethod == actual.fillMethod &&
                Mathf.Approximately(
                    expected.fillAmount,
                    actual.fillAmount) &&
                expected.fillClockwise == actual.fillClockwise &&
                expected.fillOrigin == actual.fillOrigin &&
                Approximately(expected.color, actual.color) &&
                expected.material == actual.material &&
                expected.raycastTarget == actual.raycastTarget &&
                expected.maskable == actual.maskable &&
                expected.useSpriteMesh == actual.useSpriteMesh &&
                Mathf.Approximately(
                    expected.pixelsPerUnitMultiplier,
                    actual.pixelsPerUnitMultiplier);
        }

        private static int RemoveChildrenNamed(
            Transform root,
            string targetName)
        {
            List<GameObject> matches = new();
            CollectNamedChildren(root, targetName, matches);

            foreach (GameObject match in matches)
            {
                Object.DestroyImmediate(match);
            }

            return matches.Count;
        }

        private static void CollectNamedChildren(
            Transform root,
            string targetName,
            List<GameObject> matches)
        {
            foreach (Transform child in root)
            {
                if (child.name == targetName)
                {
                    matches.Add(child.gameObject);
                }
                else
                {
                    CollectNamedChildren(
                        child,
                        targetName,
                        matches);
                }
            }
        }

        private static Transform GetSceneRoot(Scene scene)
        {
            GameObject[] roots = scene.GetRootGameObjects();

            Transform canvas = roots
                .SelectMany(root =>
                    root.GetComponentsInChildren<Canvas>(true))
                .Select(item => item.transform)
                .FirstOrDefault();

            return canvas != null
                ? canvas
                : roots.FirstOrDefault()?.transform;
        }

        private static Transform Find(Transform root, string name)
        {
            if (root == null)
            {
                return null;
            }

            if (root.name == name)
            {
                return root;
            }

            foreach (Transform child in root)
            {
                Transform result = Find(child, name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static bool Approximately(Vector2 a, Vector2 b)
        {
            return Vector2.SqrMagnitude(a - b) < 0.0001f;
        }

        private static bool Approximately(Vector3 a, Vector3 b)
        {
            return Vector3.SqrMagnitude(a - b) < 0.0001f;
        }

        private static bool Approximately(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) < 0.001f &&
                Mathf.Abs(a.g - b.g) < 0.001f &&
                Mathf.Abs(a.b - b.b) < 0.001f &&
                Mathf.Abs(a.a - b.a) < 0.001f;
        }
    }
}
#endif
