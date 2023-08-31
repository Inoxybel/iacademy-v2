namespace IAcademy.Test.Integration.Fakes;

public static class FakeServiceProvider
{
    private static Dictionary<Type, object> fakers = new Dictionary<Type, object>();

    public static void Add<T>(T faker) where T : class
    {
        Remove<T>();
        fakers.Add(typeof(T), faker);
    }

    public static T Get<T>() where T : class => Contains<T>() ? (T)fakers[typeof(T)] : default;

    public static bool Contains<T>() where T : class => fakers.ContainsKey(typeof(T));

    public static void Remove<T>() where T : class
    {
        if (Contains<T>())
            fakers.Remove(typeof(T));
    }

    public static void Clear() => fakers = new Dictionary<Type, object>();
}