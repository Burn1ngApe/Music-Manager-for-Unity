using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _musicToPlay;
    [HideInInspector]
    private AudioSource _audioSource;

    [SerializeField] private AnimationCurve _onCurve, _offCurve;
    private float _timeSinceStartOfCoroutine = 0.0f;
    private int _playedTracks = 0;

    void Start()
    {
        SetAudioSource();

        //SET CURVES, BETTER TO DELETE IF CUSTOM CURVES ARE NEEDED TO BE SET IN THE EDITOR
        SetCurves();

        StartCoroutine("SetClip", 0); 
    }



    private void SetAudioSource()
    {
        _audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
    }



    private void SetCurves()
    {
        _onCurve.AddKey(0, 0f); _onCurve.AddKey(3, 0.6f);
        _offCurve.AddKey(0, 0.6f); _offCurve.AddKey(3, 0f);
    }



    IEnumerator SetClip(int index)
    {
        //SET AUDIO CLIP AND START PLAYING MUSIC
        _audioSource.clip = _musicToPlay[index];
        _audioSource.Play();


        //SMOOTHLY INCREASE MUSIC VOLUME
        while (_timeSinceStartOfCoroutine <= _onCurve.keys[1].time)
        {
            _timeSinceStartOfCoroutine += Time.deltaTime;
            _audioSource.volume = _onCurve.Evaluate(_timeSinceStartOfCoroutine);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        _timeSinceStartOfCoroutine = 0;


        //START SMOOTHLY DECREASING MUSIC VOLUME TO THE END OF THE CLIP
        yield return new WaitForSeconds(_musicToPlay[index].length - 9f);


        while (_timeSinceStartOfCoroutine <= _offCurve.keys[1].time)
        {
            _timeSinceStartOfCoroutine += Time.deltaTime;
            _audioSource.volume = _offCurve.Evaluate(_timeSinceStartOfCoroutine);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        _timeSinceStartOfCoroutine = 0;


        _playedTracks++;
        StartCoroutine("SetClip", _playedTracks);
    }
}
