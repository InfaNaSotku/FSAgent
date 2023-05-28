using FSAgent.Target;
using FSAgent.Core;
using System;

namespace FSAgent.Core.Agent
{
    public class AgentBase<TargetType> : Agent<TargetType> where
        TargetType : BaseTargetType, new()
    {
        internal List<Behavior<TargetType>> _behaviors;
        internal TargetType _target;
        internal Generator<TargetType> _generator;
         
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
        internal override void AddAction(Func<IEnumerable<int>>
            action, string? name)
        {
            _behaviors.Add(new Behavior<TargetType>(action, name));
        }

        internal void SetDriver<Driver>(Driver driver)
        {
            _target.SetDriver(driver);
            _target.Start();
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
