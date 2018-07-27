#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class MeleeAttackSMB : SceneLinkedSMB<PlayerCharacter>
    {
        private readonly int m_HashAirborneMeleeAttackState = Animator.StringToHash("AirborneMeleeAttack");

        public override void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.ForceNotHoldingGun();
            m_MonoBehaviour.EnableMeleeAttack();
            m_MonoBehaviour.SetHorizontalMovement(m_MonoBehaviour.meleeAttackDashSpeed * m_MonoBehaviour.GetFacing());
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!m_MonoBehaviour.CheckForGrounded())
                animator.Play(m_HashAirborneMeleeAttackState, layerIndex, stateInfo.normalizedTime);

            m_MonoBehaviour.GroundedHorizontalMovement(false);
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.DisableMeleeAttack();
        }
    }
}