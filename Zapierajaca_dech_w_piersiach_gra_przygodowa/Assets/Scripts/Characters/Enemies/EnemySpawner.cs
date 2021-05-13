using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private LayerMask SolidGround;
    public int maxActiveMods;
    public int rangeX;
    public int rangeZ;
    List<GameObject> _enemyList = new List<GameObject>();
    bool _incorrectType;

    private void Start()
    {
        AbstractCharacter[] tmp = CheckType();
        if (tmp != null)
        {
            _incorrectType = false;
            foreach (var enemy in tmp)
            {
                _enemyList.Add(enemy.transform.gameObject);
            }
        }
        else
        {
            _incorrectType = true;
            Debug.LogError(enemyPrefab + "is not the child of AbstractCharacter!!!");
        }
    }

    void Update()
    {
        if (!_incorrectType)
        {
            if (_enemyList.Count < maxActiveMods)
            {
                Vector3 walkPoint = new Vector3(Random.Range(-rangeX, rangeX), 1, Random.Range(-rangeZ, rangeZ));

                if (Physics.Raycast(walkPoint, -transform.up, 2f, SolidGround))
                {
                    _enemyList.Add(Instantiate(enemyPrefab) as GameObject);
                    _enemyList[_enemyList.Count - 1].transform.position = walkPoint;
                    float angle = Random.Range(0, 360);
                    _enemyList[_enemyList.Count - 1].transform.Rotate(0, angle, 0);
                }
            }
            _enemyList = _enemyList.Where(x => x != null).ToList();
        }
    }

    AbstractCharacter[] CheckType()
    {
        if(enemyPrefab.GetComponent<ZombieDefault>() != null)
        {
            return FindObjectsOfType<ZombieDefault>();
        }
        else if(enemyPrefab.GetComponent<ZombieGirl>() != null)
        {
            return FindObjectsOfType<ZombieGirl>();
        }
        else
        {
            return null;
        }
    }
}
