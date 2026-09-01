using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;

public class JsonGraphEditorWindow : EditorWindow
{
    private JsonGraphView graphView;
    private TextField filePathField;
    private string currentFilePath = "";
    private Dictionary<JsonNodeView, JsonNodeData> nodeDataMap = new Dictionary<JsonNodeView, JsonNodeData>();

    [MenuItem("Window/Json Graph Editor")]
    public static void OpenWindow()
    {
        var window = GetWindow<JsonGraphEditorWindow>();
        window.titleContent = new GUIContent("Json Graph Editor");
        window.minSize = new Vector2(800, 600);
    }

    private void OnEnable()
    {
        CreateUI();
    }

    private void CreateUI()
    {
        rootVisualElement.Clear();

        // Top toolbar
        var toolbar = new Toolbar();
        toolbar.style.paddingBottom = 5;
        rootVisualElement.Add(toolbar);

        // File path input
        filePathField = new TextField("File Path");
        filePathField.value = currentFilePath;
        filePathField.style.flexGrow = 1;
        toolbar.Add(filePathField);

        // Load button
        var loadButton = new Button(LoadJsonFile) { text = "Load JSON" };
        toolbar.Add(loadButton);

        // Save button
        var saveButton = new Button(SaveJsonFile) { text = "Save JSON" };
        toolbar.Add(saveButton);

        // Add node buttons
        var addObjectButton = new Button(() => AddNode(JsonNodeType.Object)) { text = "Add Object" };
        toolbar.Add(addObjectButton);

        var addArrayButton = new Button(() => AddNode(JsonNodeType.Array)) { text = "Add Array" };
        toolbar.Add(addArrayButton);

        var addStringButton = new Button(() => AddNode(JsonNodeType.String)) { text = "Add String" };
        toolbar.Add(addStringButton);

        var addNumberButton = new Button(() => AddNode(JsonNodeType.Number)) { text = "Add Number" };
        toolbar.Add(addNumberButton);

        var addBooleanButton = new Button(() => AddNode(JsonNodeType.Boolean)) { text = "Add Boolean" };
        toolbar.Add(addBooleanButton);

        // Graph view
        graphView = new JsonGraphView();
        graphView.style.flexGrow = 1;
        rootVisualElement.Add(graphView);

        // Style
        rootVisualElement.style.paddingLeft = 5;
        rootVisualElement.style.paddingRight = 5;
        rootVisualElement.style.paddingTop = 5;
    }

    private void AddNode(JsonNodeType nodeType)
    {
        var mousePos = graphView.contentViewContainer.WorldToLocal(Event.current.mousePosition);
        if (mousePos == Vector2.zero)
        {
            mousePos = new Vector2(100, 100);
        }

        var nodeView = graphView.CreateNodeView(nodeType, mousePos);
        nodeDataMap[nodeView] = new JsonNodeData { type = nodeType };
    }

    private void LoadJsonFile()
    {
        currentFilePath = filePathField.value;

        if (!File.Exists(currentFilePath))
        {
            EditorUtility.DisplayDialog("Error", "File not found: " + currentFilePath, "OK");
            return;
        }

        try
        {
            string jsonContent = File.ReadAllText(currentFilePath);
            graphView.DeleteElements(graphView.nodes.ToList().Cast<GraphElement>().ToList());
            nodeDataMap.Clear();

            // Parse JSON and create nodes
            ParseJsonToGraph(jsonContent);

            EditorUtility.DisplayDialog("Success", "JSON file loaded successfully!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", "Failed to load JSON: " + e.Message, "OK");
        }
    }

    private void ParseJsonToGraph(string jsonContent)
    {
        // Simple JSON parsing - create nodes based on JSON structure
        var jsonNode = JsonUtility.FromJson<JsonNodeData>(jsonContent);
        
        var nodeView = graphView.CreateNodeView(JsonNodeType.Object, new Vector2(200, 200));
        nodeDataMap[nodeView] = jsonNode;
    }

    private void SaveJsonFile()
    {
        currentFilePath = filePathField.value;

        if (string.IsNullOrEmpty(currentFilePath))
        {
            EditorUtility.DisplayDialog("Error", "Please specify a file path", "OK");
            return;
        }

        try
        {
            string json = ConvertGraphToJson();
            
            string directory = Path.GetDirectoryName(currentFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(currentFilePath, json);
            EditorUtility.DisplayDialog("Success", "JSON file saved successfully to:\n" + currentFilePath, "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", "Failed to save JSON: " + e.Message, "OK");
        }
    }

    private string ConvertGraphToJson()
    {
        var rootNodes = graphView.nodes.ToList().FindAll(n => 
        {
            var nodeView = n as JsonNodeView;
            return nodeView != null && nodeView.inputContainer.childCount == 0;
        });

        if (rootNodes.Count == 0)
        {
            return "{}";
        }

        var rootNode = rootNodes[0] as JsonNodeView;
        return ConvertNodeToJson(rootNode);
    }

    private string ConvertNodeToJson(JsonNodeView nodeView)
    {
        if (nodeView == null) return "null";

        switch (nodeView.nodeType)
        {
            case JsonNodeType.String:
                return "\"" + nodeView.GetValue() + "\"";
            case JsonNodeType.Number:
                return nodeView.GetValue();
            case JsonNodeType.Boolean:
                return nodeView.GetValue().ToLower();
            case JsonNodeType.Null:
                return "null";
            case JsonNodeType.Object:
                return "{\"" + nodeView.GetKey() + "\": " + nodeView.GetValue() + "}";
            case JsonNodeType.Array:
                return "[" + nodeView.GetValue() + "]";
            default:
                return "{}";
        }
    }
}

[System.Serializable]
public class JsonNodeData
{
    public JsonNodeType type;
    public string key;
    public string value;
    public List<JsonNodeData> children = new List<JsonNodeData>();
}
