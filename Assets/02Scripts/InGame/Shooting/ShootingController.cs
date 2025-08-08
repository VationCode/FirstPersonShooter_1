using TMPro;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    public Animator PlayerAnimator;
    public Camera[] Cameras;
    //public Transform FirePoint;
    public float FireRange = 100f;
    private float m_fireRate = 10f;
    private float m_nextFireTime = 0f;

    public bool IsAuto = false;

    private int m_currentGunIndex = 0;
    private int m_maxAmmo;
    private int m_currentAmmo;
    public float reloadTime = 1.5f;
    private bool m_isReloading = false;

    [Header("Effect")]
    //public Gun[] Gun;
    public ParticleSystem BloodEffect;
    private int m_damagePerShot = 10;


    [Header("Sound")]
    public AudioSource SoundAudioSource;
    //public AudioClip ShootingSoundClip;
    public AudioClip ReloadSoundClip;

    public TextMeshProUGUI CurrentAmmoTMP;
    private void Start()
    {
        for (int i = 0; i < GameManager.Instance.Guns.Length; i++)
        {
            GameManager.Instance.Guns[i].CurrentAmmo = GameManager.Instance.Guns[i].MaxAmmo;
        }

        m_currentGunIndex = GameManager.Instance.CurrentWeaponIndex;
        m_maxAmmo = GameManager.Instance.Guns[m_currentGunIndex].MaxAmmo;

        m_currentAmmo = m_maxAmmo;
        CurrentAmmoTMP.text = m_currentAmmo.ToString() + " / " + m_maxAmmo;
    }
    private void Update()
    {
        m_currentGunIndex = GameManager.Instance.CurrentWeaponIndex;
        
        SetAmmoTMP();

        if (m_isReloading) return;
        Shoot();    //자동
        if (Input.GetKeyDown(KeyCode.R) && m_currentAmmo < GameManager.Instance.Guns[m_currentGunIndex].MaxAmmo) Reload();
    }

    private void SetAmmoTMP()
    {
        m_maxAmmo = GameManager.Instance.Guns[m_currentGunIndex].MaxAmmo;
        m_currentAmmo = GameManager.Instance.Guns[m_currentGunIndex].CurrentAmmo;

        CurrentAmmoTMP.text = m_currentAmmo.ToString() + " / " + m_maxAmmo;
    }
        
    private void Shoot()
    {
        bool _getFire = false;
        Gun _currentGun = GameManager.Instance.Guns[m_currentGunIndex];
        
        if (_currentGun.GunTypeE != GunType.Sniper) _getFire = Input.GetButton("Fire1");        // 자동 발사 방식
        else _getFire = Input.GetButtonDown("Fire1");   // 수동 발사 방식

        if (_getFire && Time.time >= m_nextFireTime)
        {
            // Time.time이 m_nextFireTime까지 발사x 5초에 발사 m_nextFireTime = 5.1f이기에 0.1f초뒤 가능
            
            m_currentAmmo = _currentGun.CurrentAmmo;
            m_fireRate = _currentGun.Rate;
            m_nextFireTime = Time.time + m_fireRate;
            
            if (_currentGun.CurrentAmmo > 0)
            {
                CheckShootTarget();

                _currentGun.MuzzleFlash.Play();

                if (_currentGun.GunTypeE == GunType.Sniper)
                    PlayerAnimator.SetBool("Sniper", true);
                else PlayerAnimator.SetBool("Sniper", false);

                PlayerAnimator.SetBool("Shoot", true);
                _currentGun.CurrentAmmo--;

                SoundAudioSource.PlayOneShot(_currentGun.SFXClip);
            }
            else // 총알 없을 시 자동 재장전
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
        Camera _currentCamera = Cameras[0].gameObject.activeInHierarchy ? Cameras[0] : Cameras[1];
        Ray _ray = _currentCamera.ViewportPointToRay(new Vector3(0.5f,0.5f));
        RaycastHit _hit;
        Gun _currentGun = GameManager.Instance.Guns[m_currentGunIndex];
        if (Physics.Raycast(_ray, out _hit, FireRange))
        {
           ZombieAI _zombieAI = _hit.collider.GetComponent<ZombieAI>();

                m_damagePerShot = _currentGun.Damage;
            

            if (_zombieAI != null)
            {
                _zombieAI.TakeDamage(m_damagePerShot);

                // Play blood effect particle System at the point.
                ParticleSystem blood = Instantiate(BloodEffect, _hit.point, Quaternion.LookRotation(_hit.normal));
                Destroy(blood.gameObject, blood.main.duration); // 동작시간 이후 제거
            }
            WayPointZobieAI _waypointzombieAI = _hit.collider.GetComponent<WayPointZobieAI>();
            if (_waypointzombieAI != null)
            {
                _waypointzombieAI.TakeDamage(m_damagePerShot);

                // Play blood effect particle System at the point.
                ParticleSystem blood = Instantiate(BloodEffect, _hit.point, Quaternion.LookRotation(_hit.normal));
                Destroy(blood.gameObject, blood.main.duration); // 동작시간 이후 제거
            }
        }
    }

    private void Reload()
    {
        Gun _currentGun = GameManager.Instance.Guns[m_currentGunIndex];
        if (!m_isReloading && m_currentAmmo < _currentGun.MaxAmmo)
        {
            //reload anim
            PlayerAnimator.SetTrigger("Reload");
            m_isReloading = true;
            SoundAudioSource.PlayOneShot(ReloadSoundClip);
            Invoke("FinishReloading", reloadTime); // reloadTime : 애니메이션 시간에 맞게
        }
    }

    private void FinishReloading()
    {
        Gun _currentGun = GameManager.Instance.Guns[m_currentGunIndex];
        _currentGun.CurrentAmmo = _currentGun.MaxAmmo;
        m_currentAmmo = _currentGun.CurrentAmmo;
        m_isReloading = false;
        PlayerAnimator.ResetTrigger("Reload");
    }
}
