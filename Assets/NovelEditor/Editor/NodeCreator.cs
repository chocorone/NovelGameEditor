using UnityEngine;
using UnityEditor;
using static NovelData;
using static NovelData.ParagraphData;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
public static class NodeCreator
{
    internal static void RestoreGraph(GraphView graphView, NovelData data)
    {
        RestoreParagraphNode(graphView, data.paragraphsList);
        RestoreChoiceNode(graphView, data.choiceList);
        RestoreParagraphEdge(graphView);
        RestoreChoiceEdge(graphView);
    }

    static void RestoreParagraphNode(GraphView graphView, List<ParagraphData> paragraphData)
    {
        ParagraphNode.nodes.Clear();
        for (int i = 0; i < paragraphData.Count; i++)
        {
            ParagraphNode.nodes.Add(null);
        }

        foreach (ParagraphData pdata in paragraphData)
        {
            if (pdata.enabled)
            {
                ParagraphNode node = new ParagraphNode(pdata);
                graphView.AddElement(node);
            }
        }
    }

    static void RestoreChoiceNode(GraphView graphView, List<ChoiceData> choiceData)
    {
        ChoiceNode.nodes.Clear();
        for (int i = 0; i < choiceData.Count; i++)
        {
            ChoiceNode.nodes.Add(null);
        }

        foreach (ChoiceData cdata in choiceData)
        {
            if (cdata.enabled)
            {
                ChoiceNode node = new ChoiceNode(cdata);
                graphView.AddElement(node);
            }
        }
    }

    static void RestoreParagraphEdge(GraphView graphView)
    {
        //ノードを接続する
        foreach (ParagraphNode node in ParagraphNode.nodes)
        {
            if (node == null) continue;

            if (node.data.next == Next.Continue)
            {
                //ParagraphからParagraphにつなぐ
                if (node.data.nextParagraphIndex == -1)
                    continue;

                Edge edge = node.CountinuePort.ConnectTo(ParagraphNode.nodes[node.data.nextParagraphIndex].InputPort);
                graphView.AddElement(edge);
            }

            else if (node.data.next == Next.Choice)
            {
                //ParagraphからChoiceにつなぐ
                for (int i = 0; i < node.data.nextChoiceIndexes.Count; i++)
                {
                    int index = node.data.nextChoiceIndexes[i];
                    if (index == -1)
                        continue;

                    Edge edge = node.choicePorts[i].ConnectTo(ChoiceNode.nodes[index].InputPort);
                    graphView.AddElement(edge);
                }
            }
        }
    }

    static void RestoreChoiceEdge(GraphView graphView)
    {
        //ノードを接続する
        foreach (ChoiceNode node in ChoiceNode.nodes)
        {
            if (node == null) continue;
            //ChoiceからParagraphにつなぐ
            if (node.data.nextParagraphIndex == -1)
                continue;

            Edge edge = node.CountinuePort.ConnectTo(ParagraphNode.nodes[node.data.nextParagraphIndex].InputPort);
            graphView.AddElement(edge);
        }
    }


}
