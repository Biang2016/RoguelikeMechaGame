using UnityEngine;

public class Projectile : PoolObject
{
    [SerializeField] private float hitOffset = 0f;
    [SerializeField] private bool UseFirePointRotation;
    [SerializeField] private Vector3 rotationOffset = new Vector3(0, 0, 0);
    private Rigidbody Rigidbody;
    private Collider Collider;
    private ParticleSystem ParticleSystem;
    private ParticleSystem.TrailModule ParticleSystemTrail;
    private bool useTrail = false;
    [SerializeField] private GameObject[] Detached;

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

        Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        Collider.enabled = false;
        curSpeed = 0;

        if (useTrail) ParticleSystemTrail.enabled = false;
        base.PoolRecycle();
    }

    internal ProjectileInfo ProjectileInfo;

    public void Initialize(ProjectileInfo projectileInfo)
    {
        ProjectileInfo = projectileInfo;
    }

    private float curSpeed;

    public void Play()
    {
        if (useTrail) ParticleSystemTrail.enabled = true;
        Rigidbody.constraints = RigidbodyConstraints.None;
        Collider.enabled = true;
        curSpeed = ProjectileInfo.Speed;
        ParticleSystem.Play(true);
        if (GameObjectPoolManager.Instance.ProjectileFlashDict.ContainsKey(ProjectileInfo.ProjectileType))
        {
            ProjectileFlash flash = GameObjectPoolManager.Instance.ProjectileFlashDict[ProjectileInfo.ProjectileType].AllocateGameObject<ProjectileFlash>(ProjectileManager.Instance.transform);
            flash.transform.position = transform.position;
            flash.transform.rotation = Quaternion.identity;
            flash.transform.forward = gameObject.transform.forward;
            flash.ParticleSystem.Play(true);
            flash.PoolRecycle(flash.ParticleSystem.main.duration);
        }

        PoolRecycle(ParticleSystem.main.duration);
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
        ContactPoint contact = collision.contacts[0];

        if (GameObjectPoolManager.Instance.ProjectileHitDict.ContainsKey(ProjectileInfo.ProjectileType))
        {
            ProjectileHit hit = GameObjectPoolManager.Instance.ProjectileHitDict[ProjectileInfo.ProjectileType].AllocateGameObject<ProjectileHit>(ProjectileManager.Instance.transform);
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