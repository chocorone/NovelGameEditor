﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace NovelEditorPlugin
{
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
        [SerializeField, HideInInspector] internal Vector3 graphScale = new Vector3(1, 1, 1);
        [SerializeField, HideInInspector] internal Vector3 graphPosition = new Vector3(0, 0, 0);
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

        [SerializeField] internal List<Image> newLocations = new List<Image>();

        //使っていないデータを入れるスタック
        [SerializeField, HideInInspector]
        internal Stack<ParagraphData> ParagraphStack = new Stack<ParagraphData>();

        //使ってないデータを入れるスタック
        [SerializeField, HideInInspector]
        internal Stack<ChoiceData> ChoiceStack = new Stack<ChoiceData>();


        #endregion

        #region プロパティ
        public List<Image> locations => _locations;
        public List<ParagraphData> paragraphList => _paragraphList;
        public List<ChoiceData> choiceList => _choiceList;

        #endregion

        internal void changeLocation(List<Image> newLocations)
        {
            _locations = newLocations;
        }

        public void ResetData()
        {
            _paragraphList.Clear();

            ParagraphData pdata = CreateParagraph();
            pdata.dialogueList[0].text = "FirstParagraph";
            pdata.SetIndex(0);
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
            data.dialogueList[0].charaFadeColor = new Color[locations.Count];
            data.dialogueList[0].charaEffects = new Effect[locations.Count];
            data.dialogueList[0].charaEffectStrength = new int[locations.Count];
            data.ResetNext(Next.End);

            return data;
        }

        public ParagraphData CreateParagraphFromJson(string sdata)
        {
            ParagraphData data = JsonUtility.FromJson<ParagraphData>(sdata);
            ParagraphData popData = CreateParagraph();
            popData.ChangeDialogue(data.dialogueList);
            return popData;
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

        public ChoiceData CreateChoiceFromJson(string sdata)
        {
            ChoiceData data = JsonUtility.FromJson<ChoiceData>(sdata);
            ChoiceData popData = CreateChoice();
            popData.text = data.text;
            return popData;
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
            public abstract void SetNodeDeleted(NovelData editingData);
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

            public override void SetNodeDeleted(NovelData editingData)
            {
                this.SetEnable(false);
                editingData.ChoiceStack.Push(this);
            }
        }


        [System.SerializableAttribute]
        public class ParagraphData : NodeData
        {

            #region 段落の基本データ
            [SerializeField, HideInInspector]
            List<Dialogue> _dialogueList = new List<Dialogue>();

            [SerializeField, HideInInspector]
            public bool detailOpen = false;

            [SerializeField, HideInInspector]
            Next _next = Next.End;

            //次のポート番号,choiceID
            [SerializeField, HideInInspector]
            List<int> _nextChoiceIndexes = new List<int>();

            #endregion

            #region プロパティ
            public List<Dialogue> dialogueList => _dialogueList;
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

            public override void SetNodeDeleted(NovelData editingData)
            {
                this.ChangeNextParagraph(-1);
                this._dialogueList.Clear();
                this.SetEnable(false);
                editingData.ParagraphStack.Push(this);
            }


            internal void ChangeDialogue(List<Dialogue> newDialogueList)
            {
                _dialogueList = newDialogueList;
            }

            internal void UpdateOrder(NovelData editingData)
            {
                int back = 0;
                int[] charas = new int[editingData.locations.Count];


                foreach (var data in dialogueList)
                {
                    for (int i = 0; i < editingData.locations.Count; i++)
                    {
                        if (data.howCharas[i] == CharaChangeStyle.UnChange)
                        {
                            if (charas[i] == 0)
                            {
                                data.charas[i] = null;
                            }
                            else
                            {
                                data.charas[i] = (Sprite)EditorUtility.InstanceIDToObject(charas[i]);
                            }

                        }
                        else
                        {
                            if (data.charas[i] == null)
                            {
                                charas[i] = 0;
                            }
                            else
                            {
                                charas[i] = data.charas[i].GetInstanceID();
                            }

                        }
                    }

                    if (data.howBack == BackChangeStyle.UnChange)
                    {
                        if (back == 0)
                        {
                            data.back = null;
                        }
                        else
                        {
                            data.back = (Sprite)EditorUtility.InstanceIDToObject(back);
                        }
                    }
                    else
                    {
                        if (data.back == null)
                        {
                            back = 0;
                        }
                        else
                        {
                            back = data.back.GetInstanceID();
                        }
                    }
                }

            }

            //会話文ごとのデータ
            //SerializedPropertyで参照するため全部public
            [System.SerializableAttribute]
            public class Dialogue
            {
                public int index = 0;
                public bool open;
                public int elementNum = 7;

                public string Name = "";
                public string text;

                public Sprite back;
                public BackChangeStyle howBack;
                public Color backFadeColor = Color.white;
                public float backFadeSpeed = 1;

                [SerializeField] public CharaChangeStyle[] howCharas;
                [SerializeField] public Sprite[] charas;
                [SerializeField] public Color[] charaFadeColor;

                public bool changeFont = false;
                public TMP_FontAsset font;
                public Color fontColor = Color.white;
                public int fontSize = 30;

                public bool changeNameFont = false;
                public TMP_FontAsset nameFont;
                public Color nameColor = Color.white;


                public AudioClip BGM;
                public SoundStyle BGMStyle;
                public LoopMode BGMLoop;
                public int BGMCount = 1;
                public float BGMSecond = 20;
                public float BGMFadeTime = 3;
                public float BGMEndFadeTime = 3;

                public AudioClip SE;
                public SoundStyle SEStyle;
                public LoopMode SELoop;
                public int SECount = 1;
                public float SESecond = 20;
                public float SEFadeTime = 3;
                public float SEEndFadeTime = 3;


                public Effect backEffect;
                public int backEffectStrength;
                public Effect[] charaEffects;
                public int[] charaEffectStrength;
                public Effect FrontEffect;
                public int FrontEffectStrength;
                public Effect AllEffect;
                public int AllEffectStrength;
            }

        }

    }

}
