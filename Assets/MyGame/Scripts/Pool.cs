using System;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T>
    where T : Component
{
    #region ===== Fields =====

    private readonly Queue<T> waitingItems = null;
    private readonly List<T> usingItems = null;
    private readonly Func<T> createItem = null;
    private readonly GameObject rootEnable = null;
    private readonly GameObject rootDisable = null;

    public Pool(GameObject root, Func<T> create)
    {
        waitingItems    = new Queue<T>();
        usingItems      = new List<T>();
        createItem      = create;

        rootEnable = new GameObject("Enable");
        rootEnable.SetActive(true);
        rootEnable.transform.SetParent(root.transform);

        rootDisable = new GameObject("Disable");
        rootDisable.SetActive(false);
        rootDisable.transform.SetParent(root.transform);
    }

    public T Request()
    {
        var item    = waitingItems.Count > 0
                    ? waitingItems.Dequeue()
                    : createItem();
        item.transform.SetParent(rootEnable.transform);
        usingItems.Add(item);
        return item;
    }

    public bool Return(T item)
    {
        if (!usingItems.Contains(item))
            return false;
        item.transform.SetParent(rootDisable.transform);
        usingItems.Remove(item);
        waitingItems.Enqueue(item);
        return true;
    }

    private T Create()
    {
        return createItem?.Invoke() ?? throw new ArgumentNullException();
    }

    #endregion

    #region ===== Methods =====

    #endregion
}
