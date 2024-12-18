using robot_controller_api.Models;

public interface IUserAccess
{
    public List<UserModel> GetUsers();
    public void InsertUsers(UserModel user);
    public void UpdateUsers(UserModel user);
    public void DeleteUsers(int id);
}