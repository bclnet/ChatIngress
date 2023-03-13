namespace ChatIngress.Slack
{
    /// <summary>
    /// ISlackConnectionString
    /// </summary>
    public interface ISlackConnectionString
    {
        string this[string name] { get; }
    }
}
