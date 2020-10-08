using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoEditor
{
    public partial class ProgressBar : Form
    {
        public ProgressBar()
        {
            InitializeComponent();
        }

        private void ProgressBar_Load(object sender, EventArgs e)
        {

        }

        public void ProgressBarLimits(int min, int max)
        {
            this.progressBar1.Maximum = max;
            this.progressBar1.Minimum = min;
        }

        public void updateProgress(int progress)
        {
            progressBar1.Value = progress;
            if (progress == this.progressBar1.Maximum)
            {
                this.Close();
            }
            else
            {
                this.Refresh();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            EditPhoto.CancelEdit = true;
        }
    }
}
