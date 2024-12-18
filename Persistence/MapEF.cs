using robot_controller_api.Models;

public class MapEF : RobotContext, IMapDataAccess
{
    public readonly RobotContext _context;

    public MapEF(RobotContext context)
    {
        _context = context;
    }

    List<Map> IMapDataAccess.GetMaps()
    {
        return _context.Maps.ToList();
    }

        void IMapDataAccess.InsertMaps(Map newMapCommand)
        {
            newMapCommand.ModifiedDate = newMapCommand.CreatedDate = DateTime.Now;
            _context.Maps.Add(newMapCommand);
            _context.SaveChanges();
        }
    void IMapDataAccess.UpdateMaps(Map map)
    {
        Map maps = _context.Maps.FirstOrDefault(cmd => cmd.Id == map.Id)!;
        if (maps != null)
        {
            maps.Name = map.Name;
            maps.Description = map.Description;
            maps.Columns = map.Columns;
            maps.Rows = map.Rows;
            maps.ModifiedDate = DateTime.Now;

            _context.Maps.Update(maps);
            _context.SaveChanges();
        }
    }
    void IMapDataAccess.DeleteMaps(int id)
    {
        var mp = _context.Maps.FirstOrDefault(cmd => cmd.Id == id);
        if (mp != null)
        {
            _context.Maps.Remove(mp);
            _context.SaveChanges();
        }
    }
}