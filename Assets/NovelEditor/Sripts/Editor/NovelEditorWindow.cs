﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// ウィンドウを表示するためのクラス
/// </summary>
public class NovelEditorWindow : EditorWindow
{
    [SerializeField] NovelData _editingData;
    internal static NovelData editingData => Instance._editingData;
    internal static bool Compiled = true;

    private static NovelEditorWindow instance;
    public static NovelEditorWindow Instance
    {
        get
        {
            if (instance == null)
            {
                instance = EditorWindow.GetWindow<NovelEditorWindow>("EditorWindow", typeof(UnityEditor.SceneView));
                if (instance == null)
                {
                    Debug.LogError("NullRefarenceException NovelEditorWindow SingltonError");
                }
            }
            return instance;
        }
    }


    /// <summary>
    /// ウィンドウを表示する
    /// </summary>
    internal void Init(NovelData data)
    {
        _editingData = data;
        rootVisualElement.Clear();
        rootVisualElement.Bind(new SerializedObject(this));

        Draw();
    }

    void OnEnable()
    {
        Draw();

    }

    void Draw()
    {
        Compiled = true;
        GraphController controller = new GraphController();
        NovelGraphView graphView = controller.CreateGraph();
        rootVisualElement.Add(graphView);

        string name = "NoData";
        if (_editingData != null)
        {
            name = _editingData.name;
        }

        var box = new Box();
        box.Add(new Label() { text = name });
        rootVisualElement.Add(box);
    }

}