using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


/// <summary>
/// NovelEditorを制御するためのクラス
/// </summary>
public class NovelEditor
{
    /// <summary>
    /// NovelEditorWindowを開き、データを保存できるようにする
    /// </summary>
    /// <param name="data">編集したいデータ</param>
    public static void Open(NovelData data)
    {
        Debug.Log("bbb");
        //ウィンドウ作成
        var window = EditorWindow.GetWindow<NovelEditorWindow>(typeof(UnityEditor.SceneView));
        window.Init(data);
    }

    [MenuItem("Tool/NovelEditor")]
    public static void Open()
    {
        Debug.Log("bbb");
        //ウィンドウ作成
        var window = EditorWindow.GetWindow<NovelEditorWindow>(typeof(UnityEditor.SceneView));
        window.Init(null);
    }
}
