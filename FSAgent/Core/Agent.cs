﻿using FSAgent.Target;

namespace FSAgent.Core
{
    public class Null<TargetType> : Agent<TargetType>
        where TargetType : BaseTargetType, new()
    {
        public override void CreateBehavior() { }
        public override void RunBehavior() { }
        public override void PrintBehavior() { }
        internal override void AddAction(Action action,
            string? name) { }
        internal override TargetType GetTarget() => new TargetType();
    }
    public abstract class Agent<TargetType> where TargetType : BaseTargetType
    {
        public abstract void CreateBehavior();
        public abstract void RunBehavior();
        public abstract void PrintBehavior();
        internal abstract void AddAction(Action action,
            string? name);
        internal abstract TargetType GetTarget();
    }
}
