namespace ChatIngress.Teams
{
    /// <summary>
    /// ITeamsConnectionString
    /// </summary>
    public interface ITeamsConnectionString
    {
        string this[string name] { get; }
    }
}
