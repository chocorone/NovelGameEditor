using System;
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
    internal List<ParagraphData> ParagraphStack = new List<ParagraphData>();

    //使ってないデータを入れるスタック
    [SerializeField, HideInInspector]
    internal List<ChoiceData> ChoiceStack = new List<ChoiceData>();


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
        ParagraphData data = new ParagraphData();
        data.SetEnable(true);
        data.SetIndex(MaxParagraphID);
        data.dialogueList.Add(new ParagraphData.Dialogue());
        data.dialogueList[0].text = "Paragraph";
        data.dialogueList[0].charas = new Sprite[locations.Count];
        data.dialogueList[0].howCharas = new CharaChangeStyle[locations.Count];
        data.ResetNext(Next.End);
        _paragraphList.Add(data);

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
        ChoiceData data = new ChoiceData();
        data.SetEnable(true);
        data.text = "Choice";
        data.SetIndex(MaxChoiceCnt);
        _choiceList.Add(data);
        EditorUtility.SetDirty(this);
        return data;
    }



    [System.SerializableAttribute]
    public class NodeData
    {
        #region ノード基本データ
        [SerializeField, HideInInspector] bool _enabled = true;
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
        public void SetNodeDeleted()
        {
            _enabled = false;
        }
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
