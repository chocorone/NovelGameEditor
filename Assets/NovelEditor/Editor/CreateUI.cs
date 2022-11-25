using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace NovelEditor.Editor
{
    public class CreateUI
    {
        [MenuItem("GameObject/UI/NovelUI", false, 10)]
        private static void CreateNovelUI(MenuCommand menuCommand)
        {
            // ゲームオブジェクトを生成します
            var novelUI = Resources.Load<GameObject>("NovelPlayer"); ;

            var obj = GameObject.Instantiate(novelUI);

            // 親を設定して同じレイヤーを継承
            GameObjectUtility.SetParentAndAlign(obj, menuCommand.context as GameObject);

            // Undo できるように
            Undo.RegisterCreatedObjectUndo(obj, "Create NovelUI");

            // 生成したゲームオブジェクトを選択状態に
            Selection.activeObject = obj;
        }
    }
}