using FSAgent.Target;

namespace FSAgent.Core
{
    public abstract class Agent<TargetType> where TargetType : BaseTargetType
    {
        public abstract void CreateBehavior();
        public abstract void RunBehavior();
        public abstract void PrintBehavior();
        internal abstract void AddAction(Action action, string? name);
        internal abstract TargetType GetTarget();
    }
}
