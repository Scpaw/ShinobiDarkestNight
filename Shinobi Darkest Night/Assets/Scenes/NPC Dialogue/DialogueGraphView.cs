using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{

    private readonly Vector2 defaultNodeSize = new Vector2();
    public DialogueGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var gridBG = new GridBackground();
        Insert(0, gridBG);
        gridBG.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
    }

    public override List<Port> GetCompatiblePorts(Port startport, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach((port) =>
        {
            if (startport != port && startport.node != port.node)
            { 
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts;
    }

    private Port GeneratePort(DIalogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection,capacity,typeof(float));
    }

    public void CreateNode(string nodeName)
    {
        AddElement(CreateDialogueNode(nodeName));
    }

    public DIalogueNode CreateDialogueNode(string NodeName)
    {
        var dialogueNode = new DIalogueNode
        {
            title = NodeName,
            DialogueText = NodeName,
            GUID = Guid.NewGuid().ToString(),
        };

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        var button = new Button(() => { AddChoicePort(dialogueNode); });
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);



        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));
        return dialogueNode;
    }

    private DIalogueNode GenerateEntryPointNode()
    {
        var node = new DIalogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "Entry point",
            EntryPoint = true
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(x: 100, y: 200, 100, 150));
        return node;
    }

    private void AddChoicePort(DIalogueNode dialogueNode)
    {
        var generatedPort = GeneratePort(dialogueNode,Direction.Output);

        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        generatedPort.portName = $"Choice {outputPortCount}";

        dialogueNode.outputContainer.Add(generatedPort);

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }
}
