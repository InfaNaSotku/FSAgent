using FSAgent.Target;
using FSAgent.Core;

namespace FSAgent.Core
{
    internal class Generator<TargetType> where TargetType : BaseTargetType
    {
        private TargetType _target;
        private List<Behavior<TargetType>> _behaviors;
        private bool IsGenerate;

        public Generator(TargetType target, List<Behavior<TargetType>> behaviors)
        {
            _target = target;
            _behaviors = behaviors;
            IsGenerate = true;
        }
        public void Run()
        {
            IsGenerate = false;
            Execute();
        }
        public void Create()
        {
            IsGenerate = true;
            Execute();
        }



        private void Execute()
        {




        }

    }
}
