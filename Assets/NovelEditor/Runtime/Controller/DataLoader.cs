using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
            NovelData.ParagraphData.Dialogue first = savedData.novelData.paragraphList[savedData.passedParagraphId[0]].dialogueList[0];
            NovelData.ParagraphData.Dialogue data = JsonUtility.FromJson<NovelData.ParagraphData.Dialogue>(JsonUtility.ToJson(first));

            foreach (var i in savedData.passedParagraphId)
            {
                NovelData.ParagraphData nowParagraph = savedData.novelData.paragraphList[i];
                for (int j = 0; j < nowParagraph.dialogueList.Count; j++)
                {
                    NovelData.ParagraphData.Dialogue nowDialogue = nowParagraph.dialogueList[j];

                    data.back = nowDialogue.howBack != BackChangeStyle.UnChange ? nowDialogue.back : data.back;

                    data.backEffect = nowDialogue.backEffect != Effect.UnChange ? nowDialogue.backEffect : data.backEffect;
                    data.backEffectStrength = nowDialogue.backEffectStrength;

                    data.DialogueEffect = nowDialogue.DialogueEffect != Effect.UnChange ? nowDialogue.DialogueEffect : data.DialogueEffect;
                    data.DialogueEffectStrength = nowDialogue.DialogueEffectStrength;

                    if (nowDialogue.changeFont)
                    {
                        data.font = nowDialogue.font;
                        data.fontColor = nowDialogue.fontColor;
                        data.fontSize = nowDialogue.fontSize;
                    }

                    if (nowDialogue.changeNameFont)
                    {
                        data.nameFont = nowDialogue.nameFont;
                        data.nameColor = nowDialogue.nameColor;
                    }

                    data.SE = nowDialogue.SE;
                    data.SEStyle = nowDialogue.SEStyle != SoundStyle.UnChange ? nowDialogue.SEStyle : data.SEStyle;
                    data.SELoop = nowDialogue.SELoop;
                    data.SECount = nowDialogue.SECount;
                    data.SEFadeTime = nowDialogue.SEFadeTime;
                    data.SEEndFadeTime = nowDialogue.SEEndFadeTime;

                    data.BGM = nowDialogue.BGM;
                    data.BGMStyle = nowDialogue.BGMStyle != SoundStyle.UnChange ? nowDialogue.BGMStyle : data.BGMStyle;
                    data.BGMLoop = nowDialogue.BGMLoop;
                    data.BGMCount = nowDialogue.BGMCount;
                    data.BGMFadeTime = nowDialogue.BGMFadeTime;
                    data.BGMEndFadeTime = nowDialogue.BGMEndFadeTime;

                    for (int charaIndex = 0; charaIndex < data.charas.Length; charaIndex++)
                    {
                        data.charas[i] = data.howCharas[i] != CharaChangeStyle.UnChange ? nowDialogue.charas[i] : data.charas[i];
                        data.charaEffects[i] = data.charaEffects[i] != Effect.UnChange ? nowDialogue.charaEffects[i] : data.charaEffects[i];
                        data.charaEffectStrength[i] = data.charaEffectStrength[i];
                    }


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
                EditorUtility.ClearProgressBar();
            }

            return data;
        }

        internal NovelSaveData SaveDialogue(NovelData novelData, int paragraphIndex, int dialogueIndex, List<int> passedParagraphId)
        {
            NovelSaveData savedData = new(novelData, paragraphIndex, dialogueIndex, passedParagraphId);
            return savedData;
        }
    }

    public struct NovelSaveData
    {
        public NovelSaveData(NovelData novelData, int paragraphIndex, int dialogueIndex, List<int> passedParagraphId)
        {
            this.novelData = novelData;
            this.paragraphIndex = paragraphIndex;
            this.dialogueIndex = dialogueIndex;
            this.passedParagraphId = passedParagraphId;
        }

        public NovelData novelData;
        public int paragraphIndex;
        public int dialogueIndex;
        public List<int> passedParagraphId;
    }
}
