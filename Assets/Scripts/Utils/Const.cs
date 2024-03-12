using UnityEngine;

namespace StartledSeal
{
    public static class Const
    {
        public const string PlayerTag = "Player";
        public const string EnemyTag = "Enemy";
        public const string MovingPlatform = "MovingPlatform";
        
        public const string RightArmAnimatorLayerIndex = "Right Arm";

        public const float NoNearbyEnemyDistance = float.MaxValue;
        
        public static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        public static readonly int JumpHash = Animator.StringToHash("Jump");
        public static readonly int DashHash = Animator.StringToHash("Dash");
        public static readonly int AttackHash = Animator.StringToHash("Attack");
        public static readonly int DeadHash = Animator.StringToHash("Dead");
        public static readonly int FlyHash = Animator.StringToHash("Fly");
        public static readonly int Speed = Animator.StringToHash("Speed");
    }
}