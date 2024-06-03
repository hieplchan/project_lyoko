using UnityEngine;

namespace StartledSeal
{
    public static partial class Const
    {
        public const string RightArmAnimatorLayerIndex = "Right Arm";
        public static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        public static readonly int JumpHash = Animator.StringToHash("Jump");
        public static readonly int DashHash = Animator.StringToHash("Dash");
        public static readonly int AttackHash = Animator.StringToHash("Attack");
        public static readonly int DeadHash = Animator.StringToHash("Dead");
        public static readonly int FlyHash = Animator.StringToHash("Fly");
        public static readonly int SwimHash = Animator.StringToHash("Swim");
        public static readonly int ShieldHash = Animator.StringToHash("Shield");
        public static readonly int Speed = Animator.StringToHash("Speed");
        public static readonly int UpperBodyAnimLayer = 1;
    }
}