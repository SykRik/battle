using AxieMixer.Unity;
using Spine;
using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : SingletonMono<GameManager>
{
    #region ===== Fields =====

    private CharacterData dataAttacker = null;
    private CharacterData dataDefender = null;

    #endregion

    #region ===== Properties =====

    #endregion

    protected override void Init()
    {
        dataAttacker = new CharacterData
        {
            AxieID = "4191804",
            Genes = "0x20000000000001000281a000810c0000000100102080830a000100141061020a000100101880c4060001000c0880c50c000100101820c4060001001008804402",
            HP = 16
        };
        dataDefender = new CharacterData
        {
            AxieID = "2724598",
            Genes = "0x18000000000001000240a050c210000000010008286045040001000c08814302000100042021430c0001000c2861430a0001000c3061830c0001000c08404302",
            HP = 16
        };

        Mixer.Init();
    }
}

public class CharacterData
{
    public string   AxieID  = string.Empty;
    public string   Genes   = string.Empty;
    public int      HP      = 0;
}

public class CharacterController : MonoBehaviour
{
    #region ===== Fields =====

    private SkeletonAnimation   skeleton    = null;
    private ProgressBar         probarHP    = null;

    private string              axieID      = string.Empty;
    private string              genes       = string.Empty;
    private int                 maxHP       = 0;
    private int                 hp          = 0;
    private int                 number      = 0;

    #endregion

    #region ===== Properties =====

    private AnimationManager    AnimationManager    => AnimationManager.Instance;
    private AudioManager        AudioManager        => AudioManager.Instance;

    #endregion

    #region ===== Methods =====

    public void Awake()
    {
        skeleton = GetComponent<SkeletonAnimation>()
                   ?? throw new ArgumentNullException(nameof(skeleton));
        probarHP = GetComponent<ProgressBar>()
                   ?? throw new ArgumentNullException(nameof(probarHP));
    }

    public void Init(CharacterData data)
    {
        axieID  = data.AxieID;
        genes   = data.Genes;
        maxHP   = data.HP;
        hp      = data.HP;
        number  = UnityEngine.Random.Range(0, int.MaxValue) % 3;

        LoadSkeleton();
    }

    private void LoadSkeleton()
    {
        Mixer.SpawnSkeletonAnimation(skeleton, axieID, genes);

        skeleton.transform.SetParent(transform, false);
        skeleton.transform.localPosition = Vector3.zero;
        skeleton.transform.localScale = new Vector3(1, 1, 1);
        skeleton.skeleton.FindSlot("shadow").Attachment = null;
        skeleton.state.End += SpineEndHandler;

        DoAnimation(AnimationEnum.IDLE_NORMAL);
    }

    private void SpineEndHandler(TrackEntry trackEntry)
    {
        var animationName = AnimationManager.GetAnimation(AnimationEnum.MOVE_FORWARD);
        if (animationName.Equals(trackEntry.Animation.Name))
        {
            DoAnimation(AnimationEnum.IDLE_NORMAL);
        }
    }

    public void DealDamage(CharacterController target)
    {
        var factor = (number - target.number + 3) % 3;
        var damage = factor switch
        {
            0 => 4,
            1 => 5,
            2 => 3,
            _ => throw new NotImplementedException()
        };
        target.TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        if (hp > damage)
            hp -= damage;
        else
            hp = 0;

        probarHP.SetValue(1f * hp / maxHP);
    }

    public void DoAnimation(AnimationEnum animationType)
    {
        switch (animationType)
        {
            case AnimationEnum.IDLE_NORMAL:
            case AnimationEnum.IDLE_RANDOM_01:
            case AnimationEnum.IDLE_RANDOM_02:
            case AnimationEnum.IDLE_RANDOM_03:
            case AnimationEnum.IDLE_RANDOM_04:
            case AnimationEnum.IDLE_RANDOM_05:
                {
                    var animationName = AnimationManager.GetAnimation(animationType);
                    skeleton.timeScale = 0.5f;
                    skeleton.AnimationState.SetAnimation(0, animationName, true);
                }
                break;
            default:
                {
                    var animationName = AnimationManager.GetAnimation(animationType);
                    skeleton.timeScale = 1f;
                    skeleton.AnimationState.SetAnimation(0, animationName, false);
                }
                break;
        }
    }

    #endregion
}

public class ProgressBar : MonoBehaviour
{
    #region ===== Fields =====

    private LTDescr tween       = null;
    [SerializeField]
    private Slider  sliderHP    = null;
    [SerializeField]
    private float   duration    = 0.5f;

    #endregion

    #region ===== Methods =====

    private void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        if (tween != null)
        {
            LeanTween.cancel(tween.uniqueId);
        }
        if (sliderHP != null)
        {
            sliderHP.value = 1f;
        }
    }

    public void SetValue(float value)
    {
        if (tween != null)
        {
            LeanTween.cancel(tween.uniqueId);
        }
        if (sliderHP != null)
        {
            tween = LeanTween.value(gameObject, sliderHP.value, value, duration)
                             .setOnUpdate(value => sliderHP.value = value)
                             .setOnComplete(() => tween = null);
        }
    }

    #endregion
}
