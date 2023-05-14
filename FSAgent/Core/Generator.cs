using FSAgent.Target;
using FSAgent.Core;

namespace FSAgent.Core
{
    internal class Generator<TargetType> where TargetType : BaseTargetType
    {
        private TargetType _target;
        private List<Behavior<TargetType>> _behaviors;
        private bool IsGenerate;

        private class Chain
        {
            internal int _points;
            internal int _behavior_pos;
            internal Chain(int points, int behavior_pos)
            {
                _points = points;
                _behavior_pos = behavior_pos;
            }
        }

        public Generator(TargetType target, List<Behavior<TargetType>> behaviors)
        {
            _target = target;
            _behaviors = behaviors;
            IsGenerate = true;
        }
        public void Run()
        {
            IsGenerate = false;
            if(!Execute())
            {
                _target.Alarm();
                //log
            }
        }
        public void Create()
        {
            IsGenerate = true;
            if(!Execute())
            {
                //log
            }
        }
        private int EstimateChain(int cur_hash)
        {
            int reward = 0;
            foreach (var behavior in _behaviors)
            {
                if (behavior._conditions.ContainsKey(cur_hash))
                {
                    if(_target.IsFail(behavior._conditions[cur_hash]))
                    {
                        continue;
                    }
                    if (_target.IsFinish(behavior._conditions[cur_hash]))
                        return _target.HashToCondition(behavior.
                            _conditions[cur_hash]).GetReward();
                    int cur_reward =
                        EstimateChain(behavior._conditions[cur_hash]);
                    if (cur_reward > reward)
                    {
                        reward = cur_reward;
                    }
                }
            }
            return reward;
        }
        
        private bool Execute()
        {
            Condition cond = _target.GetCondition();
            if(_target.IsFinish(cond))
            {
                return true;
            }
            if(_target.IsFail(cond))
            {
                return false;
            }
            int cond_hash = cond.ToHash();
            List<Chain> bes_points = new List<Chain>(_behaviors.Count());
            for (int i = 0; i < _behaviors.Count(); ++i)
            {
                if (_behaviors[i]._conditions.ContainsKey(cond_hash))
                {
                    bes_points.Add(new Chain(EstimateChain(cond_hash), i));
                }
                else
                {
                    bes_points.Add(new Chain(0, i));
                }
            }
            bes_points.Sort((Chain first, Chain second) =>
            second._points.CompareTo(first._points)
            );
            foreach(Chain be_points in bes_points)
            {
                // Agent doesn't know what it should to do
                if(be_points._points == 0 &&
                    !_behaviors[be_points._behavior_pos].
                        _conditions.ContainsKey(cond_hash) &&
                        !IsGenerate)
                {
                    _target.Alarm();
                }

                _behaviors[be_points._behavior_pos].Execute();

                if(!_behaviors[be_points._behavior_pos].
                        _conditions.ContainsKey(cond_hash))
                {
                    _behaviors[be_points._behavior_pos].
                        _conditions.Add(cond_hash,
                        _target.GetCondition().ToHash());
                }
                else
                {
                    _behaviors[be_points._behavior_pos].
                        _conditions[cond_hash] = _target.
                        GetCondition().ToHash();
                }

                if(Execute())
                {
                    return true;
                }

                _target.TargetReset();
            }
            return false;
        }

    }
}
