using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponShootType
{
    Manual,
    Automatic
}

public class Weapon : MonoBehaviour
{
    [Header("References")]
    //-----------------------------------------------------
    [SerializeField] Transform firePoint;
    [SerializeField] Muzzleflash muzzleFlash;
    [SerializeField] LayerMask raycastHittableLayers; // should not include: projectiles

    [Header("Shooting Parameters")]
    //-----------------------------------------------------
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] WeaponShootType shootType;
    public WeaponShootType ShootType { get { return shootType; } }
    [SerializeField] float shotDelay = 0.1f;

    [Header("Recoil")]
    //-----------------------------------------------------
    [SerializeField] float recoilForce = 1f;
    [SerializeField] float maxRecoilDistance = 0.15f;
    [Tooltip("How fast the recoil moves. Higher = faster")]
    [SerializeField] float recoilSpeed = 90f;
    [Tooltip("How fast the recoil resets")]
    [SerializeField] float recoilResetSpeed = 30f;
    //--
    [Space(10)]
    //--
    [SerializeField] float recoilRotationAmount = -12f;
    [Tooltip("How fast the recoil rotates. Higher = faster")]
    [SerializeField] float recoilRotationSpeed = 90f;
    [Tooltip("How fast the rotation resets")]
    [SerializeField] float recoilRotationResetSpeed = 30f;

    [Header("Audio")]
    //-----------------------------------------------------
    [SerializeField] AudioClip shootSfx;

    Camera mainCamera;
    Collider ownerCollider; // collider attached to this weapon's parent
    public GameObject impactParent { get; set; }
    AudioSource audioSource;

    float lastShotTime = Mathf.NegativeInfinity;
    // recoil position
    Vector3 accumulatedRecoil_Position;
    Vector3 recoil_Position;
    // recoil rotation
    Vector3 initialRotation;
    Quaternion accumulatedRecoil_Rotation;
    bool isRecoilRotating = false;

    void Awake()
    {
        mainCamera = GetComponentInParent<Camera>();
        ownerCollider = GetComponentInParent<Collider>();
        initialRotation = transform.localEulerAngles;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        UpdateWeaponRecoil();

        if (recoil_Position != Vector3.zero) transform.localPosition = recoil_Position;
    }

    void UpdateWeaponRecoil()
    {
        // if the accumulated recoil is further away from the current position, make the current position move towards the recoil target
        if (recoil_Position.z >= accumulatedRecoil_Position.z * 0.99f)
        {
            recoil_Position = Vector3.Lerp(recoil_Position, accumulatedRecoil_Position, recoilSpeed * Time.deltaTime);
        }
        // otherwise, move recoil position to make it recover towards its resting pose
        else
        {
            recoil_Position = Vector3.Lerp(recoil_Position, Vector3.zero, recoilResetSpeed * Time.deltaTime);
            accumulatedRecoil_Position = recoil_Position;
        }

        // rotating up
        if (isRecoilRotating)
        {
            // rotate gradually to accumulated rotation
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, accumulatedRecoil_Rotation, recoilRotationSpeed * Time.deltaTime);

            // if weapon has reached target rotation (or close to)
            if (Quaternion.Angle(transform.localRotation, accumulatedRecoil_Rotation) <= 0.01f)
            {
                isRecoilRotating = false;
            }
        }
        // reseting rotation
        else
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(initialRotation), recoilRotationResetSpeed * Time.deltaTime);
        }
    }

    public void TryShoot()
    {
        if (lastShotTime + shotDelay <= Time.time)
        {
            lastShotTime = Time.time;

            // TODO: ammo checks

            // cast ray from camera to crosshair
            Vector3 targetPosition = ProcessRaycast();
            // direction of projectile from firePoint to center of screen
            Vector3 shotDirection = targetPosition - firePoint.position;

            // activate muzzleflash
            StartCoroutine(muzzleFlash.Activate());

            // create the projectile
            CreateProjectile(shotDirection);

            // weapon sfx
            audioSource.PlayOneShot(shootSfx);

            // recoil
            ProcessRecoil();
        }
    }

    Vector3 ProcessRaycast()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        // fire from camera to crosshair (centre of screen)
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastHittableLayers))
        {
            Debug.Log(hit.collider);
            return hit.point; // on hit
        }
        else
        {
            return ray.GetPoint(75); // on miss - get point far away from camera
        }
    }

    void CreateProjectile(Vector3 shotDirection)
    {
        // create projectile
        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // assign owner collider to projectile
        projectile.ownerCollider = ownerCollider;
        // tell projectile which object is impactVFX
        projectile.impactVFX = impactParent;

        // turn projectile to shot direction
        projectile.transform.up = shotDirection;
        // call projectile shoot method (this moves the object)
        projectile.Shoot(shotDirection);
    }

    void ProcessRecoil()
    {
        // POSITION

        // weapon recoils backwards
        accumulatedRecoil_Position += Vector3.back * recoilForce;
        // limit recoil distance
        accumulatedRecoil_Position = Vector3.ClampMagnitude(accumulatedRecoil_Position, maxRecoilDistance);

        // ROTATION

        // weapon rotates up        
        accumulatedRecoil_Rotation = Quaternion.Euler((Vector3.right * recoilRotationAmount) + initialRotation);
        isRecoilRotating = true;
    }
}
