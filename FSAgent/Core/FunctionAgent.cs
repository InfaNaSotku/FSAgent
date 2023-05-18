using FSAgent.Target;

namespace FSAgent.Core
{
	public class FunctionAgent<TargetType, RTargetType> where
        TargetType : BaseTargetType, new() where
		RTargetType : BaseTargetType, new()
    {
		private Agent<TargetType> _agent_adaptive;
		private Agent<RTargetType> _agent_reaction;

		public FunctionAgent()
		{
			_agent_adaptive = new AgentAdaptive<TargetType>();
			_agent_reaction = new AgentReaction<RTargetType>();
        }

		public void UpdateAdaptive<NewMove>() where
			NewMove : AgentDecorator<TargetType>, new()
		{
			_agent_adaptive = new NewMove().
				Wrap(_agent_adaptive);
        }
		public void UpdateReaction<NewMove>() where
            NewMove : AgentDecorator<RTargetType>, new()
        {
			_agent_reaction = new NewMove().
				Wrap(_agent_reaction);
        }

		public void Run()
		{
			_agent_adaptive.RunBehavior();
		}

		public void CreateAdaptiveBehavior()
		{
			_agent_adaptive.CreateBehavior();
		}

		public void PrintAdaptiveBehavior()
		{
			_agent_adaptive.PrintBehavior();
		}
		
	}
}

