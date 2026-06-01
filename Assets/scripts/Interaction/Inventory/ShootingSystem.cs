using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Collections;

public class ShootingSystem : MonoBehaviour
{
    public static ShootingSystem Instance;

    [Header("Shooting Settings")]
    public float shootRange = 50f;
    public int damagePerShot = 25;
    public float fireRate = 1f;
    public int shotsPerShell = 1;
    public int maxChamberSize = 6;

    [Header("Shotgun Spread")]
    public int pelletsPerShot = 8;
    public float spreadAngle = 4f;

    [Header("Reload Settings")]
    public float reloadTime = 2f;
    public KeyCode reloadKey = KeyCode.R;

    [Header("References")]
    public Camera playerCamera;
    public AudioSource shootSound;
    public AudioSource reloadSound;

    [Header("Tracer")]
    private Transform muzzlePoint;
    public Material tracerMaterial;
    public float tracerDuration = 0.05f;

    private float nextFireTime = 0f;
    private bool canShoot = false;
    private bool isReloading = false;
    private int currentChamber = 0;
    private Animator shotgunAnimator;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        canShoot = EquippingSystem.Instance.HasItemEquipped() &&
                   EquippingSystem.Instance.GetEquippedItem().itemType
                   == ItemData.ItemType.Weapon;

        if (canShoot && shotgunAnimator == null)
        {
            shotgunAnimator = EquippingSystem.Instance
                .GetEquippedObject()
                ?.GetComponent<Animator>();

            GameObject shotgun =
                EquippingSystem.Instance
                .GetEquippedObject();

            if (shotgun != null)
            {
                Transform foundMuzzle =
                    shotgun.transform.Find("MuzzlePoint");

                if (foundMuzzle != null)
                    muzzlePoint = foundMuzzle;
            }

            if (currentChamber == 0)
                currentChamber = maxChamberSize;
        }

        if (!canShoot)
        {
            shotgunAnimator = null;
            return;
        }

        // Shoot
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime && !isReloading) 
        {
            // 1. SAFEGUARD: Don't shoot if the game is paused (Time.timeScale is 0)
            if (Time.timeScale == 0f) return;

        // 2. SAFEGUARD: Don't shoot if the mouse pointer is clicking on a UI element (Inventory/Pause Menu)
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return; // Block the shot because they are interacting with the UI!
        }
            Shoot(); 
        }


        // Reload
        if (Input.GetKeyDown(reloadKey) && !isReloading && currentChamber < maxChamberSize)
            StartCoroutine(Reload());

        // Auto reload when empty
        if (currentChamber <= 0 && !isReloading)
            StartCoroutine(Reload());
    }

    void Shoot()
    {
        if (currentChamber <= 0)
        {
            Debug.Log("Chamber empty! Press R to reload!");
            return;
        }

        if (!AmmoSystem.Instance.UseAmmo(shotsPerShell))
        {
            Debug.Log("No ammo to reload with!");
            return;
        }

        nextFireTime = Time.time + 1f / fireRate;
        currentChamber--;

        Debug.Log($"Shot fired! Chamber: {currentChamber}/{maxChamberSize}");

        // Play shoot animation
        if (shotgunAnimator != null)
            shotgunAnimator.SetTrigger("Shoot");

        if (shootSound != null)
            shootSound.Play();

        // Raycast
        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2, 0)
        );

        Debug.DrawRay(ray.origin, ray.direction * shootRange, Color.red, 1f);

        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 spreadDirection =
                playerCamera.transform.forward;

            spreadDirection +=
                playerCamera.transform.right *
                Random.Range(
                    -spreadAngle,
                    spreadAngle
                ) * 0.01f;

            spreadDirection +=
                playerCamera.transform.up *
                Random.Range(
                    -spreadAngle,
                    spreadAngle
                ) * 0.01f;

            spreadDirection.Normalize();

            Ray spreadRay =
                new Ray(
                    playerCamera.transform.position,
                    spreadDirection
                );

            Vector3 endPoint;

            if (Physics.Raycast(
                spreadRay,
                out RaycastHit hit,
                shootRange))
            {
                endPoint = hit.point;

                HealthSystem health =
                    hit.collider.GetComponent<HealthSystem>();

                if (health != null)
                    health.TakeDamage(
                        damagePerShot
                    );

                Lock lockComponent =
                    hit.collider.GetComponent<Lock>();

                if (lockComponent != null)
                    lockComponent.ShootLock();
            }
            else
            {
                endPoint =
                    spreadRay.origin +
                    spreadRay.direction *
                    shootRange;
            }

            StartCoroutine(
                SpawnTracer(
                    muzzlePoint.position,
                    endPoint
                )
            );

            UIManager.Instance?.UpdateAmmoDisplay(currentChamber,currentChamber,AmmoSystem.Instance.GetAmmo());
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        // Play reload animation
        if (shotgunAnimator != null)
            shotgunAnimator.SetTrigger("Reload");

        if (reloadSound != null)
            reloadSound.Play();

        yield return new WaitForSeconds(reloadTime);

        // Calculate how many shells to load
        int needed = maxChamberSize - currentChamber;
        int available = AmmoSystem.Instance.GetAmmo();
        int toLoad = Mathf.Min(needed, available);

        if (toLoad > 0)
        {
            AmmoSystem.Instance.UseAmmo(toLoad);
            currentChamber += toLoad;
            Debug.Log($"Reloaded! Chamber: {currentChamber}/{maxChamberSize}");
        }
        else
        {
            Debug.Log("No ammo to reload with!");
        }

        UIManager.Instance?.UpdateAmmoDisplay(currentChamber,currentChamber,AmmoSystem.Instance.GetAmmo());

        isReloading = false;
    }

    IEnumerator SpawnTracer(
    Vector3 startPos,
    Vector3 endPos)
    {
        GameObject tracer =
            new GameObject("Tracer");

        LineRenderer lr =
            tracer.AddComponent<LineRenderer>();

        lr.material =
            tracerMaterial;

        lr.positionCount = 2;

        lr.startWidth = 0.03f;
        lr.endWidth = 0.01f;

        lr.useWorldSpace = true;

        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);

        yield return new WaitForSeconds(
            tracerDuration
        );

        Destroy(tracer);
    }
    public int GetCurrentChamber() => currentChamber;
    public int GetMaxChamber() => maxChamberSize;
    public bool IsReloading() => isReloading;
}