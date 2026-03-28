using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nhom_03_Paint
{
    public partial class Form1 : Form
    {
        private DrawingManager drawingManager;
        private bool isDrawing = false;
        private Point startPoint = Point.Empty;

        public Form1()
        {
            InitializeComponent();
            drawingManager = new DrawingManager();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.DoubleBuffered = true; // Kích hoạt Double Buffering cho Form
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            drawingManager?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
