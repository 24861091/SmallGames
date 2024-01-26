
public class CodeSingleton<T> where T : class, new()
{
    public static T Instance { get { return Creator.Instance; } }

    protected CodeSingleton() { }
        
    private class Creator
    {
        internal static readonly T Instance = new T();
        static Creator() { }
    }
}
