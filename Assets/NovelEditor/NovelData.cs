﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


[CreateAssetMenu(menuName = "Scriptable/Create NovelData")]
public class NovelData : ScriptableObject
{
    #region 会話基本データ

    //立ち絵の位置
    [SerializeField] List<Image> _locations = new List<Image>();

    //段落のノードのリスト
    [SerializeField, HideInInspector]
    List<ParagraphData> _paragraphList = new List<ParagraphData>();

    //選択肢のノードのリスト
    [SerializeField, HideInInspector]
    List<ChoiceData> _choiceList = new List<ChoiceData>();

    #endregion

    #region グラフ情報
    //グラフのズーム量
    [SerializeField, HideInInspector] internal float graphZoomValue;
    #endregion

    #region エディタから使用する情報
    //段落のノード数(非アクティブ含む)
    internal int MaxParagraphID => _paragraphList.Count;
    //選択肢のノード数(非アクティブ含む)
    internal int MaxChoiceCnt => _choiceList.Count;
    //データが新しいか
    internal bool newData => _paragraphList.Count == 0;

    [SerializeField, HideInInspector]
    internal bool havePreLocations = false;

    [SerializeField, HideInInspector]
    internal List<Image> prelocations = new List<Image>();

    //使っていないデータを入れるスタック
    [SerializeField, HideInInspector]
    internal Stack<ParagraphData> ParagraphStack = new Stack<ParagraphData>();

    //使ってないデータを入れるスタック
    [SerializeField, HideInInspector]
    internal Stack<ChoiceData> ChoiceStack = new Stack<ChoiceData>();


    #endregion

    #region プロパティ
    public List<Image> locations => _locations;
    public List<ParagraphData> paragraphsList => _paragraphList;
    public List<ChoiceData> choiceList => _choiceList;

    #endregion


    public void ResetData()
    {
        _paragraphList.Clear();

        ParagraphData pdata = new ParagraphData();
        pdata.SetEnable(true);
        pdata.dialogueList.Add(new ParagraphData.Dialogue());
        pdata.dialogueList[0].text = "FirstParagraph";
        pdata.SetIndex(0);
        pdata.ResetNext(Next.End);

        _paragraphList.Add(pdata);
    }

    public ParagraphData CreateParagraph()
    {
        ParagraphData data;
        if (ParagraphStack.Count == 0)
        {
            data = new ParagraphData();
            data.SetIndex(MaxParagraphID);
            _paragraphList.Add(data);
        }
        else
        {
            data = ParagraphStack.Pop();
        }

        data.SetEnable(true);
        data.dialogueList.Add(new ParagraphData.Dialogue());
        data.dialogueList[0].text = "Paragraph";
        data.dialogueList[0].charas = new Sprite[locations.Count];
        data.dialogueList[0].howCharas = new CharaChangeStyle[locations.Count];
        data.ResetNext(Next.End);

        Debug.Log(data.index);

        return data;
    }

    public ParagraphData CreateParagraphFromJson(string sdata)
    {
        ParagraphData data = JsonUtility.FromJson<ParagraphData>(sdata);
        data.SetEnable(true);
        data.SetIndex(MaxParagraphID);
        data.ResetNext(Next.End);
        _paragraphList.Add(data);
        return data;
    }

    public ChoiceData CreateChoice()
    {
        ChoiceData data;
        if (ChoiceStack.Count == 0)
        {
            data = new ChoiceData();
            data.SetIndex(MaxChoiceCnt);
            _choiceList.Add(data);
        }
        else
        {
            data = ChoiceStack.Pop();
        }
        data.text = "Choice";
        data.SetEnable(true);

        return data;
    }



    [System.SerializableAttribute]
    public abstract class NodeData
    {
        #region ノード基本データ
        [SerializeField, HideInInspector] protected bool _enabled = true;
        [SerializeField, HideInInspector] int _index;
        [SerializeField, HideInInspector] Rect _nodePosition;
        [SerializeField, HideInInspector] int _nextParagraphIndex = -1;
        #endregion

