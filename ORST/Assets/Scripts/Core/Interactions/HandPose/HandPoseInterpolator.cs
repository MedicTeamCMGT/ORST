using Oculus.Interaction.HandGrab;

namespace ORST.Core.Interactions {
    public static class HandPoseInterpolator {
        public static void Interpolate(HandPoseData handPoseA, HandPoseData handPoseB, HandPose interpolatedPose, float t) {
            HandPose.Lerp(handPoseA.HandPose, handPoseB.HandPose, t, ref interpolatedPose);
        }
    }
}