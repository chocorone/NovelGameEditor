using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// ウィンドウを表示するためのクラス
/// </summary>
//internalにしたいが継承なので動くかどうか
public class NovelEditorWindow : EditorWindow
{
    [SerializeField] NovelData _editingData;


    /// <summary>
    /// ウィンドウを表示する
    /// </summary>
    internal void Init(NovelData data)
    {
        _editingData = data;
        rootVisualElement.Clear();
        rootVisualElement.Bind(new SerializedObject(this));
        Draw();
        Undo.undoRedoPerformed += () =>
            {
                rootVisualElement.Clear();
                Draw();
            };
    }

    void OnEnable()
    {
        Draw();

    }

    void Draw()
    {

        // GraphController controller = new GraphController();
        // NovelGraphView graphView = controller.CreateGraph();
        // rootVisualElement.Add(graphView);

        // string name = "NoData";
        // if (SaveUtility.Current.Data != null)
        // {
        //     name = SaveUtility.Current.DataName;
        // }

        // var box = new Box();
        // box.Add(new Label() { text = name });
        // rootVisualElement.Add(box);

    }

}