using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static NovelData;
using static NovelData.ParagraphData;

internal class ChoiceNode : BaseNode
{
    public static List<ChoiceNode> nodes = new List<ChoiceNode>();
    public ChoiceData data => (ChoiceData)nodeData;

    public static void RestoreNode(GraphView graphView, List<ChoiceData> choiceData)
    {
        nodes = new List<ChoiceNode>();
        for (int i = 0; i < choiceData.Count; i++)
        {
            nodes.Add(null);
        }

        foreach (ChoiceData cdata in choiceData)
        {
            if (cdata.index != -1)
            {
                ChoiceNode node = new ChoiceNode(cdata);
                graphView.AddElement(node);
            }
        }
    }

    public static void RestoreEdge(GraphView graphView)
    {
        //ノードを接続する
        foreach (ChoiceNode node in nodes)
        {
            if (node == null || node.data.index == -1) continue;
            //ChoiceからParagraphにつなぐ
            if (node.data.nextParagraphIndex == -1)
                continue;

            Edge edge = node.CountinuePort.ConnectTo(ParagraphNode.nodes[node.data.nextParagraphIndex].InputPort);
            graphView.AddElement(edge);
        }
    }

    //0から作られるとき
    public ChoiceNode()
    {
        //データを作成する
        nodeData = NovelEditorWindow.editingData.CreateChoice();
        NodeSet();
        nodes.Add(this);
    }

    //データをもとに作られるとき
    public ChoiceNode(ChoiceData Cdata)
    {
        nodeData = Cdata;

        NodeSet();
        SetPosition(data.nodePosition);
        nodes[data.index] = this;
    }


    private protected override void NodeSet()
    {
        base.NodeSet();

        SetTitle();

        //InputPort作成
        InputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(ChoiceNode));
        InputPort.portColor = new Color(0.7f, 0.7f, 0.0f);
        InputPort.portName = "prev";
        inputContainer.Add(InputPort);

        //OutputPort作成
        CountinuePort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(BaseNode));
        CountinuePort.portName = "next";
        outputContainer.Add(CountinuePort);
    }

    protected override void SetTitle()
    {
        title = "Choice";
        if (data != null)
        {
            title = data.text.Substring(0, Math.Min(data.text.Length, 10));
        }
    }

    internal override void AddNext(BaseNode nextNode, Port outPort)
    {
        data.ChangeNextParagraph(((ParagraphNode)nextNode).data.index);
    }

    public override void ResetNext(Edge edge)
    {
        data.ChangeNextParagraph(-1);
    }

}