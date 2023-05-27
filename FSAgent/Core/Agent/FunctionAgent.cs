using FSAgent.Target;
using FSAgent.Core;
namespace FSAgent.Core.Agent
{
	public class FunctionAgent<TargetType, RTargetType,
		Driver> where
        TargetType : BaseTargetType, new() where
		RTargetType : BaseTargetType, new()
    {
		private Agent<TargetType> _agent_adaptive;
		private Agent<RTargetType> _agent_reaction;

		public FunctionAgent(Driver driver)
		{
			_agent_adaptive = new AgentAdaptive<TargetType>();
			_agent_reaction = new AgentReaction<RTargetType>();
			((AgentReaction<RTargetType>)_agent_reaction).
				SetDriver(driver);
            ((AgentAdaptive<TargetType>)_agent_adaptive).
                SetDriver(driver);
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
			Execute(_agent_adaptive.RunBehavior);
		}

		public void CreateAdaptiveBehavior()
		{
            Execute(_agent_adaptive.CreateBehavior);
		}

		public void CreateReactionBehavior()
		{
			_agent_reaction.CreateBehavior();
		}

		public void PrintAdaptiveBehavior()
		{
			_agent_adaptive.PrintBehavior();
		}

		private void Execute(Action action)
		{
			while(true)
			{
                Task adaptive_agent_task = Task.Factory.StartNew(action);
				while (!((AgentReaction<RTargetType>)_agent_reaction).IsNeedReaction())
				{
					if(adaptive_agent_task.IsCompleted)
					{
						return;
					}
				}
				((AgentAdaptive<TargetType>)_agent_adaptive).
					CancelExecute();
				_agent_reaction.RunBehavior();
				adaptive_agent_task.Wait();
            }

        }
		
	}
}

