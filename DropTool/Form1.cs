using System;
using System.Windows.Forms;


namespace DropTool
{
    public enum FilterType
    {
        AllMonsters,
        MonsterID,
        MonsterName,
        MonsterLocation
    }
    public partial class MainWindow : Form
    {
        private DBManager dbManager;
        private MonsterLoader monsterLoader;
        private DropLoader dropLoader;
        private DropInfoLoader dropInfoLoader;
        private DropUpdater dropUpdater;

        private FilterType filterType;

        public MainWindow()
        {
            InitializeComponent();
            dbManager = new DBManager();
            monsterLoader = new MonsterLoader(dbManager, ListView_Monsters);
            dropLoader = new DropLoader(dbManager, TreeView_DropView);
            dropInfoLoader = new DropInfoLoader(dbManager, this);
            dropUpdater = new DropUpdater(dbManager, this);
            Load += async (sender, e) => await ItemNameCache.GetInstance(dbManager).InitializeCacheAsync();
            Load += async (sender, e) => await monsterLoader.LoadMonstersAsync();
        }

        private async void Btn_LoadFromDB_Click(object sender, EventArgs e)
        {
            await monsterLoader.LoadMonstersAsync();
        }

        private async void ListView_Monsters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListView_Monsters.SelectedItems.Count > 0)
            {
                int dropLinkId = Convert.ToInt32(ListView_Monsters.SelectedItems[0].SubItems[2].Text);
                int monsterLevel = Convert.ToInt32(ListView_Monsters.SelectedItems[0].Tag);
                string monsterLocation = ListView_Monsters.SelectedItems[0].SubItems[3].Text;

                Label_MonsterInfo.Text = $"Level:{monsterLevel}  /  Location: {monsterLocation}";
                await dropLoader.LoadDropsAsync(dropLinkId);
            }
        }

        private async void TreeView_DropView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is int monsterId)
            {
                await dropInfoLoader.LoadDropInfoAsync(monsterId);
            }
            else if (e.Node.Tag is string monsterString)
            {
                string[] parts = monsterString.Split(',');
                await dropInfoLoader.LoadDropInfoAsync(int.Parse(parts[0]), int.Parse(parts[1]));
            }
        }

        private  void MainWindow_Load(object sender, EventArgs e)
        {

        }

        private void ComboBox_SearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBox_SearchType.SelectedIndex < 0 || ComboBox_SearchType.SelectedIndex > 3)
            {
                return; 
            }

            filterType = (FilterType)ComboBox_SearchType.SelectedIndex;

        }

        private async void Btn_FilterMonsters_Click(object sender, EventArgs e)
        {
            if (filterType == FilterType.AllMonsters)
            {
                await monsterLoader.ShowAllMonstersSortedByIdAsync();
            }
            else
            {
                using (var searchDialog = new SearchDialogForm(filterType))
                {
                    if (searchDialog.ShowDialog() == DialogResult.OK)
                    {
                        var input = searchDialog.SearchInput;

                        switch (filterType)
                        {
                            case FilterType.MonsterID:
                                if (int.TryParse(input, out int monsterId))
                                {
                                    await monsterLoader.FilterMonstersByIdAsync(monsterId);
                                }
                                break;
                            case FilterType.MonsterName:
                                await monsterLoader.FilterMonstersByNameAsync(input);
                                break;
                            case FilterType.MonsterLocation:
                                await monsterLoader.FilterMonstersByLocationAsync(input);
                                break;
                        }
                    }
                }
            }
        }

        private async void Btn_SaveDrop_Click(object sender, EventArgs e)
        {
            bool isNumber = int.TryParse(TextBox_MonsterID.Text, out int number);
            if (!isNumber) { return; }
            await dropUpdater.UpdateDropInfoAsync();
            MessageBox.Show("Update Compleated", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
