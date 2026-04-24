using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ReaLTaiizor.Controls;
using THL_Project.Models;
using Button = System.Windows.Forms.Button;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using THL_Project.Logic;
using Color = System.Drawing.Color;

namespace THL_Project.UI;

public partial class MainForm : Form
{
    // ==========================================
    // --- 1. UI Theme Colors (System.Drawing) ---
    // ==========================================
    private readonly Color ThemeBackground = Color.FromArgb(20, 20, 28);
    private readonly Color ThemeGridLine = Color.FromArgb(40, 40, 55);
    private readonly Color ThemeCellBg = Color.FromArgb(24, 24, 31);
    private readonly Color ThemeHeaderBg = Color.FromArgb(31, 31, 42);
    private readonly Color ThemeText = Color.White;
    private readonly Color ThemePlaceholder = Color.Gray;

    // ==========================================================
    // --- 2. MSAGL Graph Colors (Microsoft.Msagl.Drawing) ---
    // ==========================================================
    private readonly Microsoft.Msagl.Drawing.Color GraphNodeFill = new Microsoft.Msagl.Drawing.Color(255, 24, 24, 31);
    private readonly Microsoft.Msagl.Drawing.Color GraphNodeBorder = Microsoft.Msagl.Drawing.Color.White;
    private readonly Microsoft.Msagl.Drawing.Color GraphNodeText = Microsoft.Msagl.Drawing.Color.White;
    private readonly Microsoft.Msagl.Drawing.Color GraphInitialState = Microsoft.Msagl.Drawing.Color.DarkGreen;
    private readonly Microsoft.Msagl.Drawing.Color GraphFinalState = Microsoft.Msagl.Drawing.Color.Cyan;
    private readonly Microsoft.Msagl.Drawing.Color GraphEdgeLine = Microsoft.Msagl.Drawing.Color.Black;
    private readonly Microsoft.Msagl.Drawing.Color GraphEdgeText = Microsoft.Msagl.Drawing.Color.Black;

    private Automaton automaton;
    private AutomatonRecognizer recognizer;

    public MainForm()
    {
        InitializeComponent();

        InitialStateColorKey.Text = $"Initial State Color : Dark Green";

        this.Font = new Font("Consolas", 9.5f, System.Drawing.FontStyle.Regular);

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

        recognizer = new AutomatonRecognizer(automaton);
    }

