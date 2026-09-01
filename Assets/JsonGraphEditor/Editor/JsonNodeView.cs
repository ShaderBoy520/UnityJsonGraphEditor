using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class JsonNodeView : Node
{
    public JsonNodeType nodeType;
    public string jsonKey = "key";
    public string jsonValue = "value";

    private TextField keyField;
    private TextField valueField;

    public JsonNodeView(JsonNodeType type)
    {
        nodeType = type;
        
        // Setup node styling
        AddToClassList("json-node");
        
        // Create input port
        var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float));
        inputPort.portName = "Input";
        inputContainer.Add(inputPort);

        // Create output port
        var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(float));
        outputPort.portName = "Output";
        outputContainer.Add(outputPort);

        // Create main container
        var mainContainer = new VisualElement();
        mainContainer.AddToClassList("node-main-container");

        // Key input field
        keyField = new TextField("Key");
        keyField.value = jsonKey;
        keyField.RegisterValueChangedCallback(evt => jsonKey = evt.newValue);
        mainContainer.Add(keyField);

        // Value input field (for primitive types)
        if (type == JsonNodeType.String || type == JsonNodeType.Number || type == JsonNodeType.Boolean)
        {
            valueField = new TextField("Value");
            valueField.value = jsonValue;
            valueField.RegisterValueChangedCallback(evt => jsonValue = evt.newValue);
            mainContainer.Add(valueField);
        }

        // Add type label
        var typeLabel = new Label($"Type: {type}");
        typeLabel.AddToClassList("node-type-label");
        mainContainer.Add(typeLabel);

        mainContainer.style.padding = new Length(10);
        extensionContainer.Add(mainContainer);
    }

    public string GetKey() => jsonKey;
    public string GetValue() => jsonValue;
    public void SetKey(string key) => jsonKey = key;
    public void SetValue(string value) => jsonValue = value;
}

public enum JsonNodeType
{
    Object,
    Array,
    String,
    Number,
    Boolean,
    Null
}
