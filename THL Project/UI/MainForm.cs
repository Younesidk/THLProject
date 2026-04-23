using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ReaLTaiizor.Controls;
using THL_Project.Models;
using Button = System.Windows.Forms.Button;

namespace THL_Project.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.Font = new Font("Consolas", 9.5f, FontStyle.Regular);

            StyleControls(this);

            LayoutCombos();

            CalculatingPanelHeights();

            this.Resize += (s, e) =>
            {
                CalculatingPanelHeights();
            };

            DataGridViewInitialize();


            AddPlaceHolderComboBox(FromComboBox, "From");
            AddPlaceHolderComboBox(SymbolComboBox, "Symbol");
            AddPlaceHolderComboBox(ToComboBox, "To");

            ButtonBorderRadius(AddTransitionButton);
            ButtonBorderRadius(GenerateAutomatonButton);

            automaton = new();
        }

        private void DataGridViewInitialize()
        {
            MatrixDataGridView.Dock = DockStyle.Fill;
            MatrixDataGridView.BackgroundColor = Color.FromArgb(20, 20, 28);
            MatrixDataGridView.GridColor = Color.FromArgb(40, 40, 55);
            MatrixDataGridView.DefaultCellStyle.BackColor = Color.FromArgb(24, 24, 31);
            MatrixDataGridView.DefaultCellStyle.ForeColor = Color.White;
            MatrixDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 31, 42);
            MatrixDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            MatrixDataGridView.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 31, 42);
            MatrixDataGridView.RowHeadersDefaultCellStyle.ForeColor = Color.White;
            MatrixDataGridView.EnableHeadersVisualStyles = false;
            MatrixDataGridView.AllowUserToAddRows = false;
            MatrixDataGridView.ReadOnly = true;
        }

        private void ButtonBorderRadius(Button bt)
        {
            bt.FlatStyle = FlatStyle.Flat;
            bt.FlatAppearance.BorderSize = 0;

            int radius = 20;
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            var rect = bt.ClientRectangle;

            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();

            bt.FlatAppearance.BorderSize = 0;
            bt.FlatAppearance.MouseDownBackColor = bt.BackColor;
            bt.FlatAppearance.MouseOverBackColor = bt.BackColor;

            bt.TabStop = false;
            bt.Click += (s, e) => this.ActiveControl = null;

            bt.Region = new Region(path);
        }

        private void AddPlaceHolderComboBox(ComboBox cb, string message)
        {
            cb.Items.Add(message);
            cb.SelectedIndex = 0;

            cb.SelectedIndexChanged += (s, e) =>
            {
                if (cb.SelectedIndex == 0)
                    cb.ForeColor = Color.Gray;
                else
                    cb.ForeColor = Color.White;
            };
        }

        private void LayoutCombos()
        {
            int maxTotalWidth = 250;
            int gap = 10;
            int comboCount = 3;

            int totalGapWidth = gap * (comboCount - 1);
            int availableForCombos = maxTotalWidth - totalGapWidth;
            int comboWidth = availableForCombos / comboCount;

            int startX = FromComboBox.Left;
            int y = FromComboBox.Top;

            FromComboBox.SetBounds(startX, y, comboWidth, FromComboBox.Height);
            SymbolComboBox.SetBounds(startX + comboWidth + gap, y, comboWidth, SymbolComboBox.Height);
            ToComboBox.SetBounds(startX + (comboWidth * 2) + (gap * 2), y, comboWidth, ToComboBox.Height);
        }

        private void StyleControls(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox tb)
                {
                    tb.BackColor = Color.FromArgb(24, 24, 31);
                    tb.ForeColor = Color.White;
                    tb.BorderStyle = BorderStyle.FixedSingle;
                }
                if (c.HasChildren) StyleControls(c);
            }
        }

        private void CalculatingPanelHeights()
        {
            var halfHeight = this.ClientSize.Height / 2;

            AutomatonPanel.Height = halfHeight;
            MatrixPanel.Height = halfHeight;
        }

        private Automaton automaton;

        private void AlphabetTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            var text = AlphabetTextBox.Text;
            //if(text.Length > 1)

        }
    }
}
