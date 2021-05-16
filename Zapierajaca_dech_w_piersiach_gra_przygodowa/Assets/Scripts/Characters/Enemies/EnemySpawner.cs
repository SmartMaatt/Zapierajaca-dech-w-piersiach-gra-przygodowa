using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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
                Debug.Log(enemy.transform.gameObject);
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
            Debug.Log(_enemyList.Count);
            if (_enemyList.Count < maxActiveMods)
            {
                Vector3 walkPoint = transform.position + new Vector3(Random.Range(-rangeX, rangeX), 0, Random.Range(-rangeZ, rangeZ));
                Debug.Log(walkPoint);

                RaycastHit hit;
                var ray = new Ray(walkPoint, -transform.up);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, SolidGround))
                {
                    Debug.Log("After raycast: " + hit.transform.position);
                    _enemyList.Add(Instantiate(enemyPrefab) as GameObject);
                    _enemyList[_enemyList.Count - 1].transform.position = new Vector3(walkPoint.x, hit.transform.position.y+1, walkPoint.z);
                    float angle = Random.Range(0, 360);
                    _enemyList[_enemyList.Count - 1].transform.Rotate(0, angle, 0);

                    _enemyList[_enemyList.Count - 1].gameObject.GetComponent<NavMeshAgent>().enabled = false;
                    _enemyList[_enemyList.Count - 1].gameObject.GetComponent<NavMeshAgent>().enabled = true;
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