    private void DataGridViewInitialize()
    {
        MatrixDataGridView.Dock = DockStyle.Fill;

        // Applied UI Theme Colors Here
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
            // Applied UI Theme Colors Here
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
                // Applied UI Theme Colors Here
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



    private void AlphabetTextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            string text = AlphabetTextBox.Text.Trim(); // Remove accidental spaces

            // 1. Check if the input is exactly one character
            if (char.TryParse(text, out char symbol))
            {
                // 2. Check if the automaton already contains this symbol 
                // (Assuming your automaton has a method like ContainsSymbol)
                if (automaton.GetSymbolNames().Contains($"{symbol}"))
                {
                    AlphabetErrorLabel.Text = $"'{symbol}' is already in the alphabet!";
                }
                else
                {
                    automaton.AddSymbol(symbol);
                    AlphabetErrorLabel.Text = ""; // Clear error on success
                    AlphabetTextBox.Clear();     // Optional: Clear box for next input
                    MatrixDataGridView.Columns.Add($"{symbol}", $"{symbol}");
                    SymbolComboBox.Items.Add(text);
                }
            }
            else
            {
                AlphabetErrorLabel.Text = "Please enter exactly one character.";
            }

            e.Handled = true; // Still kills the 'ding' sound
        }
    }

    private void StatesTextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            var stateName = StatesTextBox.Text.Trim();
            var isInit = IsInitialCheckBox.Checked;
            var isFinal = IsFinalCheckBox.Checked;

            //Checking if this State exists already or not
            if (automaton.GetStateNames().Contains(stateName))
            {
                StateInputErrorLabel.Text = $"\"{stateName}\" is already in the Automaton";
            }
            else if (isInit && automaton.InitExist())
            {
                StateInputErrorLabel.Text = $"There already exists an Initial State";
            }
            else
            {
                var state = new State(stateName, isInit, isFinal);
                automaton.AddState(state);

                // --- FIX IS HERE ---
                // 1. Add an empty row and grab its index
                int rowIndex = MatrixDataGridView.Rows.Add();

                // 2. Assign the stateName to the Row Header instead of a cell
                MatrixDataGridView.Rows[rowIndex].HeaderCell.Value = stateName;
                // -------------------

                StatesTextBox.Clear();
                IsInitialCheckBox.Checked = false;
                IsFinalCheckBox.Checked = false;
                AddStatesToComboBoxes(stateName);
            }

            e.Handled = true;
        }
    }

    private void AddStatesToComboBoxes(string stateName)
    {
        FromComboBox.Items.Add(stateName);
        ToComboBox.Items.Add(stateName);
    }

    private void AddTransitionButton_Click(object sender, EventArgs e)
    {
        // 1. Validate that none of the ComboBoxes are on the placeholder index (0)
        if (FromComboBox.SelectedIndex <= 0 ||
            SymbolComboBox.SelectedIndex <= 0 ||
            ToComboBox.SelectedIndex <= 0)
        {
            TransitionErrorLabel.Text = "Please select valid 'From', 'Symbol', and 'To' values.";
            return; // Stop execution here so it doesn't crash
        }

        // Clear the error label if validation passes
        TransitionErrorLabel.Text = "";

        // 2. Extract the actual values
        string fromState = FromComboBox.SelectedItem.ToString();
        string symbol = SymbolComboBox.SelectedItem.ToString();
        string toState = ToComboBox.SelectedItem.ToString();

        // 3. Find the actual State objects
        var from = automaton.GetStates().FirstOrDefault(s => s.Name == fromState);
        var to = automaton.GetStates().FirstOrDefault(s => s.Name == toState);

        if (from is null || to is null)
        {
            TransitionErrorLabel.Text = "Error locating states.";
            return;
        }

        // 4. Create and add the transition to your Automaton backend
        var transition = new Transition(from, to, char.Parse(symbol));
        automaton.AddTransition(transition);

        // 5. Update the DataGridView visually
        // Find the row index that corresponds to the 'From' state
        int targetRowIndex = -1;
        foreach (DataGridViewRow row in MatrixDataGridView.Rows)
        {
            if (row.HeaderCell.Value != null && row.HeaderCell.Value.ToString() == fromState)
            {
                targetRowIndex = row.Index;
                break;
            }
        }

        if (targetRowIndex != -1)
        {
            // Access the specific cell using the target row and the column name (symbol)
            var cell = MatrixDataGridView.Rows[targetRowIndex].Cells[symbol];

            // If a state is already there (NFA), append the new one separated by a comma
            if (cell.Value != null && !string.IsNullOrWhiteSpace(cell.Value.ToString()))
            {
                string currentText = cell.Value.ToString();

                // Prevent visual duplicates if the exact same transition is added twice
                if (!currentText.Split(',').Select(s => s.Trim()).Contains(toState))
                {
                    cell.Value = currentText + ", " + toState;
                }
            }
            else
            {
                // First transition for this symbol on this state
                cell.Value = toState;
            }
        }

        // 6. Reset the ComboBoxes back to placeholders after adding
        FromComboBox.SelectedIndex = 0;
        SymbolComboBox.SelectedIndex = 0;
        ToComboBox.SelectedIndex = 0;
    }

    private void GenerateAutomatonButton_Click(object sender, EventArgs e)
    {
        Graph graph = new Graph("Automaton Graph");

        // --- FIX FOR OVERLAPPING BI-DIRECTIONAL EDGES ---
        // Setting the EdgeRoutingMode to 'Spline' forces MSAGL to draw curved paths. 
        // This stops q0 -> q1 and q1 -> q0 from stacking on top of each other as a single line.
        var layoutSettings = new Microsoft.Msagl.Layout.Layered.SugiyamaLayoutSettings();
        layoutSettings.EdgeRoutingSettings.EdgeRoutingMode = Microsoft.Msagl.Core.Routing.EdgeRoutingMode.Spline;
        graph.LayoutAlgorithmSettings = layoutSettings;
        // -------------------------------------------------

        foreach (var state in automaton.GetStates())
        {
            var node = graph.AddNode(state.Name);

            // Applied MSAGL Theme Colors Here
            node.Attr.FillColor = GraphNodeFill;
            node.Attr.Color = GraphNodeBorder;
            node.Label.FontColor = GraphNodeText;
            node.Attr.Shape = Shape.Circle;

            if (state.IsEnd)
            {
                node.Attr.Shape = Shape.DoubleCircle;
                node.Attr.Color = GraphFinalState;
            }

            if (state.IsInitial)
            {
                node.Attr.FillColor = GraphInitialState;
            }
        }

        var states = automaton.GetStates();
        var symbols = automaton.GetSymbolNames();

        foreach (var fromState in states)
        {
            foreach (var symbolStr in symbols)
            {
                char symbol = symbolStr[0];
                var targetStates = automaton.GetTransition(fromState, symbol);

                if (targetStates != null)
                {
                    foreach (var toState in targetStates)
                    {
                        var edge = graph.AddEdge(fromState.Name, symbolStr, toState.Name);

                        // Applied MSAGL Theme Colors Here
                        edge.Attr.Color = GraphEdgeLine;
                        edge.Label.FontColor = GraphEdgeText;
                    }
                }
            }
        }

        DisplayGraphInPanel(graph);
    }

    private void DisplayGraphInPanel(Graph graph)
    {
        AutomatonPanel.Controls.Clear();

        GViewer viewer = new GViewer();
        viewer.Graph = graph;
        viewer.Dock = DockStyle.Fill;

        // Matches your dark theme perfectly now without hardcoding
        viewer.OutsideAreaBrush = new SolidBrush(ThemeBackground);
        viewer.ToolBarIsVisible = false;

        AutomatonPanel.Controls.Add(viewer);
    }

    private void WordInputTextBox_TextChanged(object sender, EventArgs e)
    {
        recognizer = new AutomatonRecognizer(automaton);

        var word = WordInputTextBox.Text;

        if (word.Length == 0 && AcceptsEmptyString())
        {
            WordInputInformationLabel.Text = "Empty Word is Accepted";
            WordInputInformationLabel.ForeColor = Color.LightBlue;
            return;
        }

        var result = recognizer.RecognizeWord(word);

        if (!result.Accepted)
        {
            WordInputInformationLabel.Text = result.RejectionReason;
            WordInputInformationLabel.ForeColor = Color.Red;
            return;
        }

        WordInputInformationLabel.Text = "Word is Accepted";
        WordInputInformationLabel.ForeColor = Color.LightBlue;
    }

    private bool AcceptsEmptyString()
    {
        // Find the state marked as IsInitial
        var startState = automaton.GetStates().FirstOrDefault(s => s.IsInitial);

        // If it exists and is also marked IsEnd/IsFinal, return true
        return startState != null && startState.IsEnd;
    }

    // ==========================================
    // --- 3. Reusable Refresh Functions ---
    // ==========================================

    private void RefreshGraph()
    {
        // Reuse your existing button click logic but extracted to a call
        GenerateAutomatonButton_Click(null, EventArgs.Empty);
    }

    private void RefreshMatrix()
    {
        // Clear the existing rows and columns to rebuild from the new automaton
        MatrixDataGridView.Rows.Clear();
        MatrixDataGridView.Columns.Clear();

        // 1. Re-add Alphabet Columns
        var symbols = automaton.GetSymbolNames();
        foreach (var symbol in symbols)
        {
            MatrixDataGridView.Columns.Add(symbol, symbol);
        }

        // 2. Re-add State Rows
        foreach (var state in automaton.GetStates())
        {
            int rowIndex = MatrixDataGridView.Rows.Add();
            MatrixDataGridView.Rows[rowIndex].HeaderCell.Value = state.Name;

            // 3. Populate Transitions for this state
            foreach (var symbolStr in symbols)
            {
                char symbol = symbolStr[0];
                var targets = automaton.GetTransition(state, symbol);

                if (targets != null && targets.Any())
                {
                    // Join target state names with commas (handles both DFA and NFA visually)
                    string cellText = string.Join(", ", targets.Select(s => s.Name));
                    MatrixDataGridView.Rows[rowIndex].Cells[symbolStr].Value = cellText;
                }
            }
        }
    }

    // ==========================================
    // --- 4. Updated Logic Function ---
    // ==========================================

    private void TurnIntoDFAButton_Click(object sender, EventArgs e)
    {
        if (automaton.IsDeterministic())
        {
            MessageBox.Show("The Automaton is already Deterministic", "Automaton",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Perform the conversion
        automaton = AutomatonDeterminizer.NFAtoDFA(automaton);

        // Refresh the UI Components
        RefreshMatrix();
        RefreshGraph();

        MessageBox.Show("Automaton successfully converted to DFA.", "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ResetButton_Click(object sender, EventArgs e)
    {
        // 1. Reset the backend data structures
        automaton = new Automaton();
        recognizer = new AutomatonRecognizer(automaton);

        // 2. Clear UI input controls
        AlphabetTextBox.Clear();
        StatesTextBox.Clear();
        WordInputTextBox.Clear();
        IsInitialCheckBox.Checked = false;
        IsFinalCheckBox.Checked = false;

        // 3. Reset ComboBoxes (keeping the placeholders)
        ResetComboBox(FromComboBox, "From");
        ResetComboBox(SymbolComboBox, "Symbol");
        ResetComboBox(ToComboBox, "To");

        // 4. Wipe the visual displays
        RefreshMatrix(); // Clears rows and columns

        // For the Graph, we clear the panel directly since there is no data to render
        AutomatonPanel.Controls.Clear();

        // Reset status labels
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
}


