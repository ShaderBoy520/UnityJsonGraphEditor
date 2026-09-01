using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class JsonSerializer
{
    public static string SerializeToJson(JsonNodeView rootNode, Dictionary<JsonNodeView, JsonNodeData> nodeDataMap)
    {
        if (rootNode == null)
            return "{}";

        return SerializeNode(rootNode, nodeDataMap);
    }

    private static string SerializeNode(JsonNodeView node, Dictionary<JsonNodeView, JsonNodeData> nodeDataMap)
    {
        if (node == null)
            return "null";

        switch (node.nodeType)
        {
            case JsonNodeType.Object:
                return SerializeObject(node, nodeDataMap);
            case JsonNodeType.Array:
                return SerializeArray(node, nodeDataMap);
            case JsonNodeType.String:
                return "\"" + EscapeString(node.GetValue()) + "\"";
            case JsonNodeType.Number:
                return node.GetValue();
            case JsonNodeType.Boolean:
                return node.GetValue().ToLower();
            case JsonNodeType.Null:
                return "null";
            default:
                return "null";
        }
    }

    private static string SerializeObject(JsonNodeView node, Dictionary<JsonNodeView, JsonNodeData> nodeDataMap)
    {
        var sb = new StringBuilder();
        sb.Append("{");

        var childNodes = GetChildNodes(node);
        for (int i = 0; i < childNodes.Count; i++)
        {
            var childNode = childNodes[i];
            sb.Append("\"" + EscapeString(childNode.GetKey()) + "\":");
            sb.Append(SerializeNode(childNode, nodeDataMap));

            if (i < childNodes.Count - 1)
                sb.Append(",");
        }

        sb.Append("}");
        return sb.ToString();
    }

    private static string SerializeArray(JsonNodeView node, Dictionary<JsonNodeView, JsonNodeData> nodeDataMap)
    {
        var sb = new StringBuilder();
        sb.Append("[");

        var childNodes = GetChildNodes(node);
        for (int i = 0; i < childNodes.Count; i++)
        {
            sb.Append(SerializeNode(childNodes[i], nodeDataMap));
            if (i < childNodes.Count - 1)
                sb.Append(",");
        }

        sb.Append("]");
        return sb.ToString();
    }

    private static List<JsonNodeView> GetChildNodes(JsonNodeView parentNode)
    {
        var children = new List<JsonNodeView>();
        
        foreach (var connection in parentNode.outputContainer.Query<Port>().ForEach(port => port))
        {
            if (connection.connections != null)
            {
                foreach (var edge in connection.connections)
                {
                    var childNode = edge.target.node as JsonNodeView;
                    if (childNode != null)
                        children.Add(childNode);
                }
            }
        }

        return children;
    }

    private static string EscapeString(string str)
    {
        if (string.IsNullOrEmpty(str))
            return "";

        return str.Replace("\"", "\\\"")
                  .Replace("\\", "\\\\")
                  .Replace("\n", "\\n")
                  .Replace("\r", "\\r")
                  .Replace("\t", "\\t");
    }

    public static void DeserializeFromJson(string json, JsonGraphView graphView, Dictionary<JsonNodeView, JsonNodeData> nodeDataMap)
    {
        try
        {
            var root = JsonUtility.FromJson<JsonObject>(json);
            if (root != null)
            {
                var rootNode = graphView.CreateNodeView(JsonNodeType.Object, new Vector2(200, 200));
                nodeDataMap[rootNode] = new JsonNodeData { type = JsonNodeType.Object };
                
                CreateNodesFromObject(root, rootNode, graphView, nodeDataMap);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to deserialize JSON: " + e.Message);
        }
    }

    private static void CreateNodesFromObject(JsonObject obj, JsonNodeView parentNode, JsonGraphView graphView, Dictionary<JsonNodeView, JsonNodeData> nodeDataMap)
    {
        if (obj == null || obj.properties == null)
            return;

        float yOffset = 100;
        foreach (var prop in obj.properties)
        {
            var childNode = graphView.CreateNodeView(DetermineJsonType(prop.value), new Vector2(400, 200 + yOffset));
            childNode.SetKey(prop.key);
            childNode.SetValue(prop.value);
            
            nodeDataMap[childNode] = new JsonNodeData { type = DetermineJsonType(prop.value), key = prop.key, value = prop.value };
            
            yOffset += 80;
        }
    }

    private static JsonNodeType DetermineJsonType(string value)
    {
        if (value == null)
            return JsonNodeType.Null;

        value = value.Trim();

        if (value == "null")
            return JsonNodeType.Null;

        if (value.Equals("true", System.StringComparison.OrdinalIgnoreCase) || value.Equals("false", System.StringComparison.OrdinalIgnoreCase))
            return JsonNodeType.Boolean;

        if (value.StartsWith("{") && value.EndsWith("}"))
            return JsonNodeType.Object;

        if (value.StartsWith("[") && value.EndsWith("]"))
            return JsonNodeType.Array;

        if (float.TryParse(value, out _))
            return JsonNodeType.Number;

        return JsonNodeType.String;
    }
}

[System.Serializable]
public class JsonObject
{
    public JsonProperty[] properties;
}

[System.Serializable]
public class JsonProperty
{
    public string key;
    public string value;
}
