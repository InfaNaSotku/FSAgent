using FSAgent.Target;
using FSAgent.Core;
namespace FSAgent.Core.Agent
{
	public class AgentAdaptive<TargetType> : AgentBase<TargetType> where
		TargetType : BaseTargetType, new()
	{
		public AgentAdaptive() : base() { }

		public void CancelExecute()
		{
			_generator.IsCancel = true;
		}
	}
}

