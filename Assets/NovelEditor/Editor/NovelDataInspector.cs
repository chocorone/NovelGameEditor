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
        if (GUILayout.Button("Open"))
        {
            if (noveldata.newData)
            {
                noveldata.ResetData();
                Debug.Log("NewData");
            }
            NovelEditor.Open(noveldata);
            Debug.Log(noveldata.MaxParagraphID);


        }
        serializedObject.ApplyModifiedProperties();
    }

}
