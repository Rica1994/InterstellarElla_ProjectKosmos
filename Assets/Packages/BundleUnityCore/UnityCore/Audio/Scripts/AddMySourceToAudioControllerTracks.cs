using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;
using static UnityCore.Audio.AudioController;



/// <summary>
/// Add this script to objects in he scene when they hold the AudioSource component
/// </summary>
public class AddMySourceToAudioControllerTracks : MonoBehaviour
{
    [SerializeField]
    private AudioTrack _myAudioTrack;


    void Start()
    {
        if (_myAudioTrack.Source != null)
        {
            switch (_myAudioTrack.Type)
            {
                case UnityCore.Audio.AudioType.OST:
                    AudioController.Instance.TracksOST.Add(_myAudioTrack);
                    break;
                case UnityCore.Audio.AudioType.SFX_UI:
                    AudioController.Instance.TracksUI.Add(_myAudioTrack);
                    break;
                case UnityCore.Audio.AudioType.SFX_Player:
                    AudioController.Instance.TracksPlayer.Add(_myAudioTrack);
                    break;
                case UnityCore.Audio.AudioType.SFX_World:
                    AudioController.Instance.TracksWorld.Add(_myAudioTrack);
                    break;
                default:
                    Debug.Log("You forgot to assign an AudioType to " + this.gameObject);
                    break;
            }
        }
        else
        {
            Debug.Log("You forgot to assign the AudioSource to " + this.gameObject);
        }
      
    }


}
