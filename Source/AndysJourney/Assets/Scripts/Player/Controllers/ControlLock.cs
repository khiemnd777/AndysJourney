using System.Collections.Generic;

public interface IControlLocker
{
    void Lock(string name);
    void ReleaseLock(string name);
}

public class ControlLock
{
    static IDictionary<string, IControlLocker> _list = new Dictionary<string, IControlLocker>();

    public static void Register(string name, IControlLocker controller)
    {
        if (_list.ContainsKey(name))
            return;
        _list.Add(name, controller);
    }

    public static void Register(IControlLocker controller, params string[] names)
    {
        foreach (var name in names)
        {
            if (_list.ContainsKey(name))
                continue;
            _list.Add(name, controller);
        }
    }

    public static void Lock(params string[] names)
    {
        foreach (var name in names)
        {
            if (!_list.ContainsKey(name))
                throw new KeyNotFoundException("No found [" + name + "]");
            _list[name].Lock(name);
        }
    }

    public static void ReleaseLock(params string[] names)
    {
        foreach (var name in names)
        {
            if (!_list.ContainsKey(name))
                throw new KeyNotFoundException();
            _list[name].ReleaseLock(name);
        }
    }
}