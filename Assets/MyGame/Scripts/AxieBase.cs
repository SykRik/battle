using AxieMixer.Unity;
using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum Team
{
    Attacker,
    Defender,
    Undefine
}

public abstract class AxieBase : MonoBehaviour
{
    #region ===== Fields =====

    protected string            axieID              = string.Empty;
    protected string            genes               = string.Empty;
    protected Team              team                = Team.Undefine;
    protected int               maxHP               = 0;
    protected int               hp                  = 0;
    protected int               number              = 0;
    protected bool              alive               = true;
    protected Node              node                = null;
    protected SkeletonAnimation skeletonAnimation   = null;
    protected HealthBar         healthBar           = null;

    #endregion

    #region ===== Properties =====

    public Node Node    => node;
    public Team Team    => team;
    public int  HP      => hp;

    #endregion

    #region ===== Methods =====

    public virtual void Init(string axieID, string genes, int hp, Node node)
    {
        this.axieID             = axieID;
        this.genes              = genes;
        this.node               = node;
        this.maxHP              = hp;
        this.hp                 = hp;
        this.alive              = true;
        this.number             = UnityEngine.Random.Range(0, int.MaxValue) % 3;
        this.skeletonAnimation  = GetComponentInChildren<SkeletonAnimation>();
        this.healthBar          = GetComponentInChildren<HealthBar>();

        gameObject.SetActive(true);
        healthBar.SetValue(1f);
        if (node != null)
        {
            transform.localPosition = node.pos;
            node.Enter(this);
        }
        {
            Mixer.Init();
            Mixer.SpawnSkeletonAnimation(skeletonAnimation, axieID, genes);

            skeletonAnimation.transform.SetParent(transform, false);
            skeletonAnimation.transform.localPosition = Vector3.down * 0.4f;
            skeletonAnimation.transform.localScale = Vector3.one * 0.6f;
            skeletonAnimation.timeScale = 0.5f;
            skeletonAnimation.skeleton.FindSlot("shadow").Attachment = null;
            skeletonAnimation.state.End += SpineEndHandler;
            
            var scaleXabs = Mathf.Abs(skeletonAnimation.skeleton.ScaleX);
            var scaleX = node.pos.x > 0 ? 1 : -1;
            skeletonAnimation.skeleton.ScaleX = scaleX * scaleXabs;
            DoAnimation(AnimationEnum.IDLE_NORMAL);
        }
    }

    private void SpineEndHandler(TrackEntry trackEntry)
    {
        var animationName = trackEntry.Animation.Name;
        var animationType = AnimationManager.Instance.GetAnimationType(animationName);

        switch (animationType)
        {
            case AnimationEnum.MOVE_BACK:
            case AnimationEnum.MOVE_FORWARD:
                DoAnimation(AnimationEnum.IDLE_NORMAL);
                break;
        }
    }

