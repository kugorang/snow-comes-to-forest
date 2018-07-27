#region

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Gamekit2D
{
    public class InteractOnButton : InteractOnTrigger
    {
        public string buttonName = "X";

        private bool m_CanExecuteButtons;
        public UnityEvent OnButtonPress;

        protected override void ExecuteOnEnter(Collider other)
        {
            m_CanExecuteButtons = true;
        }

        protected override void ExecuteOnExit(Collider other)
        {
            m_CanExecuteButtons = false;
        }

        private void Update()
        {
            if (m_CanExecuteButtons && Input.GetButtonDown(buttonName)) OnButtonPress.Invoke();
        }
    }
}