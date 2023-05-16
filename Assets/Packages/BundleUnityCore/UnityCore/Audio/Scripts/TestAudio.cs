
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

            private AudioController _audioController;

            private void Start()
            {
                _audioController = ServiceLocator.Instance.GetService<AudioController>();
            }


            private void Update()
            {
                /// world sounds testing ///
                if (Input.GetKeyUp(KeyCode.T))
                {
                    _audioController.PlayAudio(AudioWorld[0]); 
                }
                if (Input.GetKeyUp(KeyCode.G))
                {
                    _audioController.StopAudio(AudioWorld[0].Type);
                }
                if (Input.GetKeyUp(KeyCode.B))
                {
                    _audioController.RestartAudio(AudioWorld[0].Type);
                }

                if (Input.GetKeyUp(KeyCode.Y))
                {
                    _audioController.PlayAudio(AudioWorld[1]);
                }


                /// OST testing ///
                if (Input.GetKeyUp(KeyCode.U))
                {
                     _audioController.PlayAudio(AudioOST[0]);
                }
                if (Input.GetKeyUp(KeyCode.J))
                {
                    _audioController.StopAudio(AudioOST[0].Type);
                }
                if (Input.GetKeyUp(KeyCode.M))
                {
                    _audioController.RestartAudio(AudioOST[0].Type);
                }

                if (Input.GetKeyUp(KeyCode.I))
                {
                    _audioController.PlayAudio(AudioOST[1]);
                }



                /// UI testing ///
                if (Input.GetKeyUp(KeyCode.E))
                {
                    _audioController.PlayAudio(AudioUI[0]);
                }
                if (Input.GetKeyUp(KeyCode.D))
                {
                    _audioController.StopAudio(AudioUI[0].Type);
                }
                if (Input.GetKeyUp(KeyCode.C))
                {
                    _audioController.RestartAudio(AudioUI[0].Type);
                }

                if (Input.GetKeyUp(KeyCode.R))
                {
                    _audioController.PlayAudio(AudioUI[1]);
                }
            }
        }
    }
}

