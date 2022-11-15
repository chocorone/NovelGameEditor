using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;
using static NovelData;

internal class MenuWindow : ScriptableObject, ISearchWindowProvider
{
    private NovelGraphView graphView;
    private EditorWindow _editorWindow;

    public void Init(NovelGraphView newGraphView)
    {
        graphView = newGraphView;
        _editorWindow = Resources.FindObjectsOfTypeAll<NovelEditorWindow>()[0] as EditorWindow;

        graphView.nodeCreationRequest += context =>
        {
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), this);
        };
        graphView.OnContextMenuNodeCreate = OnContextMenuNodeCreate;
        graphView.CopyNodes = OnContextMenuNodeCopy;
        graphView.PasteOnNode = OnContextMenuPasteOnNode;
        graphView.PasteOnGraph = OnContextMenuPasteOnGraph;
    }

    //右クリックで開くメニュー
    List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
    {
        var entries = new List<SearchTreeEntry>();
        entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));
        entries.Add(new SearchTreeEntry(new GUIContent(nameof(ParagraphNode))) { level = 1, userData = typeof(ParagraphNode) });
        entries.Add(new SearchTreeEntry(new GUIContent(nameof(ChoiceNode))) { level = 1, userData = typeof(ChoiceNode) });

        return entries;
    }

    //メニュからノードを追加する処理
    bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        var type = searchTreeEntry.userData as Type;
        Undo.RecordObject(NovelEditorWindow.editingData, "Create Node");
        var node = Activator.CreateInstance(type) as BaseNode;

        // マウスの位置にノードを追加
        var worldMousePosition = _editorWindow.rootVisualElement.ChangeCoordinatesTo(_editorWindow.rootVisualElement.parent, context.screenMousePosition - _editorWindow.position.position);
        var localMousePosition = graphView.contentViewContainer.WorldToLocal(worldMousePosition);
        node.SetPosition(new Rect(localMousePosition, new Vector2(100, 100)));

        //GraphViewにノードを追加して位置をセーブ
        graphView.AddElement(node);
        return true;
    }


    //Create Nodeを選んだ時の動作
    void OnContextMenuNodeCreate(DropdownMenuAction a)
    {
        if (graphView.nodeCreationRequest == null)
            return;

        var editorWindow = Resources.FindObjectsOfTypeAll<NovelEditorWindow>()[0] as EditorWindow; ;

        Vector2 screenPoint = editorWindow.position.position + a.eventInfo.mousePosition;
        graphView.nodeCreationRequest(new NodeCreationContext() { screenMousePosition = screenPoint });
    }

    string OnContextMenuNodeCopy(IEnumerable<GraphElement> elements)
    {
        CopyData copyData = new CopyData();
        foreach (GraphElement element in elements)
        {
            GraphElement e = element;
            if (e is ParagraphNode)
            {
                copyData.pnodes.Add(((ParagraphNode)e).data);
            }
            if (e is ChoiceNode)
            {
                copyData.cnodes.Add(((ChoiceNode)e).data);
            }
        }
        Debug.Log(copyData.pnodes.Count);
        string data = JsonUtility.ToJson(copyData);
        Debug.Log(data);
        return data;
    }

    void OnContextMenuPasteOnNode(string data, BaseNode node)
    {
        //node.overrideNode(data);
        Debug.Log(data);
    }

    void OnContextMenuPasteOnGraph(string data)
    {
        Debug.Log(data);
    }

    class CopyData
    {
        public List<ParagraphData> pnodes = new List<ParagraphData>();
        public List<ChoiceData> cnodes = new List<ChoiceData>();

    }
}
