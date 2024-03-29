using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data.Common;

namespace DropTool
{
    public class DropInfoLoader
    {
        private DBManager dbManager;
        private TreeView treeView;
        private Dictionary<string, Control> controls;

        public DropInfoLoader(DBManager dbManager, TreeView treeView, Form form)
        {
            this.dbManager = dbManager;
            this.treeView = treeView;

            // Zainicjalizuj kontrolki tutaj lub w oddzielnej metodzie
            controls = new Dictionary<string, Control>
            {
                { "TextBox_MonsterID", form.Controls["TextBox_MonsterID"] as TextBox },
                { "TextBox_DropName_1", form.Controls["TextBox_DropName_1"] as TextBox },
                { "TextBox_DropName_2",  form.Controls["TextBox_DropName_2" ] as TextBox },
                { "TextBox_DropName_3",  form.Controls["TextBox_DropName_3" ] as TextBox },
                { "TextBox_DropName_4",  form.Controls["TextBox_DropName_4" ] as TextBox },
                { "TextBox_DropName_5",  form.Controls["TextBox_DropName_5" ] as TextBox },
                { "TextBox_DropName_6",  form.Controls["TextBox_DropName_6" ] as TextBox },
                { "TextBox_DropName_7",  form.Controls["TextBox_DropName_7" ] as TextBox },
                { "TextBox_DropName_8",  form.Controls["TextBox_DropName_8" ] as TextBox },
                { "TextBox_DropName_9",  form.Controls["TextBox_DropName_9" ] as TextBox },
                { "TextBox_DropName_10", form.Controls["TextBox_DropName_10"] as TextBox },
                { "UpDown_DropMin_1",     form.Controls["UpDown_DropMin_1"     ] as NumericUpDown },
                { "UpDown_DropMin_2",     form.Controls["UpDown_DropMin_2"     ] as NumericUpDown },
                { "UpDown_DropMin_3",     form.Controls["UpDown_DropMin_3"     ] as NumericUpDown },
                { "UpDown_DropMin_4",     form.Controls["UpDown_DropMin_4"     ] as NumericUpDown },
                { "UpDown_DropMin_5",     form.Controls["UpDown_DropMin_5"     ] as NumericUpDown },
                { "UpDown_DropMin_6",     form.Controls["UpDown_DropMin_6"     ] as NumericUpDown },
                { "UpDown_DropMin_7",     form.Controls["UpDown_DropMin_7"     ] as NumericUpDown },
                { "UpDown_DropMin_8",     form.Controls["UpDown_DropMin_8"     ] as NumericUpDown },
                { "UpDown_DropMin_9",     form.Controls["UpDown_DropMin_9"     ] as NumericUpDown },
                { "UpDown_DropMin_10",    form.Controls["UpDown_DropMin_10"    ] as NumericUpDown },
                { "UpDown_DropMax_1",     form.Controls["UpDown_DropMax_1"     ] as NumericUpDown },
                { "UpDown_DropMax_2",     form.Controls["UpDown_DropMax_2"     ] as NumericUpDown },
                { "UpDown_DropMax_3",     form.Controls["UpDown_DropMax_3"     ] as NumericUpDown },
                { "UpDown_DropMax_4",     form.Controls["UpDown_DropMax_4"     ] as NumericUpDown },
                { "UpDown_DropMax_5",     form.Controls["UpDown_DropMax_5"     ] as NumericUpDown },
                { "UpDown_DropMax_6",     form.Controls["UpDown_DropMax_6"     ] as NumericUpDown },
                { "UpDown_DropMax_7",     form.Controls["UpDown_DropMax_7"     ] as NumericUpDown },
                { "UpDown_DropMax_8",     form.Controls["UpDown_DropMax_8"     ] as NumericUpDown },
                { "UpDown_DropMax_9",     form.Controls["UpDown_DropMax_9"     ] as NumericUpDown },
                { "UpDown_DropMax_10",    form.Controls["UpDown_DropMax_10"    ] as NumericUpDown },
                { "TextBox_DropChance_1", form.Controls["TextBox_DropChance_1" ] as MaskedTextBox },
                { "TextBox_DropChance_2", form.Controls["TextBox_DropChance_2" ] as MaskedTextBox },
                { "TextBox_DropChance_3", form.Controls["TextBox_DropChance_3" ] as MaskedTextBox },
                { "TextBox_DropChance_4", form.Controls["TextBox_DropChance_4" ] as MaskedTextBox },
                { "TextBox_DropChance_5", form.Controls["TextBox_DropChance_5" ] as MaskedTextBox },
                { "TextBox_DropChance_6", form.Controls["TextBox_DropChance_6" ] as MaskedTextBox },
                { "TextBox_DropChance_7", form.Controls["TextBox_DropChance_7" ] as MaskedTextBox },
                { "TextBox_DropChance_8", form.Controls["TextBox_DropChance_8" ] as MaskedTextBox },
                { "TextBox_DropChance_9", form.Controls["TextBox_DropChance_9" ] as MaskedTextBox },
                { "TextBox_DropChance_10",form.Controls["TextBox_DropChance_10"] as MaskedTextBox },
                { "Label_TotalChance"    ,form.Controls["Label_TotalChance"] as Label }
        };
        }

