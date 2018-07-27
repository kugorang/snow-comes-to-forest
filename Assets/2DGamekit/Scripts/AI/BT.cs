﻿#region

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

namespace BTAI
{
    public enum BTState
    {
        Failure,
        Success,
        Continue,
        Abort
    }

    public static class BT
    {
        public static Root Root()
        {
            return new Root();
        }

        public static Sequence Sequence()
        {
            return new Sequence();
        }

        public static Selector Selector(bool shuffle = false)
        {
            return new Selector(shuffle);
        }

        public static Action RunCoroutine(Func<IEnumerator<BTState>> coroutine)
        {
            return new Action(coroutine);
        }

        public static Action Call(System.Action fn)
        {
            return new Action(fn);
        }

        public static ConditionalBranch If(Func<bool> fn)
        {
            return new ConditionalBranch(fn);
        }

        public static While While(Func<bool> fn)
        {
            return new While(fn);
        }

        public static Condition Condition(Func<bool> fn)
        {
            return new Condition(fn);
        }

        public static Repeat Repeat(int count)
        {
            return new Repeat(count);
        }

        public static Wait Wait(float seconds)
        {
            return new Wait(seconds);
        }

        public static Trigger Trigger(Animator animator, string name, bool set = true)
        {
            return new Trigger(animator, name, set);
        }

        public static WaitForAnimatorState WaitForAnimatorState(Animator animator, string name, int layer = 0)
        {
            return new WaitForAnimatorState(animator, name, layer);
        }

        public static SetBool SetBool(Animator animator, string name, bool value)
        {
            return new SetBool(animator, name, value);
        }

        public static SetActive SetActive(GameObject gameObject, bool active)
        {
            return new SetActive(gameObject, active);
        }

        public static WaitForAnimatorSignal WaitForAnimatorSignal(Animator animator, string name, string state,
            int layer = 0)
        {
            return new WaitForAnimatorSignal(animator, name, state, layer);
        }

        public static Terminate Terminate()
        {
            return new Terminate();
        }

        public static Log Log(string msg)
        {
            return new Log(msg);
        }

        public static RandomSequence RandomSequence(int[] weights = null)
        {
            return new RandomSequence(weights);
        }
    }

    public abstract class BTNode
    {
        public abstract BTState Tick();
    }

    public abstract class Branch : BTNode
    {
        protected int activeChild;
        protected List<BTNode> children = new List<BTNode>();

        public virtual Branch OpenBranch(params BTNode[] children)
        {
            for (var i = 0; i < children.Length; i++)
                this.children.Add(children[i]);
            return this;
        }

        public List<BTNode> Children()
        {
            return children;
        }

        public int ActiveChild()
        {
            return activeChild;
        }

        public virtual void ResetChildren()
        {
            activeChild = 0;
            for (var i = 0; i < children.Count; i++)
            {
                var b = children[i] as Branch;
                if (b != null) b.ResetChildren();
            }
        }
    }

    public abstract class Decorator : BTNode
    {
        protected BTNode child;

        public Decorator Do(BTNode child)
        {
            this.child = child;
            return this;
        }
    }

    public class Sequence : Branch
    {
        public override BTState Tick()
        {
            var childState = children[activeChild].Tick();
            switch (childState)
            {
                case BTState.Success:
                    activeChild++;
                    if (activeChild == children.Count)
                    {
                        activeChild = 0;
                        return BTState.Success;
                    }
                    else
                    {
                        return BTState.Continue;
                    }
                case BTState.Failure:
                    activeChild = 0;
                    return BTState.Failure;
                case BTState.Continue:
                    return BTState.Continue;
                case BTState.Abort:
                    activeChild = 0;
                    return BTState.Abort;
            }

            throw new Exception("This should never happen, but clearly it has.");
        }
    }

    /// <summary>
    ///     Execute each child until a child succeeds, then return success.
    ///     If no child succeeds, return a failure.
    /// </summary>
    public class Selector : Branch
    {
        public Selector(bool shuffle)
        {
            if (shuffle)
            {
                var n = children.Count;
                while (n > 1)
                {
                    n--;
                    var k = Mathf.FloorToInt(Random.value * (n + 1));
                    var value = children[k];
                    children[k] = children[n];
                    children[n] = value;
                }
            }
        }

        public override BTState Tick()
        {
            var childState = children[activeChild].Tick();
            switch (childState)
            {
                case BTState.Success:
                    activeChild = 0;
                    return BTState.Success;
                case BTState.Failure:
                    activeChild++;
                    if (activeChild == children.Count)
                    {
                        activeChild = 0;
                        return BTState.Failure;
                    }
                    else
                    {
                        return BTState.Continue;
                    }
                case BTState.Continue:
                    return BTState.Continue;
                case BTState.Abort:
                    activeChild = 0;
                    return BTState.Abort;
            }

            throw new Exception("This should never happen, but clearly it has.");
        }
    }

    /// <summary>
    ///     Call a method, or run a coroutine.
    /// </summary>
    public class Action : BTNode
    {
        private readonly Func<IEnumerator<BTState>> coroutineFactory;
        private readonly System.Action fn;
        private IEnumerator<BTState> coroutine;

