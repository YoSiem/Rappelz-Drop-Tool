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
            var parameters = new Dictionary<string, object>();

            for (int i = 1; i <= 10; i++)
            {
                int itemId = Convert.ToInt32(_controls[$"TextBox_DropName_{i}"].Tag);
                int min = Convert.ToInt32(_controls[$"UpDown_DropMin_{i}"].Text);
                int max = Convert.ToInt32(_controls[$"UpDown_DropMax_{i}"].Text);
                double chance = Convert.ToDouble(_controls[$"TextBox_DropChance_{i}"].Text);

                dropInfo.DropItemIds[i - 1] = itemId;
                dropInfo.DropCountMin[i - 1] = min;
                dropInfo.DropCountMax[i - 1] = max;
                dropInfo.DropItemChances[i - 1] = chance;

                queryBuilder.Append($"drop_item_id_0{i-1} = @ItemId{i}, ");
                queryBuilder.Append($"drop_min_count_0{i-1} = @Min{i}, ");
                queryBuilder.Append($"drop_max_count_0{i-1} = @Max{i}, ");
                queryBuilder.Append($"drop_percentage_0{i-1} = @Chance{i}, ");

                parameters[$"@ItemId{i}"] = itemId;
                parameters[$"@Min{i}"] = min;
                parameters[$"@Max{i}"] = max;
                parameters[$"@Chance{i}"] = chance;
            }

            queryBuilder.Length -= 2;

            queryBuilder.Append(" WHERE id = @Id");
            parameters["@Id"] = dropInfo.Id;

            if (dropInfo.Id > 0)
            {
                queryBuilder.Append(" AND sub_id = @SubId");
                parameters["@SubId"] = dropInfo.SubId;
            }

            await _dbManager.ExecuteNonQueryAsync(queryBuilder.ToString(), parameters).ConfigureAwait(false);
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
