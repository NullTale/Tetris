using UnityEngine;
using UnityEngine.Playables;


namespace Core
{
    [System.Serializable]
    public class AudioMixerPlayableAsset : PlayableAsset
    {
        public AudioMixerPlayableBehaviour   audioMixerParameter;

        //////////////////////////////////////////////////////////////////////////
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<AudioMixerPlayableBehaviour>.Create(graph, audioMixerParameter);
            return playable;
        }
    }
}


// [TrackColor(1.0f, 0.85f, 0.1f)]