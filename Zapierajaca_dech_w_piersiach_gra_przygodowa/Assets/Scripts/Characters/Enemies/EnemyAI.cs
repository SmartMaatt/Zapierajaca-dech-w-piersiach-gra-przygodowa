
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TargetHeadAim))]
[RequireComponent(typeof(AbstractCharacter))]
public class EnemyAI : MonoBehaviour {

    private Animator _animator;
    private TargetHeadAim _sightTargetManager;
    private AbstractCharacter _characterController;

    private float _currentMoveSpeed;
    const float _standStill = 0.0f;
    private float _health;

    [Header("References")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Transform player;
        public Vector3 headHeight;
        public LayerMask SolidGround, PlayerLayer;
        public float obstacleRange = 0.5f;

    [Header("Patroling")]
        public float walkPointRange;
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
        public float attackRange;
        bool alreadyAttacked, playerInAttackRange, isChasing, isAttacking;
        //public GameObject projectile;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _sightTargetManager = GetComponent<TargetHeadAim>();
        player = FindObjectsOfType<RelativeMovement>()[0].transform;
        agent = GetComponent<NavMeshAgent>();
        _characterController = GetComponent<AbstractCharacter>();

        _currentMoveSpeed = _standStill;
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
        else if(walkPointSet && enemyReadyToPatrol) { goToPoint(); }

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

        if (Physics.Raycast(_walkPoint, -transform.up, 2f, SolidGround) && Vector2.Angle(new Vector2(_walkPoint.x, _walkPoint.z), new Vector2(transform.position.x, transform.position.z)) < 120) {
            walkPointSet = true;
            _currentMoveSpeed = _standStill;
            rotateEnemyToPoint(_walkPoint);

            StartCoroutine(enemyWalkingPause(Random.Range(minRestTime, maxRestTime)));
        } 
    }


    private void goToPoint()
    {
        agent.SetDestination(_walkPoint);
        _sightTargetManager.changeWeight(0.0f, 0.0f);
    }


    /*************************************************/


    private void ChasePlayer()
    {
        _walkPoint = player.position;
        agent.SetDestination(_walkPoint);

        enemyReadyToPatrol = false;
        isChasing = true;
        _currentMoveSpeed = _characterController.getRunSpeed();
        _animator.SetBool("Attack", false);

        _sightTargetManager.changeCurrentTargetPos(new Vector3(player.position.x, player.position.y + 1.7f, player.position.z));
        _sightTargetManager.changeWeight(1.0f, 1.0f);
    }


    /*************************************************/


    private void AttackPlayer()
    {
        _walkPoint = player.position;
        agent.SetDestination(_walkPoint);

        enemyReadyToPatrol = false;
        isChasing = false;
        _currentMoveSpeed = _standStill;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        _animator.SetBool("Attack", true);

        _sightTargetManager.changeCurrentTargetPos(new Vector3(player.position.x, player.position.y + 1.7f, player.position.z));
        _sightTargetManager.changeWeight(1.0f, 1.0f);

        if (!alreadyAttacked)
        {
            _characterController.attack();

            alreadyAttacked = true;
            StartCoroutine(resetAttack());
        }
    }

    /*************************************************/

    private void rotateEnemyToPoint(Vector3 pointToRotate)
    {
        Vector3 lookrotation = pointToRotate - transform.position;
        if (lookrotation != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), 20f * Time.deltaTime);
    }

    private void checkWhatIsInFrontOf()
    {
        if (isChasing)
        {
            rotateEnemyToPoint(agent.steeringTarget);
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


    private IEnumerator resetAttack()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        alreadyAttacked = false;
    }

    private IEnumerator enemyWalkingPause(float timeOfRest)
    {
        Debug.Log(timeOfRest);
        float elapsedTime = 0;
        _sightTargetManager.changeWeight(1.0f, 1.0f);
        Vector3 startingPos = new Vector3(transform.position.x + headHeight.x, transform.position.y + headHeight.y, transform.position.z + headHeight.z) + transform.forward * 1.0f;

        int index = 0;
        float[] x = new float[] { 0.0f, -1.0f, 0.0f, 1.0f };
        float[] y = new float[] { -1.0f, 0.0f, 1.0f, 0.0f };

        while (elapsedTime < 4)
        {
            Debug.Log("Target orb state" + index + " " + elapsedTime);
            elapsedTime += Time.deltaTime/(timeOfRest/4);
            _sightTargetManager.changeCurrentTargetPos(startingPos + transform.right * Mathf.Lerp(x[index], y[index], elapsedTime%1));
            index = (int)(elapsedTime);
            yield return new WaitForEndOfFrame();
        }

        _currentMoveSpeed = _characterController.getWalkSpeed();
        enemyReadyToPatrol = true;
    }
}
