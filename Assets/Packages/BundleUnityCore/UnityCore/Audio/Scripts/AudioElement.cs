
using System.Collections.Generic;
using UnityEngine;

namespace UnityCore
{
    namespace Audio
    {
        [System.Serializable]
        public class AudioElement
        {
            public AudioClip Clip;
            public AudioType Type;

            [Range(0f, 1f)]
            public float Volume = 1f;
            [Range(-3f, 3f)]
            public float Pitch = 1f;

            [Header("Randomizing Pitch Settings")]
            public bool RandomizePitchSlightly;

            [Range(0f, 1f)]
            public float PitchLowerLimitAddition = 0.2f;
            [Range(0f, 1f)]
            public float PitchUpperLimitAddition = 0.2f;
        }
    }

}
