﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

        private FilterType filterType;

        public MainWindow()
        {
            InitializeComponent();
            dbManager = new DBManager("Server=127.0.0.1;Database=Arcadia;User Id=sa;Password=Password123;");
            monsterLoader = new MonsterLoader(dbManager, ListView_Monsters);
            dropLoader = new DropLoader(dbManager, TreeView_DropView);
            dropInfoLoader = new DropInfoLoader(dbManager, TreeView_DropView, this);
        }

        private async void Btn_LoadFromDB_Click(object sender, EventArgs e)
        {
            await monsterLoader.LoadMonstersAsync();
        }

        private async void ListView_Monsters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListView_Monsters.SelectedItems.Count > 0)
            {
                // Zakładając, że DropLink to ID znajdujące się w pierwszej kolumnie (subitem[0])
                int dropLinkId = Convert.ToInt32(ListView_Monsters.SelectedItems[0].SubItems[2].Text);
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

        private async void MainWindow_Load(object sender, EventArgs e)
        {
            await monsterLoader.LoadMonstersAsync();
                        await dropLoader.LoadMonsterDropsAsync();
        }

        private void ComboBox_SearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBox_SearchType.SelectedIndex < 0 || ComboBox_SearchType.SelectedIndex > 3)
            {
                return; 
            }

            filterType = (FilterType)ComboBox_SearchType.SelectedIndex;

        }

        private void Btn_FilterMonsters_Click(object sender, EventArgs e)
        {
            IEnumerable<Monster> filteredMonsters = Enumerable.Empty<Monster>();

            if (filterType == FilterType.AllMonsters)
            {
                filteredMonsters = monsterLoader.ShowAllMonstersSortedById();
                monsterLoader.UpdateListView(filteredMonsters);
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
                                    filteredMonsters = monsterLoader.FilterMonstersById(monsterId);
                                }
                                break;
                            case FilterType.MonsterName:
                                filteredMonsters = monsterLoader.FilterMonstersByName(input);
                                break;
                            case FilterType.MonsterLocation:
                                filteredMonsters = monsterLoader.FilterMonstersByLocation(input);
                                break;
                        }

                        monsterLoader.UpdateListView(filteredMonsters);
                    }
                }
            }
        }

    }
}