        public Action(System.Action fn)
        {
            this.fn = fn;
        }

        public Action(Func<IEnumerator<BTState>> coroutineFactory)
        {
            this.coroutineFactory = coroutineFactory;
        }

        public override BTState Tick()
        {
            if (fn != null)
            {
                fn();
                return BTState.Success;
            }

            if (coroutine == null)
                coroutine = coroutineFactory();
            if (!coroutine.MoveNext())
            {
                coroutine = null;
                return BTState.Success;
            }

            var result = coroutine.Current;
            if (result == BTState.Continue) return BTState.Continue;

            coroutine = null;
            return result;
        }

        public override string ToString()
        {
            return "Action : " + fn.Method;
        }
    }

    /// <summary>
    ///     Call a method, returns success if method returns true, else returns failure.
    /// </summary>
    public class Condition : BTNode
    {
        public Func<bool> fn;

        public Condition(Func<bool> fn)
        {
            this.fn = fn;
        }

        public override BTState Tick()
        {
            return fn() ? BTState.Success : BTState.Failure;
        }

        public override string ToString()
        {
            return "Condition : " + fn.Method;
        }
    }

    public class ConditionalBranch : Block
    {
        public Func<bool> fn;
        private bool tested;

        public ConditionalBranch(Func<bool> fn)
        {
            this.fn = fn;
        }

        public override BTState Tick()
        {
            if (!tested) tested = fn();
            if (tested)
            {
                var result = base.Tick();
                if (result == BTState.Continue) return BTState.Continue;

                tested = false;
                return result;
            }

            return BTState.Failure;
        }

        public override string ToString()
        {
            return "ConditionalBranch : " + fn.Method;
        }
    }

    /// <summary>
    ///     Run all children, while method returns true.
    /// </summary>
    public class While : Block
    {
        public Func<bool> fn;

        public While(Func<bool> fn)
        {
            this.fn = fn;
        }

        public override BTState Tick()
        {
            if (fn())
            {
                base.Tick();
            }
            else
            {
                //if we exit the loop
                ResetChildren();
                return BTState.Failure;
            }

            return BTState.Continue;
        }

        public override string ToString()
        {
            return "While : " + fn.Method;
        }
    }

    public abstract class Block : Branch
    {
        public override BTState Tick()
        {
            switch (children[activeChild].Tick())
            {
                case BTState.Continue:
                    return BTState.Continue;
                default:
                    activeChild++;
                    if (activeChild == children.Count)
                    {
                        activeChild = 0;
                        return BTState.Success;
                    }

                    return BTState.Continue;
            }
        }
    }

    public class Root : Block
    {
        public bool isTerminated;

        public override BTState Tick()
        {
            if (isTerminated) return BTState.Abort;
            while (true)
                switch (children[activeChild].Tick())
                {
                    case BTState.Continue:
                        return BTState.Continue;
                    case BTState.Abort:
                        isTerminated = true;
                        return BTState.Abort;
                    default:
                        activeChild++;
                        if (activeChild == children.Count)
                        {
                            activeChild = 0;
                            return BTState.Success;
                        }

                        continue;
                }
        }
    }

    /// <summary>
    ///     Run a block of children a number of times.
    /// </summary>
    public class Repeat : Block
    {
        public int count = 1;
        private int currentCount;

        public Repeat(int count)
        {
            this.count = count;
        }

        public override BTState Tick()
        {
            if (count > 0 && currentCount < count)
            {
                var result = base.Tick();
                switch (result)
                {
                    case BTState.Continue:
                        return BTState.Continue;
                    default:
                        currentCount++;
                        if (currentCount == count)
                        {
                            currentCount = 0;
                            return BTState.Success;
                        }

                        return BTState.Continue;
                }
            }

            return BTState.Success;
        }

        public override string ToString()
        {
            return "Repeat Until : " + currentCount + " / " + count;
        }
    }

    public class RandomSequence : Block
    {
        private readonly int[] m_Weight;
        private int[] m_AddedWeight;

        /// <summary>
        ///     Will select one random child everytime it get triggered again
        /// </summary>
        /// <param name="weight">
        ///     Leave null so that all child node have the same weight.
        ///     If there is less weight than children, all subsequent child will have weight = 1
        /// </param>
        public RandomSequence(int[] weight = null)
        {
            activeChild = -1;

            m_Weight = weight;
        }

        public override Branch OpenBranch(params BTNode[] children)
        {
            m_AddedWeight = new int[children.Length];

            for (var i = 0; i < children.Length; ++i)
            {
                var weight = 0;
                var previousWeight = 0;

                if (m_Weight == null || m_Weight.Length <= i)
                    weight = 1;
                else
                    weight = m_Weight[i];

                if (i > 0)
                    previousWeight = m_AddedWeight[i - 1];

                m_AddedWeight[i] = weight + previousWeight;
            }

            return base.OpenBranch(children);
        }

        public override BTState Tick()
        {
            if (activeChild == -1)
                PickNewChild();

            var result = children[activeChild].Tick();

            switch (result)
            {
                case BTState.Continue:
                    return BTState.Continue;
                default:
                    PickNewChild();
                    return result;
            }
        }

