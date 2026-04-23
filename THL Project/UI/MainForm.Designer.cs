namespace THL_Project.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SidePanel = new Panel();
            GenerateAutomatonButton = new Button();
            AddTransitionButton = new ReaLTaiizor.Controls.CrownButton();
            ToComboBox = new ReaLTaiizor.Controls.CrownComboBox();
            SymbolComboBox = new ReaLTaiizor.Controls.CrownComboBox();
            FromComboBox = new ReaLTaiizor.Controls.CrownComboBox();
            label3 = new Label();
            StatesTextBox = new TextBox();
            label2 = new Label();
            label1 = new Label();
            AlphabetTextBox = new TextBox();
            AutomatonPanel = new Panel();
            MatrixPanel = new Panel();
            MatrixDataGridView = new DataGridView();
            label4 = new Label();
            SidePanel.SuspendLayout();
            MatrixPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MatrixDataGridView).BeginInit();
            SuspendLayout();
            // 
            // SidePanel
            // 
            SidePanel.BackColor = Color.FromArgb(17, 17, 24);
            SidePanel.Controls.Add(label4);
            SidePanel.Controls.Add(GenerateAutomatonButton);
            SidePanel.Controls.Add(AddTransitionButton);
            SidePanel.Controls.Add(ToComboBox);
            SidePanel.Controls.Add(SymbolComboBox);
            SidePanel.Controls.Add(FromComboBox);
            SidePanel.Controls.Add(label3);
            SidePanel.Controls.Add(StatesTextBox);
            SidePanel.Controls.Add(label2);
            SidePanel.Controls.Add(label1);
            SidePanel.Controls.Add(AlphabetTextBox);
            SidePanel.Dock = DockStyle.Left;
            SidePanel.Location = new Point(0, 0);
            SidePanel.Name = "SidePanel";
            SidePanel.Size = new Size(300, 794);
            SidePanel.TabIndex = 0;
            // 
            // GenerateAutomatonButton
            // 
            GenerateAutomatonButton.Anchor = AnchorStyles.Bottom;
            GenerateAutomatonButton.BackColor = Color.FromArgb(29, 199, 125);
            GenerateAutomatonButton.ForeColor = Color.Black;
            GenerateAutomatonButton.Location = new Point(28, 720);
            GenerateAutomatonButton.Name = "GenerateAutomatonButton";
            GenerateAutomatonButton.Size = new Size(250, 40);
            GenerateAutomatonButton.TabIndex = 14;
            GenerateAutomatonButton.Text = "-> Generate Automaton";
            GenerateAutomatonButton.UseVisualStyleBackColor = false;
            // 
            // AddTransitionButton
            // 
            AddTransitionButton.Location = new Point(28, 238);
            AddTransitionButton.Name = "AddTransitionButton";
            AddTransitionButton.Padding = new Padding(5);
            AddTransitionButton.Size = new Size(250, 40);
            AddTransitionButton.TabIndex = 13;
            AddTransitionButton.Text = "+ Add Transition";
            // 
            // ToComboBox
            // 
            ToComboBox.DrawMode = DrawMode.OwnerDrawVariable;
            ToComboBox.FormattingEnabled = true;
            ToComboBox.Location = new Point(206, 194);
            ToComboBox.Name = "ToComboBox";
            ToComboBox.Size = new Size(72, 24);
            ToComboBox.TabIndex = 12;
            // 
            // SymbolComboBox
            // 
            SymbolComboBox.DrawMode = DrawMode.OwnerDrawVariable;
            SymbolComboBox.FormattingEnabled = true;
            SymbolComboBox.Location = new Point(117, 194);
            SymbolComboBox.Name = "SymbolComboBox";
            SymbolComboBox.Size = new Size(67, 24);
            SymbolComboBox.TabIndex = 11;
            // 
            // FromComboBox
            // 
            FromComboBox.DrawMode = DrawMode.OwnerDrawVariable;
            FromComboBox.FormattingEnabled = true;
            FromComboBox.Location = new Point(28, 194);
            FromComboBox.Name = "FromComboBox";
            FromComboBox.Size = new Size(73, 24);
            FromComboBox.TabIndex = 10;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.ButtonFace;
            label3.Location = new Point(28, 176);
            label3.Name = "label3";
            label3.Size = new Size(64, 15);
            label3.TabIndex = 4;
            label3.Text = "Transitions";
            // 
            // StatesTextBox
            // 
            StatesTextBox.Location = new Point(28, 114);
            StatesTextBox.Name = "StatesTextBox";
            StatesTextBox.Size = new Size(250, 23);
            StatesTextBox.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ButtonFace;
            label2.Location = new Point(28, 96);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 2;
            label2.Text = "States";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Black;
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(28, 20);
            label1.Name = "label1";
            label1.Size = new Size(55, 15);
            label1.TabIndex = 1;
            label1.Text = "Alphabet";
            // 
            // AlphabetTextBox
            // 
            AlphabetTextBox.Location = new Point(28, 38);
            AlphabetTextBox.Name = "AlphabetTextBox";
            AlphabetTextBox.Size = new Size(250, 23);
            AlphabetTextBox.TabIndex = 0;
            AlphabetTextBox.KeyPress += AlphabetTextBox_KeyPress;
            // 
            // AutomatonPanel
            // 
            AutomatonPanel.BackColor = Color.FromArgb(13, 13, 18);
            AutomatonPanel.Dock = DockStyle.Bottom;
            AutomatonPanel.Location = new Point(300, 392);
            AutomatonPanel.Name = "AutomatonPanel";
            AutomatonPanel.Size = new Size(1029, 402);
            AutomatonPanel.TabIndex = 1;
            // 
            // MatrixPanel
            // 
            MatrixPanel.BackColor = Color.FromArgb(20, 20, 28);
            MatrixPanel.Controls.Add(MatrixDataGridView);
            MatrixPanel.Dock = DockStyle.Top;
            MatrixPanel.Location = new Point(300, 0);
            MatrixPanel.Name = "MatrixPanel";
            MatrixPanel.Size = new Size(1029, 386);
            MatrixPanel.TabIndex = 2;
            // 
            // MatrixDataGridView
            // 
            MatrixDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            MatrixDataGridView.Dock = DockStyle.Fill;
            MatrixDataGridView.Location = new Point(0, 0);
            MatrixDataGridView.Name = "MatrixDataGridView";
            MatrixDataGridView.Size = new Size(1029, 386);
            MatrixDataGridView.TabIndex = 0;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(28, 64);
            label4.Name = "label4";
            label4.Size = new Size(38, 15);
            label4.TabIndex = 15;
            label4.Text = "label4";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(1329, 794);
            Controls.Add(MatrixPanel);
            Controls.Add(AutomatonPanel);
            Controls.Add(SidePanel);
            Name = "MainForm";
            Text = "MainForm";
            SidePanel.ResumeLayout(false);
            SidePanel.PerformLayout();
            MatrixPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)MatrixDataGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel SidePanel;
        private Panel AutomatonPanel;
        private Panel MatrixPanel;
        private TextBox AlphabetTextBox;
        private Label label1;
        private Label label2;
        private TextBox StatesTextBox;
        private Label label3;
        private ReaLTaiizor.Controls.CrownComboBox FromComboBox;
        private ReaLTaiizor.Controls.CrownComboBox ToComboBox;
        private ReaLTaiizor.Controls.CrownComboBox SymbolComboBox;
        private ReaLTaiizor.Controls.CrownButton AddTransitionButton;
        private Button GenerateAutomatonButton;
        private DataGridView MatrixDataGridView;
        private Label label4;
    }
}