using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DropTool
{
    public class DropLoader
    {
        private DBManager dbManager;
        private TreeView treeView;
        ItemNameCache itemNameCache;
        public List<DropStruct> DropCache = new List<DropStruct>();

        public DropLoader(DBManager dbManager, TreeView treeView)
        {
            this.dbManager = dbManager;
            this.treeView = treeView;
            this.itemNameCache = ItemNameCache.GetInstance(dbManager);
        }

        public async Task LoadMonsterDropsAsync()
        {
            string query = $"SELECT id, sub_id, {string.Join(", ", Enumerable.Range(0, 10).Select(i => $"drop_item_id_0{i}, drop_min_count_0{i}, drop_max_count_0{i}, drop_percentage_0{i}"))} FROM MonsterDropTableResource UNION ALL SELECT id, 0 AS sub_id, {string.Join(", ", Enumerable.Range(0, 10).Select(i => $"drop_item_id_0{i}, drop_min_count_0{i}, drop_max_count_0{i}, drop_percentage_0{i}"))} FROM DropGroupResource;";

            DataTable dataTable = await dbManager.ExecuteQueryAsync(query);

            DropCache.Clear();

            foreach (DataRow row in dataTable.Rows)
            {
                int id = Convert.ToInt32(row["id"]);
                int subId = Convert.ToInt32(row["sub_id"]);

                int[] itemIDs = Enumerable.Range(0, 10).Select(i => Convert.ToInt32(row[$"drop_item_id_0{i}"])).ToArray();
                int[] minDrops = Enumerable.Range(0, 10).Select(i => Convert.ToInt32(row[$"drop_min_count_0{i}"])).ToArray();
                int[] maxDrops = Enumerable.Range(0, 10).Select(i => Convert.ToInt32(row[$"drop_max_count_0{i}"])).ToArray();
                float[] dropChances = Enumerable.Range(0, 10).Select(i => float.Parse(row[$"drop_percentage_0{i}"].ToString())).ToArray();

                DropStruct newDrop = new DropStruct(id, itemIDs, minDrops, maxDrops, dropChances, subId);
                DropCache.Add(newDrop);
            }
        }

        public async Task LoadDropsAsync(int dropLinkId)
        {
            treeView.Nodes.Clear();

            // Filtruj DropCache, aby znaleźć wszystkie wpisy pasujące do podanego dropLinkId
            var filteredDrops = DropCache.Where(drop => drop.Id == dropLinkId).ToList();

            for (int subId = 1; subId <= 9; subId++)
            {
                // Znajdź wpis w cache odpowiadający danemu subId
                var drop = filteredDrops.FirstOrDefault(d => d.subId == subId);

                TreeNode subIdNode = new TreeNode($"Sub ID #{subId}")
                {
                    Name = subId.ToString(),
                    Tag = $"{dropLinkId},{subId}"
                };

                // Jeżeli znaleziono odpowiadający wpis w cache, wypełnij węzeł danymi
                if (drop != null)
                {
                    await FillSubIdNode(subIdNode, drop);
                }

                treeView.Nodes.Add(subIdNode);
            }
        }


        private async Task FillSubIdNode(TreeNode subIdNode, DropStruct drop)
        {
            for (int i = 0; i < drop.ItemID.Length; i++)
            {
                if (drop.ItemID[i] > 0)
                {
                    string itemName = await itemNameCache.GetItemNameAsync(drop.ItemID[i]);
                    TreeNode itemNode = new TreeNode($"Item: {itemName}");
                    subIdNode.Nodes.Add(itemNode);
                }
                else if (drop.ItemID[i] < 0)
                {
                    var groupDrop = DropCache.FirstOrDefault(d => d.Id == drop.ItemID[i]);
                    if (groupDrop != null)
                    {
                        TreeNode groupNode = new TreeNode($"DropGroup: {drop.ItemID[i]}");
                        groupNode.Tag = drop.ItemID[i];
                        subIdNode.Nodes.Add(groupNode);
                        await FillSubIdNode(groupNode, groupDrop);
                    }
                    else
                    {
                        TreeNode groupNode = new TreeNode($"DropGroup: {drop.ItemID[i]} not Found????");
                        subIdNode.Nodes.Add(groupNode);
                    }
                }
                else
                {
                    TreeNode emptyNode = new TreeNode("Empty Slot");
                    subIdNode.Nodes.Add(emptyNode);
                }
            }
        }

    }

    public class DropStruct
    {
        public int Id { get; set; }
        public int subId { get; set; }
        public int[] ItemID { get; set; }
        public int[] minDrop {  get; set; }
        public int[] maxDrop { get; set;}
        public float[] dropChance { get; set; }

        public DropStruct(int id, int[] itemID, int[] minDrop, int[] maxDrop, float[] dropChance, int subID = 0) 
        {
            this.Id = id;
            this.subId = subID;
            this.ItemID = itemID;
            this.minDrop = minDrop;
            this.maxDrop = maxDrop;
            this.dropChance = dropChance;
        }

    }
}
