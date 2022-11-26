using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace NovelEditor
{
    internal class DataLoader
    {
        internal float progress { get; private set; } = 0;

        static DataLoader instance;

        internal static DataLoader Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataLoader();
                }
                return instance;
            }
        }

        DataLoader()
        { }

        internal NovelData.ParagraphData.Dialogue LoadDialogue(NovelSaveData savedData)
        {
            progress = 0;
            NovelData.ParagraphData.Dialogue first = savedData.novelData.paragraphList[savedData.passedParagraphId[0]].dialogueList[0];
            NovelData.ParagraphData.Dialogue data = JsonUtility.FromJson<NovelData.ParagraphData.Dialogue>(JsonUtility.ToJson(first));
            if (data.BGMStyle == SoundStyle.UnChange)
                data.BGMStyle = SoundStyle.Stop;

            if (data.SEStyle == SoundStyle.UnChange)
                data.SEStyle = SoundStyle.Stop;

            foreach (var i in savedData.passedParagraphId)
            {
                NovelData.ParagraphData nowParagraph = savedData.novelData.paragraphList[i];
                for (int j = 0; j < nowParagraph.dialogueList.Count; j++)
                {
                    NovelData.ParagraphData.Dialogue nowDialogue = nowParagraph.dialogueList[j];

                    SaveNext(data, nowDialogue);

                    if (j == savedData.dialogueIndex && i == savedData.paragraphIndex)
                    {
                        data.Name = nowDialogue.Name;
                        data.text = nowDialogue.text;
                        data.howBack = BackChangeStyle.FadeAll;
                        data.backFadeColor = Color.black;
                        data.backFadeSpeed = 1.0f;

                        break;
                    }

                    progress += (100 / savedData.passedParagraphId.Count) / nowParagraph.dialogueList.Count;

#if UNITY_EDITOR
                    EditorUtility.DisplayProgressBar(
                            "NovelEditor",
                            "ロードしています",
                            progress / 100);
#endif
                }
#if UNITY_EDITOR
                EditorUtility.ClearProgressBar();
#endif
            }

            return data;
        }

        internal NovelSaveData SaveDialogue(NovelData novelData, int paragraphIndex, int dialogueIndex, List<int> passedParagraphIdList, List<string> choiceName, List<string> ParagraphName)
        {
            NovelSaveData savedData = new(novelData, paragraphIndex, --dialogueIndex, passedParagraphIdList, choiceName, ParagraphName);
            return savedData;
        }

        internal SkipedData Skip(NovelData novelData, NovelData.ParagraphData nowParagraphData, int dialogueIndex, List<int> passedParagraphID, List<string> paragraphName, Sprite nowBack)
        {
            if (dialogueIndex >= nowParagraphData.dialogueList.Count)
                dialogueIndex = nowParagraphData.dialogueList.Count - 1;
            NovelData.ParagraphData.Dialogue first = nowParagraphData.dialogueList[dialogueIndex];
            NovelData.ParagraphData.Dialogue data = JsonUtility.FromJson<NovelData.ParagraphData.Dialogue>(JsonUtility.ToJson(first));
            data.back = nowBack;
            SkipedData skipData = new SkipedData();

            while (true)
            {

                if (nowParagraphData.next == Next.End || (nowParagraphData.next == Next.Continue && nowParagraphData.nextParagraphIndex == -1))
                {
                    skipData.next = Next.End;
                    return skipData;
                }

                foreach (var dialogue in nowParagraphData.dialogueList)
                {
                    SaveNext(data, dialogue);
                }

                if (nowParagraphData.next == Next.Choice)
                {
                    skipData.dialogue = data;
                    skipData.next = Next.Choice;
                    skipData.ParagraphIndex = nowParagraphData.index;
                    return skipData;
                }
                else
                {
                    //選択肢のノードが続きになければ終わる
                    if (nowParagraphData.next == Next.End || nowParagraphData.nextParagraphIndex == -1)
                    {
                        skipData.next = Next.End;
                        return skipData;
                    }
                    nowParagraphData = novelData.paragraphList[nowParagraphData.nextParagraphIndex];
                    paragraphName.Add(nowParagraphData.name);
                    passedParagraphID.Add(nowParagraphData.index);
                }
            }
        }


        internal NovelData.ParagraphData.Dialogue SkipNextNode(NovelData novelData, NovelData.ParagraphData nowParagraphData, int dialogueIndex, Sprite nowBack)
        {
            progress = 0;
            if (dialogueIndex >= nowParagraphData.dialogueList.Count)
                dialogueIndex = nowParagraphData.dialogueList.Count - 1;
            NovelData.ParagraphData.Dialogue first = nowParagraphData.dialogueList[dialogueIndex];
            NovelData.ParagraphData.Dialogue data = JsonUtility.FromJson<NovelData.ParagraphData.Dialogue>(JsonUtility.ToJson(first));
            data.back = nowBack;
            if (nowParagraphData.next == Next.End || (nowParagraphData.next == Next.Continue && nowParagraphData.nextParagraphIndex == -1))
            {
                return null;
            }

            for (int i = dialogueIndex; i < nowParagraphData.dialogueList.Count; i++)
            {
                SaveNext(data, nowParagraphData.dialogueList[i]);
                progress += 100 / (nowParagraphData.dialogueList.Count - dialogueIndex);

#if UNITY_EDITOR
                EditorUtility.DisplayProgressBar(
                        "NovelEditor",
                        "次のノードへスキップしています",
                        progress / 100);
#endif
            }

            if (nowParagraphData.next == Next.Continue)
            {
                SaveNext(data, novelData.paragraphList[nowParagraphData.nextParagraphIndex].dialogueList[0]);
            }

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif

            return data;
        }


        void SaveNext(NovelData.ParagraphData.Dialogue data, NovelData.ParagraphData.Dialogue nowDialogue)
        {
            data.Name = nowDialogue.Name;
            data.text = nowDialogue.text;
            data.howBack = BackChangeStyle.FadeAll;
            data.backFadeColor = Color.black;
            data.backFadeSpeed = 1.0f;

            data.back = nowDialogue.howBack != BackChangeStyle.UnChange ? nowDialogue.back : data.back;

            data.backEffect = nowDialogue.backEffect != Effect.UnChange ? nowDialogue.backEffect : data.backEffect;
            data.backEffectStrength = nowDialogue.backEffectStrength;

            data.DialogueEffect = nowDialogue.DialogueEffect != Effect.UnChange ? nowDialogue.DialogueEffect : data.DialogueEffect;
            data.DialogueEffectStrength = nowDialogue.DialogueEffectStrength;

            if (nowDialogue.changeFont)
            {
                data.changeFont = true;
                data.font = nowDialogue.font;
                data.fontColor = nowDialogue.fontColor;
                data.fontSize = nowDialogue.fontSize;
            }

            if (nowDialogue.changeNameFont)
            {
                data.changeNameFont = true;
                data.nameFont = nowDialogue.nameFont;
                data.nameColor = nowDialogue.nameColor;
            }

            data.SE = nowDialogue.SEStyle != SoundStyle.UnChange ? nowDialogue.SE : data.SE;
            data.SEStyle = nowDialogue.SEStyle != SoundStyle.UnChange ? nowDialogue.SEStyle : data.SEStyle;
            data.SELoop = nowDialogue.SELoop;
            data.SECount = nowDialogue.SECount;
            data.SEFadeTime = nowDialogue.SEFadeTime;
            data.SEEndFadeTime = nowDialogue.SEEndFadeTime;

            data.BGM = nowDialogue.BGMStyle != SoundStyle.UnChange ? nowDialogue.BGM : data.BGM;
            data.BGMStyle = nowDialogue.BGMStyle != SoundStyle.UnChange ? nowDialogue.BGMStyle : data.BGMStyle;
            data.BGMLoop = nowDialogue.BGMLoop;
            data.BGMCount = nowDialogue.BGMCount;
            data.BGMFadeTime = nowDialogue.BGMFadeTime;
            data.BGMEndFadeTime = nowDialogue.BGMEndFadeTime;

            for (int charaIndex = 0; charaIndex < data.charas.Length; charaIndex++)
            {
                data.charas[charaIndex] = data.howCharas[charaIndex] != CharaChangeStyle.UnChange ? nowDialogue.charas[charaIndex] : data.charas[charaIndex];
                data.charaEffects[charaIndex] = data.charaEffects[charaIndex] != Effect.UnChange ? nowDialogue.charaEffects[charaIndex] : data.charaEffects[charaIndex];
                data.charaEffectStrength[charaIndex] = data.charaEffectStrength[charaIndex];
            }

        }
    }

    public struct NovelSaveData
    {
        public NovelSaveData(NovelData novelData, int paragraphIndex, int dialogueIndex, List<int> passedParagraphId, List<string> choiceName, List<string> ParagraphName)
        {
            this.novelData = novelData;
            this.paragraphIndex = paragraphIndex;
            this.dialogueIndex = dialogueIndex;
            this.passedParagraphId = passedParagraphId;
            this.ParagraphName = ParagraphName;
            this.choiceName = choiceName;
        }
        public NovelData novelData;
        public int paragraphIndex;
        public int dialogueIndex;
        public List<int> passedParagraphId;
        public List<string> choiceName;
        public List<string> ParagraphName;

    }

    internal struct SkipedData
    {
        public NovelData.ParagraphData.Dialogue dialogue;
        public int ParagraphIndex;
        public Next next;
    }
}
