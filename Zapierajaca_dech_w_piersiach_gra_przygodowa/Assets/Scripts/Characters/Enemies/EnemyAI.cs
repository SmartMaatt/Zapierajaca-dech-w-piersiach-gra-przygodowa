
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AbstractCharacter))]
public class EnemyAI : MonoBehaviour {

    private Animator _animator;
    private AbstractCharacter _characterController;

    private float _currentMaxSpeed;
    private float _currentMoveSpeed;
    private bool _reachedSpeedPoint;
    const float _standStill = 0.0f;

    [Header("References")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Transform player;
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
        public float minTimeAttackStartDelay;
        public float maxTimeAttackStartDelay;
        bool alreadyAttacked, playerInAttackRange, isChasing, isAttacking;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        player = FindObjectsOfType<RelativeMovement>()[0].transform;
        agent = GetComponent<NavMeshAgent>();
        _characterController = GetComponent<AbstractCharacter>();

        _currentMoveSpeed = _standStill;
        _currentMaxSpeed = _standStill;
        _reachedSpeedPoint = false;
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
        speedAcceleration();
    }


    private void Patroling()
    {
        if (!walkPointSet && !enemyReadyToPatrol) { SearchWalkPoint(); }
        else if(walkPointSet && !enemyReadyToPatrol) { _currentMaxSpeed = _standStill; }
        else if(walkPointSet && enemyReadyToPatrol) { goToPoint(); }

        Vector3 distanceToWalkPoint = transform.position - _walkPoint;

        if (distanceToWalkPoint.magnitude < 1f || isChasing) {
            walkPointSet = false; enemyReadyToPatrol = false;
        }

        _animator.SetBool("Attack", false);
        _characterController.setStateMachine(0,0,0.0f);
        isChasing = false;
    }


    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        RaycastHit hit;
        Vector3 _tmpWalkPoint = new Vector3(transform.position.x + randomX, transform.position.y + 500f, transform.position.z + randomZ);

        if (Physics.Raycast(_tmpWalkPoint, -transform.up, out hit, Mathf.Infinity, SolidGround) && Vector2.Angle(new Vector2(_tmpWalkPoint.x, _tmpWalkPoint.z), new Vector2(transform.position.x, transform.position.z)) < 120) {

            _walkPoint = hit.transform.position;
            Debug.Log(_walkPoint);
            walkPointSet = true;
            rotateEnemyToPoint(_walkPoint);

            float timeToRest = UnityEngine.Random.Range(minRestTime, maxRestTime);
            _characterController.setStateMachine(0, 1, timeToRest);
            StartCoroutine(enemyWalkingPause(timeToRest));
        } 
    }


    private void goToPoint()
    {
        agent.SetDestination(_walkPoint);
        Debug.Log(_walkPoint);
        _currentMaxSpeed = _characterController.getWalkSpeed();
    }

    /*************************************************/

    private void ChasePlayer()
    {
        _walkPoint = player.position;
        agent.SetDestination(_walkPoint);

        isChasing = true;

        _characterController.setStateMachine(1, 0, 0);
        _currentMaxSpeed = _characterController.getRunSpeed();
        _animator.SetBool("Attack", false);
    }


    /*************************************************/


    private void AttackPlayer()
    {
        _walkPoint = player.position;
        agent.SetDestination(_walkPoint);

        enemyReadyToPatrol = false;
        isChasing = false;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        _currentMaxSpeed = _standStill;
        _animator.SetBool("Attack", false);
        _characterController.setStateMachine(2, 0, 0);

        if (!alreadyAttacked)
        {
            StartCoroutine(AttackDesorientation());
            alreadyAttacked = true;
            StartCoroutine(resetAttack(timeBetweenAttacks));
        }
    }

    private IEnumerator AttackDesorientation()
    {
        float DesorientationTime = UnityEngine.Random.Range(minTimeAttackStartDelay, maxTimeAttackStartDelay);
        yield return new WaitForSeconds(DesorientationTime);
        _animator.SetBool("Attack", true);
        _characterController.attack();
    }

    /*************************************************/

    private void rotateEnemyToPoint(Vector3 pointToRotate)
    {
        Vector3 lookrotation = pointToRotate - transform.position;
        if (lookrotation != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), 20f * Time.deltaTime);
    }

    private void speedAcceleration()
    {
        if (_currentMoveSpeed != _currentMaxSpeed)
            _reachedSpeedPoint = false;

        if (!_reachedSpeedPoint)
        {
            if (Math.Round(_currentMoveSpeed,1) == _currentMaxSpeed)
            {
                _currentMoveSpeed = _currentMaxSpeed;
                _reachedSpeedPoint = true;
            }
            else if (_currentMoveSpeed < _currentMaxSpeed)
            {
                _currentMoveSpeed += Time.deltaTime * _characterController.getAcceleration(isChasing);
            }
            else if (_currentMoveSpeed > _currentMaxSpeed)
            {
                _currentMoveSpeed -= Time.deltaTime * _characterController.getAcceleration(isChasing);
            }
        }

        //Debug.Log("Speed: " + _currentMoveSpeed);
        _animator.SetFloat("Speed", _currentMoveSpeed);
        agent.speed = _currentMoveSpeed;
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
                        _currentMaxSpeed = 0.0f;
                        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                    }
                }
                else if(hitObject.GetComponent<RelativeMovement>() != null)
                {
                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                }
            }
        }
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

                    if (hitObject.GetComponent<AbstractCharacter>())
                    {
                        return true;
                    }
                    else if (playerInHearRange)
                    {
                        _characterController.setStateMachine(2, 1, 0.0f);
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

    public GameObject getPlayerObject()
    {
        return player.gameObject;
    }

    public void setAlreadyAttack(bool state)
    {
        alreadyAttacked = state;
        StartCoroutine(resetAttack(timeBetweenAttacks));
    }

    public IEnumerator resetAttack(float time)
    {
        yield return new WaitForSeconds(time);
        alreadyAttacked = false;
    }

    private IEnumerator enemyWalkingPause(float timeOfRest)
    {
        yield return new WaitForSeconds(timeOfRest);
        enemyReadyToPatrol = true;
    }
}
