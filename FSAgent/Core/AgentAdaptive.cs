using FSAgent.Target;
namespace FSAgent.Core
{
	public class AgentAdaptive<TargetType> : AgentBase<TargetType> where
		TargetType : BaseTargetType, new()
	{
		public AgentAdaptive() : base()
		{

		}
	}
}

