using System;
using System.Reflection;

public static class SingletonCreator
{
    public static T CreateSingleton<T>() where T : class, ISingleton
    {
        Type type = typeof(T);
        ConstructorInfo[] ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
        ConstructorInfo ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
        if(ctor == null)
            throw new Exception($"使用继承自{typeof(Singleton<>).Name}的子类需要一个非public的无参构造方法，请检查您的类:「{type.Name}」中是否存在这样的构造方法\n\n");

        T result = ctor.Invoke(null) as T;
        result.OnSingletonInit();
        return result;
    }
}
