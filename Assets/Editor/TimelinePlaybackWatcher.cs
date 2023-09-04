using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;

[InitializeOnLoad]
public static class TimelinePlaybackWatcher
{
    static TimelinePlaybackWatcher()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    static void OnEditorUpdate()
    {
        if (!EditorApplication.isPlaying && IsAnyPlayableDirectorPlaying())
        {
            UpdateSubtitles();
        }
    }

    static bool IsAnyPlayableDirectorPlaying()
    {
        PlayableDirector[] directors = GameObject.FindObjectsOfType<PlayableDirector>();

        foreach (var director in directors)
        {
            if (director.state == PlayState.Playing)
                return true;
        }
        return false;
    }

    static void UpdateSubtitles()
    {
        // Find all Subtitle components in the scene and update them
        Subtitle[] subtitles = GameObject.FindObjectsOfType<Subtitle>();
        foreach (var subtitle in subtitles)
        {
            subtitle.CheckForSubtitleChange();
        }
    }
}