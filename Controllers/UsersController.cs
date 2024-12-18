using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;
using robot_controller_api.Models;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;
namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserAccess _userRepo;
    public UserController(IUserAccess userAccess) => _userRepo = userAccess;

    /// <summary>
    /// Retrieves all Users.
    /// </summary>
    /// <returns>A list of all users</returns>
    [HttpGet(""), Authorize(Policy = "UserOnly")]
    public IEnumerable<UserModel> GetAllUsers() => _userRepo.GetUsers();

    /// <summary>
    /// Retrieves all admins.
    /// </summary>
    /// <returns>A list of all users with admin</returns>
    [HttpGet("admin"), Authorize(Policy = "UserOnly")]
    public IEnumerable<UserModel> GetAdmins() => _userRepo.GetUsers().Where(role => role.Role == "Admin");

    /// <summary>
    /// Retrieves users by id.
    /// </summary>
    /// <returns>Users by id</returns>
    [HttpGet("{id}", Name = "GetUser"), Authorize(Policy = "UserOnly")]
    public IActionResult GetUsersById(int id)
    {
        var containsId = _userRepo.GetUsers().FirstOrDefault(usr => usr.Id == id);

        if (containsId != null)
        {
            return Ok(_userRepo.GetUsers().Where(usr => usr.Id == id));
        }

        return NotFound();
    }

    /// <summary>
    /// Creates a user.
    /// </summary>
    /// <param name="userModel">A new user from the HTTP request.</param>
    /// <returns>A newly created user</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/users
    ///     {
    ///        "email": "user@gmail.com",
    ///        "firstName": "user",
    ///        "lastName": "user",
    ///        "password": "xyz@uniquePass",
    ///        "role": user,
    ///        "description": "test user"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created robot command</response>
    /// <response code="400">If the robot command is null</response>
    /// <response code="409">If a robot command with the same name already exists.</response>    
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost(), AllowAnonymous]
    public IActionResult AddUser(UserModel userModel)
    {
        try
        {
            if (userModel == null)
            {
                return BadRequest();
            }

            userModel.Description ??= "N/A";

            // Check if a user with the same email already exists
            var existingUser = _userRepo.GetUsers().FirstOrDefault(u => u.Email == userModel.Email);
            if (existingUser != null)
            {
                return Conflict("User with the same email already exists.");
            }

            userModel.CreatedDate = DateTime.Now;
            userModel.ModifiedDate = DateTime.Now;
            _userRepo.InsertUsers(userModel);
            userModel.Id = _userRepo.GetUsers().Max(x => x.Id);

            return CreatedAtRoute("GetRobotCommand", new { id = userModel.Id }, userModel);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }



    /// <summary>
    /// Updates a user.
    /// </summary>
    /// <param name="userModel">A updated user from the HTTP request.</param>
    ///     /// <param name="id">Updates the user with id.</param>
    /// <returns>A user updated</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/users
    ///     {
    ///        "firstName": "updatedUser",
    ///        "lastName": "updatedUser",
    ///        "role": "user",
    ///        "description": "updated test user"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the updated user</response>
    /// <response code="400">If the user is null</response>
    /// <response code="409">If a user with the same name already exists.</response>    
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateRobotCommand(int id, UserModel userModel)
    {
        var existingUser = _userRepo.GetUsers().FirstOrDefault(x => x.Id == id);
        if (existingUser == null)
        {
            return NotFound();
        }

        // Store the current email and password hash
        var currentEmail = existingUser.Email;
        var currentPasswordHash = existingUser.PasswordHash;

        // Check if userModel contains email or password
        if (!string.IsNullOrEmpty(userModel.Email) || !string.IsNullOrEmpty(userModel.PasswordHash))
        {
            return BadRequest("Email and password cannot be updated using this method. Use PATCH instead.");
        }

        // Update other properties
        existingUser.Id = id;
        existingUser.FirstName = userModel.FirstName ?? existingUser.FirstName;
        existingUser.LastName = userModel.LastName ?? existingUser.LastName;
        existingUser.Description = userModel.Description ?? existingUser.Description;
        existingUser.Role = userModel.Role ?? existingUser.Role;

        existingUser.ModifiedDate = DateTime.Now;

        _userRepo.UpdateUsers(existingUser);

        return NoContent();
    }




    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="id">Delete the user by id.</param>
    /// <returns>Nothing</returns>
    /// <remarks>
    /// Sample request:
    ///     Post /api/users/1
    /// </remarks>
    /// <response code="202">Deletes the user</response>
    /// <response code="404">If the user is null</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteRobotCommand(int id)
    {
        var temp = _userRepo.GetUsers();
        var containsId = temp.FirstOrDefault(cmd => cmd.Id == id);
        if (containsId == null)
        {
            return NotFound();
        }
        _userRepo.DeleteUsers(id);
        return NoContent();
    }

    [HttpPatch("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateUserPassword(int id, LoginModel user)
    {
        // Get user by id
        UserModel? userModel = _userRepo.GetUsers().FirstOrDefault(e => e.Id == id);

        // Check if user is null or not
        if (userModel == null)
        {
            return NotFound();
        }

        if (user == null)
        {
            return BadRequest();
        }
        if (string.IsNullOrEmpty(userModel.FirstName) || string.IsNullOrEmpty(userModel.LastName) || string.IsNullOrEmpty(userModel.Role))
        {
            return BadRequest("First name, Last name or roles can be updated using the PUT method");
        }

        string password = user.PasswordHash;
        string pwHash = BCrypt.Net.BCrypt.HashPassword(password);

        userModel.Email = user.Email;
        userModel.PasswordHash = pwHash;
        userModel.ModifiedDate = DateTime.Now;

        _userRepo.UpdateUsers(userModel);
        return Ok(userModel);
    }
}
