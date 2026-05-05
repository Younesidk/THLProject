using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using THL_Project.Logic;
using Microsoft.Msagl.GraphViewerGdi;
using Button = System.Windows.Forms.Button;

namespace THL_Project.UI
{
    public partial class MainForm : Form
    {
        // ==========================================
        // --- UI Theme Colors (System.Drawing) ---
        // ==========================================
        private readonly Color ThemeBackground = Color.FromArgb(20, 20, 28);
        private readonly Color ThemeGridLine = Color.FromArgb(40, 40, 55);
        private readonly Color ThemeCellBg = Color.FromArgb(24, 24, 31);
        private readonly Color ThemeHeaderBg = Color.FromArgb(31, 31, 42);
        private readonly Color ThemeText = Color.White;
        private readonly Color ThemePlaceholder = Color.Gray;

        private AutomatonController controller;

        public MainForm()
        {
            InitializeComponent();
            controller = new AutomatonController();

            InitialStateColorKey.Text = "Initial State Color : Dark Green";
            this.Font = new Font("Consolas", 9.5f, System.Drawing.FontStyle.Regular);

            StyleControls(this);
            LayoutCombos();
            CalculatingPanelHeights();

            this.Resize += (s, e) => CalculatingPanelHeights();

            DataGridViewInitialize();

            AddPlaceHolderComboBox(FromComboBox, "From");
            AddPlaceHolderComboBox(SymbolComboBox, "Symbol");
            AddPlaceHolderComboBox(ToComboBox, "To");

            ButtonBorderRadius(AddTransitionButton);
            ButtonBorderRadius(GenerateAutomatonButton);
        }

        // ======================
        // Control Initialization
        // ======================
        private void DataGridViewInitialize()
        {
            MatrixDataGridView.Dock = DockStyle.Fill;
            MatrixDataGridView.BackgroundColor = ThemeBackground;
            MatrixDataGridView.GridColor = ThemeGridLine;
            MatrixDataGridView.DefaultCellStyle.BackColor = ThemeCellBg;
            MatrixDataGridView.DefaultCellStyle.ForeColor = ThemeText;
            MatrixDataGridView.ColumnHeadersDefaultCellStyle.BackColor = ThemeHeaderBg;
            MatrixDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = ThemeText;
            MatrixDataGridView.RowHeadersDefaultCellStyle.BackColor = ThemeHeaderBg;
            MatrixDataGridView.RowHeadersDefaultCellStyle.ForeColor = ThemeText;
            MatrixDataGridView.EnableHeadersVisualStyles = false;
            MatrixDataGridView.AllowUserToAddRows = false;
            MatrixDataGridView.ReadOnly = true;
            MatrixDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
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
                    cb.ForeColor = ThemePlaceholder;
                else
                    cb.ForeColor = ThemeText;
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
                    tb.BackColor = ThemeCellBg;
                    tb.ForeColor = ThemeText;
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

        // ==================
        // Event Handlers
        // ==================
        private void AlphabetTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string text = AlphabetTextBox.Text.Trim();
                if (char.TryParse(text, out char symbol))
                {
                    var (success, error) = controller.AddSymbol(symbol);
                    if (success)
                    {
                        AlphabetErrorLabel.Text = "";
                        AlphabetTextBox.Clear();
                        MatrixDataGridView.Columns.Add(symbol.ToString(), symbol.ToString());
                        SymbolComboBox.Items.Add(text);
                    }
                    else
                    {
                        AlphabetErrorLabel.Text = error;
                    }
                }
                else
                {
                    AlphabetErrorLabel.Text = "Please enter exactly one character.";
                }
                e.Handled = true;
            }
        }

        private void StatesTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string stateName = StatesTextBox.Text.Trim();
                bool isInit = IsInitialCheckBox.Checked;
                bool isFinal = IsFinalCheckBox.Checked;

