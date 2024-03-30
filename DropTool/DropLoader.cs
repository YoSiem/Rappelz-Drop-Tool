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

        public DropLoader(DBManager dbManager, TreeView treeView)
        {
            this.dbManager = dbManager;
            this.treeView = treeView;
            this.itemNameCache = ItemNameCache.GetInstance(dbManager);
        }

        public async Task LoadDropsAsync(int dropLinkId)
        {
            string query = $@"
                SELECT id, sub_id, {string.Join(", ", Enumerable.Range(0, 10).Select(i => $"drop_item_id_0{i}"))} 
                FROM MonsterDropTableResource 
                WHERE id = {dropLinkId};";

            DataTable dataTable = await dbManager.ExecuteQueryAsync(query);

            treeView.Nodes.Clear();
            treeView.BeginUpdate();

            if (dataTable.Rows.Count == 0)
            {
                TreeNode noDataNode = new TreeNode("No drop data available for this Droplink ID.");
                treeView.Nodes.Add(noDataNode);
                treeView.EndUpdate();
                return;
            }

            foreach (DataRow row in dataTable.Rows)
            {
                int id = Convert.ToInt32(row["id"]);
                int subId = Convert.ToInt32(row["sub_id"]);
                TreeNode subIdNode = new TreeNode($"Sub ID #{subId}")
                {
                    Name = subId.ToString(),
                    Tag = $"{id},{subId}"
                };

                int[] itemIDs = Enumerable.Range(0, 10).Select(i => Convert.ToInt32(row[$"drop_item_id_0{i}"])).ToArray();

                for (int i = 0; i < 10; i++)
                {
                    int itemID = Convert.ToInt32(row[$"drop_item_id_0{i}"]);
                    if (itemID > 0)
                    {
                        string itemName =  itemNameCache.GetItemName(itemID);
                        TreeNode itemNode = new TreeNode($"Item: {itemName}");
                        subIdNode.Nodes.Add(itemNode);
                    }
                    else if (itemID < 0)
                    {
                        TreeNode groupNode = new TreeNode($"Drop Group: ID {itemID}");
                        subIdNode.Nodes.Add(groupNode);
                        groupNode.Tag = itemID;
                        await FillSubIdNode(groupNode, itemID);
                    }
                    else
                    {
                        TreeNode emptyNode = new TreeNode("Empty Slot");
                        subIdNode.Nodes.Add(emptyNode);
                    }
                }

                treeView.Nodes.Add(subIdNode);
            }

            treeView.EndUpdate();
        }

        private async Task FillSubIdNode(TreeNode subIdNode, int groupId)
        {
            string query = $@"
                                SELECT id, {string.Join(", ", Enumerable.Range(0, 10).Select(i => $"drop_item_id_0{i}"))}
                                FROM DropGroupResource
                                WHERE id = {groupId};";

            DataTable dataTable = await dbManager.ExecuteQueryAsync(query);

            if (dataTable.Rows.Count == 0)
            {
                subIdNode.Nodes.Add(new TreeNode("No drops found"));
                return;
            }

            foreach (DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < 10; i++)
                {
                    string itemIdColumnName = $"drop_item_id_0{i}";

                    int itemId = row[itemIdColumnName] != DBNull.Value ? Convert.ToInt32(row[itemIdColumnName]) : 0;

                    if (itemId > 0)
                    {
                        string itemName = itemNameCache.GetItemName(itemId);
                        TreeNode itemNode = new TreeNode($"Item: {itemName} (ID: {itemId})");
                        subIdNode.Nodes.Add(itemNode);
                    }
                    else if (itemId < 0)
                    {     
                        TreeNode groupNode = new TreeNode($"DropGroup: ID {itemId}");
                        subIdNode.Nodes.Add(groupNode);
                        groupNode.Tag = itemId;
                        await FillSubIdNode(groupNode, itemId);
                    }
                    else if (itemId == 0)
                    {
                        TreeNode emptyNode = new TreeNode("Empty Slot");
                        subIdNode.Nodes.Add(emptyNode);
                    }
                }
            }
        }

    }
}
