using System;

public abstract class SingletonBase<T>
    where T : SingletonBase<T>, new()
{
    private static readonly object lockObject = new object();
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = Activator.CreateInstance<T>();
                        instance.Init();
                    }
                }
            }

            return instance;
        }
    }
    protected abstract void Init();
}