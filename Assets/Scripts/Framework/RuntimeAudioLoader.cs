using System.Collections;
using UnityEngine;
using UnityEngine.Networking; // Include the Unity Networking namespace

public class RuntimeAudioLoader : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource; // Assign this via the inspector or find it via code

    [SerializeField]
    string _audioUrl;


    public void LoadAudioFromURL()
    {
        StartCoroutine(LoadAudio());
    }

    // Start is called before the first frame update
    private IEnumerator LoadAudio()
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(_audioUrl, AudioType.OGGVORBIS))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // Get the downloaded audio clip
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                // Set the clip to the audio source and then play it
                _audioSource.clip = clip;
                _audioSource.Play();
            }
        }
    }
}
