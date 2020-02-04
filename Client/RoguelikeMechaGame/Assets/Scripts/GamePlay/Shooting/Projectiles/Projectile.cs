using UnityEngine;

public class Projectile : PoolObject
{
    public ProjectileType ProjectileType;

    public float speed = 15f;
    public float hitOffset = 0f;
    public bool UseFirePointRotation;
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    private Rigidbody Rigidbody;
    private Collider Collider;
    private ParticleSystem ParticleSystem;
    private ParticleSystem.TrailModule ParticleSystemTrail;
    private bool useTrail = false;
    public GameObject[] Detached;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
        ParticleSystem = GetComponent<ParticleSystem>();
        if (!ParticleSystem)
        {
            ParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        ParticleSystemTrail = ParticleSystem.trails;
        useTrail = ParticleSystemTrail.enabled;
    }

    void Start()
    {
    }

    public override void PoolRecycle()
    {
        ParticleSystem.Stop(true);
        if (useTrail) ParticleSystemTrail.enabled = false;
        base.PoolRecycle();
    }

    private float curSpeed;

    public void Play()
    {
        if (useTrail) ParticleSystemTrail.enabled = true;
        Rigidbody.constraints = RigidbodyConstraints.None;
        Collider.enabled = true;
        curSpeed = speed;
        ParticleSystem.Play(true);
        if (GameObjectPoolManager.Instance.ProjectileFlashDict.ContainsKey(ProjectileType))
        {
            ProjectileFlash flash = GameObjectPoolManager.Instance.ProjectileFlashDict[ProjectileType].AllocateGameObject<ProjectileFlash>(ProjectileManager.Instance.transform);
            flash.transform.position = transform.position;
            flash.transform.rotation = Quaternion.identity;
            flash.transform.forward = gameObject.transform.forward;
            flash.ParticleSystem.Play(true);
            flash.PoolRecycle(flash.ParticleSystem.main.duration);
        }

        PoolRecycle(5f);
    }

    void FixedUpdate()
    {
        if (!curSpeed.Equals(0))
        {
            Rigidbody.velocity = transform.forward * curSpeed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        Collider.enabled = false;
        curSpeed = 0;

        ContactPoint contact = collision.contacts[0];

        if (GameObjectPoolManager.Instance.ProjectileHitDict.ContainsKey(ProjectileType))
        {
            ProjectileHit hit = GameObjectPoolManager.Instance.ProjectileHitDict[ProjectileType].AllocateGameObject<ProjectileHit>(ProjectileManager.Instance.transform);

            hit.transform.position = contact.point + contact.normal * hitOffset;
            hit.transform.rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            if (UseFirePointRotation)
            {
                hit.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0);
            }
            else if (rotationOffset != Vector3.zero)
            {
                hit.transform.rotation = Quaternion.Euler(rotationOffset);
            }
            else
            {
                hit.transform.LookAt(contact.point + contact.normal);
            }

            hit.ParticleSystem.Play(true);
            hit.PoolRecycle(hit.ParticleSystem.main.duration);
        }

        foreach (GameObject detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
            }
        }

        PoolRecycle();
    }
}