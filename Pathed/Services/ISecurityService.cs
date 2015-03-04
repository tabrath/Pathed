namespace Pathed.Services
{
    public interface ISecurityService
    {
        bool IsAdministrator();
        void ElevateToAdministrator(params string[] args);
    }
}
