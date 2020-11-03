using UnityEngine.Audio;
using UnityEngine.Playables;


namespace Core
{
    public class AudioMixerPlayableMixerBehaviour : PlayableBehaviour
    {
        public string           Parameter { get; set; }

        //////////////////////////////////////////////////////////////////////////
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // try to get mixer
            var audioMixer = playerData as AudioMixer ?? SoundManager.Mixer;
            if (audioMixer == null)
                return;

            var finalValue = 0.0f;

            // calculate weights
            var inputCount = playable.GetInputCount();
            for (var i = 0; i < inputCount; i++)
            {
                //if (playable.IsPlayableOfType<ScriptPlayable<MixerPlayableBehaviour>>() == false)
                //    continue;

                // get clips data
                var inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<AudioMixerPlayableBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();
                
                // add weighted impact of the clip to final value
                finalValue += input.Value * inputWeight;
            }

            // assign result
            audioMixer.SetFloat(Parameter, finalValue);
        }
    }
}