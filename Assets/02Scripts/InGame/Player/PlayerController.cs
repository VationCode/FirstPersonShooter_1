using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private CharacterController m_characterController;

    [Header("Player Health & Damage")]
    private int m_maxHealth = 100;
    public int CurrentHealth;
    public Image HealthFillImage;
    public Image StaminaFillImage;

    public DeathScreen DeathScreenUI;

    [Header("Movement & Gravity")]
    public float MoveSpeed = 5f;
    public float RotateSpeed = 20;
    public float JumpForce = 1.5f;
    public float Gravity = -9.81f;
    public Transform GroundCheckTr;
    public LayerMask GroundMask;
    public float GroundDistance = 0.4f;
    private bool m_isGrounded;
    private Vector3 m_velocity;

    [Header("Foot Seteps")]
    public AudioSource LeftFootAudioSource;
    public AudioSource RightFootAudioSource;
    public AudioClip[] FootstepSounds;
    public float FootStepInterval = 0.5f; // 발 사운드 간격
    private float m_nextFootstepTime;
    private bool m_isLeftFootStep = true;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        CurrentHealth = m_maxHealth;
        HealthFillImage.fillAmount = CurrentHealth/100;
        StaminaFillImage.fillAmount = 1;
    }
    private void Update()
    {
        GroundCheck();
        HandleMovement();
        HandleRtotate();

        HandleGravity();

        PlayFootstepSound();

        HandleJump();

        // 최종 y값에 대해 적용
        m_characterController.Move(m_velocity * Time.deltaTime);
    }

    private void HandleMovement()
    {
        if (m_isGrounded && m_velocity.y < 0)
        {
            m_velocity.y = -2f;
        }

        float _horizontal = Input.GetAxis("Horizontal");
        float _vertical = Input.GetAxis("Vertical");

        Vector3 _movement = transform.right * _horizontal + transform.forward * _vertical;
        _movement.y = 0;

        m_characterController.Move(_movement * Time.deltaTime * MoveSpeed);
    }
    private void HandleRtotate()
    {
        Quaternion _targetRotation = Camera.main.transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, RotateSpeed * Time.deltaTime);
    }
    private void GroundCheck()
    {
        m_isGrounded = Physics.CheckSphere(GroundCheckTr.position, GroundDistance, GroundMask);
    }

    private void HandleGravity()
    {
        m_velocity.y += Gravity * Time.deltaTime;
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && m_isGrounded)
        {
            m_velocity.y = Mathf.Sqrt(JumpForce * -2f * Gravity);
        }
    }

    private void PlayFootstepSound()
    {
        if (!m_isGrounded) return;
        if (m_characterController.velocity.magnitude < 0.1f) return;
        if (Time.time < m_nextFootstepTime) return;

        AudioClip _footstepClip = FootstepSounds[Random.Range(0, FootstepSounds.Length)];
        if (m_isLeftFootStep)
        {
            LeftFootAudioSource.PlayOneShot(_footstepClip);
        }
        else
        {
            RightFootAudioSource.PlayOneShot(_footstepClip);
        }

        m_isLeftFootStep = !m_isLeftFootStep;

        m_nextFootstepTime = Time.time + FootStepInterval;
    }

    // TODO : 인터페이스로 전환
    public void TakeDamage(int damageAmount)
    {
        CurrentHealth -= damageAmount;
        HealthFillImage.fillAmount = (float)CurrentHealth / 100;

        if (CurrentHealth <= 0)
        {
            HealthFillImage.fillAmount = 0;
            CurrentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        DeathScreenUI.IsShowDeadScreen = true;
        Debug.Log("Player has Died");
    }
}
