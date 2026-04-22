using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace THL_Project.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            CalculatingPanelHeights();

            this.Resize += (s, e) =>
            {
                CalculatingPanelHeights();
            };

        }

        private void CalculatingPanelHeights()
        {
            var halfHeight = this.ClientSize.Height / 2;

            AutomatonPanel.Height = halfHeight;
            MatrixPanel.Height = halfHeight;
        }

        
    }
}
