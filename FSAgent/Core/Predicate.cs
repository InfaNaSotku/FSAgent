namespace FSAgent.Core
{
	public class Predicate : ICloneable
	{
		internal string _name;
        public bool _state;
        internal int _reward;
		public Predicate(string name, bool state, int reward) 
		{
			_name = name;
			_reward = reward;
			_state = state;
		}
	
		

		public object Clone()
		{
			return new Predicate((string)_name.Clone(),
				_state, _reward);
		}
	}
	
}

