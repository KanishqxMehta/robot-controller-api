using robot_controller_api.Models;

public interface IMapDataAccess
{
    public List<Map> GetMaps();
    public void InsertMaps(Map map);
    public void UpdateMaps(Map map);
    public void DeleteMaps(int id);
}