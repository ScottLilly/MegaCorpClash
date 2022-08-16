namespace MegaCorpClash.Models.CustomEventArgs;

public abstract class BaseCustomEventArgs : EventArgs
{
    public string Id { get; }
    public string Name { get; }

    protected BaseCustomEventArgs(string id, string name)
    {
        Id = id;
        Name = name;
    }
}