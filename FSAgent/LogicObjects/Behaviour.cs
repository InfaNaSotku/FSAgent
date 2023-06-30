using System.IO;
using FSAgent.Agent.Component;

namespace FSAgent.LogicObjects
{
    internal class Behavior<TargetType> where TargetType : BaseTargetType
    {
        internal string? _name;

        private Queue<Behavior<TargetType>>?
            _compound_action;

        private Func<IEnumerable<int>>?
            _default_action;

        // Key - start condition hash
        // Value - end condition hash
        internal Dictionary<int, int> _conditions;

        internal Behavior(string? name,
            Queue<Behavior<TargetType>>?
            compound_action = null, Func<IEnumerable<int>>?
            default_action = null)
        {
            _default_action = default_action;
            _compound_action = compound_action;
            if (_default_action == null
                && _compound_action == null)
            {
                throw new ArgumentNullException();
            }
            _name = name;
            _conditions = new Dictionary<int, int>();
        }

        internal IEnumerable<int> Run()
        {
            // Executing action step by step
            if (_compound_action != null)
            {
                foreach (var action in
                    _compound_action)
                {
                    foreach (var move in
                    action.Run())
                    {
                        yield return 0;
                    }
                }
            }
            if (_default_action != null)
            {
                foreach (var move in
                    _default_action.Invoke())
                {
                    yield return 0;
                }
            }
        }

        internal List<string> GetCompoundNames()
        {
            List<string> names = new List<string>();
            if (_compound_action != null)
            {
                foreach (var action in
                    _compound_action)
                {
                    names.Add(action._name ??
                        "UnknownAction");
                }
            }
            return names;
        }

        // Imports condition Dict
        internal void Import(Dictionary<int, int> conditions)
        {
            _conditions = conditions;  
        }
        // Saves condition Dict
        internal void SaveConditions(string path)
        {
            string output = _name ??
                "UnknownAction";
            foreach (var condition in _conditions)
            {
                output += $" {condition.Key} {condition.Value}";
            }
            File.AppendAllText(path, output);
        }
        // Saves compound action
        internal void SaveCompoundAction(string path)
        {
            string output = _name ??
                "UnknownAction";
            foreach (var name in GetCompoundNames())
            {
                output += $" {name}";
            }
            File.AppendAllText(path, output);
        }
    }
}

