﻿using FSAgent.Target;
using System;

namespace FSAgent.Core
{
    public class AgentBase<TargetType> : Agent<TargetType> where TargetType : BaseTargetType, new()
    {
        private List<Behavior<TargetType>> _behaviors;
        private TargetType _target;
        private Generator<TargetType> _generator;

        public AgentBase()
        {
            _behaviors = new List<Behavior<TargetType>>();
            _target = new TargetType();
            _generator = new Generator<TargetType>(_target, _behaviors);
        }


        internal override TargetType GetTarget()
        {
            return _target;
        }
        internal override void AddAction(Action action, string? name)
        {
            _behaviors.Add(new Behavior<TargetType>(action, name));
        }



        public override void CreateBehavior()
        {
            _generator.Create();
        }
        public override void RunBehavior()
        {
            _generator.Run();
        }


        public override void PrintBehavior()
        {
            foreach(var behavior in _behaviors)
            {
                Console.WriteLine($"{behavior._name}:");
                foreach (var cond in behavior._conditions)
                {
                    for (int i = 0; i < _target._predicates.Count(); ++i)
                    {
                        Predicate start = _target.
                        HashToCondition(cond.Key).
                        _predicates[i];
                        Predicate end = _target.
                        HashToCondition(cond.Value).
                        _predicates[i];
                        Console.WriteLine($"{start.
                            _name}={start._state} -> {end.
                            _name}={end._state}");
                    }
                    Console.WriteLine('\n');
                }
            }
        }
    }
}