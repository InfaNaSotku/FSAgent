using System;
using FSAgent.Core;
namespace FSAgent.Target
{
    public abstract class BaseTargetType
    {
        public readonly List<Predicate> _predicates;
        public object _driver;

        public BaseTargetType()
        {
            _predicates = new List<Predicate>()
            {
                new Predicate("ISFINISH",
                false, int.MaxValue),
                new Predicate("ISFAIL",
                false, int.MinValue)
            };
            _driver = new object();
        }
        public abstract void TargetReset();
        // Calls when agent doesn't know what it should to do
        public abstract void Alarm();
        public abstract Condition GetCondition();
        public abstract void Log(string body);
        public abstract void Start();

        public int FindPredicate(string name)
        {
            for (int i = 0; i < _predicates.Count(); i++)
            {
                if (_predicates[i]._name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        internal Condition HashToCondition(int condition_hash)
        {
            List<Predicate> cur_preds = new List<Predicate>();
            foreach (Predicate predicate in _predicates)
            {
                cur_preds.Add(new Predicate(predicate._name,
                    condition_hash % 2 == 1 ? true : false, predicate._reward));
                condition_hash /= 2;
            }
            return new Condition(cur_preds);
        }
        // Considers that finish bit is first bit
        internal bool IsFinish(Condition condition)
        {
            return condition._predicates[0]._state;
        }
        // Considers that finish bit is first bit
        internal bool IsFinish(int condition_hash)
        { 
            return condition_hash % 2 == 1 ? true : false;
        }
        // Considers that fail bit is second bit
        internal bool IsFail(Condition condition)
        {
            return condition._predicates[1]._state;
        }
        // Considers that fail bit is second bit
        internal bool IsFail(int condition_hash)
        {
            return condition_hash % 4 == 1 ? true : false;
        }
        internal void SetDriver<Driver>(Driver driver)
        {
            _driver = driver ?? new object();
        }
    }
}
