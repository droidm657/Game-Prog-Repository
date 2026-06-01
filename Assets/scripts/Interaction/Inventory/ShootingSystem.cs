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

    [Header("Reload Settings")]
    public float reloadTime = 2f;
    public KeyCode reloadKey = KeyCode.R;

    [Header("References")]
    public Camera playerCamera;
    public AudioSource shootSound;
    public AudioSource reloadSound;

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

            // Fill chamber when first equipped
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
            Shoot();

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

        if (Physics.Raycast(ray, out RaycastHit hit, shootRange))
        {
            Debug.Log($"Hit: {hit.collider.gameObject.name} at {hit.distance}m");

            HealthSystem health = hit.collider.GetComponent<HealthSystem>();
            if (health != null)
                health.TakeDamage(damagePerShot);

            Lock lockComponent = hit.collider.GetComponent<Lock>();
            if (lockComponent != null)
                lockComponent.ShootLock();

            SpawnHitEffect(hit.point, hit.normal);
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

        isReloading = false;
    }

    void SpawnHitEffect(Vector3 point, Vector3 normal)
    {
        GameObject impact = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        impact.transform.position = point;
        impact.transform.localScale = Vector3.one * 0.05f;
        Destroy(impact, 0.5f);
    }

    public int GetCurrentChamber() => currentChamber;
    public int GetMaxChamber() => maxChamberSize;
    public bool IsReloading() => isReloading;
}