        private void PickNewChild()
        {
            var choice = Random.Range(0, m_AddedWeight[m_AddedWeight.Length - 1]);

            for (var i = 0; i < m_AddedWeight.Length; ++i)
                if (choice - m_AddedWeight[i] <= 0)
                {
                    activeChild = i;
                    break;
                }
        }

        public override string ToString()
        {
            return "Random Sequence : " + activeChild + "/" + children.Count;
        }
    }


    /// <summary>
    ///     Pause execution for a number of seconds.
    /// </summary>
    public class Wait : BTNode
    {
        private float future = -1;
        public float seconds;

        public Wait(float seconds)
        {
            this.seconds = seconds;
        }

        public override BTState Tick()
        {
            if (future < 0)
                future = Time.time + seconds;

            if (Time.time >= future)
            {
                future = -1;
                return BTState.Success;
            }

            return BTState.Continue;
        }

        public override string ToString()
        {
            return "Wait : " + (future - Time.time) + " / " + seconds;
        }
    }

    /// <summary>
    ///     Activate a trigger on an animator.
    /// </summary>
    public class Trigger : BTNode
    {
        private readonly Animator animator;
        private readonly int id;
        private readonly bool set = true;
        private readonly string triggerName;

        //if set == false, it reset the trigger istead of setting it.
        public Trigger(Animator animator, string name, bool set = true)
        {
            id = Animator.StringToHash(name);
            this.animator = animator;
            triggerName = name;
            this.set = set;
        }

        public override BTState Tick()
        {
            if (set)
                animator.SetTrigger(id);
            else
                animator.ResetTrigger(id);

            return BTState.Success;
        }

        public override string ToString()
        {
            return "Trigger : " + triggerName;
        }
    }

    /// <summary>
    ///     Set a boolean on an animator.
    /// </summary>
    public class SetBool : BTNode
    {
        private readonly Animator animator;
        private readonly int id;
        private readonly string triggerName;
        private readonly bool value;

        public SetBool(Animator animator, string name, bool value)
        {
            id = Animator.StringToHash(name);
            this.animator = animator;
            this.value = value;
            triggerName = name;
        }

        public override BTState Tick()
        {
            animator.SetBool(id, value);
            return BTState.Success;
        }

        public override string ToString()
        {
            return "SetBool : " + triggerName + " = " + value;
        }
    }

    /// <summary>
    ///     Wait for an animator to reach a state.
    /// </summary>
    public class WaitForAnimatorState : BTNode
    {
        private readonly Animator animator;
        private readonly int id;
        private readonly int layer;
        private readonly string stateName;

        public WaitForAnimatorState(Animator animator, string name, int layer = 0)
        {
            id = Animator.StringToHash(name);
            if (!animator.HasState(layer, id)) Debug.LogError("The animator does not have state: " + name);
            this.animator = animator;
            this.layer = layer;
            stateName = name;
        }

        public override BTState Tick()
        {
            var state = animator.GetCurrentAnimatorStateInfo(layer);
            if (state.fullPathHash == id || state.shortNameHash == id)
                return BTState.Success;
            return BTState.Continue;
        }

        public override string ToString()
        {
            return "Wait For State : " + stateName;
        }
    }

    /// <summary>
    ///     Set a gameobject active flag.
    /// </summary>
    public class SetActive : BTNode
    {
        private readonly bool active;

        private readonly GameObject gameObject;

        public SetActive(GameObject gameObject, bool active)
        {
            this.gameObject = gameObject;
            this.active = active;
        }

        public override BTState Tick()
        {
            gameObject.SetActive(active);
            return BTState.Success;
        }

        public override string ToString()
        {
            return "Set Active : " + gameObject.name + " = " + active;
        }
    }

    /// <summary>
    ///     Wait for a signal to be received from a SendSignal state machine behaviour on an animator.
    /// </summary>
    public class WaitForAnimatorSignal : BTNode
    {
        private readonly int id;
        private readonly string name;
        internal bool isSet;

        public WaitForAnimatorSignal(Animator animator, string name, string state, int layer = 0)
        {
            this.name = name;
            id = Animator.StringToHash(name);
            if (!animator.HasState(layer, id))
                Debug.LogError("The animator does not have state: " + name);
            else
                SendSignal.Register(animator, name, this);
        }

        public override BTState Tick()
        {
            if (!isSet) return BTState.Continue;

            isSet = false;
            return BTState.Success;
        }

        public override string ToString()
        {
            return "Wait For Animator Signal : " + name;
        }
    }

    public class Terminate : BTNode
    {
        public override BTState Tick()
        {
            return BTState.Abort;
        }
    }

    public class Log : BTNode
    {
        private readonly string msg;

        public Log(string msg)
        {
            this.msg = msg;
        }

        public override BTState Tick()
        {
            Debug.Log(msg);
            return BTState.Success;
        }
    }
}

#if UNITY_EDITOR
namespace BTAI
{
    public interface IBTDebugable
    {
        Root GetAIRoot();
    }
}
#endif