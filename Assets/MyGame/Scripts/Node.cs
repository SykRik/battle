using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private MapManager MapManager => MapManager.Instance;

    public int      id      = -1;
    public int      row     = 0;
    public int      col     = 0;
    public Vector3  pos     = Vector3.zero;
    public Node     up      = null;
    public Node     down    = null;
    public Node     left    = null;
    public Node     right   = null;
    public List<Character> characters = new List<Character>();

    public Node(int id, int row, int col, Vector3 pos)
    {
        this.id = id;
        this.row = row;
        this.col = col;
        this.pos = pos;
    }

    public void Connect(int width)
    {
        var links   = MapManager.GetLinks(id);
        up          = links.Item1;
        down        = links.Item2;
        right       = links.Item3;
        left        = links.Item4;
    }
}
