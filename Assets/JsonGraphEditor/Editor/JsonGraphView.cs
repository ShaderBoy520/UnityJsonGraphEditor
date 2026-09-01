using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class JsonGraphView : GraphView
{
    public new class UxmlFactory : UxmlFactory<JsonGraphView, UxmlTraits> { }

    private JsonNodeView selectedNodeView;

    public JsonGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new Selection());
        this.AddManipulator(new RectangleSelector());

        var gridBackground = new GridBackground();
        Insert(0, gridBackground);
        gridBackground.StretchToParentSize();

        styleSheets.Add(Resources.Load<StyleSheet>("JsonGraphEditor/JsonGraphView"));
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        var startPortView = startPort.node as JsonNodeView;

        ports.ForEach(port =>
        {
            var portView = port.node as JsonNodeView;
            if (startPortView != portView && port.direction != startPort.direction && port.portName == startPort.portName)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    public JsonNodeView CreateNodeView(JsonNodeType nodeType, Vector2 position)
    {
        var nodeView = new JsonNodeView(nodeType)
        {
            title = nodeType.ToString()
        };
        nodeView.SetPosition(new Rect(position, Vector2.zero));
        AddElement(nodeView);
        return nodeView;
    }

    public void SetSelectedNodeView(JsonNodeView nodeView)
    {
        selectedNodeView = nodeView;
    }

    public JsonNodeView GetSelectedNodeView()
    {
        return selectedNodeView;
    }
}
