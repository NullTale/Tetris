using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace Core
{
    [TrackColor(1.0f, 0.85f, 0.1f)]
    [TrackClipType(typeof(AudioMixerPlayableAsset))]
    [TrackBindingType(typeof(AudioMixer))]
    public class AudioMixerTrackAsset : TrackAsset
    {
        [AudioMixerKey]
        public string                   m_Parameter;

        //////////////////////////////////////////////////////////////////////////
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixerTrack = ScriptPlayable<AudioMixerPlayableMixerBehaviour>.Create(graph, inputCount);
            mixerTrack.GetBehaviour().Parameter = m_Parameter;
            return mixerTrack;
        }
    }
}