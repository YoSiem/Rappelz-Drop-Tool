using System.Drawing;
using System.Windows.Forms;

namespace DropTool
{
    public static class ThemeManager
    {
        public static void ApplyDarkTheme(Form form)
        {
            Color baseColor = Color.FromArgb(54, 57, 62);       // dark graphite
            Color controlColor = Color.FromArgb(64, 66, 72);    // slightly lighter
            Color listColor = Color.FromArgb(47, 49, 54);       // list/tree background
            Color foreColor = Color.FromArgb(220, 220, 220);    // light gray text

            ApplyToControl(form, baseColor, controlColor, listColor, foreColor);
        }

        private static void ApplyToControl(Control control, Color baseColor, Color controlColor, Color listColor, Color foreColor)
        {
            Color backColor = baseColor;

            if (control is TextBox || control is NumericUpDown || control is MaskedTextBox)
            {
                backColor = controlColor;
            }
            else if (control is ListView || control is TreeView)
            {
                backColor = listColor;
            }
            else if (control is Panel || control is GroupBox)
            {
                backColor = baseColor;
            }

            control.BackColor = backColor;
            control.ForeColor = foreColor;
            control.Font = new Font("Segoe UI", 9F);

            foreach (Control child in control.Controls)
            {
                ApplyToControl(child, baseColor, controlColor, listColor, foreColor);
            }
        }
    }
}
