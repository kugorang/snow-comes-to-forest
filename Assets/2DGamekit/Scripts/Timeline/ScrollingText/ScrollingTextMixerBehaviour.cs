#region

using TMPro;
using UnityEngine;
using UnityEngine.Playables;

#endregion

namespace Gamekit2D
{
    public class ScrollingTextMixerBehaviour : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var trackBinding = playerData as TextMeshProUGUI;

            if (!trackBinding)
                return;

            var inputCount = playable.GetInputCount();

            for (var i = 0; i < inputCount; i++)
            {
                var inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<ScrollingTextBehaviour>) playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();

                if (Mathf.Approximately(inputWeight, 1f))
                {
                    var message = input.GetMessage((float) inputPlayable.GetTime());
                    trackBinding.text = message;
                }
            }
        }
    }
}