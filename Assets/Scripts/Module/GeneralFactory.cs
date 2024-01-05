using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneralFactory : MonoBehaviour
{
    // All the different obstacle prefabs
    [SerializeField] private List<GameObject> _prefabs = new();
    // Pools belonging to obstacles
    private Dictionary<int, Stack<GameObject>> _pool = new();

    // Instance
    private static GeneralFactory _instance;
    public static GeneralFactory Instance { get => _instance; }


    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    private void Awake()
    {
        // Singelton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        // Setting up dictonary
        for (int i = 0; i < 6; i++)
        {
            _pool.Add(i, new());
        }
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     * Spawns obstacle. Kind of obstacle is determined by index.
     */
    public GameObject Spawn(int idx)
    {
        var go = _pool[idx].Any() ? _pool[idx].Pop() : Instantiate(_prefabs[idx]);
        go.SetActive(true);
        return go;
    }

    /*
     * Returns obstacle to pool. Kind of obstacle is determined by name of obstacle.
     * Restting is not necessary, because postion etc. will be set by PopulateModule class anyway.
     */
    public void ReturnToPool(GameObject go)
    {
        go.SetActive(false);
        _pool[Int32.Parse(go.name.Split(' ')[0])].Push(go);
    }
}