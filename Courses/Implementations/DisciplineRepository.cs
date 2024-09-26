using Courses.Abstractions;
using Courses.Configs;
using Courses.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Courses.Implementations;

public class DisciplineRepository : IDisciplineRepository
{
    private readonly string _connString;

    public DisciplineRepository(IOptions<BotConfig> config)
    {
        _connString = config.Value.ConnString;
    }
    
    public bool IsExists(int id)
    {
        using var conn = new SqlConnection(_connString);
        using var command = new SqlCommand("select top 1 * from disciplines where id = @id", conn);
        command.Parameters.AddWithValue("@id", id);
        conn.Open();
        using var reader = command.ExecuteReader();
        return reader.HasRows;
    }
    
    public bool IsExists(string name)
    {
        using var conn = new SqlConnection(_connString);
        using var command = new SqlCommand("select top 1 * from disciplines where name = @name", conn);
        command.Parameters.AddWithValue("@name", name);
        conn.Open();
        using var reader = command.ExecuteReader();
        return reader.HasRows;
    }
    
    public int Create(string name)
    {
        using var conn = new SqlConnection(_connString);
        using var command = new SqlCommand("insert into disciplines (name) output inserted.id values ( @name )", conn);
        command.Parameters.AddWithValue("@name", name);
        conn.Open();
        using var reader = command.ExecuteReader();
        reader.Read();
        return Convert.ToInt32(reader["id"]);
    }

    public DisciplineModel[] GetList()
    {
        var result = new List<DisciplineModel>();
        using var conn = new SqlConnection(_connString);
        using var command = new SqlCommand("select id, name from disciplines", conn);
        conn.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new DisciplineModel
            {
                DisciplineId = Convert.ToInt32(reader["id"]),
                DisciplineName = Convert.ToString(reader["name"]) ?? string.Empty
            });
        }

        return result.ToArray();
    }

    public void Delete(string name)
    {
        using var conn = new SqlConnection(_connString);
        using var command = new SqlCommand("delete from disciplines where name =  @name", conn);
        command.Parameters.AddWithValue("@name", name);
        conn.Open();
        command.ExecuteScalar();
    }

    public DisciplineModel[] GetAllWithTopics()
    {
        using var conn = new SqlConnection(_connString);
        using var command = new SqlCommand("select * from disciplines d inner join topics t on t.disciplineId = d.id", conn);
        var models = new List<DisciplineModel>();
        conn.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var topicModel = new TopicModel
            {
                DisciplineId = Convert.ToInt32(reader["disciplineId"]),
                TopicNumber = Convert.ToInt32(reader["topicNumber"]),
                TopicName = Convert.ToString(reader["topicName"]) ?? string.Empty,
                Lectures = Convert.ToInt32(reader["lectures"]),
                Practices = Convert.ToInt32(reader["practices"]),
            };

            var existingModel = models.FirstOrDefault(x => x.DisciplineId == topicModel.DisciplineId);
            if (existingModel == null)
            {
                existingModel = new DisciplineModel
                {
                    DisciplineId = topicModel.DisciplineId,
                    DisciplineName = Convert.ToString(reader["name"]) ?? string.Empty
                };
                models.Add(existingModel);
            }
            
            existingModel.Topics.Add(topicModel);
        }

        foreach (var m in models)
        {
            m.CalculateTotals();
        }

        return models.ToArray();
    }

    public void UpdateTopics(TopicModel[] topics)
    {
        var disciplineId = topics.First().DisciplineId;
        if (topics.Any(x => x.DisciplineId != disciplineId))
        {
            throw new Exception("All topics should be for the same discipline");
        }
        
        using var conn = new SqlConnection(_connString);
        using var command = new SqlCommand("delete from topics where disciplineId = @disciplineId", conn);
        command.Parameters.AddWithValue("@disciplineId", disciplineId);
        conn.Open();
        command.ExecuteNonQuery();

        foreach (var t in topics)
        {
            using var topicCommand = new SqlCommand("insert into topics (disciplineId, topicNumber, topicName, lectures, practices) " +
                "values (@disciplineId, @topicNumber, @topicName, @lectures, @practices)", conn);

            topicCommand.Parameters.AddWithValue("@disciplineId", disciplineId);
            topicCommand.Parameters.AddWithValue("@topicNumber", t.TopicNumber);
            topicCommand.Parameters.AddWithValue("@topicName", t.TopicName);
            topicCommand.Parameters.AddWithValue("@lectures", t.Lectures);
            topicCommand.Parameters.AddWithValue("@practices", t.Practices);

            topicCommand.ExecuteNonQuery();
        }
    }
}