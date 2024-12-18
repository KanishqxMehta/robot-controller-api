using Microsoft.AspNetCore.Identity;
using robot_controller_api.Controllers;
using robot_controller_api.Models;
using BCrypt;

public class UserEF : IRepository, IUserAccess
{
    private readonly RobotContext _context;

    public UserEF(RobotContext context)
    {
        _context = context;
    }

    public List<UserModel> GetUsers() => _context.UserModels.ToList();

    public void InsertUsers(UserModel user)
    {
        string password = user.PasswordHash!;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

        user.CreatedDate = DateTime.Now;
        user.ModifiedDate = DateTime.Now;

        _context.UserModels.Add(user);
        _context.SaveChanges();
    }

    public void UpdateUsers(UserModel user)
    {
        UserModel userModel = _context.UserModels.FirstOrDefault(usr => usr.Id == user.Id)!;

        if (userModel != null)
        {
            userModel.FirstName = user.FirstName;
            userModel.LastName = user.LastName;
            userModel.Description = user.Description;
            userModel.Role = user.Role;
            userModel.Email = user.Email;
            userModel.PasswordHash = user.PasswordHash;
            userModel.ModifiedDate = DateTime.Now;

            _context.UserModels.Update(userModel);
            _context.SaveChanges();
        }
    }

    public void DeleteUsers(int id)
    {
        var userTmp = _context.UserModels.FirstOrDefault(usr => usr.Id == id)!;
        if (userTmp != null)
        {
            _context.UserModels.Remove(userTmp);
            _context.SaveChanges();
        }
    }
}