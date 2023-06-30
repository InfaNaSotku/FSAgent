using System;
using FSAgent.LogicObjects;

namespace FSAgent.Agent.Component
{
    internal class Generator<TargetType> where
        TargetType : BaseTargetType
    {
        private TargetType _target;
        private List<Behavior<TargetType>> _behaviors;
        private bool IsGenerate;
        internal bool IsCancel;


        public Generator(TargetType target, List<Behavior<TargetType>> behaviors)
        {
            _target = target;
            _behaviors = behaviors;
            _estimate_deep = 0;
            IsGenerate = true;
            IsCancel = false;
            _chain = new Queue<Behavior<TargetType>>();
        }

        public void Run()
        {
            IsGenerate = false;
            if (!Execute())
            {
                _target.Log("Agent coudn't find right existing chain");
                _target.Alarm();
            }
        }
        public void Create()
        {
            IsGenerate = true;
            if (!Execute())
            {
                _target.Log("Agent coudn't find the chain");
            }
        }

        private int _estimate_deep;


        private int EstimateChain(Condition cur_condition)
        {
            int reward = 0;
            if (_estimate_deep > 10)
            {
                return 0;
            }
            foreach (var behavior in _behaviors)
            {
                // Cancel estimate
                if (IsCancel)
                {
                    return 0;
                }
                if (behavior._conditions.ContainsKey(cur_condition))
                {
                    if (_target.IsFail(behavior._conditions[cur_condition]))
                    {
                        continue;
                    }
                    if (_target.IsFinish(behavior._conditions[cur_condition]))
                        return _target.GetRewardFromCodition(cur_condition);
                    int cur_reward =
                        EstimateChain(behavior._conditions[cur_condition]);
                    if (cur_reward > reward)
                    {
                        reward = cur_reward;
                    }
                }
            }
            return reward;
        }

        // Current compound of actions
        private Queue<Behavior<TargetType>> _chain;


        private Queue<Behavior<TargetType>> CloneChain()
        {
            Queue<Behavior<TargetType>> clone =
                new Queue<Behavior<TargetType>>();
            foreach(var behaviour in _chain)
            {
                clone.Enqueue(behaviour);
            }
               return clone;
        }

        private bool IsBehaviorExist(Condition condition)
        {
            // We try to find behavior with end condition equal current condition
            foreach(var behavior in _behaviors)
            {
                foreach(var cond in behavior._conditions)
                {
                    if(cond.Value == condition)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void ExecuteAction(IEnumerable<int> action)
        {
            foreach (var movement in action)
            {
                if (IsCancel)
                {
                    return;
                }
            }
        }

        private bool Execute()
        {
            // Cancel execute
            if (IsCancel)
            {
                return true;
            }

            Condition cur_cond =
                _target.GetCurrentCondition();

            if (_target.IsFail(cur_cond))
            {
                return false;
            }

            // Saves cur chain which got "Save" predicate in cur condition
            if (IsGenerate &&
                _target.IsNeedToRemember(cur_cond) &&
                !IsBehaviorExist(cur_cond))
            {
                _target.Freeze();
                string name =
                    _target.
                    GetCompoundBehaviourName();
                _behaviors.Add(new Behavior<TargetType>(compound_action:
                    _chain, name: name));
                _target.UnFreeze();
            }

            if (_target.IsFinish(cur_cond))
            {
                return true;
            }

            // Rank list the all of behaviors
            // Item1 - points, Item2 - coresponding behavior pos
            List<Tuple<int, int>> rank = new List<Tuple<int, int>>();
            int pos = 0;
            foreach(var behavior in _behaviors)
            {

                // Cancel execute
                if (IsCancel)
                {
                    return true;
                }

                if (behavior._conditions.ContainsKey(cur_cond))
                {
                    rank.Add(new Tuple<int, int>(EstimateChain(cur_cond), pos));
                }
                else
                {
                    rank.Add(new Tuple<int, int>(0, pos));
                }

            }

            rank.Sort((Tuple<int, int> first, Tuple<int, int> second) =>
            second.Item1.CompareTo(first.Item1));

            // Checks sorted behavior list and execute
            foreach(var sorted_behavior in rank)
            {
                // Cancel execute
                if(IsCancel)
                {
                    return true;
                }
                
                // Agent doesn't know what it should to do
                if (sorted_behavior.Item1 == 0 &&
                        !IsGenerate)
                {
                    return true;
                }

                ExecuteAction(_behaviors[sorted_behavior.Item2].Run());

                // Cancel execute
                if (IsCancel)
                {
                    return true;
                }

                if (!_behaviors[sorted_behavior.Item2].
                        _conditions.ContainsKey(cur_cond))
                {
                    _behaviors[sorted_behavior.Item2].
                        _conditions.Add(cur_cond,
                        _target.GetCurrentCondition());
                }
                else
                {
                    _behaviors[sorted_behavior.Item2].
                        _conditions[cur_cond] = _target.
                        GetCurrentCondition();
                }


                if (Execute())
                {
                    return true;
                }
                else
                {
                    if (!IsGenerate)
                    {
                        return false;
                    }
                }

                // Cancel execute
                if (IsCancel)
                {
                    return true;
                }

                _target.TargetReset();
            }
            return false;
        }
    }
}