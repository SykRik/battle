using System.Collections.Generic;
using UnityEngine;

public class Node
{
    #region ===== Fields =====

    private readonly List<AxieBase> characters = new List<AxieBase>();
    private int id = -1;
    private int row = 0;
    private int col = 0;
    private Vector3 pos = Vector3.zero;
    private Node up = null;
    private Node down = null;
    private Node left = null;
    private Node right = null;

    #endregion

    #region ===== Properties =====

    public List<AxieBase> Characters => characters;
    public int      Row     => row;
    public int      Col     => col;
    public Vector3  Pos     => pos;
    public Node     Up      => up;
    public Node     Down    => down;
    public Node     Left    => left;
    public Node     Right   => right;

    #endregion

    #region ===== Singletons =====

    private MapManager MapManager => MapManager.Instance;


    #endregion

    #region ===== Methods =====

    public Node(int id, int row, int col, Vector3 pos)
    {
        this.id = id;
        this.row = row;
        this.col = col;
        this.pos = pos;
    }

    public void Connect(int width)
    {
        var links = MapManager.GetLinks(id);
        up = links.Item1;
        down = links.Item2;
        right = links.Item3;
        left = links.Item4;
    }

    public float Distance(Node target)
    {
        return new Vector2(target.col - col, target.row - row).magnitude;
    }

    public bool IsVacant()
    {
        return characters.Count == 0;
    }

    public bool Leave(AxieBase character)
    {
        if (character == null)
            return false;
        if (!characters.Contains(character))
            return false;
        characters.Remove(character);
        return true;
    }

    public bool Enter(AxieBase character)
    {
        if (character == null)
            return false;
        if (characters.Count > 0)
            return false;
        characters.Add(character);
        return true;
    }

    #endregion
}
