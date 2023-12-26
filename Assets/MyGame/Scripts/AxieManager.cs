using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AxieManager : SingletonMono<AxieManager>
{
    #region ===== Fields =====

    private readonly List<AxieBase>  attackers       = new List<AxieBase>();
    private readonly List<AxieBase>  defenders       = new List<AxieBase>();
    private readonly Queue<AxieBase> queueDefender   = new Queue<AxieBase>();
    private readonly Queue<AxieBase> queueAttacker   = new Queue<AxieBase>();

    [SerializeField]
    private AxieAttacker    prefabAttacker  = null;
    [SerializeField]
    private AxieDefender    prefabDefender  = null;
    [SerializeField]
    private GameObject      rootPrefabs     = null;
    private GameObject      rootAttacker    = null;
    private GameObject      rootDefender    = null;
    private int             numberAttacker  = 30;
    private int             numberDefender  = 20;
    private float           durationTurn    = 2f;
    private float           durationMove    = 0f;
    private float           durationAttack  = 0f;
    private int             maxHPAttacker   = 0;
    private int             hpAttacker      = 0;
    private int             maxHPDefender   = 0;
    private int             hpDefender      = 0;
    private bool            endGame         = true;
    #endregion

    #region ===== Properties =====

    public List<AxieBase> Defenders => defenders;
    public List<AxieBase> Attackers => attackers;

    #endregion

    #region ===== Singletons =====

    private GameManager GameManager => GameManager.Instance;
    private UIManager   UIManager   => UIManager.Instance;
    private MapManager  MapManager  => MapManager.Instance;

    #endregion

    #region ===== Methods =====

    public override void Init()
    {
        Reset();
    }
    
    public void Reset()
    {
        var random = new System.Random();
        var numbers = Enumerable.Range(0, MapManager.Count()).OrderBy(x => random.Next());
        var numberQ = new Queue<int>(numbers);

        if (rootAttacker == null)
        {
            rootAttacker = new GameObject("Red");
            rootAttacker.transform.SetParent(transform);
            rootAttacker.transform.localScale = Vector3.one;
            rootAttacker.transform.localPosition = Vector3.zero;
            rootAttacker.transform.localRotation = Quaternion.identity;
        }
        if (rootDefender == null)
        {
            rootDefender = new GameObject("Blue");
            rootDefender.transform.SetParent(transform);
            rootDefender.transform.localScale = Vector3.one;
            rootDefender.transform.localPosition = Vector3.zero;
            rootDefender.transform.localRotation = Quaternion.identity;
        }
        // Defender
        {
            for (int i = 0; i < numberDefender; i++)
            {
                var nodeID = numberQ.Dequeue();
                var node = MapManager.GetNode(nodeID);
                var data = GameManager.Instance.DataDefender;
                var go = queueDefender.Count > 0 ? queueDefender.Dequeue() : Instantiate(prefabDefender);
                go.transform.SetParent(rootDefender.transform);
                var axie = go.GetComponent<AxieDefender>();
                axie.Init(data.AxieID, data.Genes, data.HP, node);
                defenders.Add(axie);
            }
        }
        // Attacker
        {
            for (int i = 0; i < numberAttacker; i++)
            {
                var nodeID = numberQ.Dequeue();
                var node = MapManager.GetNode(nodeID);
                var data = GameManager.Instance.DataAttacker;
                var go = queueAttacker.Count > 0 ? queueAttacker.Dequeue() : Instantiate(prefabAttacker);
                go.transform.SetParent(rootAttacker.transform);
                var axie = go.GetComponent<AxieAttacker>();
                axie.Init(data.AxieID, data.Genes, data.HP, node);
                attackers.Add(axie);
            }
        }

        hpDefender      = maxHPDefender = defenders.Sum(x => x.HP);
        hpAttacker      = maxHPAttacker = attackers.Sum(x => x.HP);
        durationMove    = durationTurn / 2;
        durationAttack  = durationTurn / 2;
        endGame         = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (endGame)
            return;

        if (durationMove > 0)
        {
            durationMove -= Time.deltaTime;
            if (durationMove > 0)
                return;

            foreach (var attacker in attackers)
            {
                attacker.DealDamage();
            }
            foreach (var defender in defenders)
            {
                defender.DealDamage();
            }
            return;
        }

        if (durationAttack > 0)
        {
            durationAttack -= Time.deltaTime;
            if (durationAttack > 0)
                return;

            foreach (var attacker in attackers)
            {
                attacker.CheckHealth();
            }
            foreach (var defender in defenders)
            {
                defender.CheckHealth();
            }
            return;
        }

        // Start Turn
        if (hpAttacker == 0 && hpDefender == 0)
        {
            // Draw
            endGame = true;
            StartCoroutine(ShowResult(ResultType.Draw));
        }
        else if (hpAttacker == 0)
        {
            // Defender Win
            endGame = true;
            StartCoroutine(ShowResult(ResultType.WinDefender));
        }
        else if (hpDefender == 0)
        {
            // Attacker Win
            endGame = true;
            StartCoroutine(ShowResult(ResultType.WinAttacker));
        }
        else
        {
            durationMove = durationTurn / 2;
            durationAttack = durationTurn / 2;
            foreach (var attacker in attackers)
            {
                attacker.MoveToTarget();
            }
            return;
        }
    }

    public void UpdateTeamHP(Team team, int damage)
    {
        switch (team)
        {
            case Team.Attacker:
                {
                    hpAttacker -= damage;
                    hpAttacker = Math.Max(hpAttacker, 0);
                    UIManager.UpdateHPBar(team: team,
                                          value: 1f * hpAttacker / maxHPAttacker,
                                          text: $"{hpAttacker} / {maxHPAttacker}");
                }
                break;
            case Team.Defender:
                {
                    hpDefender -= damage;
                    hpDefender = Math.Max(hpDefender, 0);
                    UIManager.UpdateHPBar(team: team,
                                          value: 1f * hpDefender / maxHPDefender,
                                          text: $"{hpDefender} / {maxHPDefender}");
                }
                break;
        }
    }

    public List<AxieBase> GetEnemies(Team team)
    {
        return team switch
        {
            Team.Attacker => defenders,
            Team.Defender => attackers,
            _ => null
        };
    }

    private IEnumerator ShowResult(ResultType resultType)
    {
        // Axie Pool
        foreach (var defender in defenders)
        {
            defender.transform.SetParent(rootPrefabs.transform);
            queueDefender.Enqueue(defender);
        }
        defenders.Clear();
        foreach (var attacker in attackers)
        {
            attacker.transform.SetParent(rootPrefabs.transform);
            queueAttacker.Enqueue(attacker);
        }
        attackers.Clear();
        // Result
        UIManager.ShowResult(resultType);
        yield return new WaitForSeconds(3);
        UIManager.HideResult();
        GameManager.Reset();
    }

    #endregion
}
