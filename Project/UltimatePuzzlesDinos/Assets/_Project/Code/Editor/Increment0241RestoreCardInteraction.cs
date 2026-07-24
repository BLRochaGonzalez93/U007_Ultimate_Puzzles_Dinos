#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.UI.Screens;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment0241RestoreCardInteraction
    {
        private const string ScenePath =
            "Assets/_Project/Scenes/LevelSelection.unity";

        public static void Run()
        {
            if (!File.Exists(ScenePath))
            {
                Debug.LogError($"[Increment 024.1] Falta {ScenePath}.");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(
                ScenePath,
                OpenSceneMode.Single);

            Transform root = GetSceneRoot(scene);
            LevelSelectionScreen screen = root != null
                ? root.GetComponentInChildren<LevelSelectionScreen>(true)
                : null;

            if (screen == null)
            {
                Debug.LogError(
                    "[Increment 024.1] No se encontró LevelSelectionScreen.");
                return;
            }

            List<Button> buttons = new();
            List<Text> labels = new();
            List<Image> previews = new();
            List<Text> placeholders = new();
            List<GameObject> lockIndicators = new();
            List<string> problems = new();

            for (int level = 1; level <= 60; level++)
            {
                Transform card = Find(root, $"LevelCard_{level:00}");
                if (card == null)
                {
                    problems.Add($"Falta LevelCard_{level:00}");
                    AddNullEntries(
                        buttons,
                        labels,
                        previews,
                        placeholders,
                        lockIndicators);
                    continue;
                }

                Button button = card.GetComponent<Button>();
                Image cardImage = card.GetComponent<Image>();

                if (button == null)
                {
                    button = card.gameObject.AddComponent<Button>();
                }

                button.enabled = true;
                button.interactable = true;
                button.transition = Selectable.Transition.ColorTint;
                button.navigation = new UnityEngine.UI.Navigation
                {
                    mode = UnityEngine.UI.Navigation.Mode.Automatic
                };

                if (cardImage != null)
                {
                    cardImage.raycastTarget = true;
                    button.targetGraphic = cardImage;
                }

                CanvasGroup[] groups =
                    card.GetComponentsInParent<CanvasGroup>(true);

                foreach (CanvasGroup group in groups)
                {
                    if (group == null)
                    {
                        continue;
                    }

                    group.interactable = true;
                    group.blocksRaycasts = true;
                    EditorUtility.SetDirty(group);
                }

                Transform lockIndicator = Find(card, "LockIndicator");
                if (lockIndicator != null)
                {
                    Graphic[] lockGraphics =
                        lockIndicator.GetComponentsInChildren<Graphic>(true);

                    foreach (Graphic graphic in lockGraphics)
                    {
                        graphic.raycastTarget = false;
                        EditorUtility.SetDirty(graphic);
                    }
                }

                Text label = Find(card, "LevelLabel")?.GetComponent<Text>();
                Image preview = Find(card, "Preview")?.GetComponent<Image>();
                Text placeholder =
                    Find(card, "PreviewPlaceholder")?.GetComponent<Text>();

                if (preview != null)
                {
                    preview.raycastTarget = false;
                }

                if (label != null)
                {
                    label.raycastTarget = false;
                }

                if (placeholder != null)
                {
                    placeholder.raycastTarget = false;
                }

                buttons.Add(button);
                labels.Add(label);
                previews.Add(preview);
                placeholders.Add(placeholder);
                lockIndicators.Add(
                    lockIndicator != null
                        ? lockIndicator.gameObject
                        : null);

                EditorUtility.SetDirty(button);
                if (cardImage != null)
                {
                    EditorUtility.SetDirty(cardImage);
                }
            }

            RestoreSerializedLists(
                screen,
                buttons,
                labels,
                previews,
                placeholders,
                lockIndicators);

            EditorUtility.SetDirty(screen);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string message =
                "[Increment 024.1] Interacción restaurada en las 60 Cards. " +
                "Las listas de LevelSelectionScreen se han reconstruido y " +
                "LockCover deja pasar los raycasts.";

            if (problems.Count > 0)
            {
                message += "\nIncidencias:\n- " +
                    string.Join("\n- ", problems);
                Debug.LogWarning(message);
            }
            else
            {
                Debug.Log(message);
            }

            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 024.1 completado.\n\n" +
                "Se ha restaurado la interacción de las 60 Cards.",
                "OK");
        }

        public static void Validate()
        {
            Scene scene = EditorSceneManager.OpenScene(
                ScenePath,
                OpenSceneMode.Single);

            Transform root = GetSceneRoot(scene);
            LevelSelectionScreen screen = root != null
                ? root.GetComponentInChildren<LevelSelectionScreen>(true)
                : null;

            List<string> problems = new();

            if (screen == null)
            {
                problems.Add("No existe LevelSelectionScreen.");
            }

            for (int level = 1; level <= 60; level++)
            {
                Transform card = Find(root, $"LevelCard_{level:00}");
                if (card == null)
                {
                    problems.Add($"Falta LevelCard_{level:00}.");
                    continue;
                }

                Button button = card.GetComponent<Button>();
                Image cardImage = card.GetComponent<Image>();

                if (button == null)
                {
                    problems.Add($"{card.name} no tiene Button.");
                }
                else
                {
                    if (!button.enabled)
                    {
                        problems.Add($"{card.name}/Button está deshabilitado.");
                    }

                    if (cardImage != null && button.targetGraphic != cardImage)
                    {
                        problems.Add(
                            $"{card.name}/Button no apunta al Image de la Card.");
                    }
                }

                Transform lockIndicator = Find(card, "LockIndicator");
                if (lockIndicator != null)
                {
                    Graphic blockingGraphic =
                        lockIndicator
                            .GetComponentsInChildren<Graphic>(true)
                            .FirstOrDefault(graphic => graphic.raycastTarget);

                    if (blockingGraphic != null)
                    {
                        problems.Add(
                            $"{card.name}/{blockingGraphic.name} bloquea raycasts.");
                    }
                }
            }

            if (problems.Count == 0)
            {
                Debug.Log(
                    "[Increment 024.1] Validación correcta: las 60 Cards " +
                    "reciben interacción y sus overlays no bloquean raycasts.");

                EditorUtility.DisplayDialog(
                    "Validación correcta",
                    "Las 60 Cards conservan su interacción.",
                    "OK");
                return;
            }

            Debug.LogError(
                "[Increment 024.1] Validación fallida:\n- " +
                string.Join("\n- ", problems));

            EditorUtility.DisplayDialog(
                "Validación fallida",
                $"Se detectaron {problems.Count} incidencias. " +
                "Consulta la consola.",
                "OK");
        }

        private static void RestoreSerializedLists(
            LevelSelectionScreen screen,
            IReadOnlyList<Button> buttons,
            IReadOnlyList<Text> labels,
            IReadOnlyList<Image> previews,
            IReadOnlyList<Text> placeholders,
            IReadOnlyList<GameObject> lockIndicators)
        {
            SerializedObject serialized = new(screen);

            AssignList(serialized.FindProperty("levelButtons"), buttons);
            AssignList(serialized.FindProperty("levelLabels"), labels);
            AssignList(serialized.FindProperty("previewImages"), previews);
            AssignList(
                serialized.FindProperty("previewPlaceholders"),
                placeholders);
            AssignList(
                serialized.FindProperty("lockIndicators"),
                lockIndicators);

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AssignList<T>(
            SerializedProperty property,
            IReadOnlyList<T> values)
            where T : Object
        {
            if (property == null)
            {
                return;
            }

            property.arraySize = values.Count;
            for (int index = 0; index < values.Count; index++)
            {
                property.GetArrayElementAtIndex(index)
                    .objectReferenceValue = values[index];
            }
        }

        private static void AddNullEntries(
            ICollection<Button> buttons,
            ICollection<Text> labels,
            ICollection<Image> previews,
            ICollection<Text> placeholders,
            ICollection<GameObject> lockIndicators)
        {
            buttons.Add(null);
            labels.Add(null);
            previews.Add(null);
            placeholders.Add(null);
            lockIndicators.Add(null);
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
    }
}
#endif
