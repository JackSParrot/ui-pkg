using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[InitializeOnLoad]
public class Startup
{
    static Startup()
    {
        ImportEssentials();
    }

    [MenuItem("JackSParrot/Import essentials")]
    static void ImportEssentials()
    {
        CreateIfNeeded("JackSParrot__BaseView-NewView.cs");
        CreateIfNeeded("JackSParrot__PopupView-NewPopup.cs");
    }

    static void CreateIfNeeded(string file)
    {
        string path = Application.dataPath + "/ScriptTemplates";

        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }
        path += $"/{file}.txt";
        if (!System.IO.File.Exists(path))
        {
            System.IO.File.WriteAllText(path, Resources.Load<TextAsset>(file).text);
        }
    }
}

public class MenuComponents
{
    [MenuItem("GameObject/UI/JackSParrot/Button")]
    static void CreateButtonView()
    {
        var go = Object.Instantiate(Resources.Load<GameObject>("ButtonView"));
        go.name = "Button";
        AddToUI(go.transform);
    }

    [MenuItem("GameObject/UI/JackSParrot/Popup")]
    static void CreatePopupView()
    {
        var go = Object.Instantiate(Resources.Load<GameObject>("PopupView"));
        go.name = "Popup";
        AddToUI(go.transform);
    }

    [MenuItem("GameObject/UI/JackSParrot/Scroll List")]
    static void CreateScrollListView()
    {
        GameObject go = Object.Instantiate(Resources.Load<GameObject>("ScrollList"));
        go.name = "Scroll List";
        AddToUI(go.transform);
    }

    static void AddToUI(Transform element)
    {
        if (Selection.activeTransform != null && Selection.activeTransform is RectTransform)
        {
            element.SetParent(Selection.activeTransform, false);
        }
        else
        {
            var canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                canvas = CreateNewUI();
            }
            element.SetParent(canvas.transform, false);
        }
        Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
        Selection.activeGameObject = element.gameObject;
    }

    static public Canvas CreateNewUI()
    {
        var root = new GameObject("Canvas");
        root.layer = LayerMask.NameToLayer("Default");
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);
        CreateEventSystem();
        return canvas;
    }

    private static void CreateEventSystem()
    {
        var esys = Object.FindObjectOfType<EventSystem>();
        if (esys == null)
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
        }
    }

    public static void CreatePopupScript(string name)
    {
        bool found = false;
        string[] guids1 = AssetDatabase.FindAssets(name, null);

        foreach (string guid1 in guids1)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid1);
            Debug.Log(path);
            var parts = path.Split('/');
            if (parts.Length > 1)
            {
                var last = parts[parts.Length - 1];
                if (last.Equals(name + ".cs"))
                {
                    found = true;
                    break;
                }
            }
        }
        if (!found)
        {
            string directory = Application.dataPath + "/Generated Popups";
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            directory += "/" + name;
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            string content = Resources.Load<TextAsset>("PopupView-NewPopup.cs").text;
            var replaced = content.Replace("#SCRIPTNAME#", name);
            System.IO.File.WriteAllText($"{directory}/{name}.cs", replaced);
            AssetDatabase.Refresh();
        }
    }
}
