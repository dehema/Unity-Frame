/***
 * 
 * 不继承Monobehaviour的单例模板
 * 
 * **/
public abstract class Singleton<T> : System.IDisposable where T : new()
{
    private static T instance;
    public static T Ins { get { return GetInstance(); } }
    public static T GetInstance()
    {
        if (instance == null)
        {
            instance = new T();
        }
        return instance;
    }
    public virtual void Dispose()
    {
    }

}

