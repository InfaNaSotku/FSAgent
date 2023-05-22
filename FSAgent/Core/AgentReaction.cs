using FSAgent.Target;
namespace FSAgent.Core
{
	public class AgentReaction<RTargetType> : AgentBase<RTargetType> where

        RTargetType : BaseTargetType, new ()
    {
       

        public AgentReaction() : base()
        {
            
            //+реализация перехвата и просмотра состояния объекта
        }

        public bool IsNeedReaction()
        {
            return true;
        }
    }
}

