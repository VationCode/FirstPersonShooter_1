using UnityEngine;

public class ShootingController : MonoBehaviour
{
    public Animator PlayerAnimator;
    [Tooltip("Cam")]
    public Transform FirePoint;
    public float FireRange = 25f;
    public float FireRate = 10f;
    private float m_nextFireTime = 0f;

    public bool IsAuto = false;

    public int MaxAmmo = 30;
    private int m_currentAmmo;
    public float reloadTime = 1.5f;
    private bool m_isReloading = false;

    public ParticleSystem MuzzleFlash;
    private void Start()
    {
        m_currentAmmo = MaxAmmo;
    }
    private void Update()
    {
        if (m_isReloading) return;
        Shoot();    //�ڵ�
        if (Input.GetKeyDown(KeyCode.R) && m_currentAmmo < MaxAmmo) Reload();
    }

    private void Shoot()
    {
        bool _getFire = false;
        if (IsAuto) _getFire = Input.GetButton("Fire1");        // �ڵ� �߻� ���
        else _getFire = Input.GetButtonDown("Fire1");   // ���� �߻� ���

        if (_getFire && Time.time >= m_nextFireTime)
        {
            // Time.time�� m_nextFireTime���� �߻�x 5�ʿ� �߻� m_nextFireTime =5.1f�̱⿡ 0.1f�ʵ� ����
            m_nextFireTime = Time.time + 1f / FireRate;
            Debug.Log("Fire");
            if (m_currentAmmo > 0)
            {
                CheckShootTarget();
                MuzzleFlash.Play();
                PlayerAnimator.SetBool("Shoot", true);
                m_currentAmmo--;
            }
            else // �Ѿ� ���� �� �ڵ� ������
            {
                //Reload
                Reload();
            }
        }
        else
        {
            PlayerAnimator.SetBool("Shoot", false);

        }
    }

    private void CheckShootTarget()
    {
        RaycastHit _hit;
        if (Physics.Raycast(FirePoint.position, FirePoint.forward, out _hit, FireRange))
        {
            Debug.Log(_hit.transform.name);

            //apply damage zombie
        }
    }

    private void Reload()
    {
        if(!m_isReloading && m_currentAmmo < MaxAmmo)
        {
            //reload anim
            PlayerAnimator.SetTrigger("Reload");
            m_isReloading = true;
            Invoke("FinishReloading", reloadTime); // reloadTime : �ִϸ��̼� �ð��� �°�
        }
    }

    private void FinishReloading()
    {
        m_currentAmmo = MaxAmmo;
        m_isReloading = false;
        PlayerAnimator.ResetTrigger("Reload");
    }
}
