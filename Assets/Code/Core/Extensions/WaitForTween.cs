using DG.Tweening;
using UnityEngine;

namespace Code.Core.Extensions
{
    public class WaitForTween : CustomYieldInstruction
    {
        public override bool keepWaiting => _tweener.IsActive() && !_tweener.IsComplete();

        private Tweener _tweener;
        public WaitForTween(Tweener tweener)
        {
            _tweener = tweener;
            
        }

        public void Complete()
        {
            _tweener.Complete();
        }
    }

    public class WaitForTweenSeq : CustomYieldInstruction
    {
        public override bool keepWaiting => _seq.IsActive() && !_seq.IsComplete();

        private Sequence _seq;
        public WaitForTweenSeq(Sequence seq)
        {
            _seq = seq;
            
        }

        public void Complete()
        {
            _seq.Complete();
        }
    }
}