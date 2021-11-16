namespace EW2
{
    public abstract class ResourceBase
    {
        protected ResourceBase(ResourceType type)
        {
            this.Type = type;
        }
        
        public ResourceType Type { get;}
        
        public abstract void Add(params object[] args);
        
        public abstract void Sub(params object[] args);
        
        public abstract object Get(params object[] args);
        
        public abstract void Clear();
    }
}