    public void DealDamage()
    {
        if (alive)
        {
            var target = FindCloseTarget();
            if (target != null)
            {
                if (target.Node.col == node.col)
                {
                    var animation   = GetComponentInChildren<SkeletonAnimation>();
                    var scaleXabs   = Mathf.Abs(animation.skeleton.ScaleX);
                    var scaleX      = target.Node.row > node.row ? -1 : 1;
                    animation.skeleton.ScaleX = scaleX * scaleXabs;
                }
                else
                {
                    var animation   = GetComponentInChildren<SkeletonAnimation>();
                    var scaleXabs   = Mathf.Abs(animation.skeleton.ScaleX);
                    var scaleX      = target.Node.col > node.col ? -1 : 1;
                    animation.skeleton.ScaleX = scaleX * scaleXabs;
                }
                var factor = (number - target.number + 3) % 3;
                var damage = factor switch
                {
                    0 => 4,
                    1 => 5,
                    2 => 3,
                    _ => throw new NotImplementedException()
                };
                DoAnimation(AnimationEnum.NORMAL_ATTACK);
                target.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (alive)
        {
            if (hp > 0)
            {
                var dmg = hp > damage ? damage : hp;
                hp -= dmg;
                healthBar.SetValue(1f * hp / maxHP);
                CharacterManager.Instance.TeamTakeDamage(team, dmg);
            }
        }
    }

    public void CheckHealth()
    {
        if (alive)
        {
            if (hp <= 0)
            {
                alive = false;
                gameObject.SetActive(false);
                node.Leave(this);
            }
        }
    }

    public void MoveToTarget()
    {
        if (alive)
        {
            var target = FindTarget();
            if (target == null)
            {
                Idle();
                return;
            }
            else
            {
                if (target.Node.col == node.col)
                {
                    var animation   = GetComponentInChildren<SkeletonAnimation>();
                    var scaleXabs   = Mathf.Abs(animation.skeleton.ScaleX);
                    var scaleX      = target.Node.row > node.row ? -1 : 1;
                    animation.skeleton.ScaleX = scaleX * scaleXabs;
                }
                else
                {
                    var animation   = GetComponentInChildren<SkeletonAnimation>();
                    var scaleXabs   = Mathf.Abs(animation.skeleton.ScaleX);
                    var scaleX      = target.Node.col > node.col ? -1 : 1;
                    animation.skeleton.ScaleX = scaleX * scaleXabs;
                }
            }
            if (Mathf.Abs(target.node.col - node.col) + Mathf.Abs(target.node.row - node.row) == 1)
            {
                Idle();
                return;
            }
            if (target.node.col == node.col && target.node.row == node.row)
            {
                Idle();
                return;
            }
            if (target.node.row > node.row && node.up.IsVacant())
            {
                MoveTo(node.up);
                return;
            }
            if (target.node.row < node.row && node.down.IsVacant())
            {
                MoveTo(node.down);
                return;
            }
            if (target.node.col > node.col && node.right.IsVacant())
            {
                MoveTo(node.right);
                return;
            }
            if (target.node.col < node.col && node.left.IsVacant())
            {
                MoveTo(node.left);
                return;
            }
            // Default
            {
                Idle();
                return;
            }
        }
    }

    public void DoAnimation(AnimationEnum animationType)
    {
        var parameter = animationType switch
        {
            AnimationEnum.IDLE_NORMAL       => Tuple.Create(0.5f, true),
            AnimationEnum.IDLE_RANDOM_01    => Tuple.Create(0.5f, true),
            AnimationEnum.IDLE_RANDOM_02    => Tuple.Create(0.5f, true),
            AnimationEnum.IDLE_RANDOM_03    => Tuple.Create(0.5f, true),
            AnimationEnum.IDLE_RANDOM_04    => Tuple.Create(0.5f, true),
            AnimationEnum.IDLE_RANDOM_05    => Tuple.Create(0.5f, true),
            AnimationEnum.SLEEP             => Tuple.Create(1.0f, true),
            _                               => Tuple.Create(1.0f, false),
        };
        var animationName   = AnimationManager.Instance.GetAnimationName(animationType);
        var timeScale       = parameter.Item1;
        var loop            = parameter.Item2;
        var skeleton        = GetComponentInChildren<SkeletonAnimation>();
        skeleton.timeScale  = timeScale;
        skeleton.AnimationState.SetAnimation(0, animationName, loop);
    }

    private void MoveTo(Node targetNode)
    {
        if (targetNode == null)
            return;

        node?.Leave(this);
        node = targetNode;
        node?.Enter(this);
        // Play anim move
        DoAnimation(AnimationEnum.MOVE_FORWARD);
        LeanTween.moveLocal(gameObject, targetNode.pos, 0.5f);
    }

    private void Idle()
    {
        // Play anim idle
        DoAnimation(AnimationEnum.IDLE_NORMAL);
    }


    public AxieBase FindTarget()
    {
        var targetsAll      = CharacterManager.Instance.GetEnemies(team);
        var targetsLive     = targetsAll?.Where(x => x.HP > 0).ToList();
        var targetsClose    = targetsLive?.OrderBy(x => node.Distance(x.Node)).ToList();

        return targetsClose?.FirstOrDefault();
    }

    public AxieBase FindCloseTarget()
    {
        if (node == null)
            return null;
        var random  = new System.Random();
        var targets = new List<AxieBase>();

        if (node.up != null)
            targets.AddRange(node.up.Characters);
        if (node.down != null)
            targets.AddRange(node.down.Characters);
        if (node.right != null)
            targets.AddRange(node.right.Characters);
        if (node.left != null)
            targets.AddRange(node.left.Characters);

        return targets.Where(x => x.Team != Team)
                      .OrderBy(x => random.Next())
                      .FirstOrDefault();
    }

    #endregion
}
