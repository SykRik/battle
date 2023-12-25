using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterAttacker : Character
{
    public override Character FindTarget()
    {
        var characters = CharacterManager.Instance.defenders;
        var charactersLive = characters == null || characters.Count == 0 ? null : characters.Where(x => x.hp > 0).ToList();
        var charactersClose = charactersLive == null || charactersLive.Count == 0 ? null : charactersLive.OrderBy(x => Mathf.Abs(x.node.col - node.col) + Mathf.Abs(x.node.row - node.row)).ToList();
        var character = charactersClose == null || charactersClose.Count == 0 ? null : charactersClose.First();

        return character;
    }
    public override Character FindCloseTarget()
    {
        var characters = CharacterManager.Instance.defenders;
        var charactersLive = characters == null || characters.Count == 0 ? null : characters.Where(x => x.hp > 0).ToList();
        var charactersClose = charactersLive == null || charactersLive.Count == 0 ? null : charactersLive.Where(x => Mathf.Abs(x.node.col - node.col) + Mathf.Abs(x.node.row - node.row) == 1).ToList();
        var character = charactersClose == null || charactersClose.Count == 0 ? null : charactersClose.First();

        return character;
    }
}

public class CharacterDefender : Character
{
    public override Character FindTarget()
    {
        var characters = CharacterManager.Instance.attackers;
        var characterLive = characters == null || characters.Count == 0 ? null : characters.Where(x => x.GetComponent<Character>().hp > 0).ToList();
        var characterClose = characterLive == null || characterLive.Count == 0 ? null : characterLive.OrderBy(x => Mathf.Abs(x.node.col - node.col) + Mathf.Abs(x.node.row - node.row)).ToList();
        var character = characterClose == null || characterClose.Count == 0 ? null : characterClose.First();

        return character;
    }
    public override Character FindCloseTarget()
    {
        var characters = CharacterManager.Instance.attackers;
        var characterLive = characters == null || characters.Count == 0 ? null : characters.Where(x => x.GetComponent<Character>().hp > 0).ToList();
        var characterClose = characterLive == null || characterLive.Count == 0 ? null : characterLive.Where(x => Mathf.Abs(x.node.col - node.col) + Mathf.Abs(x.node.row - node.row) == 1).ToList();
        var character = characterClose == null || characterClose.Count == 0 ? null : characterClose.First();

        return character;
    }
}


public abstract class Character : MonoBehaviour
{
    public Node node = null;
    public int hp = 10;
    public bool alive = true;

    public void SetNode(Node node)
    {
        if (node == null)
            return;
        if (node.characters == null)
            return;
        if (node.characters.Count > 0)
            return;

        if (this.node != null && this.node.characters != null)
            this.node.characters.Remove(this);
        this.node = node;
        node.characters.Add(this);
        LeanTween.move(gameObject, node.pos, 0.5f);
    }

    public void DealDamage()
    {
        if (alive)
        {
            if (hp <= 0)
                return;
            var target = FindCloseTarget();
            if (target == null)
                return;
            target.TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (alive)
        {
            if (hp > 0)
                hp -= damage;
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
                node.characters.Remove(this);
            }
        }
    }

    public abstract Character FindTarget();
    public abstract Character FindCloseTarget();

    public void MoveToTarget()
    {
        if (alive)
        {
            var target = FindTarget();
            if (target == null)
                return;
            if (Mathf.Abs(target.node.col - node.col) + Mathf.Abs(target.node.row - node.row) == 1)
                return;
            if (target.node.col == node.col && target.node.row == node.row)
                return;

            if (target.node.row > node.row && node.up.characters.Count == 0)
            {
                MoveUp();
                return;
            }
            if (target.node.row < node.row && node.down.characters.Count == 0)
            {
                MoveDown();
                return;
            }
            if (target.node.col > node.col && node.right.characters.Count == 0)
            {
                MoveRight();
                return;
            }
            if (target.node.col < node.col && node.left.characters.Count == 0)
            {
                MoveLeft();
                return;
            }
            return;
        }
    }

    private void MoveUp()
    {
        SetNode(node.up);
    }

    private void MoveDown()
    {
        SetNode(node.down);
    }

    private void MoveRight()
    {
        SetNode(node.right);
    }

    private void MoveLeft()
    {
        SetNode(node.left);
    }
}

public class CharacterManager : SingletonMono<CharacterManager>
{
    #region ===== Fields =====

    [SerializeField]
    private GameObject prefabAttacker = null;
    [SerializeField]
    private GameObject prefabDefender = null;

    private GameObject rootAttacker = null;
    private GameObject rootDefender = null;
    public List<Character> attackers = new List<Character>();
    public List<Character> defenders = new List<Character>();
    //private Vector2 size = new Vector2(10, 10);
    private int numberAttacker = 13;
    private int numberDefender = 14;

    //private MapManager MapManager = MapManager.Instance;

    #endregion

    private void Start()
    {
        var mapManager = MapManager.Instance;

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
        var random = new System.Random();
        var numbers = Enumerable.Range(0, mapManager.Count()).ToList().OrderBy(x => random.Next());
        var numberQ = new Queue<int>(numbers);
        for (int i = 0; i < numberDefender; i++)
        {
            var nodeID = numberQ.Dequeue();
            var node = mapManager.GetNode(nodeID);
            var defender = Instantiate(prefabDefender, rootDefender.transform).AddComponent<CharacterDefender>();
            defender.SetNode(node);
            defenders.Add(defender);
        }
        for (int i = 0; i < numberAttacker; i++)
        {
            var nodeID = numberQ.Dequeue();
            var node = mapManager.GetNode(nodeID);
            var targetIndex = i % defenders.Count;
            var target = defenders[targetIndex].GetComponent<CharacterDefender>();
            var attacker = Instantiate(prefabAttacker, rootAttacker.transform).AddComponent<CharacterAttacker>();
            attacker.SetNode(node);
            attackers.Add(attacker);
        }
    }

    private float intervalTime = 1f;
    private float remaindTimeMove = 1f;
    private float remaindTimeAttack = 1f;

    // Update is called once per frame
    void Update()
    {
        if (remaindTimeMove <= 0 && remaindTimeAttack <= 0)
        {
            remaindTimeMove = intervalTime / 2;
            remaindTimeAttack = intervalTime / 2;
            foreach (var attacker in attackers)
            {
                attacker.MoveToTarget();
            }
        }
        else if (remaindTimeMove > 0)
        {
            remaindTimeMove -= Time.deltaTime;
            if (remaindTimeMove <= 0)
            {
                foreach (var attacker in attackers)
                {
                    attacker.DealDamage();
                }
                foreach (var defender in defenders)
                {
                    defender.DealDamage();
                }
            }
        }
        else
        {
            remaindTimeAttack -= Time.deltaTime;
            if (remaindTimeMove <= 0)
            {
                foreach (var attacker in attackers)
                {
                    attacker.CheckHealth();
                }
                foreach (var defender in defenders)
                {
                    defender.CheckHealth();
                }
            }
        }
    }

    protected override void Init()
    {
    }
}
