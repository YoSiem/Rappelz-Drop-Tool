using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DropTool
{
    public partial class SearchDialogForm : Form
    {
        public string SearchInput => TextBox_SearchInput.Text;

        public SearchDialogForm(FilterType type)
        {
            InitializeComponent();

            if (type == FilterType.MonsterID)
            {
                TextBox_SearchInput.Mask = "9999999999999";
                Label_MainText.Text = "Provide Monster ID";
            }
            else if (type == FilterType.MonsterName)
            {
                TextBox_SearchInput.Mask = "";
                Label_MainText.Text = "Provide Monster Name";
            }
            else if (type == FilterType.MonsterLocation)
            {
                TextBox_SearchInput.Mask = "";
                Label_MainText.Text = "Provide Monster Location";
            }
        }

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
