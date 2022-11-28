using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelEditor.Sample
{
    public class SaveSample : MonoBehaviour
    {
        [SerializeField] NovelSaveData data;

        [SerializeField] NovelPlayer player;

        public void Save()
        {
            data = player.save();
        }

        public void Load()
        {
            player.Load(data, true);
        }

    }

}