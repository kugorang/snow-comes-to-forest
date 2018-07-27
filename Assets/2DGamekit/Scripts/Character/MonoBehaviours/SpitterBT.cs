#region

using BTAI;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class SpitterBT : MonoBehaviour
    {
        private readonly Root m_Ai = BT.Root();
        private Animator m_Animator;
        private Damageable m_Damageable;
        private EnemyBehaviour m_EnemyBehaviour;

        private void OnEnable()
        {
            m_EnemyBehaviour = GetComponent<EnemyBehaviour>();
            m_Animator = GetComponent<Animator>();

            m_Ai.OpenBranch(
                BT.If(() => { return m_EnemyBehaviour.Target != null; }).OpenBranch(
                    BT.Call(m_EnemyBehaviour.CheckTargetStillVisible),
                    BT.Call(m_EnemyBehaviour.OrientToTarget),
                    BT.Trigger(m_Animator, "Shooting"),
                    BT.Call(m_EnemyBehaviour.RememberTargetPos),
                    BT.WaitForAnimatorState(m_Animator, "Attack")
                ),
                BT.If(() => { return m_EnemyBehaviour.Target == null; }).OpenBranch(
                    BT.Call(m_EnemyBehaviour.ScanForPlayer)
                )
            );
        }

        private void Update()
        {
            m_Ai.Tick();
        }
    }
}