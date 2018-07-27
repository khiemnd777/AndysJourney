using System.Collections.Generic;

public interface IStateHandler
{
    void HandleState(string name, string state);
}

public class StateHandling
{
    static IDictionary<string, IStateHandler> _list = new Dictionary<string, IStateHandler>();

    public static void Register(IStateHandler controller, params string[] names)
    {
        foreach (var name in names)
        {
            if (_list.ContainsKey(name))
                continue;
            _list.Add(name, controller);
        }
    }

    public static void Handle(string name, string state)
    {
        if (!_list.ContainsKey(name))
                return;
            _list[name].HandleState(name, state);
    }
}