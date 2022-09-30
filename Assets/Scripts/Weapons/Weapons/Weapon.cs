
public class Weapon
{
    public WeaponDetailsSO weaponDetails;
    public int weaponListPosition;

    public float reloadTimer;
    public bool isReloading = false;

    public int clipAmmo;
    public int totalAmmo;

    static public Weapon CreateWeapon(WeaponDetailsSO weaponDetails)
    {
        return new Weapon()
        {
            weaponDetails = weaponDetails,
            reloadTimer = 0f,
            isReloading = false,
            clipAmmo = weaponDetails.ammoClipCapacity,
            totalAmmo = weaponDetails.ammoCapacity,
        };
    }
}
