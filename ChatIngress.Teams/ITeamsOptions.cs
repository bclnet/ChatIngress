namespace ChatIngress.Teams
{
    public interface ITeamsOptions
    {
        Azure.Core.TokenCredential Token { get; }
    }
}
