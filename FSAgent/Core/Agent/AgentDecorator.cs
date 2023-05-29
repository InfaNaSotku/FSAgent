using FSAgent.Target;
using FSAgent.Core;
using System.ComponentModel;

namespace FSAgent.Core.Agent
{
    public abstract class AgentDecorator<TargetType> :
        Agent<TargetType> where TargetType : BaseTargetType, new()
    {
        private Agent<TargetType> _wrapped_entity;
        protected TargetType _target;

        public AgentDecorator()
        {
            _target = new TargetType();
            _wrapped_entity = new NullAgent<TargetType>();
        }

        internal AgentDecorator<TargetType>
            Wrap(Agent<TargetType> wrapped_entity)
        {
            _wrapped_entity = wrapped_entity;
            _target = GetTarget();
            AddAction(Action, TypeDescriptor.GetClassName(this));
            return this;
        }

        protected abstract IEnumerable<int> Action();

        internal override TargetType GetTarget()
        {
            return _wrapped_entity.GetTarget();
        }
        internal override void AddAction(Func<IEnumerable<int>> action, string? name)
        {
            _wrapped_entity.AddAction(action, name);
        }
        public override void CreateBehavior()
        {
            _wrapped_entity.CreateBehavior();
        }
        public override void RunBehavior()
        {
            _wrapped_entity.RunBehavior();
        }
        public override void PrintBehavior()
        {
            _wrapped_entity.PrintBehavior();
        }
        public override void DropTarget()
        {
            _wrapped_entity.DropTarget();
        }
    }
}
