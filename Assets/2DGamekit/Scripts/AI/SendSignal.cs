#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace BTAI
{
    public class SendSignal : StateMachineBehaviour
    {
        private readonly List<WaitForAnimatorSignal> listeners = new List<WaitForAnimatorSignal>();
        public bool fired;

        public string signal = "";

        [Range(0, 1)] public float time;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            fired = false;
            SetFalse();
        }

        private void SetFalse()
        {
            foreach (var n in listeners)
                n.isSet = false;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetFalse();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!fired && stateInfo.normalizedTime >= time)
            {
                foreach (var n in listeners)
                    n.isSet = true;
                fired = true;
            }
        }

        public static void Register(Animator animator, string name, WaitForAnimatorSignal node)
        {
            var found = false;
            foreach (var ss in animator.GetBehaviours<SendSignal>())
                if (ss.signal == name)
                {
                    found = true;
                    ss.listeners.Add(node);
                }

            if (!found) Debug.LogError("Signal does not exist in animator: " + name);
        }
    }
}