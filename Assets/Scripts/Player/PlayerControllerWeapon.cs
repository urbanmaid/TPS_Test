using UnityEngine;

public class PlayerControllerWeapon : MonoBehaviour
{
    [SerializeField] WeaponTypes weaponOnLoad;

    [Header("Current Status")]
    private int magazineLoad;
    private int magazineLoadCur;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeWeaponStat();

        Debug.Log("Current weapon: " + weaponOnLoad.weaponName + ", Magazine size: " + magazineLoad);
    }

    void InitializeWeaponStat()
    {
        magazineLoad = weaponOnLoad.magazineSize;
        magazineLoadCur = magazineLoad;
    }
}
