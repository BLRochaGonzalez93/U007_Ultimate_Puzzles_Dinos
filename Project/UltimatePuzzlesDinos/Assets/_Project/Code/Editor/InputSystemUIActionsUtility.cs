#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    internal static class InputSystemUIActionsUtility
    {
        internal const string AssetFolder = "Assets/_Project/Config/Input";
        internal const string AssetPath = AssetFolder + "/UIInputActions.asset";

        internal static InputActionAsset GetOrCreateAsset()
        {
            InputActionAsset asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetPath);
            if (asset != null)
            {
                return asset;
            }

            EnsureFolder(AssetFolder);

            asset = ScriptableObject.CreateInstance<InputActionAsset>();
            asset.name = "UIInputActions";

            InputActionMap ui = asset.AddActionMap("UI");

            InputAction point = CreateAction(ui, "Point", InputActionType.PassThrough, "Vector2");
            point.AddBinding("<Pointer>/position");

            InputAction leftClick = CreateAction(ui, "LeftClick", InputActionType.PassThrough, "Button");
            leftClick.AddBinding("<Pointer>/press");

            InputAction rightClick = CreateAction(ui, "RightClick", InputActionType.PassThrough, "Button");
            rightClick.AddBinding("<Mouse>/rightButton");

            InputAction middleClick = CreateAction(ui, "MiddleClick", InputActionType.PassThrough, "Button");
            middleClick.AddBinding("<Mouse>/middleButton");

            InputAction scrollWheel = CreateAction(ui, "ScrollWheel", InputActionType.PassThrough, "Vector2");
            scrollWheel.AddBinding("<Mouse>/scroll");

            InputAction move = CreateAction(ui, "Move", InputActionType.PassThrough, "Vector2");
            move.AddBinding("<Gamepad>/leftStick");
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/s")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/a")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/d")
                .With("Right", "<Keyboard>/rightArrow");

            InputAction submit = CreateAction(ui, "Submit", InputActionType.Button, "Button");
            submit.AddBinding("<Keyboard>/enter");
            submit.AddBinding("<Keyboard>/numpadEnter");
            submit.AddBinding("<Gamepad>/buttonSouth");

            InputAction cancel = CreateAction(ui, "Cancel", InputActionType.Button, "Button");
            cancel.AddBinding("<Keyboard>/escape");
            cancel.AddBinding("<Gamepad>/buttonEast");

            InputAction trackedPosition = CreateAction(ui, "TrackedDevicePosition", InputActionType.PassThrough, "Vector3");
            trackedPosition.AddBinding("<TrackedDevice>/devicePosition");

            InputAction trackedOrientation = CreateAction(ui, "TrackedDeviceOrientation", InputActionType.PassThrough, "Quaternion");
            trackedOrientation.AddBinding("<TrackedDevice>/deviceRotation");

            AssetDatabase.CreateAsset(asset, AssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceUpdate);
            return AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetPath);
        }

        internal static void Configure(InputSystemUIInputModule module)
        {
            InputActionAsset asset = GetOrCreateAsset();
            module.actionsAsset = asset;
            module.point = GetOrCreateReference(asset, "UI/Point");
            module.leftClick = GetOrCreateReference(asset, "UI/LeftClick");
            module.rightClick = GetOrCreateReference(asset, "UI/RightClick");
            module.middleClick = GetOrCreateReference(asset, "UI/MiddleClick");
            module.scrollWheel = GetOrCreateReference(asset, "UI/ScrollWheel");
            module.move = GetOrCreateReference(asset, "UI/Move");
            module.submit = GetOrCreateReference(asset, "UI/Submit");
            module.cancel = GetOrCreateReference(asset, "UI/Cancel");
            module.trackedDevicePosition = GetOrCreateReference(asset, "UI/TrackedDevicePosition");
            module.trackedDeviceOrientation = GetOrCreateReference(asset, "UI/TrackedDeviceOrientation");
            EditorUtility.SetDirty(module);
        }

        private static InputAction CreateAction(
            InputActionMap map,
            string name,
            InputActionType type,
            string expectedControlType)
        {
            InputAction action = map.AddAction(name, type);
            action.expectedControlType = expectedControlType;
            return action;
        }

        private static InputActionReference GetOrCreateReference(InputActionAsset asset, string actionPath)
        {
            InputAction action = asset.FindAction(actionPath, true);
            InputActionReference reference = AssetDatabase.LoadAllAssetsAtPath(AssetPath)
                .OfType<InputActionReference>()
                .FirstOrDefault(candidate => candidate.action != null && candidate.action.id == action.id);

            if (reference != null)
            {
                return reference;
            }

            reference = InputActionReference.Create(action);
            reference.name = action.name + "Reference";
            AssetDatabase.AddObjectToAsset(reference, asset);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            return reference;
        }

        private static void EnsureFolder(string folder)
        {
            string[] parts = folder.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }
                current = next;
            }
        }
    }
}
#endif
