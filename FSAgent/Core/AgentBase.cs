using FSAgent.Target;
using System;

namespace FSAgent.Core
{
    public class AgentBase<TargetType> : Agent<TargetType> where TargetType : BaseTargetType, new()
    {
        private List<Behavior<TargetType>> _behaviors;
        private TargetType _target;
        private List<string> _predicates;
        private Generator<TargetType> _generator;

        public AgentBase(List<string> predicates)
        {
            _predicates = predicates;
            _behaviors = new List<Behavior<TargetType>>();
            _target = new TargetType();
            _generator = new Generator<TargetType>(_target, _behaviors);
        }


        internal override TargetType GetTarget()
        {
            return _target;
        }
        internal override void AddAction(Action action)
        {
            _behaviors.Add(new Behavior<TargetType>(action: action));
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
           
        }
    }
}
