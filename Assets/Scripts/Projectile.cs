using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Stats")]
    [SerializeField] float aliveTime = 1f;
    [SerializeField] float speed = 50f;
    [SerializeField] float damage = 10f;

    Rigidbody rbody;
    Collider projCollider;
    MeshRenderer projMesh;

    public Collider ownerCollider { get; set; }
    public GameObject impactVFX { get; set; }

    Collision collision;

    // has the projectile collided with something?
    bool collided = false;

    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Destroy(gameObject, aliveTime);
    }

    void FixedUpdate()
    {
        if (collided) Impact(collision.contacts[0].point, collision.contacts[0].normal);
    }

    void OnCollisionEnter(Collision other)
    {
        collision = other;
        collided = true;
    }

    public void Shoot(Vector3 shotDirection)
    {
        // ignore collider of entity that "shot" this projectile
        Physics.IgnoreCollision(GetComponent<Collider>(), ownerCollider);

        // move projectile
        rbody.AddForce(shotDirection.normalized * speed, ForceMode.Impulse);
    }

    void Impact(Vector3 point, Vector3 normal)
    {
        Destroy(gameObject);
        // place, rotate & play impact particle
        impactVFX.transform.position = point;
        impactVFX.transform.forward = normal;
        impactVFX.GetComponentInChildren<ParticleSystem>().Play();
    }
}
