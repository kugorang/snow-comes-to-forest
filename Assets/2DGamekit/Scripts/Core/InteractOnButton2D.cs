#region

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Gamekit2D
{
    public class InteractOnButton2D : InteractOnTrigger2D
    {
        private bool m_CanExecuteButtons;
        public UnityEvent OnButtonPress;

        protected override void ExecuteOnEnter(Collider2D other)
        {
            m_CanExecuteButtons = true;
            OnEnter.Invoke();
        }

        protected override void ExecuteOnExit(Collider2D other)
        {
            m_CanExecuteButtons = false;
            OnExit.Invoke();
        }

        private void Update()
        {
            if (m_CanExecuteButtons)
                if (OnButtonPress.GetPersistentEventCount() > 0 && PlayerInput.Instance.Interact.Down)
                    OnButtonPress.Invoke();
        }
    }
}