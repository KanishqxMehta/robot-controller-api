using robot_controller_api.Models;

public class RobotCommandEF : IRepository, IRobotCommandDataAccess
{
    private readonly RobotContext _context;

    public RobotCommandEF(RobotContext context)
    {
        _context = context;
    }


    public List<RobotCommand> GetRobotCommands() => _context.Robotcommands.ToList();

    public void InsertRobotCommand(RobotCommand command)
    {
            command.CreatedDate = DateTime.Now;
            command.ModifiedDate = DateTime.Now;

            _context.Robotcommands.Add(command);
            _context.SaveChanges();
    }

    public void UpdateRobotCommand(RobotCommand robotCommand)
    {
        RobotCommand command = _context.Robotcommands.FirstOrDefault(cmd => cmd.Id == robotCommand.Id)!;
        if (command != null)
        {
            command.Name = robotCommand.Name;
            command.Description = robotCommand.Description;
            command.IsMoveCommand = robotCommand.IsMoveCommand;
            command.ModifiedDate = DateTime.Now;

            _context.Robotcommands.Update(command);
            _context.SaveChanges();
        }
    }
    public void DeleteRobotCommand(int id)
    {
        var command = _context.Robotcommands.FirstOrDefault(cmd => cmd.Id == id);
        if (command != null)
        {
            _context.Robotcommands.Remove(command);
            _context.SaveChanges();
        }
    }
}