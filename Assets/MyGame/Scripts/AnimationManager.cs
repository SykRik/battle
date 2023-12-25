using System.Collections.Generic;
using System.Linq;

public class AnimationManager : SingletonBase<AnimationManager>
{
    private Dictionary<AnimationEnum, string> animations = new Dictionary<AnimationEnum, string>();

    protected override void Init()
    {
        animations[AnimationEnum.APPEAR]                    = "activity/appear";
        animations[AnimationEnum.BATH]                      = "activity/bath";
        animations[AnimationEnum.CAST_FLY]                  = "attack/ranged/cast-fly";
        animations[AnimationEnum.CAST_HIGH]                 = "attack/ranged/cast-high";
        animations[AnimationEnum.CAST_LOW]                  = "attack/ranged/cast-low";
        animations[AnimationEnum.CAST_MULTI]                = "attack/ranged/cast-multi";
        animations[AnimationEnum.CAST_TAIL]                 = "attack/ranged/cast-tail";
        animations[AnimationEnum.EAT_BITE]                  = "activity/eat-bite";
        animations[AnimationEnum.EAT_CHEW]                  = "activity/eat-chew";
        animations[AnimationEnum.ENTRANCE]                  = "activity/entrance";
        animations[AnimationEnum.EVADE]                     = "defense/evade";
        animations[AnimationEnum.EVOLVE]                    = "activity/evolve";
        animations[AnimationEnum.GET_BUFF]                  = "battle/get-buff";
        animations[AnimationEnum.GET_DEBUFF]                = "battle/get-debuff";
        animations[AnimationEnum.HIT_BY_NORMAL]             = "defense/hit-by-normal";
        animations[AnimationEnum.HIT_BY_NORMAL_CRIT]        = "defense/hit-by-normal-crit";
        animations[AnimationEnum.HIT_BY_NORMAL_DRAMATIC]    = "defense/hit-by-normal-dramatic";
        animations[AnimationEnum.HIT_BY_RANGED_ATTACK]      = "defense/hit-by-ranged-attack";
        animations[AnimationEnum.HIT_WITH_SHIELD]           = "defense/hit-with-shield";
        animations[AnimationEnum.HORN_GORE]                 = "attack/melee/horn-gore";
        animations[AnimationEnum.MOUTH_BITE]                = "attack/melee/mouth-bite";
        animations[AnimationEnum.MOVE_BACK]                 = "action/move-back";
        animations[AnimationEnum.MOVE_FORWARD]              = "action/move-forward";
        animations[AnimationEnum.MULTI_ATTACK]              = "attack/melee/multi-attack";
        animations[AnimationEnum.IDLE_NORMAL]               = "action/idle/normal";
        animations[AnimationEnum.NORMAL_ATTACK]             = "attack/melee/normal-attack";
        animations[AnimationEnum.PREPARE]                   = "activity/prepare";
        animations[AnimationEnum.IDLE_RANDOM_01]            = "action/idle/random-01";
        animations[AnimationEnum.IDLE_RANDOM_02]            = "action/idle/random-02";
        animations[AnimationEnum.IDLE_RANDOM_03]            = "action/idle/random-03";
        animations[AnimationEnum.IDLE_RANDOM_04]            = "action/idle/random-04";
        animations[AnimationEnum.IDLE_RANDOM_05]            = "action/idle/random-05";
        animations[AnimationEnum.RUN]                       = "action/run";
        animations[AnimationEnum.RUN_ORIGIN]                = "draft/run-origin";
        animations[AnimationEnum.SHRIMP]                    = "attack/melee/shrimp";
        animations[AnimationEnum.SLEEP]                     = "activity/sleep";
        animations[AnimationEnum.TAIL_MULTI_SLAP]           = "attack/melee/tail-multi-slap";
        animations[AnimationEnum.TAIL_ROLL]                 = "attack/melee/tail-roll";
        animations[AnimationEnum.TAIL_SMASH]                = "attack/melee/tail-smash";
        animations[AnimationEnum.TAIL_THRASH]               = "attack/melee/tail-thrash";
        animations[AnimationEnum.VICTORY_POSE_BACK_FLIP]    = "activity/victory-pose-back-flip";
    }

    public string GetAnimationName(AnimationEnum type)
    {
        return animations.TryGetValue(type, out var animation) ? animation : string.Empty;
    }

    public AnimationEnum GetAnimationType(string name)
    {
        return animations.FirstOrDefault(x => x.Value.Equals(name)).Key;
    }
}

public enum AnimationEnum
{
    APPEAR,
    BATH,
    CAST_FLY,
    CAST_HIGH,
    CAST_LOW,
    CAST_MULTI,
    CAST_TAIL,
    EAT_BITE,
    EAT_CHEW,
    ENTRANCE,
    EVADE,
    EVOLVE,
    GET_BUFF,
    GET_DEBUFF,
    HIT_BY_NORMAL,
    HIT_BY_NORMAL_CRIT,
    HIT_BY_NORMAL_DRAMATIC,
    HIT_BY_RANGED_ATTACK,
    HIT_WITH_SHIELD,
    HORN_GORE,
    MOUTH_BITE,
    MOVE_BACK,
    MOVE_FORWARD,
    MULTI_ATTACK,
    IDLE_NORMAL,
    NORMAL_ATTACK,
    PREPARE,
    IDLE_RANDOM_01,
    IDLE_RANDOM_02,
    IDLE_RANDOM_03,
    IDLE_RANDOM_04,
    IDLE_RANDOM_05,
    RUN,
    RUN_ORIGIN,
    SHRIMP,
    SLEEP,
    TAIL_MULTI_SLAP,
    TAIL_ROLL,
    TAIL_SMASH,
    TAIL_THRASH,
    VICTORY_POSE_BACK_FLIP
}