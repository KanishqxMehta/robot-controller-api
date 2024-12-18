using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using robot_controller_api.Persistence;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

public interface IRepository
{
    private const string CONNECTION_STRING = "Host=localhost;Username=Kanishq_Mehta;Password=Kani@2004;Database=sit331";
    List<T> ExecuteReader<T>(string sqlCommand, NpgsqlParameter[] dbParams = null!) where T : class, new()
    {
        var entities = new List<T>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(sqlCommand, conn);

        if (dbParams is not null)
        {
            cmd.Parameters.AddRange(dbParams.Where(x => x.Value is not null).ToArray());
        }
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var entity = new T();
            dr.MapTo(entity);
            entities.Add(entity);
        }
        return entities;
    }
}