﻿using AxieMixer.Unity;
using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            transform.localPosition = node.Pos;
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
            var scaleX = node.Pos.x > 0 ? 1 : -1;
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
                var factor = (number - target.number + 3) % 3;
                var damage = factor switch
                {
                    0 => 4,
                    1 => 5,
                    2 => 3,
                    _ => throw new NotImplementedException()
                };
                LookToTarget(target);
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
                AxieManager.Instance.UpdateTeamHP(team, dmg);
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
                LookToTarget(target);
            }
            if (Mathf.Abs(target.node.Col - node.Col) + Mathf.Abs(target.node.Row - node.Row) == 1)
            {
                Idle();
                return;
            }
            if (target.node.Col == node.Col && target.node.Row == node.Row)
            {
                Idle();
                return;
            }
            if (target.node.Row > node.Row && node.Up.IsVacant())
            {
                Move(node.Up);
                return;
            }
            if (target.node.Row < node.Row && node.Down.IsVacant())
            {
                Move(node.Down);
                return;
            }
            if (target.node.Col > node.Col && node.Right.IsVacant())
            {
                Move(node.Right);
                return;
            }
            if (target.node.Col < node.Col && node.Left.IsVacant())
            {
                Move(node.Left);
                return;
            }
            // Default
            {
                Idle();
                return;
            }
        }
    }

    private void LookToTarget(AxieBase target)
    {
        if (target == null)
            return;

        if (target.Node.Col == node.Col)
        {
            var animation = GetComponentInChildren<SkeletonAnimation>();
            var scaleXabs = Mathf.Abs(animation.skeleton.ScaleX);
            var scaleX = target.Node.Row > node.Row ? -1 : 1;
            animation.skeleton.ScaleX = scaleX * scaleXabs;
        }
        else
        {
            var animation = GetComponentInChildren<SkeletonAnimation>();
            var scaleXabs = Mathf.Abs(animation.skeleton.ScaleX);
            var scaleX = target.Node.Col > node.Col ? -1 : 1;
            animation.skeleton.ScaleX = scaleX * scaleXabs;
        }
    }

    private void DoAnimation(AnimationEnum animationType)
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

    private void Move(Node targetNode)
    {
        if (targetNode == null)
            return;

        node?.Leave(this);
        node = targetNode;
        node?.Enter(this);
        // Play anim move
        DoAnimation(AnimationEnum.MOVE_FORWARD);
        LeanTween.moveLocal(gameObject, targetNode.Pos, 0.5f);
    }

    private void Idle()
    {
        // Play anim idle
        DoAnimation(AnimationEnum.IDLE_NORMAL);
    }

    public AxieBase FindTarget()
    {
        var targetsAll      = AxieManager.Instance.GetEnemies(team);
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

        if (node.Up != null)
            targets.AddRange(node.Up.Characters);
        if (node.Down != null)
            targets.AddRange(node.Down.Characters);
        if (node.Right != null)
            targets.AddRange(node.Right.Characters);
        if (node.Left != null)
            targets.AddRange(node.Left.Characters);

        return targets.Where(x => x.Team != Team)
                      .OrderBy(x => random.Next())
                      .FirstOrDefault();
    }

    #endregion
}
