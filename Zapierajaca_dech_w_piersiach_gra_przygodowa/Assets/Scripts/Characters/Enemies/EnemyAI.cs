
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {

    private Animator _animator;
    private float _currentMoveSpeed;
    private float _health;

    [Header("References")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Transform player;
        public LayerMask SolidGround, PlayerLayer;
 
  
    [Header("Patroling")]
        public float walkPointRange;
        public float patrolMoveSpeed;
        public float sightRange;
        Vector3 walkPoint;
        bool walkPointSet, enemyReady, playerInSightRange;

    [Header("Attacking")]
        public float timeBetweenAttacks;
        public float attackMoveSpeed;
        public float attackRange;
        bool alreadyAttacked, playerInAttackRange;
        //public GameObject projectile;

    private void Awake()
    {
        //TO_DO: Correct player finding
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        _animator = GetComponent<Animator>();
        _currentMoveSpeed = patrolMoveSpeed;
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, PlayerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, PlayerLayer);

        if (playerInAttackRange && playerInSightRange) { AttackPlayer(); }
        else if (playerInSightRange && !playerInAttackRange) { ChasePlayer(); }
        else if (!playerInSightRange && !playerInAttackRange) { Patroling(); }

        _animator.SetFloat("Speed", _currentMoveSpeed);
        agent.speed = _currentMoveSpeed;
        Debug.Log("Current speed: " + _currentMoveSpeed + " ,Animation speed: " + _animator.GetFloat("Speed"));
    }

    private void Patroling()
    {
        if (!walkPointSet && !enemyReady) { SearchWalkPoint(); }
        else if(walkPointSet && enemyReady) { agent.SetDestination(walkPoint); }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f) { walkPointSet = false; enemyReady = false; }
        _animator.SetBool("Attack", false);
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);


        if (Physics.Raycast(walkPoint, -transform.up, 2f, SolidGround)) {
            walkPointSet = true;
            _currentMoveSpeed = 0.0f;
            StartCoroutine(enemyWalkingPause());
        } 
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        _currentMoveSpeed = attackMoveSpeed;
        _animator.SetBool("Attack", false);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        //agent.SetDestination(player.position);
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

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

            Debug.Log("Attack!");
            _animator.SetBool("Attack", true);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private IEnumerator enemyWalkingPause()
    {
        float timeDelay = Random.Range(1.0f, 2.0f);
        Debug.Log(timeDelay);
        yield return new WaitForSeconds(timeDelay);
        _currentMoveSpeed = patrolMoveSpeed;
        enemyReady = true;
    }
}
