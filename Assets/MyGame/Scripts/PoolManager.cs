using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonMono<PoolManager>
{
    private readonly Dictionary<GameObject, List<GameObject>> poolDictionary = new Dictionary<GameObject, List<GameObject>>();

    public override void Init()
    {
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (poolDictionary.ContainsKey(prefab))
        {
            List<GameObject> pool = poolDictionary[prefab];
            foreach (GameObject obj in pool)
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }
        }

        // If no inactive object found in the pool, create a new one
        GameObject newObj = Instantiate(prefab);
        newObj.SetActive(true);

        // Add the new object to the pool dictionary
        if (poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab].Add(newObj);
        }
        else
        {
            poolDictionary.Add(prefab, new List<GameObject> { newObj });
        }

        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
