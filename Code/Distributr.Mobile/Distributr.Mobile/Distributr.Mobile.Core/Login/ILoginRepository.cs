namespace Distributr.Mobile.Login
{
    public interface ILoginRepository
    {
        User FindUser(string username);
        LastLoggedInUser GetLastUser();
        void SetLastUser(string username, string costCentreApplicationId);
    }
}