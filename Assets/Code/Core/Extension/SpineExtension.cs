using System.Collections;
using Spine;
using Spine.Unity;

namespace Code.Core.Extension
{
    public static class SpineExtension
    {
        private static IEnumerator GraphicDelayFreeze(SkeletonGraphic sk)
        {
            yield return null;
            sk.freeze = true;
        }
        
        private static IEnumerator AnimationDelayFreeze(SkeletonAnimation sa)
        {
            yield return null;
            sa.UpdateMode = UpdateMode.Nothing;
        }
        
        public static TrackEntry PlayAnimations(this SkeletonGraphic skeletonGraphic, int trackIndex, bool loop,
            params string[] animations)
        {
            if (animations == null || animations.Length == 0) return null;

            skeletonGraphic.freeze = false;

            var animationState = skeletonGraphic.AnimationState;
            var trackEntry =
                animationState.SetAnimation(trackIndex, animations[0], 0 == animations.Length - 1 ? loop : false);

            for (var i = 1; i < animations.Length; i++)
            {
                trackEntry = animationState.AddAnimation(trackIndex, animations[i],
                    i == animations.Length - 1 ? loop : false, 0);
            }

            if (!loop)
            {
                trackEntry.Complete += e =>
                {
                    skeletonGraphic.Update();
                    skeletonGraphic.LateUpdate();
                    skeletonGraphic.freeze = true;
                };
                // trackEntry.Complete += e => skeletonGraphic.StartCoroutine(GraphicDelayFreeze(skeletonGraphic));
            }

            return trackEntry;
        }
        
        public static TrackEntry PlayAnimations(this SkeletonAnimation skeletonAnimation, int trackIndex, bool loop,
            params string[] animations)
        {
            if (animations == null || animations.Length == 0) return null;

            skeletonAnimation.UpdateMode = UpdateMode.FullUpdate;

            var animationState = skeletonAnimation.AnimationState;
            var trackEntry =
                animationState.SetAnimation(trackIndex, animations[0], 0 == animations.Length - 1 ? loop : false);

            for (var i = 1; i < animations.Length; i++)
            {
                trackEntry = animationState.AddAnimation(trackIndex, animations[i],
                    i == animations.Length - 1 ? loop : false, 0);
            }

            if (!loop)
            {
                trackEntry.Complete += e => skeletonAnimation.StartCoroutine(AnimationDelayFreeze(skeletonAnimation));
            }

            return trackEntry;
        }
    }
}