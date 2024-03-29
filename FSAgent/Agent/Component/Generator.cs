﻿using System;
using System.IO;
using System.Collections.Generic;
using FSAgent.LogicObjects;
using System.Runtime.ExceptionServices;
using System.Linq;
using System.ComponentModel;

namespace FSAgent.Agent.Component
{
    internal class Generator<TargetType> where
        TargetType : BaseTargetType, new()
    {
        private TargetType _target;
        private List<Behavior<TargetType>> _behaviors;
        private List<Behavior<TargetType>> _leveled_behaviors;
        private bool IsGenerate;
        internal bool IsCancel;

        public Generator()
        {
            _target = new TargetType();
            _behaviors = new List<Behavior<TargetType>>();
            // More to less level
            _leveled_behaviors = new List<Behavior<TargetType>>();
            _estimate_deep = 0;
            _execute_deep = 0;
            IsGenerate = true;
            IsCancel = false;
            _chain = new Stack<int>();
            _start_condition = new Condition(0);
        }

        public Generator(TargetType target, List<Behavior<TargetType>> behaviors)
        {
            _target = target;
            _behaviors = behaviors;
            _leveled_behaviors = new List<Behavior<TargetType>>();
            foreach (var behavior in _behaviors)
            {
                _leveled_behaviors.Add(behavior);
            }
            _leveled_behaviors.Sort((Behavior<TargetType> first, Behavior<TargetType> second) =>
            second.Level.CompareTo(first.Level));
            _estimate_deep = 0;
            _execute_deep = 0;
            IsGenerate = true;
            IsCancel = false;
            _chain = new Stack<int>();
            _start_condition = new Condition(0);
        }

        public void Run()
        {
            if (_behaviors.Count == 0)
            {
                throw new InvalidOperationException();
            }
            if (FindBehaviorFromName("NullAction") == -1)
            {
                //throw new InvalidOperationException();
            }
            IsGenerate = false;
            if (!Execute())
            {
                _target.Log("Agent coudn't find right existing chain");
                _target.Alarm();
            }
            else
            {
                _target.Log("Agent successfuly found right chain");
            }
        }
        public void Create()
        {
            if (_behaviors.Count == 0)
            {
                throw new InvalidOperationException();
            }
            if (FindBehaviorFromName("NullAction") == -1)
            {
                //throw new InvalidOperationException();
            }
            IsGenerate = true;
            if (!Execute())
            {
                _target.Log("Agent coudn't find the chain");
            }
            else
            {
                _target.Log("Agent successfuly found right chain");
            }
        }

        private int _estimate_deep;
        private int _execute_deep;

        private int EstimateChain(Condition cur_condition)
        {
            int reward = 0;
            if (_estimate_deep > 5)
            {
                return 0;
            }
            foreach (var l_behavior in _leveled_behaviors)
            {
                // Cancel estimate
                if (IsCancel)
                {
                    return 0;
                }
                if (l_behavior._conditions.ContainsKey(cur_condition))
                {
                    if (_target.IsFail(l_behavior._conditions[cur_condition]))
                    {
                        continue;
                    }
                    if (_target.IsFinish(l_behavior._conditions[cur_condition]))
                        return _target.GetRewardFromCondition(l_behavior._conditions[cur_condition]);
                    _estimate_deep++;
                    int cur_reward =
                        EstimateChain(l_behavior._conditions[cur_condition]);
                    _estimate_deep--;
                    if (cur_reward > reward)
                    {
                        reward = cur_reward;
                    }
                }
            }
            return reward;
        }

        // Current compound of actions
        private Stack<int> _chain;

        private Condition _start_condition;

        private Queue<Behavior<TargetType>> CloneChain()
        {
            Queue<Behavior<TargetType>> clone =
                new Queue<Behavior<TargetType>>();
            foreach(var pos in _chain.Reverse())
            {
                clone.Enqueue(_leveled_behaviors[pos]);
            }
               return clone;
        }

        private int FindCompoundBehaviorFromCond(Condition start_cond, Condition end_cond)
        {
            int pos = 0;
            // We try to find behavior with end condition equal current condition
            foreach(var behavior in _behaviors)
            {
                /*
                 * We need to find only compound action
                 * Thus we have only one compound action 
                 * with this end condition
                 */
                if (behavior.Size == 1)
                {
                    continue;
                }
                foreach(var cond in behavior._conditions)
                {
                    if(cond.Value == end_cond && cond.Key == start_cond)
                    {
                        return pos;
                    }
                }
                pos++;
            }
            return -1;
        }

        internal int FindBehaviorFromName(string name)
        {
            int pos = 0;
            foreach (var behavior in _behaviors)
            {
                if (behavior._name == name)
                {
                    return pos;
                }
                pos++;
            }
            return -1;
        }

        private void ExecuteAction(IEnumerable<int> action)
        {
            if (IsGenerate)
            {
                _target.TargetSave();
            }
            foreach (var movement in action)
            {

                if (IsCancel)
                {
                    return;
                }
            }
        }

        private bool Compare(Behavior<TargetType> first,
            Behavior<TargetType> second) =>
            first.Size < second.Size;


        /*
         * 0 - return true
         * 1 - return false
         * 2 - continue execute
         */
        private int CheckCondition(Condition condition)
        {


            if (_target.IsFail(condition))
            {
                return 1;
            }

            // Saves cur chain which got "Save" predicate in cur condition
            if (IsGenerate &&
                _target.IsNeedToRemember(condition))
            {
                int find_pos = FindCompoundBehaviorFromCond(_start_condition, condition);
                Behavior<TargetType> new_behavior =
                    new Behavior<TargetType>(compound_action:
                    CloneChain());
                /*
                 * We need to have comnpund action 
                 * with lowest count of default action
                 */
                if (
                    (find_pos != -1 &&
                    Compare(new_behavior, _behaviors[find_pos]))
                    ||
                    find_pos == -1
                    )
                {
                    _target.Freeze();
                    string name =
                        _target.
                        GetCompoundBehaviourName();
                    int number = 1;
                    while (FindBehaviorFromName(name) != -1)
                    {
                        string temp = name.Replace("_" + (number - 1).ToString(), "_" + number.ToString());
                        if (temp == name)
                        {
                            name += ("_" + number.ToString());
                        }
                    }
                    new_behavior._name = name;
                    new_behavior._conditions.Add(_start_condition, condition);
                    _behaviors.Add(new_behavior);
                    for (int i = 0; i < _leveled_behaviors.Count; i++)
                    {
                        /*
                         * 10 6 5 3 1
                         *     | 
                         *     5
                         */
                        if (_leveled_behaviors[i].Level <= new_behavior.Level)
                        {
                            _leveled_behaviors.Insert(i, new_behavior);
                            break;
                        }
                    }
                    _target.UnFreeze();
                }
            }

            if (_target.IsFinish(condition))
            {
                return 0;
            }
            return 2;
        }

        private List<Tuple<int, int>> GetBehaviorRank(Condition condition)
        {
            List<Tuple<int, int>> rank = new List<Tuple<int, int>>();
            int pos = 0;
            foreach (var l_behavior in _leveled_behaviors)
            {

                // Cancel execute
                if (IsCancel)
                {
                    return rank;
                }

                if (l_behavior._conditions.ContainsKey(condition) && !_target.IsFail(l_behavior._conditions[condition]))
                {
                    int reward = _target.GetRewardFromCondition(l_behavior._conditions[condition]);
                    reward += EstimateChain(l_behavior._conditions[condition]);
                    if (reward > 1e7)
                    {
                        reward = (int)1e7;
                    }
                    rank.Add(new Tuple<int, int>(reward, pos));
                }
                else
                {
                    rank.Add(new Tuple<int, int>(0, pos));
                }
                pos++;
            }

            rank.Sort((Tuple<int, int> first, Tuple<int, int> second) =>
            {
                if (first.Item1 > second.Item1)
                {
                    return -1;
                }
                if (second.Item1 > first.Item1)
                {
                    return 1;
                }
                if (_leveled_behaviors[first.Item2].Level > 
                _leveled_behaviors[second.Item2].Level)
                {
                    return -1;
                }
                if (_leveled_behaviors[second.Item2].Level >
                _leveled_behaviors[first.Item2].Level)
                {
                    return 1;
                }
                return 0;
            });
            return rank;
        }

        // [left, right)
        private List<int> GetRandomIndexList(int left, int right)
        {
            List<int> random_list = new List<int>();
            for (int i = left; i < right; i++)
            {
                random_list.Add(i);
            }
            Random rand = new Random();
            // Lfet - right swaps
            for (int i = left; i < right; i++)
            {
                int first = rand.Next(left, right - 1);
                int second = rand.Next(left, right - 1);
                int temp = random_list[first];
                random_list[first] = random_list[second];
                random_list[second] = temp;
            }
            return random_list;
    }

        /*
         * 0 - return true
         * 1 - return false
         * 2 - continue execute
         */
        private int CheckSortedBehavior(Tuple<int, int> sorted_behavior,
            Condition condition)
        {
            // Cancel execute
            if (IsCancel)
            {
                return 0;
            }
            // Agent doesn't know what it should to do
            if (sorted_behavior.Item1 == 0 &&
                    !IsGenerate)
            {
                return 0;
            }

            ExecuteAction(_leveled_behaviors[sorted_behavior.Item2].Run());

            // Cancel execute
            if (IsCancel)
            {
                return 0;
            }

            if (!_leveled_behaviors[sorted_behavior.Item2].
                    _conditions.ContainsKey(condition))
            {
                _leveled_behaviors[sorted_behavior.Item2].
                    _conditions.Add(condition,
                    _target.GetCurrentCondition());
            }
            else
            {
                _leveled_behaviors[sorted_behavior.Item2].
                    _conditions[condition] = _target.
                    GetCurrentCondition();
            }

            _chain.Push(sorted_behavior.Item2);

            if (Execute())
            {
                _chain.Pop();
                return 0;
            }
            else
            {
                if (!IsGenerate)
                {
                    _chain.Pop();
                    return 1;
                }
            }



            // Cancel execute
            if (IsCancel)
            {
                return 0;
            }
            _target.TargetReset();
            _chain.Pop();

            return 2;
        }

        private bool CheckSortedBehaviors(List<Tuple<int, int>> rank,
            Condition condition)
        {
            int pos = 0;
            foreach (var sorted_behavior in rank)
            {
                if (sorted_behavior.Item1 == 0)
                {
                    break;
                }
                    int state = CheckSortedBehavior(sorted_behavior, condition);
                switch (state)
                {
                    case 0:
                        return true;
                    case 1:
                        return false;
                }
                pos++;

            }

            // [left, right)
            List<int> random_positions = GetRandomIndexList(pos, rank.Count);
            // Levels sort
            random_positions.Sort((int first, int second) =>
            {

                return _leveled_behaviors[rank[second].Item2].Level.
                    CompareTo(_leveled_behaviors[rank[first].Item2].Level);
            });
            
            foreach (var rand_pos in random_positions)
            {
                int state = CheckSortedBehavior(rank[rand_pos], condition);
                switch (state)
                {
                    case 0:
                        return true;
                    case 1:
                        return false;
                }
                pos++;
            }

            return false;
        }



        private bool Execute()
        {
            _execute_deep++;

            // Cancel execute
            if (IsCancel)
            {
                _execute_deep--;
                return true;
            }

            Condition cur_cond =
                _target.GetCurrentCondition();
            if (_execute_deep == 1)
            {
                _start_condition = cur_cond;
            }
            int state = CheckCondition(cur_cond);
            switch (state)
            {
                case 0:
                    _execute_deep--;
                    return true;
                case 1:
                    _execute_deep--;
                    return false;
            }
            // Sorted rank list the all of behaviors
            // Item1 - points, Item2 - coresponding behavior pos
            List<Tuple<int, int>> rank = GetBehaviorRank(cur_cond);
            bool rezult = CheckSortedBehaviors(rank, cur_cond);
            _execute_deep--;
            // Checks sorted behavior list and execute
            return rezult;
        }
    }   
}