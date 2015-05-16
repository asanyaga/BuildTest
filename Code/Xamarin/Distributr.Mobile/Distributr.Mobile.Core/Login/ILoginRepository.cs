namespace Distributr.Mobile.Login
{
    public interface ILoginRepository
    {
        User FindUser(string username);
        string GetLastUser();
        void SetLastUser(string username);
    }
}