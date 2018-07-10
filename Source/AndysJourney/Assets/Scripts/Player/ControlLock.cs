using System.Collections.Generic;

public interface IControlLocker{
    void Lock(string name);
    void ReleaseLock(string name);
}

public class ControlLock
{
    static IDictionary<string, IControlLocker> _list = new Dictionary<string, IControlLocker>();

    public static void Register(string name, IControlLocker controller)
    {
        if(_list.ContainsKey(name))
            return;
        _list.Add(name, controller);
    }

    public static void Lock(string name, System.Action then = null)
    {
        if(!_list.ContainsKey(name))
            throw new KeyNotFoundException();
        _list[name].Lock(name);
        if(then != null)
            then();
    }

    public static void ReleaseLock(string name, System.Action then = null)
    {
        if(!_list.ContainsKey(name))
            throw new KeyNotFoundException();
        _list[name].ReleaseLock(name);
        if(then != null)
            then();
    }
}