                var (success, error) = controller.AddState(stateName, isInit, isFinal);
                if (success)
                {
                    StateInputErrorLabel.Text = "";
                    StatesTextBox.Clear();
                    IsInitialCheckBox.Checked = false;
                    IsFinalCheckBox.Checked = false;

                    int rowIndex = MatrixDataGridView.Rows.Add();
                    MatrixDataGridView.Rows[rowIndex].HeaderCell.Value = stateName;

                    FromComboBox.Items.Add(stateName);
                    ToComboBox.Items.Add(stateName);
                }
                else
                {
                    StateInputErrorLabel.Text = error;
                }
                e.Handled = true;
            }
        }

        private void AddTransitionButton_Click(object sender, EventArgs e)
        {
            if (FromComboBox.SelectedIndex <= 0 ||
                SymbolComboBox.SelectedIndex <= 0 ||
                ToComboBox.SelectedIndex <= 0)
            {
                TransitionErrorLabel.Text = "Please select valid 'From', 'Symbol', and 'To' values.";
                return;
            }

            TransitionErrorLabel.Text = "";
            string fromState = FromComboBox.SelectedItem.ToString();
            string symbol = SymbolComboBox.SelectedItem.ToString();
            string toState = ToComboBox.SelectedItem.ToString();

            var (success, error) = controller.AddTransition(fromState, symbol[0], toState);
            if (!success)
            {
                TransitionErrorLabel.Text = error;
                return;
            }

            int targetRowIndex = -1;
            foreach (DataGridViewRow row in MatrixDataGridView.Rows)
            {
                if (row.HeaderCell.Value?.ToString() == fromState)
                {
                    targetRowIndex = row.Index;
                    break;
                }
            }

            if (targetRowIndex != -1)
            {
                var cell = MatrixDataGridView.Rows[targetRowIndex].Cells[symbol];
                if (cell.Value != null && !string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    string currentText = cell.Value.ToString();
                    if (!currentText.Split(',').Select(s => s.Trim()).Contains(toState))
                        cell.Value = currentText + ", " + toState;
                }
                else
                {
                    cell.Value = toState;
                }
            }

            FromComboBox.SelectedIndex = 0;
            SymbolComboBox.SelectedIndex = 0;
            ToComboBox.SelectedIndex = 0;
        }

        private void GenerateAutomatonButton_Click(object sender, EventArgs e)
        {
            var graph = controller.BuildGraph();
            DisplayGraphInPanel(graph);
        }

        private void DisplayGraphInPanel(Microsoft.Msagl.Drawing.Graph graph)
        {
            AutomatonPanel.Controls.Clear();
            GViewer viewer = new GViewer();
            viewer.Graph = graph;
            viewer.Dock = DockStyle.Fill;
            viewer.OutsideAreaBrush = new SolidBrush(ThemeBackground);
            viewer.ToolBarIsVisible = false;
            AutomatonPanel.Controls.Add(viewer);
        }

        private void WordInputTextBox_TextChanged(object sender, EventArgs e)
        {
            string word = WordInputTextBox.Text;
            var (accepted, reason) = controller.RecognizeWord(word);
            if (accepted)
            {
                WordInputInformationLabel.Text = reason;
                WordInputInformationLabel.ForeColor = Color.LightBlue;
            }
            else
            {
                WordInputInformationLabel.Text = reason;
                WordInputInformationLabel.ForeColor = Color.Red;
            }
        }

        private void TurnIntoDFAButton_Click(object sender, EventArgs e)
        {
            if (controller.IsDeterministic())
            {
                MessageBox.Show("The Automaton is already Deterministic", "Automaton",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            controller.ConvertToDFA();
            RefreshMatrix();
            RefreshGraph();
            MessageBox.Show("Automaton successfully converted to DFA.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            controller.Reset();

            AlphabetTextBox.Clear();
            StatesTextBox.Clear();
            WordInputTextBox.Clear();
            IsInitialCheckBox.Checked = false;
            IsFinalCheckBox.Checked = false;

            ResetComboBox(FromComboBox, "From");
            ResetComboBox(SymbolComboBox, "Symbol");
            ResetComboBox(ToComboBox, "To");

            RefreshMatrix();
            AutomatonPanel.Controls.Clear();

            AlphabetErrorLabel.Text = "";
            StateInputErrorLabel.Text = "";
            TransitionErrorLabel.Text = "";
            WordInputInformationLabel.Text = "";
        }

        private void ResetComboBox(ComboBox cb, string placeholder)
        {
            cb.Items.Clear();
            cb.Items.Add(placeholder);
            cb.SelectedIndex = 0;
        }

        // ==================
        // UI Refresh Helpers
        // ==================
        private void RefreshMatrix()
        {
            MatrixDataGridView.Rows.Clear();
            MatrixDataGridView.Columns.Clear();

            var symbols = controller.GetSymbolNames();
            foreach (var sym in symbols)
                MatrixDataGridView.Columns.Add(sym, sym);

            foreach (var state in controller.GetStates())
            {
                int rowIndex = MatrixDataGridView.Rows.Add();
                MatrixDataGridView.Rows[rowIndex].HeaderCell.Value = state.Name;

                foreach (var sym in symbols)
                {
                    var targets = controller.GetTargetStateNames(state.Name, sym[0]);
                    if (targets.Any())
                    {
                        string cellText = string.Join(", ", targets);
                        MatrixDataGridView.Rows[rowIndex].Cells[sym].Value = cellText;
                    }
                }
            }
        }

        private void RefreshGraph()
        {
            var graph = controller.BuildGraph();
            DisplayGraphInPanel(graph);
        }
    }
}