#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class TriggerVFX : StateMachineBehaviour
    {
        public bool attachToParent;
        private int m_VfxId;
        public Vector3 offset = Vector3.zero;
        public bool OnEnter = true;
        public bool OnExit;
        public float startDelay;
        public string vfxName;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (OnEnter) Trigger(animator.transform);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (OnExit) Trigger(animator.transform);
        }

        private void Trigger(Transform transform)
        {
            var flip = false;
            var spriteRender = transform.GetComponent<SpriteRenderer>();
            if (spriteRender)
                flip = spriteRender.flipX;
            VFXController.Instance.Trigger(vfxName, offset, startDelay, flip, attachToParent ? transform : null);
        }
    }
}