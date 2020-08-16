namespace BiangStudio.Singleton
{
    public abstract class TSingletonBaseManager<T> : TSingleton<T> where T : new()
    {
        public virtual void Awake()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void LateUpdate()
        {
        }

        public virtual void FixedUpdate()
        {
        }
    }
}