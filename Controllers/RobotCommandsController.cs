using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;
using robot_controller_api.Models;
using Microsoft.AspNetCore.Authorization;
namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{ 
    private readonly IRobotCommandDataAccess _robotCommandsRepo; 
    public RobotCommandsController(IRobotCommandDataAccess robotCommandsRepo) => _robotCommandsRepo = robotCommandsRepo;
    

    /// <summary>
    /// Retrieves all robot commands.
    /// </summary>
    /// <returns>A list of all robot commands</returns>
    [HttpGet(""), Authorize(Policy = "UserOnly")]
    public IEnumerable<RobotCommand> GetAllRobotCommands() { return _robotCommandsRepo.GetRobotCommands(); }
 
    /// <summary>
    /// Retrieves all robot commands with move.
    /// </summary>
    /// <returns>A list of all robot commands with move == true</returns>
    [HttpGet("move"), Authorize(Policy = "UserOnly")]
    public IEnumerable<RobotCommand> GetMoveCommandsOnly()
    {
        return _robotCommandsRepo.GetRobotCommands().Where(command => command.IsMoveCommand);
    }

    /// <summary>
    /// Retrieves robot commands by id.
    /// </summary>
    /// <returns>Robot command by id</returns>
    [HttpGet("{id}", Name = "GetRobotCommand"), Authorize(Policy = "UserOnly")]
    public IActionResult GetRobotCommandsById(int id)
    {
        var temp = _robotCommandsRepo.GetRobotCommands();
        var containsId = temp.FirstOrDefault(cmd => cmd.Id == id);

        if (containsId != null)
        {
            return Ok(temp.Where(command => command.Id == id));
        }
        else
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Creates a robot command.
    /// </summary>
    /// <param name="newCommand">A new robot command from the HTTP request.</param>
    /// <returns>A newly created robot command</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/robot-commands
    ///     {
    ///        "name": "DANCE",
    ///        "isMoveCommand": true,
    ///        "description": "Salsa on the Moon"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created robot command</response>
    /// <response code="400">If the robot command is null</response>
    /// <response code="409">If a robot command with the same name already exists.</response>    
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost(), Authorize(Policy = "AdminOnly")]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        if (newCommand == null)
        {
            return BadRequest();
        }

        if(newCommand.Description == null)
        {
            newCommand.Description = "N/A";
        }
        newCommand.CreatedDate = DateTime.Now;
        newCommand.ModifiedDate = DateTime.Now;
        _robotCommandsRepo.InsertRobotCommand(newCommand);
        newCommand.Id = _robotCommandsRepo.GetRobotCommands().Max(x => x.Id) + 1;

        return CreatedAtRoute("GetRobotCommand", new { id = newCommand.Id }, newCommand);
    }

    /// <summary>
    /// Updates a robot command.
    /// </summary>
    /// <param name="updatedCommand">A updated robot command from the HTTP request.</param>
    /// <param name="id">Updates the command with id.</param>
    /// <returns>A updated robot command</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/robot-commands
    ///     {
    ///        "name": "DANCE",
    ///        "isMoveCommand": true,
    ///        "description": "Salsa on the Moon"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created robot command</response>
    /// <response code="400">If the robot command is null</response>
    /// <response code="409">If a robot command with the same name already exists.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        var temp = _robotCommandsRepo.GetRobotCommands().FirstOrDefault(x => x.Id == id);
        if (temp == null)
        {
            return NotFound();
        }

        if(updatedCommand.Description == null)
        {
            updatedCommand.Description = "N/A";
        }

        // Check if the existing command is found
        if (updatedCommand != null)
        {
            // Update the fields of the existing command with the provided updatedCommand
            updatedCommand.Id = id;
            updatedCommand.IsMoveCommand = false;
            updatedCommand.ModifiedDate = DateTime.Now;

            _robotCommandsRepo.UpdateRobotCommand(updatedCommand);

            // Return a success response with NoContent status
            return NoContent();
        }
        else
        {
            // If the existing command is not found, return NotFound
            return NotFound();
        }
    }

    /// <summary>
    /// Deletes a robot command.
    /// </summary>
    /// <param name="id">Delete the robotcommand by id.</param>
    /// <returns>Nothing</returns>
    /// <remarks>
    /// Sample request:
     ///     Post /api/robot-commands/1
    /// </remarks>
    /// <response code="202">Deletes the command</response>
    /// <response code="404">If the robot command is null</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteRobotCommand(int id)
    {
        var temp = _robotCommandsRepo.GetRobotCommands();
        var containsId = temp.FirstOrDefault(cmd => cmd.Id == id);
        if (containsId == null)
        {
            return NotFound();
        }
        _robotCommandsRepo.DeleteRobotCommand(id);
        return NoContent();
    }
}