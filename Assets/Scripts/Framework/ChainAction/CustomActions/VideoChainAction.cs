using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoChainAction : ChainAction
{
    [SerializeField]
    private VideoPlayer _videoPlayer;
    [SerializeField]
    private RawImage _rawImage;
    protected override void Awake()
    {
        base.Awake();

        _maxTime = (float)_videoPlayer.length;

        if (_rawImage != null)
        {
            _rawImage.color = Color.black;
        }

    }

    public override void Execute()
    {
        base.Execute();
        _videoPlayer.Play();
        if (_rawImage != null)
        {
            StartCoroutine(SetRawTextureColorToWhite());
        }
    }

    private IEnumerator SetRawTextureColorToWhite()
    {
        yield return new WaitForSeconds(0.5f);
        _rawImage.color = Color.white;
    }
}
