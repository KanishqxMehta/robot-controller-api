using robot_controller_api.Models;

public interface IRobotCommandDataAccess
{
    public List<RobotCommand> GetRobotCommands();
    public void InsertRobotCommand(RobotCommand robotCommand);
    public void UpdateRobotCommand(RobotCommand robotCommand);
    public void DeleteRobotCommand(int id);
}