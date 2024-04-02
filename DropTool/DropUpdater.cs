using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DropTool
{
    public class DropUpdater
    {
        private readonly DBManager _dbManager;
        private readonly Dictionary<string, Control> _controls;

        public DropUpdater(DBManager dbManager, Form form)
        {
            _dbManager = dbManager ?? throw new ArgumentNullException(nameof(dbManager));

            _controls = form.Controls.OfType<Control>()
                .Where(c => c.Name.StartsWith("TextBox_") || c.Name.StartsWith("UpDown_") || c.Name.StartsWith("Label_"))
                .ToDictionary(c => c.Name, c => c);
        }

        public async Task UpdateDropInfoAsync()
        {
            DropRow dropInfo = new DropRow();
            dropInfo.Id = Convert.ToInt32(_controls["TextBox_MonsterID"].Text);
            dropInfo.SubId = Convert.ToInt32(_controls["TextBox_MonsterID"].Tag);

            string tableName = dropInfo.Id < 0 ? "DropGroupResource" : "MonsterDropTableResource";
            var queryBuilder = new StringBuilder($"UPDATE {tableName} SET ");

            for (int i = 1; i <= 10; i++)
            {
                var textBoxDropName = _controls[$"TextBox_DropName_{i}"].Tag.ToString();
                var upDownMin = _controls[$"UpDown_DropMin_{i}"].Text;
                var upDownMax = _controls[$"UpDown_DropMax_{i}"].Text;
                var textBoxDropChance = _controls[$"TextBox_DropChance_{i}"].Text;

                dropInfo.DropItemIds[i - 1] = Convert.ToInt32(textBoxDropName);
                dropInfo.DropCountMin[i - 1] = Convert.ToInt32(upDownMin);
                dropInfo.DropCountMax[i - 1] = Convert.ToInt32(upDownMax);
                dropInfo.DropItemChances[i - 1] = Convert.ToDouble(textBoxDropChance);

                queryBuilder.Append($"drop_item_id_0{i-1} = {dropInfo.DropItemIds[i - 1]}, ");
                queryBuilder.Append($"drop_min_count_0{i-1} = {dropInfo.DropCountMin[i - 1]}, ");
                queryBuilder.Append($"drop_max_count_0{i-1} = {dropInfo.DropCountMax[i - 1]}, ");
                queryBuilder.Append($"drop_percentage_0{i-1} = {dropInfo.DropItemChances[i - 1].ToString(System.Globalization.CultureInfo.InvariantCulture)}, ");
            }

            queryBuilder.Length -= 2;

            queryBuilder.Append($" WHERE id = {dropInfo.Id}");

            if (dropInfo.Id > 0)
            {
                queryBuilder.Append($" AND sub_id = {dropInfo.SubId}");
            }

            await _dbManager.ExecuteNonQueryAsync(queryBuilder.ToString());
            //MessageBox.Show(queryBuilder.ToString(), "Wygenerowane zapytanie SQL", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

    }

    public class DropRow
    {
        public int Id = 0;
        public int SubId = 0;
        public int[] DropItemIds = new int[10];
        public int[] DropCountMin = new int[10];
        public int[] DropCountMax = new int[10];
        public double[] DropItemChances = new double[10];
    }
}
