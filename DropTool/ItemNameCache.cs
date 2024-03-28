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

    public async Task<string> GetItemNameAsync(int itemId)
    {
        // Jeśli nazwa jest już w cache, zwróć ją
        if (_cache.TryGetValue(itemId, out string name))
        {
            return name;
        }

        // W przeciwnym razie, pobierz nazwę z bazy danych
        string query = $@"
            SELECT n.[value] as ItemName
            FROM ItemResource as id
            LEFT JOIN StringResource as n ON id.[name_id] = n.[code]
            WHERE id.[id] = {itemId}";

        DataTable result = await _dbManager.ExecuteQueryAsync(query);

        string itemName = result.Rows.Count > 0 ? result.Rows[0]["ItemName"].ToString() : "Unknown Item";

        // Dodaj nazwę przedmiotu do cache
        _cache[itemId] = itemName;

        return itemName;
    }

    public void AddToCache(int itemId, string itemName)
    {
        if (!_cache.ContainsKey(itemId))
        {
            _cache[itemId] = itemName;
        }
    }
}
