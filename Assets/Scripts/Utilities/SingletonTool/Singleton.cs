public abstract class Singleton<T> : ISingleton where T : Singleton<T>
{
    protected static T m_Instance;
    private static object m_Lock = new object();
    public static T Instance {
        get {
            lock(m_Lock) {
                if(m_Instance == null)
                    m_Instance = SingletonCreator.CreateSingleton<T>();
            }
            return m_Instance;
        }
    }
    public void OnSingletonInit() { }
}
