using System;
using UnityEngine;

namespace RuneForger.Character
{
    [Serializable]
    public struct ComboInfo
    {
        public float Duration;
        public float TransTime;
        public float ExitTime;
    }

    [CreateAssetMenu(fileName = "ComboAsset", menuName = "RF_Character/ComboAsset")]
    public class ComboAsset : ScriptableObject
    {
        [field: SerializeField]
        public ComboInfo[] ComboInfos { get; private set; }
        public int ComboCount => ComboInfos.Length;
    }
}