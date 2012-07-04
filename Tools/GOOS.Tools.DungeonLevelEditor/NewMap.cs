using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GOOS.Tools.DungeonLevelEditor
{
    public partial class NewMap : Form
    {
        public int Xvalue
        {
            get
            {
                return (int)numericUpDown1.Value;
            }
        }

        public int Yvalue
        {
            get
            {
                return (int)numericUpDown2.Value;
            }
        }

        public NewMap()
        {
            InitializeComponent();
        }

        private void NewMap_Load(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
