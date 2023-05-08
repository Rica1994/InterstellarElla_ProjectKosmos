
using System.Collections.Generic;
using UnityEngine;

namespace UnityCore
{
    namespace Audio
    {
        public class TestAudio : MonoBehaviour
        {
            public List<AudioElement> AudioWorld = new List<AudioElement>();
            public List<AudioElement> AudioOST = new List<AudioElement>();
            public List<AudioElement> AudioUI = new List<AudioElement>();


            private void Update()
            {
                /// world sounds testing ///
                if (Input.GetKeyUp(KeyCode.T))
                {
                    AudioController.Instance.PlayAudio(AudioWorld[0]); 
                }
                if (Input.GetKeyUp(KeyCode.G))
                {
                    AudioController.Instance.StopAudio(AudioWorld[0].Type);
                }
                if (Input.GetKeyUp(KeyCode.B))
                {
                    AudioController.Instance.RestartAudio(AudioWorld[0].Type);
                }

                if (Input.GetKeyUp(KeyCode.Y))
                {
                    AudioController.Instance.PlayAudio(AudioWorld[1]);
                }


                /// OST testing ///
                if (Input.GetKeyUp(KeyCode.U))
                {
                    AudioController.Instance.PlayAudio(AudioOST[0]);
                }
                if (Input.GetKeyUp(KeyCode.J))
                {
                    AudioController.Instance.StopAudio(AudioOST[0].Type);
                }
                if (Input.GetKeyUp(KeyCode.M))
                {
                    AudioController.Instance.RestartAudio(AudioOST[0].Type);
                }

                if (Input.GetKeyUp(KeyCode.I))
                {
                    AudioController.Instance.PlayAudio(AudioOST[1]);
                }



                /// UI testing ///
                if (Input.GetKeyUp(KeyCode.E))
                {
                    AudioController.Instance.PlayAudio(AudioUI[0]);
                }
                if (Input.GetKeyUp(KeyCode.D))
                {
                    AudioController.Instance.StopAudio(AudioUI[0].Type);
                }
                if (Input.GetKeyUp(KeyCode.C))
                {
                    AudioController.Instance.RestartAudio(AudioUI[0].Type);
                }

                if (Input.GetKeyUp(KeyCode.R))
                {
                    AudioController.Instance.PlayAudio(AudioUI[1]);
                }
            }
        }
    }
}

