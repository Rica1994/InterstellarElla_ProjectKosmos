using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SetTimelineReferences : MonoBehaviour
{

    void Start()
    { 
        // Set AudioSource reference from ServiceLocator to track in Timeline: MaggieTrack
        PlayableDirector playableDirector = GetComponent<PlayableDirector>();
        TimelineAsset timeline = playableDirector.playableAsset as TimelineAsset;
        if (timeline == null) return;
        TrackAsset maggieTrack = timeline.GetOutputTracks().FirstOrDefault(t => t.name == "MaggieTrack");
        if (maggieTrack != null)
        {
            playableDirector.SetGenericBinding(maggieTrack, ServiceLocator.Instance.GetService<AudioController>().TracksMaggie[0].Source as AudioSource);
        }
    }
}