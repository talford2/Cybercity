using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    #region Public Properties

    public Transform EquippedWeapon;
    public float ShootTime;
    public AnimationCurve ShootCurve;
    public float RestoreTime;
    public AnimationCurve RestoreCurve;

    [Header("Muzzle Effect")]
    public float FlashTime;
    public Light FlashLight;
    public MeshRenderer MuzzleRenderer;

    [Header("Missile")]
    public GameObject MissilePrefab;

    #endregion

    #region Private Variables

    private bool isShooting;
    private float shootCooldown;
    private bool isRestoring;
    private float restoreCooldown;
    private bool isAiming;

    private Vector3 initialOffset;
    private Quaternion initialRotation;

    private Vector3 shootOffset;
    private Quaternion shootRotation;

    private Vector3 aimingOffset;
    private Quaternion aimingRotation;

    private Vector3 currentOffset;
    private Quaternion currentRotation;

    private int missilePoolCount = 20;
    private List<GameObject> missileInstances;
    private int curMissileIndex;

    private float omega;

    #endregion

    private void Awake()
    {
        initialOffset = EquippedWeapon.localPosition;
        initialRotation = EquippedWeapon.localRotation;

        shootOffset = new Vector3(0.02f, 0f, -0.15f);
        shootRotation = Quaternion.Euler(-10f, 0f, 0f);

        aimingOffset = new Vector3(0f, -0.2f, 0.1f);
        aimingRotation = Quaternion.Euler(0f, 90f, 0f);

        FlashLight.enabled = false;
        MuzzleRenderer.enabled = false;

        missileInstances = new List<GameObject>();
        for (var i = 0; i < missilePoolCount; i++)
        {
            missileInstances.Add((GameObject)Instantiate(MissilePrefab, MuzzleRenderer.transform.position, MuzzleRenderer.transform.rotation));
        }
        curMissileIndex = 0;
        omega = 0f;
    }

    private void ShootMissile()
    {
        var missileInstance = missileInstances[curMissileIndex];
        missileInstance.transform.position = MuzzleRenderer.transform.position;
        missileInstance.transform.forward = MuzzleRenderer.transform.forward;
        var laser = missileInstance.GetComponent<Laser>();
        laser.SetOwner(gameObject);
        laser.Shoot(MuzzleRenderer.transform.position, MuzzleRenderer.transform.forward, Vector3.zero);
        curMissileIndex++;
        if (curMissileIndex >= missilePoolCount)
            curMissileIndex = 0;
    }

    private void Update()
    {
        if (!isShooting && !isRestoring)
        {
            if (Input.GetMouseButton(0))
            {
                isShooting = true;
                shootCooldown = ShootTime;
                FlashLight.enabled = true;
                EquippedWeapon.transform.localRotation = initialRotation;
                MuzzleRenderer.enabled = true;
                MuzzleRenderer.transform.localRotation = Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward);
                FlashLight.intensity = 1f;

                ShootMissile();
            }
        }

        if (Input.GetMouseButton(1))
        {
            isAiming = true;
            currentOffset = aimingOffset;
            currentRotation = aimingRotation;
        }
        else
        {
            isAiming = false;
            currentOffset = initialOffset;
            currentRotation = initialRotation;
        }

        if (isShooting)
        {
            if (shootCooldown >= 0f)
            {
                shootCooldown -= Time.deltaTime;

                if (shootCooldown < (ShootTime - FlashTime))
                {
                    FlashLight.enabled = false;
                    MuzzleRenderer.enabled = false;
                }

                if (shootCooldown < 0f)
                {
                    isShooting = false;
                    isRestoring = true;
                    restoreCooldown = RestoreTime;
                }
            }
        }

        if (isRestoring)
        {
            if (restoreCooldown >= 0f)
            {
                restoreCooldown -= Time.deltaTime;
                if (restoreCooldown < 0f)
                {
                    isRestoring = false;
                    EquippedWeapon.localPosition = currentOffset;
                    EquippedWeapon.localRotation = currentRotation;
                }
            }
        }

        if (isShooting)
        {
            var shootFraction = ShootCurve.Evaluate(1f - Mathf.Clamp01(shootCooldown/ShootTime));
            EquippedWeapon.localPosition = Vector3.Lerp(currentOffset, currentOffset + shootOffset, shootFraction);
            EquippedWeapon.localRotation = Quaternion.Lerp(currentRotation, shootRotation * currentRotation, shootFraction);
            FlashLight.intensity = 2f*Mathf.Clamp01(shootCooldown/ShootTime);
        }

        if (isRestoring)
        {
            var restoreFraction = RestoreCurve.Evaluate(1f - Mathf.Clamp01(restoreCooldown/RestoreTime));
            EquippedWeapon.localPosition = Vector3.Lerp(currentOffset + shootOffset, GetIdleOffset(), restoreFraction);
            EquippedWeapon.localRotation = Quaternion.Lerp(shootRotation*currentRotation, GetIdleRotation(), restoreFraction);
        }

        omega = (omega + 90f*Time.deltaTime)%360f;
        if (!isShooting && !isRestoring)
        {
            EquippedWeapon.localPosition = GetIdleOffset();
            EquippedWeapon.localRotation = GetIdleRotation();
        }
    }

    private Vector3 GetIdleOffset()
    {
        if (isAiming)
            return currentOffset + new Vector3(0f, 0.002f * Mathf.Sin(omega * Mathf.Deg2Rad) - 0.0001f, 0f);
        return currentOffset + new Vector3(0f, 0.01f * Mathf.Sin(omega * Mathf.Deg2Rad) - 0.005f, 0f);
    }

    private Quaternion GetIdleRotation()
    {
        if (isAiming)
            return currentRotation * Quaternion.AngleAxis(1f * Mathf.Sin(omega * Mathf.Deg2Rad), Vector3.up);
        return currentRotation * Quaternion.AngleAxis(5f * Mathf.Sin(omega * Mathf.Deg2Rad), Vector3.up);
    }

    /*
    private void OnGUI()
    {
        GUI.Label(new Rect(20f, 20f, 100f, 30f), string.Format("SHOOT: {0:f2}", Mathf.Clamp01(shootCooldown / ShootTime)));
        GUI.Label(new Rect(20f, 50f, 100f, 30f), string.Format("RESTORE: {0:f2}", Mathf.Clamp01(restoreCooldown / RestoreTime)));
    }
    */
}
