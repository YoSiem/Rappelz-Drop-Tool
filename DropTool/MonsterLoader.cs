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
        public List<Monster> MonsterCache = new List<Monster>();

        public MonsterLoader(DBManager dbManager, ListView listView)
        {
            this.dbManager = dbManager;
            this.listView = listView;
        }

        public async Task LoadMonstersAsync()
        {          
            string query = "SELECT n.[value] as Monstername, [id], [drop_table_link_id], [level], loc.value as Location FROM dbo.MonsterResource id join dbo.StringResource n on id.[name_id] = n.[code] join dbo.StringResource loc on [location_id] = loc.code order by id asc;";
            DataTable dataTable = await dbManager.ExecuteQueryAsync(query);

            listView.Items.Clear();

            foreach (DataRow row in dataTable.Rows)
            {
                int M_ID = Convert.ToInt32(row["id"]);
                string M_Name = Convert.ToString(row["Monstername"]);
                string M_Location = Convert.ToString(row["Location"]);
                int M_Level = Convert.ToInt32(row["level"]);
                int M_DropLink = Convert.ToInt32(row["drop_table_link_id"]);

                ListViewItem item = new ListViewItem(M_Name);
                item.SubItems.Add(M_ID.ToString());
                item.SubItems.Add(M_DropLink.ToString());
                item.SubItems.Add(M_Location);

                MonsterCache.Add(new Monster(M_ID, M_Name, M_Location, M_Level, M_DropLink));
                listView.Items.Add(item);
            }
        }
        public IEnumerable<Monster> FilterMonstersById(int monsterId)
        {
            return MonsterCache.Where(monster => monster.MonsterID == monsterId);
        }
        public IEnumerable<Monster> FilterMonstersByName(string name)
        {
            return MonsterCache.Where(monster => monster.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public IEnumerable<Monster> FilterMonstersByLocation(string location)
        {
            return MonsterCache.Where(monster => monster.Location.IndexOf(location, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public IEnumerable<Monster> ShowAllMonstersSortedById()
        {
            return MonsterCache.OrderBy(monster => monster.MonsterID);
        }

        public void UpdateListView(IEnumerable<Monster> filteredMonsters)
        {
            listView.Items.Clear();

            foreach (var monster in filteredMonsters)
            {
                ListViewItem item = new ListViewItem(monster.Name);
                item.SubItems.Add(monster.MonsterID.ToString());
                item.SubItems.Add(monster.DropLink.ToString());
                item.SubItems.Add(monster.Location);
                listView.Items.Add(item);
            }
        }
    }

    public class Monster
    {
        public int MonsterID { get; private set; }
        public string Name { get; private set; }
        public string Location { get; private set; }
        public int Level { get; private set; }
        public int DropLink { get; private set; }

        public Monster(int monsterID, string name, string location, int level, int dropLink)
        {
            MonsterID = monsterID;
            Name = name;
            Location = location;
            Level = level;
            DropLink = dropLink;
        }
    }
}
