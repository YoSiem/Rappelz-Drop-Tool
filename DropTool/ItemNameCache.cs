using DropTool;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

public class ItemNameCache
{
    private static ItemNameCache _instance;
    private readonly Dictionary<int, string> _cache = new Dictionary<int, string>();
    private readonly DBManager _dbManager;

    private ItemNameCache(DBManager dbManager)
    {
        _dbManager = dbManager;
    }

    public static ItemNameCache GetInstance(DBManager dbManager)
    {
        if (_instance == null)
        {
            _instance = new ItemNameCache(dbManager);
        }
        return _instance;
    }

    public async Task InitializeCacheAsync()
    {
        string query = @"
            SELECT id, n.[value] as ItemName
            FROM ItemResource as id
            LEFT JOIN StringResource as n ON id.[name_id] = n.[code];";

        DataTable result = await _dbManager.ExecuteQueryAsync(query);

        foreach (DataRow row in result.Rows)
        {
            int id = row.Field<int>("id");
            string itemName = row.Field<string>("ItemName");
            _cache[id] = itemName;
        }
    }

    public string GetItemName(int itemId)
    {
        if (_cache.TryGetValue(itemId, out string name))
        {
            return name;
        }

        return "Unknown Item";
    }


}
