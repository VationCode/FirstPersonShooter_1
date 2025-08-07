using TMPro;
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

    [Header("Effect")]
    //public ParticleSystem MuzzleFlash;
    public Gun[] GunMuzzleFlashs;
    public ParticleSystem BloodEffect;
    public int DamagePerShot = 10;


    [Header("Sound")]
    public AudioSource SoundAudioSource;
    public AudioClip ShootingSoundClip;
    public AudioClip ReloadSoundClip;

    public TextMeshProUGUI CurrentAmmoTMP;
    private void Start()
    {
        m_currentAmmo = MaxAmmo;
        CurrentAmmoTMP.text = m_currentAmmo.ToString() + " / 30";
    }
    private void Update()
    {
        CurrentAmmoTMP.text = m_currentAmmo.ToString() + " / 30";

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
            // Time.time�� m_nextFireTime���� �߻�x 5�ʿ� �߻� m_nextFireTime = 5.1f�̱⿡ 0.1f�ʵ� ����
            int num1 = 0;
            for (int i = 0; i < GunMuzzleFlashs.Length; i++)
            {
                if (!GunMuzzleFlashs[i].gameObject.activeSelf) continue;
                num1 = i;
            }

            FireRate = GunMuzzleFlashs[num1].Rate;
            m_nextFireTime = Time.time + FireRate;
            
            if (m_currentAmmo > 0)
            {
                CheckShootTarget();
                int num = 0;
                for (int i = 0; i < GunMuzzleFlashs.Length; i++)
                {
                    if (!GunMuzzleFlashs[i].gameObject.activeSelf) continue;
                    GunMuzzleFlashs[i].MuzzleFlash.Play();
                    num = i;
                }
                if (GunMuzzleFlashs[num].GunTypeE == GunType.Sniper)
                    PlayerAnimator.SetBool("Sniper", true);
                else PlayerAnimator.SetBool("Sniper", false);

                PlayerAnimator.SetBool("Shoot", true);
                m_currentAmmo--;

                SoundAudioSource.PlayOneShot(ShootingSoundClip);
            }
            else // �Ѿ� ���� �� �ڵ� ������
            {
                //Reload
                Reload();
            }
        }
        else if(!_getFire)
        {
            PlayerAnimator.SetBool("Sniper", false);
            PlayerAnimator.SetBool("Shoot", false);
        }
    }

    private void CheckShootTarget()
    {
        Ray _ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f));
        RaycastHit _hit;
        if (Physics.Raycast(_ray, out _hit, FireRange))
        {
           ZombieAI _zombieAI = _hit.collider.GetComponent<ZombieAI>();
            for (int i = 0; i < GunMuzzleFlashs.Length; i++)
            {
                if (!GunMuzzleFlashs[i].gameObject.activeSelf) continue;
                DamagePerShot = GunMuzzleFlashs[i].Damage;
            }

            if (_zombieAI != null)
            {
                _zombieAI.TakeDamage(DamagePerShot);

                // Play blood effect particle System at the point.
                ParticleSystem blood = Instantiate(BloodEffect, _hit.point, Quaternion.LookRotation(_hit.normal));
                Destroy(blood.gameObject, blood.main.duration); // ���۽ð� ���� ����
            }
            WayPointZobieAI _waypointzombieAI = _hit.collider.GetComponent<WayPointZobieAI>();
            if (_waypointzombieAI != null)
            {
                _waypointzombieAI.TakeDamage(DamagePerShot);

                // Play blood effect particle System at the point.
                ParticleSystem blood = Instantiate(BloodEffect, _hit.point, Quaternion.LookRotation(_hit.normal));
                Destroy(blood.gameObject, blood.main.duration); // ���۽ð� ���� ����
            }
        }
    }

    private void Reload()
    {
        if(!m_isReloading && m_currentAmmo < MaxAmmo)
        {
            //reload anim
            PlayerAnimator.SetTrigger("Reload");
            m_isReloading = true;
            SoundAudioSource.PlayOneShot(ReloadSoundClip);
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
