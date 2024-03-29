using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DropTool
{
    public class MonsterLoader
    {
        private DBManager dbManager;
        private ListView listView;

        string SelectQuerry = " n.[value] as Monstername, [id], [drop_table_link_id], [level], loc.value as Location FROM dbo.MonsterResource id join dbo.StringResource n on id.[name_id] = n.[code] join dbo.StringResource loc on [location_id] = loc.code ";

        public MonsterLoader(DBManager dbManager, ListView listView)
        {
            this.dbManager = dbManager;
            this.listView = listView;
        }

        public async Task LoadMonstersAsync()
        {
            string query = $"SELECT {SelectQuerry} order by id asc;";
            await UpdateListViewAsync(query);
        }

        public async Task FilterMonstersByIdAsync(int monsterId)
        {
            string query = $"SELECT {SelectQuerry} WHERE id = {monsterId};";
            await UpdateListViewAsync(query);
        }

        public async Task FilterMonstersByNameAsync(string name)
        {
            string query = $"SELECT {SelectQuerry} WHERE n.[value] LIKE '%{name}%';";
            await UpdateListViewAsync(query);
        }

        public async Task FilterMonstersByLocationAsync(string location)
        {
            string query = $"SELECT {SelectQuerry} WHERE loc.value LIKE '%{location}%';";
            await UpdateListViewAsync(query);
        }

        public async Task ShowAllMonstersSortedByIdAsync()
        {
            string query = "SELECT {SelectQuerry} ORDER BY id ASC;";
            await UpdateListViewAsync(query);
        }

        private async Task UpdateListViewAsync(string query)
        {
            DataTable dataTable = await dbManager.ExecuteQueryAsync(query);
            listView.Invoke(new Action(() => listView.Items.Clear()));

            foreach (DataRow row in dataTable.Rows)
            {

                ListViewItem item = new ListViewItem(row["Monstername"].ToString());
                item.SubItems.Add(row["id"].ToString());
                item.SubItems.Add(row["drop_table_link_id"].ToString());
                item.SubItems.Add(row["Location"].ToString());
                item.Tag = row["level"];

                listView.Invoke(new Action(() => listView.Items.Add(item)));
            }
        }
    }
}
