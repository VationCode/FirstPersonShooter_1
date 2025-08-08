using UnityEngine;

public enum GunType
{ 
    Rifle,
    Sniper,
    SMG
}

public class Gun : MonoBehaviour
{
    public GunType GunTypeE;
    public ParticleSystem MuzzleFlash;
    public float Rate;
    public int Damage;
    public int MaxAmmo;
    public int CurrentAmmo;
    public AudioClip SFXClip;
}