        public async Task LoadDropInfoAsync(int monsterId, int sub_id = -1)
        {
            ItemNameCache itemNameCache = ItemNameCache.GetInstance(dbManager);


            var itemIds = Enumerable.Range(0, 10).Select(i => $"drop_item_id_0{i}");

            string tableName = monsterId < 0 ? "DropGroupResource" : "MonsterDropTableResource";
            // Stwórz zapytanie zbierające informacje o przedmiotach
            string query = $@"SELECT * FROM {tableName} WHERE id = {monsterId}";

            if (sub_id != -1 && monsterId > 0)
            {
                query += $" AND sub_id = {sub_id}";
            }

            // Wykonaj zapytanie i odbierz wyniki
            DataTable dropInfoTable = await dbManager.ExecuteQueryAsync(query);

            if (dropInfoTable.Rows.Count > 0)
            {
                // Przypisz wyniki do kontrolek
                DataRow row = dropInfoTable.Rows[0];

                controls["TextBox_MonsterID"].Text = monsterId.ToString();

                for (int i = 0; i < 10; i++)
                {
                    int itemId = Convert.ToInt32(row[$"drop_item_id_0{i}"]);
                    if (itemId > 0)
                    {
                        string itemName = itemNameCache.GetItemName(itemId);
                        controls[$"TextBox_DropName_{i + 1}"].Text = itemName;
                    }
                    else if (itemId == 0)
                    {
                        controls[$"TextBox_DropName_{i + 1}"].Text = "Empty Slot";

                    }
                    else if (itemId <  0)
                    {
                        controls[$"TextBox_DropName_{i + 1}"].Text = row[$"drop_item_id_0{i}"].ToString();
                    }
                    controls[$"UpDown_DropMin_{i + 1}"].Text = row[$"drop_min_count_0{i}"].ToString();
                    controls[$"UpDown_DropMax_{i + 1}"].Text = row[$"drop_max_count_0{i}"].ToString();
                    controls[$"TextBox_DropChance_{i + 1}"].Text = row[$"drop_percentage_0{i}"].ToString();
                }
                SumDropChances();
            }
            else
            {
                MessageBox.Show("No drop information found for the selected monster.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SumDropChances()
        {
            double sum = 0.0;
            for (int i = 0;i < 10;i++)
            {
                sum += double.Parse(controls[$"TextBox_DropChance_{i + 1}"].Text);
            }
            controls["Label_TotalChance"].Text = $"Total: {sum.ToString("F9")}%";
        }

    }
}
