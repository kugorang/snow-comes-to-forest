#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class PlayerInput : InputComponent, IDataPersister
    {
        protected static PlayerInput s_Instance;

        [HideInInspector] public DataSettings dataSettings;

        public InputAxis Horizontal = new InputAxis(KeyCode.D, KeyCode.A, XboxControllerAxes.LeftstickHorizontal);
        public InputButton Interact = new InputButton(KeyCode.E, XboxControllerButtons.Y);
        public InputButton Jump = new InputButton(KeyCode.Space, XboxControllerButtons.A);

        protected bool m_DebugMenuIsOpen;

        protected bool m_HaveControl = true;
        public InputButton MeleeAttack = new InputButton(KeyCode.K, XboxControllerButtons.X);

        public InputButton Pause = new InputButton(KeyCode.Escape, XboxControllerButtons.Menu);
        public InputButton RangedAttack = new InputButton(KeyCode.O, XboxControllerButtons.B);
        public InputAxis Vertical = new InputAxis(KeyCode.W, KeyCode.S, XboxControllerAxes.LeftstickVertical);

        public static PlayerInput Instance
        {
            get { return s_Instance; }
        }


        public bool HaveControl
        {
            get { return m_HaveControl; }
        }

        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public Data SaveData()
        {
            return new Data<bool, bool>(MeleeAttack.Enabled, RangedAttack.Enabled);
        }

        public void LoadData(Data data)
        {
            var playerInputData = (Data<bool, bool>) data;

            if (playerInputData.value0)
                MeleeAttack.Enable();
            else
                MeleeAttack.Disable();

            if (playerInputData.value1)
                RangedAttack.Enable();
            else
                RangedAttack.Disable();
        }

        private void Awake()
        {
            if (s_Instance == null)
                s_Instance = this;
            else
                throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " +
                                         s_Instance.name + " and " + name + ".");
        }

        private void OnEnable()
        {
            if (s_Instance == null)
                s_Instance = this;
            else if (s_Instance != this)
                throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " +
                                         s_Instance.name + " and " + name + ".");

            PersistentDataManager.RegisterPersister(this);
        }

        private void OnDisable()
        {
            PersistentDataManager.UnregisterPersister(this);

            s_Instance = null;
        }

        protected override void GetInputs(bool fixedUpdateHappened)
        {
            Pause.Get(fixedUpdateHappened, inputType);
            Interact.Get(fixedUpdateHappened, inputType);
            MeleeAttack.Get(fixedUpdateHappened, inputType);
            RangedAttack.Get(fixedUpdateHappened, inputType);
            Jump.Get(fixedUpdateHappened, inputType);
            Horizontal.Get(inputType);
            Vertical.Get(inputType);

            if (Input.GetKeyDown(KeyCode.F12)) m_DebugMenuIsOpen = !m_DebugMenuIsOpen;
        }

        public override void GainControl()
        {
            m_HaveControl = true;

            GainControl(Pause);
            GainControl(Interact);
            GainControl(MeleeAttack);
            GainControl(RangedAttack);
            GainControl(Jump);
            GainControl(Horizontal);
            GainControl(Vertical);
        }

        public override void ReleaseControl(bool resetValues = true)
        {
            m_HaveControl = false;

            ReleaseControl(Pause, resetValues);
            ReleaseControl(Interact, resetValues);
            ReleaseControl(MeleeAttack, resetValues);
            ReleaseControl(RangedAttack, resetValues);
            ReleaseControl(Jump, resetValues);
            ReleaseControl(Horizontal, resetValues);
            ReleaseControl(Vertical, resetValues);
        }

        public void DisableMeleeAttacking()
        {
            MeleeAttack.Disable();
        }

        public void EnableMeleeAttacking()
        {
            MeleeAttack.Enable();
        }

        public void DisableRangedAttacking()
        {
            RangedAttack.Disable();
        }

        public void EnableRangedAttacking()
        {
            RangedAttack.Enable();
        }

        private void OnGUI()
        {
            if (m_DebugMenuIsOpen)
            {
                const float height = 100;

                GUILayout.BeginArea(new Rect(30, Screen.height - height, 200, height));

                GUILayout.BeginVertical("box");
                GUILayout.Label("Press F12 to close");

                var meleeAttackEnabled = GUILayout.Toggle(MeleeAttack.Enabled, "Enable Melee Attack");
                var rangeAttackEnabled = GUILayout.Toggle(RangedAttack.Enabled, "Enable Range Attack");

                if (meleeAttackEnabled != MeleeAttack.Enabled)
                {
                    if (meleeAttackEnabled)
                        MeleeAttack.Enable();
                    else
                        MeleeAttack.Disable();
                }

                if (rangeAttackEnabled != RangedAttack.Enabled)
                {
                    if (rangeAttackEnabled)
                        RangedAttack.Enable();
                    else
                        RangedAttack.Disable();
                }

                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }
    }
}