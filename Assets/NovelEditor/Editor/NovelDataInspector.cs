using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using static NovelData;
using static NovelData.ParagraphData;

[CustomEditor(typeof(NovelData))]
internal class NovelDataInspector : Editor
{
    NovelData noveldata;

    void OnEnable()
    {
        noveldata = target as NovelData;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        if (GUILayout.Button("プレハブをセットしたら押す　同じ名前なら引き継がれます"))
        {
            //ここどうにかしたい
            //ResetLocations();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Open"))
        {
            if (noveldata.newData)
            {
                noveldata.ResetData();
            }
            NovelEditor.Open(noveldata);
        }
        serializedObject.ApplyModifiedProperties();
    }



}