        #region プロパティ
        public bool enabled => _enabled;
        public int index => _index;
        public int nextParagraphIndex => _nextParagraphIndex;
        public Rect nodePosition => _nodePosition;

        #endregion

        public void SavePosition(Rect rect)
        {
            _nodePosition = rect;
        }
        public abstract void SetNodeDeleted();
        public void ChangeNextParagraph(int nextIndex)
        {
            _nextParagraphIndex = nextIndex;
        }
        public void SetIndex(int newIndex)
        {
            _index = newIndex;
        }

        public void SetEnable(bool flag)
        {
            _enabled = flag;
        }

    }

    [System.SerializableAttribute]
    public class ChoiceData : NodeData
    {
        public string text;

        public override void SetNodeDeleted()
        {
            this.SetEnable(false);
            NovelEditorWindow.editingData.ChoiceStack.Push(this);
        }
    }


    [System.SerializableAttribute]
    public class ParagraphData : NodeData
    {

        #region 段落の基本データ
        [SerializeField, HideInInspector]
        List<Dialogue> _dialogueList = new List<Dialogue>();

        [SerializeField, HideInInspector]
        bool _detailOpen = false;

        [SerializeField, HideInInspector]
        Next _next = Next.End;

        //次のポート番号,choiceID
        [SerializeField, HideInInspector]
        List<int> _nextChoiceIndexes = new List<int>();

        #endregion

        #region プロパティ
        public List<Dialogue> dialogueList => _dialogueList;
        public bool detailOpen => _detailOpen;
        public Next next => _next;
        public List<int> nextChoiceIndexes => _nextChoiceIndexes;
        #endregion

        internal void RemoveChoice(int removeIndex)
        {
            _nextChoiceIndexes.RemoveAt(removeIndex);
        }
        internal void ChangeNextChoice(int portIndex, int nextIndex)
        {
            _nextChoiceIndexes[portIndex] = nextIndex;
        }
        internal void ResetNext(Next newNext)
        {
            _next = newNext;
            _nextChoiceIndexes.Clear();
            _nextChoiceIndexes.Add(-1);
            ChangeNextParagraph(-1);
        }
        internal void AddNext()
        {
            _nextChoiceIndexes.Add(-1);
        }

        public override void SetNodeDeleted()
        {
            this.ChangeNextParagraph(-1);
            this._dialogueList.Clear();
            this.SetEnable(false);
            //ここよくない
            NovelEditorWindow.editingData.ParagraphStack.Push(this);
        }

        //会話文ごとのデータ
        //SerializedPropertyで参照するため全部public
        [System.SerializableAttribute]
        public class Dialogue
        {
            public int index = 0;
            public bool open;
            public int elementNum = 7;

            [SerializeField] public CharaChangeStyle[] howCharas;
            [SerializeField] public Sprite[] charas;

            public Sprite back;
            public BackChangeStyle howBack;
            public Color backFadeColor = Color.white;

            public string Name = "";
            [TextArea(1, 6)]
            public string text;

            public bool changeFont = false;
            public Font font;
            public Color fontColor = Color.white;
            public FontStyle fontStyle;
            public int fontSize = 30;

            public Font nameFont;
            public Color nameColor = Color.white;
            public FontStyle nameFontStyle;


            public AudioClip BGM;
            public SoundStyle BGMStyle;
            public LoopMode BGMLoop;
            public int BGMCount = 1;
            public float BGMSecond = 20;
            public bool BGMFade = false;
            public float BGMFadeTime = 3;
            public bool BGMEndFade = false;
            public float BGMEndFadeTime = 3;

            public AudioClip SE;
            public SoundStyle SEStyle;
            public LoopMode SELoop;
            public int SECount = 1;
            public float SESecond = 20;
            public bool SEFade = false;
            public float SEFadeTime = 3;
            public bool SEEndFade = false;
            public float SEEndFadeTime = 3;


            public Effect backEffect;
            public int backEffectStrength;
            public Effect[] charaEffects;
            public int[] charaEffectStrength;
            public Effect dialogueEffect;
            public int dialogueFieldEffectStrength;
        }

    }

}