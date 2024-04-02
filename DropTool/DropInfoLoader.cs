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

        public DropInfoLoader(DBManager dbManager, Form form)
        {
            _dbManager = dbManager ?? throw new ArgumentNullException(nameof(dbManager));

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
                string query = $"SELECT * FROM {tableName} WHERE id = {monsterId}" + (subId != -1 ? $" AND sub_id = {subId}" : string.Empty);

                var dropInfoTable = await _dbManager.ExecuteQueryAsync(query).ConfigureAwait(false);

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
            string itemName;
            var dropID = Convert.ToInt32(row["id"]);
            var textboxDropID = _controls["TextBox_MonsterID"];
            textboxDropID.Invoke(new Action(() => textboxDropID.Text = dropID.ToString()));
            textboxDropID.Invoke(new Action(() => textboxDropID.Tag = subId.ToString()));

            for (int i = 1; i <= 10; i++)
            {
                var itemId = Convert.ToInt32(row[$"drop_item_id_0{i - 1}"]);

                var textBoxDropName = _controls[$"TextBox_DropName_{i}"];

                if (itemId == 0)
                {
                    itemName = "Empty Slot";
                    textBoxDropName.Invoke(new Action(() => textBoxDropName.Tag = 0));
                }

                else if (itemId > 0)
                {
                    itemName = itemNameCache.GetItemName(itemId);
                    textBoxDropName.Invoke(new Action(() => textBoxDropName.Tag = itemId));
                }
                else
                {
                    itemName = $"Group: {itemId}";
                    textBoxDropName.Invoke(new Action(() => textBoxDropName.Tag = itemId));
                }

                textBoxDropName.Invoke(new Action(() => textBoxDropName.Text = itemName));

                var upDownMin = _controls[$"UpDown_DropMin_{i}"] as NumericUpDown;
                var upDownMax = _controls[$"UpDown_DropMax_{i}"] as NumericUpDown;


                if (upDownMin != null && upDownMax != null)
                {
                    var minCount = Convert.ToDecimal(row[$"drop_min_count_0{i - 1}"]);
                    var maxCount = Convert.ToDecimal(row[$"drop_max_count_0{i - 1}"]);

                    upDownMin.Invoke(new Action(() => upDownMin.Value = minCount));
                    upDownMax.Invoke(new Action(() => upDownMax.Value = maxCount));
                }


                var textBoxDropChance = _controls[$"TextBox_DropChance_{i}"] as MaskedTextBox;
                if (textBoxDropChance != null)
                {
                    var dropChanceText = row[$"drop_percentage_0{i - 1}"].ToString();
                    textBoxDropChance.Invoke(new Action(() => textBoxDropChance.Text = dropChanceText));
                }
            }
            SumDropChances();
        }
        private void SumDropChances()
        {
            double sum = 0.0;
            for (int i = 1; i <= 10; i++)
            {
                if (double.TryParse(_controls[$"TextBox_DropChance_{i}"].Text, out double chance))
                {
                    sum += chance;
                }
            }

            var labelTotalChance = _controls["Label_TotalChance"];
            if (labelTotalChance != null)
            {
                labelTotalChance.Invoke(new Action(() => labelTotalChance.Text = $"Total: {sum:F5}%"));
            }
        }


    }
}
