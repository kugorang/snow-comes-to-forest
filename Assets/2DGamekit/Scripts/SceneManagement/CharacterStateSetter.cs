#region

using System;
using System.Collections;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    /// <summary>
    ///     This class is used to put the player character into a specific state, usually upon entering a scene.
    /// </summary>
    public class CharacterStateSetter : MonoBehaviour
    {
        public Animator animator;
        public string animatorStateName;
        public Vector2 characterVelocity;
        public bool faceLeft;

        private int m_HashStateName;
        private Coroutine m_SetCharacterStateCoroutine;
        public ParameterSetter[] parameterSetters;

        public PlayerCharacter playerCharacter;

        public bool setCharacterFacing;

        public bool setCharacterVelocity;

        public bool setParameters;

        public bool setState;

        private void Awake()
        {
            m_HashStateName = Animator.StringToHash(animatorStateName);

            for (var i = 0; i < parameterSetters.Length; i++)
                parameterSetters[i].Awake();
        }

        public void SetCharacterState()
        {
            if (m_SetCharacterStateCoroutine != null)
                StopCoroutine(m_SetCharacterStateCoroutine);

            if (setCharacterVelocity)
                playerCharacter.SetMoveVector(characterVelocity);

            if (setCharacterFacing)
                playerCharacter.UpdateFacing(faceLeft);

            if (setState)
                animator.Play(m_HashStateName);

            if (setParameters)
                for (var i = 0; i < parameterSetters.Length; i++)
                    parameterSetters[i].SetParameter(animator);
        }

        public void SetCharacterState(float delay)
        {
            if (m_SetCharacterStateCoroutine != null)
                StopCoroutine(m_SetCharacterStateCoroutine);
            m_SetCharacterStateCoroutine = StartCoroutine(CallWithDelay(delay, SetCharacterState));
        }

        private IEnumerator CallWithDelay(float delay, Action call)
        {
            yield return new WaitForSeconds(delay);
            call();
        }

        [Serializable]
        public class ParameterSetter
        {
            public enum ParameterType
            {
                Bool,
                Float,
                Int,
                Trigger
            }

            public bool boolValue;
            public float floatValue;
            public int intValue;

            protected int m_Hash;

            public string parameterName;
            public ParameterType parameterType;

            public void Awake()
            {
                m_Hash = Animator.StringToHash(parameterName);
            }

            public void SetParameter(Animator animator)
            {
                switch (parameterType)
                {
                    case ParameterType.Bool:
                        animator.SetBool(m_Hash, boolValue);
                        break;
                    case ParameterType.Float:
                        animator.SetFloat(m_Hash, floatValue);
                        break;
                    case ParameterType.Int:
                        animator.SetInteger(m_Hash, intValue);
                        break;
                    case ParameterType.Trigger:
                        animator.SetTrigger(m_Hash);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}