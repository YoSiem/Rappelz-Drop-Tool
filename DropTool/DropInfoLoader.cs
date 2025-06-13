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
        private readonly DBManager _dbManager;
        private readonly Dictionary<string, Control> _controls;
        private readonly Form _form;

        public DropInfoLoader(DBManager dbManager, Form form)
        {
            _dbManager = dbManager ?? throw new ArgumentNullException(nameof(dbManager));
            _form = form ?? throw new ArgumentNullException(nameof(form));

            _controls = form.Controls.OfType<Control>()
                .Where(c => c.Name.StartsWith("TextBox_") || c.Name.StartsWith("UpDown_") || c.Name.StartsWith("Label_"))
                .ToDictionary(c => c.Name, c => c);
        }

        public async Task LoadDropInfoAsync(int monsterId, int subId = -1)
        {
            try
            {
                var itemNameCache = ItemNameCache.GetInstance(_dbManager);
                string tableName = monsterId < 0 ? "DropGroupResource" : "MonsterDropTableResource";
                string query = $"SELECT * FROM {tableName} WHERE id = @id" + (subId != -1 ? " AND sub_id = @sub" : string.Empty);
                var parameters = new Dictionary<string, object> { {"@id", monsterId} };
                if (subId != -1)
                {
                    parameters.Add("@sub", subId);
                }

                var dropInfoTable = await _dbManager.ExecuteQueryAsync(query, parameters).ConfigureAwait(false);

                if (dropInfoTable.Rows.Count > 0)
                {
                    var row = dropInfoTable.Rows[0];
                    UpdateControls(row, itemNameCache, subId);
                }
                else
                {
                    MessageBox.Show("No drop information found for the selected monster.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load drop information: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateControls(DataRow row, ItemNameCache itemNameCache, int subId)
        {
            var dropId = Convert.ToInt32(row["id"]);

            var itemNames = new string[10];
            var itemTags = new int[10];
            var minCounts = new decimal[10];
            var maxCounts = new decimal[10];
            var dropChances = new string[10];

            for (int i = 1; i <= 10; i++)
            {
                int itemId = Convert.ToInt32(row[$"drop_item_id_0{i - 1}"]);
                string name;

                if (itemId == 0)
                {
                    name = "Empty Slot";
                }
                else if (itemId > 0)
                {
                    name = itemNameCache.GetItemName(itemId);
                }
                else
                {
                    name = $"Group: {itemId}";
                }

                itemNames[i - 1] = name;
                itemTags[i - 1] = itemId;
                minCounts[i - 1] = Convert.ToDecimal(row[$"drop_min_count_0{i - 1}"]);
                maxCounts[i - 1] = Convert.ToDecimal(row[$"drop_max_count_0{i - 1}"]);
                dropChances[i - 1] = row[$"drop_percentage_0{i - 1}"].ToString();
            }

            _form.Invoke(new Action(() =>
            {
                var textboxDropID = _controls["TextBox_MonsterID"];
                textboxDropID.Text = dropId.ToString();
                textboxDropID.Tag = subId.ToString();

                for (int i = 1; i <= 10; i++)
                {
                    var textBoxDropName = _controls[$"TextBox_DropName_{i}"];
                    textBoxDropName.Text = itemNames[i - 1];
                    textBoxDropName.Tag = itemTags[i - 1];

                    if (_controls[$"UpDown_DropMin_{i}"] is NumericUpDown upDownMin &&
                        _controls[$"UpDown_DropMax_{i}"] is NumericUpDown upDownMax)
                    {
                        upDownMin.Value = minCounts[i - 1];
                        upDownMax.Value = maxCounts[i - 1];
                    }

                    if (_controls[$"TextBox_DropChance_{i}"] is MaskedTextBox textBoxDropChance)
                    {
                        textBoxDropChance.Text = dropChances[i - 1];
                    }
                }

                double sum = 0.0;
                for (int i = 1; i <= 10; i++)
                {
                    if (double.TryParse(dropChances[i - 1], out double chance))
                    {
                        sum += chance;
                    }
                }

                if (_controls.TryGetValue("Label_TotalChance", out var labelTotalChance))
                {
                    labelTotalChance.Text = $"Total: {sum:F5}%";
                }
            }));
        }



    }
}
