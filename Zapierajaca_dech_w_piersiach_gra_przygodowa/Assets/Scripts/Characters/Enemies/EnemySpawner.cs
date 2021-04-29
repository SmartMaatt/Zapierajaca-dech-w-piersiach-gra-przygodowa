using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    public int maxActiveMods;
    public int rangeX;
    public int rangeZ;
    List<GameObject> _enemyList = new List<GameObject>();

    private void Start()
    {
        foreach (var enemy in FindObjectsOfType<ZombieDefault>())
        {
           _enemyList.Add(enemy.transform.gameObject);
        }
    }

    void Update()
    {
        if (_enemyList.Count < maxActiveMods)
        {
            _enemyList.Add(Instantiate(enemyPrefab) as GameObject);
            _enemyList[_enemyList.Count-1].transform.position = new Vector3(Random.Range(-rangeX, rangeX), 1, Random.Range(-rangeZ, rangeZ));
            float angle = Random.Range(0, 360);
            _enemyList[_enemyList.Count - 1].transform.Rotate(0, angle, 0);
        }
        _enemyList = _enemyList.Where(x => x != null).ToList();
    }
}
