using FSAgent.Target;

namespace FSAgent.Core
{
    public abstract class AgentDecorator<TargetType> : Agent<TargetType> where TargetType : BaseTargetType
    {
        private readonly Agent<TargetType> _wrapped_entity;
        protected TargetType _target;

        public AgentDecorator(Agent<TargetType> wrapped_entity)
        {
            _wrapped_entity = wrapped_entity;
            _target = GetTarget();
            AddAction(Action);
        }

        protected abstract void Action();

        internal override TargetType GetTarget()
        {
            return _wrapped_entity.GetTarget();
        }
        internal override void AddAction(Action action)
        {
            _wrapped_entity.AddAction(action);
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
    }
}
