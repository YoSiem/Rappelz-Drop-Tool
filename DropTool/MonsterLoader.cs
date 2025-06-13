using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DropTool
{
    public class MonsterLoader
    {
        private DBManager dbManager;
        private ListView listView;

        private const string SelectQuery = " n.[value] as Monstername, [id], [drop_table_link_id], [level], loc.value as Location FROM dbo.MonsterResource id join dbo.StringResource n on id.[name_id] = n.[code] join dbo.StringResource loc on [location_id] = loc.code ";

        public MonsterLoader(DBManager dbManager, ListView listView)
        {
            this.dbManager = dbManager;
            this.listView = listView;
        }

        public async Task LoadMonstersAsync()
        {
            string query = $"SELECT {SelectQuery} order by id asc;";
            await UpdateListViewAsync(query, null);
        }

        public async Task FilterMonstersByIdAsync(int monsterId)
        {
            string query = $"SELECT {SelectQuery} WHERE id = @id;";
            var parameters = new Dictionary<string, object> { {"@id", monsterId} };
            await UpdateListViewAsync(query, parameters);
        }

        public async Task FilterMonstersByNameAsync(string name)
        {
            string query = $"SELECT {SelectQuery} WHERE n.[value] LIKE @name;";
            var parameters = new Dictionary<string, object> { {"@name", $"%{name}%"} };
            await UpdateListViewAsync(query, parameters);
        }

        public async Task FilterMonstersByLocationAsync(string location)
        {
            string query = $"SELECT {SelectQuery} WHERE loc.value LIKE @loc;";
            var parameters = new Dictionary<string, object> { {"@loc", $"%{location}%"} };
            await UpdateListViewAsync(query, parameters);
        }

        public async Task ShowAllMonstersSortedByIdAsync()
        {
            string query = $"SELECT {SelectQuery} ORDER BY id ASC;";
            await UpdateListViewAsync(query, null);
        }

        private async Task UpdateListViewAsync(string query, Dictionary<string, object> parameters)
        {
            DataTable dataTable = await dbManager.ExecuteQueryAsync(query, parameters).ConfigureAwait(false);

            var items = new List<ListViewItem>();

            foreach (DataRow row in dataTable.Rows)
            {
                ListViewItem item = new ListViewItem(row["Monstername"].ToString());
                item.SubItems.Add(row["id"].ToString());
                item.SubItems.Add(row["drop_table_link_id"].ToString());
                item.SubItems.Add(row["Location"].ToString());
                item.Tag = row["level"];
                items.Add(item);
            }

            listView.Invoke(new Action(() =>
            {
                listView.BeginUpdate();
                listView.Items.Clear();
                listView.Items.AddRange(items.ToArray());
                listView.EndUpdate();
            }));
        }
    }
}
