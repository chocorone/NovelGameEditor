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
        }

        return graphView;
    }

    internal void LoadNodes()
    {
        NovelData data = NovelEditorWindow.editingData;
        //データからノードを作る
        ParagraphNode.RestoreNode(graphView, data.paragraphsList);

        ChoiceNode.RestoreNode(graphView, data.choiceList);

        ParagraphNode.RestoreEdge(graphView, data.paragraphsList);

        ChoiceNode.RestoreEdge(graphView);
    }

    //グラフが変化した時の処理
    public GraphViewChange OnGraphChange(GraphViewChange change)
    {
        //エッジが作成されたとき、接続情報を保存
        if (change.edgesToCreate != null)
        {
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

        return change;
    }
}