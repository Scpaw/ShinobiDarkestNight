using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView graphView;
    private string fileName = "New Narrative";

    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    { 
        var window =GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent(text: "Dialoge Graph");
    }


    private void ConstructGraphView()
    {
        graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };

        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void GenerateToolBar()
    {
        var toolbar = new Toolbar();

        var fileNameTextField = new TextField("File Name");
        fileNameTextField.SetValueWithoutNotify(fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        var nodecreationButton = new Button(()=> 
        {
            graphView.CreateNode("Dialogue Node");
        
        });
        nodecreationButton.text = "Create Node";
        toolbar.Add(nodecreationButton);

        rootVisualElement.Add(toolbar);
    }


    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolBar();
    }


    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }
}
