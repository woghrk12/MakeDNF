public class AnimatorKey
{
    public class Character
    {
        public static readonly string NORMAL_SPEED = "NormalSpeed";
        public static readonly string MOVE_SPEED = "MoveSpeed";
        public static readonly string ATTACK_SPEED = "AttackSpeed";

        public static readonly string IS_WALK = "IsWalk";
        public static readonly string IS_JUMP = "IsJump";
        public static readonly string IS_JUMP_UP = "IsJumpUp";
        public static readonly string IS_JUMP_DOWN = "IsJumpDown";
        public static readonly string IS_ATTACK = "IsAttack";
        public static readonly string IS_CONTINUE_ATTACK = "IsContinueAttack";
        public static readonly string END_ATTACK = "EndAttack";
        public static readonly string DO_HIT = "DoHit";
        public static readonly string IS_HIT = "IsHit";

        public static readonly string BASE_ATTACK = "BaseAttack";

        public class FireKnight
        {
            public static readonly string SLASH_COMBO = "SlashCombo";
            public static readonly string CRESCENT = "Crescent";
            public static readonly string DODGE = "Dodge";
        }
    }

    public class Projectile
    {
        public static readonly string SHOT = "Shot";
        public static readonly string MOTION_SPEED = "MotionSpeed";
    }
}