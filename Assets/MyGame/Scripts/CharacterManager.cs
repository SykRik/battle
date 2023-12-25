using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{ }

public class CharacterManager : MonoBehaviour
{
    #region ===== Fields =====

    [SerializeField]
    private GameObject prefabAttacker = null;
    [SerializeField]
    private GameObject prefabDefender = null;

    private GameObject rootAttacker = null;
    private GameObject rootDefender = null;
    private List<GameObject> attackers = new List<GameObject>();
    private List<GameObject> defenders = new List<GameObject>();
    //private Vector2 size = new Vector2(10, 10);
    private int numberAttacker = 13;
    private int numberDefender = 14;

    //private MapManager MapManager = MapManager.Instance;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
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
        var numbers = Enumerable.Range(0, MapManager.Instance.Count()).ToList().OrderBy(x => random.Next());
        var numberQ = new Queue<int>(numbers);
        for (int i = 0; i < numberAttacker; i++)
        {
            var nodeID = numberQ.Dequeue();
            var node = MapManager.Instance.GetNode(nodeID);
            var attacker = Instantiate(prefabAttacker, rootAttacker.transform);
            attacker.transform.localScale = Vector3.one;
            attacker.transform.localPosition = node.pos;
            attacker.transform.localRotation = Quaternion.identity;
            attackers.Add(attacker);
        }
        for (int i = 0; i < numberDefender; i++)
        {
            var nodeID = numberQ.Dequeue();
            var node = MapManager.Instance.GetNode(nodeID);
            var defender = Instantiate(prefabDefender, rootDefender.transform);
            defender.transform.localScale = Vector3.one;
            defender.transform.localPosition = node.pos;
            defender.transform.localRotation = Quaternion.identity;
            defenders.Add(defender);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
