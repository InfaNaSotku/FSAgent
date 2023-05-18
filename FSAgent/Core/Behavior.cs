using FSAgent.Target;

namespace FSAgent.Core
{
    internal class Behavior<TargetType> where TargetType : BaseTargetType
    {
        internal string? _name;
        private Action _action;
        internal Dictionary<int, int> _conditions;
        
        internal Behavior(Action action, string? name)
        {
            _action = action;
            _name = name;
            _conditions = new Dictionary<int, int>();
        }

        internal void Execute()
        {
            _action();
        }

    }
}
