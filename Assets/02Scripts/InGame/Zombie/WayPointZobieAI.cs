using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class WayPointZobieAI : MonoBehaviour
{
    public NavMeshAgent NavAgent;
     public enum ZobieState
    {
        Patrol,
        Chase,  // 추적
        Attack,
        Dead
    }
    public ZobieState CurrentState = ZobieState.Patrol;
    public Transform Player;
    public Animator ZombieAnimator;
    public float ChaseDistance = 10;
    public float AttackDistnace = 2f;
    public float AttackCooldown = 2f;
    public float AttackeDelay = 1.5f;
    public int Damage = 10;
    public int Health = 100;
    public CapsuleCollider ZombieCapsuleCollider;
    private bool m_isMoving = false;
    private bool m_isAttacking;
    private float m_lastAttackTime;

    public GameObject BloodScreenEffect;
    private GameObject m_instantiateObject;

    private void Awake()
    {
        GameObject _playerObj = GameObject.FindWithTag("Player");
        if (_playerObj != null) Player = _playerObj.transform;
        else Debug.Log("Player obj not found!");

        NavAgent = GetComponent<NavMeshAgent>();
        ZombieCapsuleCollider = GetComponent<CapsuleCollider>();
        ZombieAnimator = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        m_lastAttackTime = -AttackCooldown;
    }

    private void Update()
    {
        SwitchState();
    }

    private void SwitchState()
    {
        switch (CurrentState)
        {
            case ZobieState.Patrol:
                if (!m_isMoving || NavAgent.remainingDistance < 0.1f)
                {
                    Patrol();
                }
                if (IsPlayerInRange(ChaseDistance))
                {
                    CurrentState = ZobieState.Chase;
                }
                break;
            case ZobieState.Chase:
                ChasePlayer();
                
                if (IsPlayerInRange(AttackDistnace))
                {
                    CurrentState = ZobieState.Attack;
                }
                break;
            case ZobieState.Attack:
                AttackPlayer();
                
                if (!IsPlayerInRange(AttackDistnace))
                {
                    CurrentState = ZobieState.Chase;
                }

                break;
            case ZobieState.Dead:
                // Set Dead Values
                NavAgent.enabled = false;
                ZombieCapsuleCollider.enabled = false;
                enabled = false;
                //increase score
                GameManager.Instance.CurrentScore += 1;
                break;
        }
    }

    // 
    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, Player.position) <= range;
    }

    private void Patrol()
    {
        NavAgent.speed = 0.3f;
        Vector3 _randomPosition = RandomNavMeshPosition();

        ZombieAnimator.SetBool("IsChasing", false);
        ZombieAnimator.SetBool("IsAttacking", false);

        NavAgent.SetDestination(_randomPosition);
        m_isMoving = true;

    }
    private Vector3 RandomNavMeshPosition()
    {
        // 무작위 3D 방향 생성
        Vector3 _randomDirection = Random.insideUnitSphere * 10f;
        _randomDirection += transform.position;
        NavMeshHit _hit;
        NavMesh.SamplePosition(_randomDirection, out _hit, 10f, NavMesh.AllAreas); // 위치 만들기
        return _hit.position;
    }

    private void ChasePlayer()
    {
        NavAgent.speed = 3f;

        ZombieAnimator.SetBool("IsChasing", true);
        ZombieAnimator.SetBool("IsAttacking", false);

        NavAgent.SetDestination(Player.position);
    }

    private void AttackPlayer()
    {
        NavAgent.SetDestination(transform.position);
        if(!m_isAttacking && Time.time - m_lastAttackTime >= AttackCooldown)
        {
            StartCoroutine(AttackWithDelay());
            StartCoroutine(ActivateBloodScreenEffect());
            m_lastAttackTime = Time.time;

            ZombieAnimator.SetBool("IsChasing", false);
            ZombieAnimator.SetBool("IsAttacking", true);

            PlayerController _playerController = Player.GetComponent<PlayerController>();
            if (_playerController != null)
            {
                _playerController.TakeDamage(Damage);
            }
        }
    }

    private IEnumerator AttackWithDelay()
    {
        m_isAttacking = true;
        yield return new WaitForSeconds(AttackeDelay);
        m_isAttacking = false;
    }

    private IEnumerator ActivateBloodScreenEffect()
    {
        InstantiateObject();
        yield return new WaitForSeconds(AttackeDelay);
        DeleteObject();
    }

    public void TakeDamage(int damageAmount)
    {
        if (CurrentState == ZobieState.Dead) return;

        Health -= damageAmount;

        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }

    private void Die()
    {
        ZombieAnimator.SetBool("IsChasing", false);
        ZombieAnimator.SetBool("IsAttacking", false);
        ZombieAnimator.SetBool("IsDead", true);

        Debug.Log("Player has Died");
        CurrentState = ZobieState.Dead;
    }

    private void InstantiateObject()
    {
        m_instantiateObject = Instantiate(BloodScreenEffect);
    }

    private void DeleteObject()
    {
        if (m_instantiateObject != null)
        {
            Destroy(m_instantiateObject);
            m_instantiateObject = null;
        }
    }
}
