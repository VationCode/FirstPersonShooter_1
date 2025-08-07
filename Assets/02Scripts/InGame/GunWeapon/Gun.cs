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
}
