using FSAgent.Target;

namespace FSAgent.Core
{
    internal class Behavior<TargetType> where TargetType : BaseTargetType
    {
        internal string? _name;
        internal Func<IEnumerable<int>> _action;
        internal Dictionary<int, int> _conditions;
        
        internal Behavior(Func<IEnumerable<int>> action,
            string? name)
        {
            _action = action;
            _name = name;
            _conditions = new Dictionary<int, int>();
        }

    }
}
