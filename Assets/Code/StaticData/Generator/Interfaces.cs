using System.Collections.Generic;


public class Interfaces 
{
    private Dictionary<string, object> dics = new Dictionary<string, object>();
    public void Register(string k, object v)
    {
        dics[k] = v;
    }

    public object Get(string k)
    {
        if(!dics.ContainsKey(k))
        {
            return null;
        }
        return dics[k];
    }

    public void Clear()
    {
        dics.Clear();
    }
             
}
