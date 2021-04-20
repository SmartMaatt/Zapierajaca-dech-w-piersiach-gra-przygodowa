
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour {

    private Animator _animator;
    private float _currentMoveSpeed;
    private float _health;

    [Header("References")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Transform player;
        public LayerMask SolidGround, PlayerLayer;
        public float obstacleRange = 0.5f;

    [Header("Patroling")]
        public float walkPointRange;
        public float patrolMoveSpeed;
        public float hearRange; 
        public float sightRange;
        [Range(10,180)]
        public float sightConeRange;
        public float maxRestTime;
        public float minRestTime;
        Vector3 _walkPoint;
        bool walkPointSet, enemyReadyToPatrol, playerInHearRange, playerInSightRange;

    [Header("Attacking")]
        public float timeBetweenAttacks;
        public float attackMoveSpeed;
        public float attackRange;
        bool alreadyAttacked, playerInAttackRange, isChasing;
        //public GameObject projectile;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _currentMoveSpeed = patrolMoveSpeed;

        player = FindObjectsOfType<RelativeMovement>()[0].transform;
        agent = GetComponent<NavMeshAgent>();

        _walkPoint = transform.position;
        walkPointSet = false;
        enemyReadyToPatrol = false;
        playerInHearRange = false;
        playerInSightRange = false;

        alreadyAttacked = false;
        playerInAttackRange = false;
        isChasing = false;
    }


    private void Update()
    {
        playerInSightRange = isPlayerSeen();
        playerInHearRange = Physics.CheckSphere(transform.position, hearRange, PlayerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, PlayerLayer);

        if (playerInAttackRange) { AttackPlayer(); }
        else if ((playerInHearRange && !playerInAttackRange) || (playerInSightRange)) { ChasePlayer(); }
        else { Patroling(); }

        checkWhatIsInFrontOf();
    }


    private void Patroling()
    {
        if (!walkPointSet && !enemyReadyToPatrol) { SearchWalkPoint(); }
        else if(walkPointSet && enemyReadyToPatrol) { agent.SetDestination(_walkPoint); }

        Vector3 distanceToWalkPoint = transform.position - _walkPoint;

        if (distanceToWalkPoint.magnitude < 1f) {
            walkPointSet = false; enemyReadyToPatrol = false;
        }
        _animator.SetBool("Attack", false);
        isChasing = false;
    }


    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        _walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(_walkPoint, -transform.up, 2f, SolidGround)) {
            walkPointSet = true;
            _currentMoveSpeed = 0.0f;
            StartCoroutine(enemyWalkingPause());
        } 
    }


    private void ChasePlayer()
    {
        _walkPoint = player.position;
        agent.SetDestination(_walkPoint);

        enemyReadyToPatrol = false;
        isChasing = true;
        _currentMoveSpeed = attackMoveSpeed;
        _animator.SetBool("Attack", false);
    }


    private void AttackPlayer()
    {
        //transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        _animator.SetBool("Attack", true);
        isChasing = true;

        if (!alreadyAttacked)
        {
            /*
            ///Attack code here
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            */
        }
    }


    /*
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }


    public void TakeDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    */


    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }


    private void checkWhatIsInFrontOf()
    {
        Vector3 lookrotation = agent.steeringTarget - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), 20f * Time.deltaTime);

        if (isChasing)
        {
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y+1.5f, transform.position.z), transform.forward);
            RaycastHit hit = new RaycastHit();

            if (Physics.SphereCast(ray, obstacleRange/2, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;
                if (hit.distance < obstacleRange)
                {
                    if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(player.position.x, player.position.z)) < obstacleRange * 4)
                    {
                        _currentMoveSpeed = 0.0f;
                        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                    }
                }
                else if(hitObject.GetComponent<RelativeMovement>() != null)
                { 
                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                }
            }
        }
        _animator.SetFloat("Speed", _currentMoveSpeed);
        agent.speed = _currentMoveSpeed;
    }

    private bool isPlayerSeen()
    {
        //[1] Check sight circle
        if (Physics.CheckSphere(transform.position, sightRange, PlayerLayer))
        {
            Vector3 playerToMonster = player.position - transform.position;
            Vector3 playerToMonsterSight = new Vector3(player.position.x, player.position.y + 1.5f, player.position.z) - new Vector3(transform.position.x, transform.position.y+1.5f, transform.position.z);
            float cosOfMaxSightAngle = Mathf.Cos(Mathf.Deg2Rad * sightConeRange / 2);
            float cosOfCurrentAngle = Vector3.Dot(playerToMonster, transform.forward) / (playerToMonster.magnitude * transform.forward.magnitude);

            //[2] Check sight angle
            if (cosOfCurrentAngle > cosOfMaxSightAngle)
            {
                Ray ray = new Ray(transform.position, playerToMonsterSight);
                RaycastHit hit;

                //[3] Check for obsticles with RayCast
                if (Physics.SphereCast(ray, 0.75f, out hit))
                {
                    GameObject hitObject = hit.transform.gameObject;
                    if (hitObject.GetComponent<RelativeMovement>() != null)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, hearRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.blue;
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), transform.forward);
        RaycastHit hit;

        if (Physics.SphereCast(ray, obstacleRange / 2, out hit))
        {
            Gizmos.DrawWireSphere(hit.transform.position, obstacleRange/2);
        }

        }


    private IEnumerator enemyWalkingPause()
    {
        float timeDelay = Random.Range(minRestTime, maxRestTime);
        yield return new WaitForSeconds(timeDelay);
        _currentMoveSpeed = patrolMoveSpeed;
        enemyReadyToPatrol = true;
    }
}
