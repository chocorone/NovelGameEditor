using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static NovelData;

internal class GraphController
{
    NovelGraphView graphView;

    internal NovelGraphView CreateGraph()
    {
        graphView = new NovelGraphView();

        if (NovelEditorWindow.editingData != null)
        {
            //グラフが変化した時の処理
            graphView.graphViewChanged += OnGraphChange;

            //右クリックで表示できるメニューの作成
            MenuWindow menuWindow = ScriptableObject.CreateInstance<MenuWindow>();
            menuWindow.Init(graphView);

            LoadNodes();

            Undo.undoRedoPerformed += () =>
            {
                NovelEditorWindow.Compiled = true;
                foreach (var element in graphView.graphElements)
                {
                    if (element is BaseNode || element is Edge)
                    {
                        element.RemoveFromHierarchy();
                    }
                }
                LoadNodes();
            };

        }

        return graphView;
    }

    internal void LoadNodes()
    {
        //データからノードを作る
        NodeCreator.RestoreGraph(graphView, NovelEditorWindow.editingData);
    }

    //グラフが変化した時の処理
    public GraphViewChange OnGraphChange(GraphViewChange change)
    {
        //エッジが作成されたとき、接続情報を保存
        if (change.edgesToCreate != null)
        {
            Undo.RecordObject(NovelEditorWindow.editingData, "Create Edge");
            //作成された全てのエッジを取得
            foreach (Edge edge in change.edgesToCreate)
            {
                //ノード同士の接続
                if (edge.output.node is BaseNode && edge.input.node is BaseNode)
                {
                    ((BaseNode)edge.output.node).AddNext((BaseNode)edge.input.node, edge.output);
                }
            }

        }

        //何かが削除された時
        if (change.elementsToRemove != null)
        {
            Undo.RecordObject(NovelEditorWindow.editingData, "Delete Graph Elememts");
            //全ての削除された要素を取得
            foreach (GraphElement e in change.elementsToRemove)
            {
                //ノードが削除されたとき
                if (e is BaseNode)
                {
                    ((BaseNode)e).DeleteNode();
                }

                //エッジが削除されたとき
                if (e.GetType() == typeof(Edge))
                {
                    Edge edge = (Edge)e;
                    ((BaseNode)edge.output.node).ResetNext(edge);
                }
            }

        }

        //ノードが動いた時、位置を保存
        if (change.movedElements != null)
        {
            foreach (GraphElement e in change.movedElements)
            {
                if (e is BaseNode)
                {
                    ((BaseNode)e).SaveCurrentPosition();
                }
            }
        }

        if (NovelEditorWindow.editingData != null)
            EditorUtility.SetDirty(NovelEditorWindow.editingData);

        return change;
    }
}