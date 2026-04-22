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
            SizePanel = new Panel();
            AutomatonPanel = new Panel();
            MatrixPanel = new Panel();
            SuspendLayout();
            // 
            // SizePanel
            // 
            SizePanel.BackColor = Color.FromArgb(17, 17, 24);
            SizePanel.Dock = DockStyle.Left;
            SizePanel.Location = new Point(0, 0);
            SizePanel.Name = "SizePanel";
            SizePanel.Size = new Size(300, 794);
            SizePanel.TabIndex = 0;
            // 
            // AutomatonPanel
            // 
            AutomatonPanel.BackColor = Color.IndianRed;
            AutomatonPanel.Dock = DockStyle.Bottom;
            AutomatonPanel.Location = new Point(300, 392);
            AutomatonPanel.Name = "AutomatonPanel";
            AutomatonPanel.Size = new Size(1029, 402);
            AutomatonPanel.TabIndex = 1;
            // 
            // MatrixPanel
            // 
            MatrixPanel.BackColor = Color.White;
            MatrixPanel.Dock = DockStyle.Top;
            MatrixPanel.Location = new Point(300, 0);
            MatrixPanel.Name = "MatrixPanel";
            MatrixPanel.Size = new Size(1029, 386);
            MatrixPanel.TabIndex = 2;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(1329, 794);
            Controls.Add(MatrixPanel);
            Controls.Add(AutomatonPanel);
            Controls.Add(SizePanel);
            Name = "MainForm";
            Text = "MainForm";
            ResumeLayout(false);
        }

        #endregion

        private Panel SizePanel;
        private Panel AutomatonPanel;
        private Panel MatrixPanel;
    }
}