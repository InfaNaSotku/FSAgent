using FSAgent.Target;
using FSAgent.Core;
namespace FSAgent.Core.Agent
{
	public class AgentReaction<RTargetType> : AgentBase<RTargetType> where

        RTargetType : BaseTargetType, new ()
    {
       

        public AgentReaction() : base() { }

        public bool IsNeedReaction()
        {
            return true;
        }
    }
}

