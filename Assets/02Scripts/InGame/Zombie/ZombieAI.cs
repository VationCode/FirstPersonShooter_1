using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum ZombieType
{
    Stand,
    Bite,
    Patrol
}

public class ZombieAI : MonoBehaviour
{
    public NavMeshAgent NavAgent;
    public enum ZobieState
    {
        Idle,
        Chase,  // 추적
        Attack,
        Dead
    }
    public ZombieType ThisZombieType;
    public ZobieState CurrentState = ZobieState.Idle;
    public Transform Player;
    public Animator ZombieAnimator;
    public float StandChaseSpeed = 0.5f;
    public float RunningCrawlSpeed = 3.0f;
    public float ChaseDistance = 10;
    public float AttackDistnace = 2f;
    public float AttackCooldown = 2f;
    public float AttackeDelay = 2.4f;
    public int Damage = 10;
    public int Health = 100;
    public CapsuleCollider ZombieCapsuleCollider;
    private bool m_isAttacking;
    private float m_lastAttackTime;

    public GameObject BloodScreenEffect;
    
    private GameObject m_instantiateObject;
    private float m_currentSpeed;

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
        m_currentSpeed = StandChaseSpeed;
    }

    private void Update()
    {
        FSMState();
    }

    private void FSMState() 
    {
        switch (CurrentState)
        {
            case ZobieState.Idle:
                // Animations
                ZombieAnimator.SetBool("IsWalking", false);
                ZombieAnimator.SetBool("IsAttacking", false);
                // Checked Chase Distance
                if (Vector3.Distance(transform.position, Player.position) <= ChaseDistance)
                    CurrentState = ZobieState.Chase;
                break;
            case ZobieState.Chase:
                // Animations
                ZombieAnimator.SetBool("IsWalking", true);
                ZombieAnimator.SetBool("IsAttacking", false);

                // Chase
                m_currentSpeed = ThisZombieType == ZombieType.Bite ? RunningCrawlSpeed : StandChaseSpeed;
                NavAgent.speed = m_currentSpeed;
                NavAgent.SetDestination(Player.position);
                if(Vector3.Distance(transform.position, Player.position) <= AttackDistnace)
                    CurrentState = ZobieState.Attack;
                break;
            case ZobieState.Attack:
                // Animations
                ZombieAnimator.SetBool("IsWalking", false);
                ZombieAnimator.SetBool("IsAttacking", true);

                // Stop Chase
                NavAgent.SetDestination(transform.position); // 목적지를 현재 위치로 설정
                
                // Attack
                if(!m_isAttacking && Time.time - m_lastAttackTime >= AttackCooldown)    //코루틴에서 m_lastAttackTime을 Time.time으로 초기화
                {
                    // Attack
                    StartCoroutine(AttackWithDelayCoroutine());

                    // blood Screen Effect
                    StartCoroutine(ActivateBloodScreenEffect());
                }
                if (Vector3.Distance(transform.position, Player.position) > AttackDistnace)
                    CurrentState = ZobieState.Chase;
                break;
            case ZobieState.Dead:
                // Dead상태는 슈팅컨트롤러에서 TakeDamage함수로 변환
                // Animations
                ZombieAnimator.SetBool("IsWalking", false);
                ZombieAnimator.SetBool("IsAttacking", false);
                ZombieAnimator.SetBool("IsDead", true);

                // Set Dead Values
                NavAgent.enabled = false;
                ZombieCapsuleCollider.enabled = false;
                enabled = false;
                //increase score
                GameManager.Instance.CurrentScore += 1;
                break;
        }
    }

    private IEnumerator AttackWithDelayCoroutine()
    {
        m_isAttacking = true;

        // damage the player TODO : 인터페이스로 변경하기
        PlayerController _playerController = Player.GetComponent<PlayerController>();
        _playerController.TakeDamage(Damage);

        yield return new WaitForSeconds(AttackeDelay);
        m_isAttacking = false;
        m_lastAttackTime = Time.time;
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
        CurrentState = ZobieState.Dead;
    }

    private void InstantiateObject()
    {
        m_instantiateObject = Instantiate(BloodScreenEffect);
    }

    private void DeleteObject()
    {
        if(m_instantiateObject != null)
        {
            Destroy(m_instantiateObject);
            m_instantiateObject = null;
        }
    }
}
