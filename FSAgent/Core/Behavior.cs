using FSAgent.Target;

namespace FSAgent.Core
{
    internal class Behavior<TargetType> where TargetType : BaseTargetType
    {
        private Action _action;
        internal List<int> _conditions;
        
        internal Behavior(Action action)
        {
            _action = action;
            _conditions = new List<int>();
        }

        internal void Execute()
        {
            _action();
        }
    }
}
