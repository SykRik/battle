using System;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonMono<MapManager>
{
    //[SerializeField]
    //private SpriteRenderer map;
    [SerializeField]
    private int numberCOL = 0;
    [SerializeField]
    private int numberROW = 0;

    private Dictionary<int, Node> nodes = new Dictionary<int, Node>();

    public override void Init()
    {
        //map.size = new Vector2(numberCOL, numberROW);

        for (int row = 0; row < numberROW; row++)
        {
            for (int col = 0; col < numberCOL; col++)
            {
                var id      = nodes.Count;
                var x       = col - (numberCOL - 1) / 2f;
                var y       = row - (numberROW - 1) / 2f;
                var pos     = new Vector3(x, y);
                var node    = new Node(id, row, col, pos);
                nodes.Add(id, node);
            }
        }
        foreach (var node in nodes.Values)
        {
            node.Connect(numberCOL);
        }
    }

    public int Count()
    {
        return nodes.Count;
    }

    public Node GetNode(int id)
    {
        if (nodes.TryGetValue(id, out var node))
            return node;
        return null;
    }

    public Tuple<Node, Node, Node, Node> GetLinks(int id)
    {
        if (nodes.TryGetValue(id + numberCOL,   out var up))    { };
        if (nodes.TryGetValue(id - numberCOL,   out var down))  { };
        if (nodes.TryGetValue(id + 1,           out var right)) { };
        if (nodes.TryGetValue(id - 1,           out var left))  { };

        return Tuple.Create(up, down, right, left);
    }

    public void Reset()
    {
        foreach (var node in nodes.Values)
        {
            node.Characters.Clear();
        }
    }
}
