﻿using FSAgent.Target;

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
                CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
                CancellationToken token = cancelTokenSource.Token;
                Task adaptive_agent_task = new Task(action, token);
				while (!((AgentReaction<RTargetType>)_agent_reaction).IsNeedReaction())
				{
					if(adaptive_agent_task.IsCompleted)
					{
						return;
					}
				}
				// здесь должен быть функционал отрубания потока adaptive_agent_task
				_agent_reaction.RunBehavior();
            }

        }
		
	}
}

