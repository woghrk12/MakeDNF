using UnityEngine;

public class AnimatorKey
{
    public class Character
    {
        [Header("Animator key for motion speed")]
        public static readonly string NORMAL_SPEED = "NormalSpeed";
        public static readonly string MOVE_SPEED = "MoveSpeed";
        public static readonly string ATTACK_SPEED = "AttackSpeed";

        [Header("Animator key for move behaviour")]
        public static readonly string IS_WALK = "IsWalk";

        [Header("Animator key for jump behaviour")]
        public static readonly string IS_JUMP = "IsJump";
        public static readonly string IS_JUMP_UP = "IsJumpUp";
        public static readonly string IS_JUMP_DOWN = "IsJumpDown";

        [Header("Animator key for attack behaviour")]
        public static readonly string IS_ATTACK = "IsAttack";
        public static readonly string IS_CONTINUE_ATTACK = "IsContinueAttack";
        public static readonly string END_ATTACK = "EndAttack";
        public static readonly string BASE_ATTACK = "BaseAttack";

        [Header("Animator key for hit behaviour")]
        public static readonly string DO_HIT = "DoHit";
        public static readonly string IS_HIT = "IsHit";

        public class FireKnight
        {
            public static readonly string SLASH_COMBO = "SlashCombo";
            public static readonly string CRESCENT = "Crescent";
            public static readonly string DODGE = "Dodge";
        }
    }

    public class Projectile
    {
        public static readonly string MOTION_SPEED = "MotionSpeed";
        public static readonly string SHOT = "Shot";
    }
}