using UnityEngine;
using UnityEditor;
using static NovelData;
using static NovelData.ParagraphData;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using UnityEditor.UIElements;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;


internal class ParagraphNode : BaseNode
{
    public static List<ParagraphNode> nodes = new List<ParagraphNode>();
    public ParagraphData data => (ParagraphData)nodeData;
    internal List<Port> choicePorts = new List<Port>();

    public static void RestoreNode(GraphView graphView, List<ParagraphData> paragraphData)
    {
        nodes = new List<ParagraphNode>();
        Debug.Log(paragraphData.Count);
        for (int i = 0; i < paragraphData.Count; i++)
        {
            nodes.Add(null);
        }

        foreach (ParagraphData pdata in paragraphData)
        {
            if (pdata.index != -1)
            {
                ParagraphNode node = new ParagraphNode(pdata);
                graphView.AddElement(node);
            }
        }
    }

    public static void RestoreEdge(GraphView graphView, List<ParagraphData> paragraphData)
    {
        //ノードを接続する
        foreach (ParagraphNode node in nodes)
        {
            if (node == null || node.data.index == -1) continue;

            if (node.data.next == Next.Continue)
            {
                //ParagraphからParagraphにつなぐ
                if (node.data.nextParagraphIndex == -1)
                    continue;

                Edge edge = node.CountinuePort.ConnectTo(nodes[node.data.nextParagraphIndex].InputPort);
                graphView.AddElement(edge);
            }

            else if (node.data.next == Next.Choice)
            {
                //ParagraphからChoiceにつなぐ
                for (int i = 0; i < node.data.nextChoiceIndexes.Count; i++)
                {
                    int index = node.data.nextChoiceIndexes[i];
                    if (index == -1)
                    {
                        continue;
                    }
                    Edge edge = node.choicePorts[i].ConnectTo(ChoiceNode.nodes[index].InputPort);
                    graphView.AddElement(edge);
                }
            }
        }
    }

    //0から作られるとき
    public ParagraphNode()
    {
        //データを作成する
        nodeData = NovelEditorWindow.editingData.CreateParagraph();
        NodeSet();
        nodes.Add(this);
    }


    //データをもとに作られるとき
    public ParagraphNode(ParagraphData Pdata)
    {
        nodeData = Pdata;

        NodeSet();
        SetPosition(data.nodePosition);
        nodes[data.index] = this;
    }

    private protected override void NodeSet()
    {
        base.NodeSet();
        //ノードの色変更
        if (data.index == 0)
        {
            titleContainer.style.backgroundColor = new Color(0.8f, 0.2f, 0.4f);
        }
        else
        {
            titleContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.4f);
        }



        SetTitle();

        //ボタン作成
        styleSheets.Add(Resources.Load<StyleSheet>("PNodeUSS"));
        var visualTree = Resources.Load<VisualTreeAsset>("NodeUXML");
        visualTree.CloneTree(titleButtonContainer);
        var addbutton = titleButtonContainer.Q<Button>("addChoiceButton");
        addbutton.clickable.clicked += () =>
        {
            data.AddNext();
            AddChoicePort();
        };

        //ドロップダウン作成
        var nextStateField = new EnumField(data.next);
        nextStateField.RegisterValueChangedCallback(evt =>
        { FieldChangedEvent((Next)nextStateField.value); });
        mainContainer.Add(nextStateField);

        //InputPort作成
        InputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(BaseNode));
        InputPort.portName = "prev";
        inputContainer.Add(InputPort);

        //OutputPort作成
        InitOutputPort();
    }

    protected override void SetTitle()
    {
        title = "Paragraph";
        if (data != null)
        {
            title = data.dialogueList[0].text.Substring(0, Math.Min(data.dialogueList[0].text.Length, 10));
        }
    }

    protected void AddChoicePort()
    {
        if (data.next == Next.Choice)
        {
            //-ボタン作成
            Button removePortButton = new Button();
            removePortButton.text = "-";
            //ChoicePort作成
            Port outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(ChoiceNode));
            outputPort.portColor = new Color(0.7f, 0.7f, 0.0f);
            removePortButton.clickable.clicked += () =>
            {
                RemoveChoicePort(removePortButton, outputPort);
            };
            outputPort.portName = "choice" + (choicePorts.Count + 1).ToString();
            outputContainer.Add(outputPort);
            outputContainer.Add(removePortButton);
            choicePorts.Add(outputPort);

            RefreshPorts();
            RefreshExpandedState();
        }
    }

    private void RemoveChoicePort(Button rmvButton, Port outPort)
    {

        int index = choicePorts.IndexOf(outPort);
        choicePorts.RemoveAt(index);
        data.RemoveChoice(index);

        //ポートから生えてるエッジを削除する
        foreach (Edge e in outPort.connections)
        {
            e.input.Disconnect(e);
            e.RemoveFromHierarchy();
        }

        //Choiceポートとボタンを削除する
        outputContainer.Remove(rmvButton);
        outputContainer.Remove(outPort);

    }

    private void InitOutputPort()
    {
        outputContainer.Clear();
        switch (data.next)
        {
            case Next.Continue:
                CountinuePort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(BaseNode));
                CountinuePort.portColor = new Color(0.8f, 0.2f, 0.4f);
                CountinuePort.portName = "next";
                outputContainer.Add(CountinuePort);
                break;

            case Next.Choice:
                //必要な数分だけ作る
                for (int i = 0; i < data.nextChoiceIndexes.Count; i++)
                {
                    AddChoicePort();
                }

                break;
        }
        RefreshExpandedState();
    }

    private void FieldChangedEvent(Next nextValue)
    {
        data.ResetNext(nextValue);
        RemoveAllOutEdge();
        InitOutputPort();
    }

    private void RemoveAllOutEdge()
    {
        foreach (Port i in choicePorts)
        {
            foreach (Edge e in i.connections)
            {
                e.input.Disconnect(e);
                e.RemoveFromHierarchy();
            }
        }
        choicePorts = new List<Port>();

        if (CountinuePort != null)
        {
            foreach (Edge e in CountinuePort.connections)
            {
                e.input.Disconnect(e);
                e.RemoveFromHierarchy();
            }
        }
    }

    internal override void AddNext(BaseNode nextNode, Port outPort)
    {
        if (nextNode is ParagraphNode)
        {
            data.ChangeNextParagraph(((ParagraphNode)nextNode).data.index);
        }
        if (nextNode is ChoiceNode)
        {
            int portNum = choicePorts.IndexOf(outPort);
            data.ChangeNextChoice(portNum, ((ChoiceNode)nextNode).data.index);
        }
    }
    public override void ResetNext(Edge edge)
    {
        if (edge.input.node is ChoiceNode)
        {
            int index = choicePorts.IndexOf(edge.output);
            data.ChangeNextChoice(index, -1);
        }

        if (edge.input.node is ParagraphNode)
        {
            data.ChangeNextParagraph(-1);
        }

    }
}