//using AxieMixer.Unity;
//using Spine;
//using Spine.Unity;
//using System;
//using UnityEngine;

//public class HeroController : MonoBehaviour
//{
//    #region ===== Fields =====

//    private SkeletonAnimation   skeleton    = null;
//    private HealthBar           healthBar   = null;

//    private string  axieID  = string.Empty;
//    private string  genes   = string.Empty;
//    private int     maxHP   = 0;
//    private int     hp      = 0;
//    private int     number  = 0;

//    #endregion

//    #region ===== Properties =====

//    private AnimationManager AnimationManager => AnimationManager.Instance;

//    #endregion

//    #region ===== Methods =====

//    public void Awake()
//    {
//        if (!TryGetComponent(out skeleton))
//            throw new ArgumentNullException(nameof(skeleton));
//        if (!TryGetComponent(out healthBar))
//            throw new ArgumentNullException(nameof(healthBar));
//    }

//    public void Init(HeroData data)
//    {
//        axieID  = data.AxieID;
//        genes   = data.Genes;
//        maxHP   = data.HP;
//        hp      = data.HP;
//        number  = UnityEngine.Random.Range(0, int.MaxValue) % 3;

//        LoadSkeleton();
//    }

//    private void LoadSkeleton()
//    {
//        Mixer.SpawnSkeletonAnimation(skeleton, axieID, genes);

//        skeleton.transform.SetParent(transform, false);
//        skeleton.transform.localPosition    = Vector3.zero;
//        skeleton.transform.localScale       = new Vector3(1, 1, 1);
//        skeleton.skeleton.FindSlot("shadow").Attachment = null;
//        skeleton.state.End += SpineEndHandler;

//        DoAnimation(AnimationEnum.IDLE_NORMAL);
//    }

//    private void SpineEndHandler(TrackEntry trackEntry)
//    {
//        var animationName = trackEntry.Animation.Name;
//        var animationType = AnimationManager.GetAnimationType(animationName);

//        switch (animationType)
//        {
//            case AnimationEnum.MOVE_BACK:
//            case AnimationEnum.MOVE_FORWARD:
//                DoAnimation(AnimationEnum.IDLE_NORMAL);
//                break;
//        }
//    }

//    public void DealDamage(HeroController target)
//    {
//        var factor = (number - target.number + 3) % 3;
//        var damage = factor switch
//        {
//            0 => 4,
//            1 => 5,
//            2 => 3,
//            _ => throw new NotImplementedException()
//        };
//        target.TakeDamage(damage);
//    }

//    public void TakeDamage(int damage)
//    {
//        if (hp > damage)
//        {
//            hp -= damage;
//            healthBar.SetValue(1f * hp / maxHP);
//            DoAnimation(AnimationEnum.HIT_BY_NORMAL);
//        }
//        else
//        {
//            hp = 0;
//            healthBar.SetValue(0f);
//            DoAnimation(AnimationEnum.SLEEP);
//        }
//    }

//    public void DoAnimation(AnimationEnum animationType)
//    {
//        var parameter = animationType switch
//        {
//            AnimationEnum.IDLE_NORMAL       => Tuple.Create(0.5f, true),
//            AnimationEnum.IDLE_RANDOM_01    => Tuple.Create(0.5f, true),
//            AnimationEnum.IDLE_RANDOM_02    => Tuple.Create(0.5f, true),
//            AnimationEnum.IDLE_RANDOM_03    => Tuple.Create(0.5f, true),
//            AnimationEnum.IDLE_RANDOM_04    => Tuple.Create(0.5f, true),
//            AnimationEnum.IDLE_RANDOM_05    => Tuple.Create(0.5f, true),
//            AnimationEnum.SLEEP             => Tuple.Create(1.0f, true),
//            _                               => Tuple.Create(1.0f, false),
//        };
//        var animationName   = AnimationManager.GetAnimationName(animationType);
//        var timeScale       = parameter.Item1;
//        var loop            = parameter.Item2;
//        skeleton.timeScale  = timeScale;
//        skeleton.AnimationState.SetAnimation(0, animationName, loop);
//    }

//    #endregion
//}
