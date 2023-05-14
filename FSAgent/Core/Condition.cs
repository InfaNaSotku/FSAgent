namespace FSAgent.Core
{
    public class Condition : ICloneable
    {
        public List<Predicate> _predicates;
        public Condition(List<Predicate> predicates)
        {
            _predicates = predicates;
        }
        internal int GetReward()
        {
            int reward = 0;
            foreach (Predicate predicate in _predicates)
            {
                reward += predicate._reward;
            }
            return reward;
        }
        // Calculates left to right
        internal int ToHash()
        {
            int hash = 0;
            int pow = 1;
            foreach (Predicate predicate in _predicates)
            {
                if(predicate._state)
                {
                    hash += pow;
                }
                pow *= 2;
            }
            return hash;
        }


        public object Clone()
        {
            List<Predicate> clone = new List<Predicate>();
            foreach(Predicate predicate in _predicates)
            {
                clone.Add((Predicate)predicate.Clone());
            }
            return new Condition(clone);
        }

    }
}

