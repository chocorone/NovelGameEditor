using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelEditor
{
    public class DataLoader
    {
        internal NovelData.ParagraphData.Dialogue LoadDialogue(NovelData novelData, int ParagraphIndex, int dialogueIndex, List<int> passedParagraphId)
        {
            NovelData.ParagraphData.Dialogue data = new();

            data = JsonUtility.FromJson<NovelData.ParagraphData.Dialogue>(JsonUtility.ToJson(novelData.paragraphList[0].dialogueList[0]));

            while (true)
            {
                break;
            }

            return data;
        }
    }
}
