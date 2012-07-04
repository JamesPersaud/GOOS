using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GOOS.JFX.Level;

namespace GOOS.Tools.DungeonLevelEditor
{
    public partial class Form1 : Form
    {
        public LevelData CurrentLevel;
        public LevelMapGridControl GridControl;

        public Form1()
        {
            InitializeComponent();

            CurrentLevel = new LevelData();
            FillFormWithLevelData();

            GridControl = new LevelMapGridControl();
            groupBox2.Controls.Add(GridControl);
            GridControl.Top = 100;
            GridControl.Left = 10;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bane Level Editor was created by James M. Persaud\r\nfor internal development use in the Grimwood Group Bane project.\r\n","About Bane Level Editor", MessageBoxButtons.OK,MessageBoxIcon.Information);
        }       

        private void newMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentLevel = new LevelData();
            this.lbllevelfilename.Text = "<none>";
            FillFormWithLevelData();
        }

        private void FillFormWithLevelData()
        {
            this.txtLevelName.Text = CurrentLevel.Name;            
            this.numX.Value = (decimal)CurrentLevel.StartLocation.X;
            this.numY.Value = (decimal)CurrentLevel.StartLocation.Y;
            this.numDegrees.Value = (decimal)CurrentLevel.StartOrientation;            
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NewMap m = new NewMap();
            m.ShowDialog();

            if (m.DialogResult == DialogResult.OK)
            {
                CurrentLevel.LevelMap = new LevelMapData(m.Xvalue, m.Yvalue);
                this.lblmapfilename.Text = "<none>";
                FillFormWithMapData();
            }
        }

        private void FillFormWithMapData()
        {
            this.txtMapName.Text = CurrentLevel.LevelMap.Name;
            this.lblmapdimentions.Text = CurrentLevel.LevelMap.Width.ToString() + " , " + CurrentLevel.LevelMap.Height.ToString();

            GridControl.InitGrid(CurrentLevel.LevelMap);
        }

        private void loadMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Load level AND MAP
            OpenFileDialog od = new OpenFileDialog();
            DialogResult dr = od.ShowDialog();

            if (dr == DialogResult.OK)
            {
                try
                {
                    CurrentLevel = LevelData.Load(od.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Loading Level File, Load Aborted.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.lbllevelfilename.Text = CurrentLevel.FileName;                
                FillFormWithLevelData();

                string mapfilename = CurrentLevel.LevelMapFileName;
                if (mapfilename.Length > 0 && mapfilename != "<none>")
                {
                    try
                    {
                        CurrentLevel.LevelMap = LevelMapData.LoadFromFile(mapfilename);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\r\nThe level was loaded but the map was not.", "Error Loading Map File, Load aborted.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    this.lblmapfilename.Text = CurrentLevel.LevelMapFileName;
                    FillFormWithMapData();
                }           
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // Load map ONLY
            OpenFileDialog od = new OpenFileDialog();
            DialogResult dr = od.ShowDialog();

            if (dr == DialogResult.OK)
            {
                try
                {
                    CurrentLevel.LevelMap = LevelMapData.LoadFromFile(od.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Loading Map, Load Aborted.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.lblmapfilename.Text = CurrentLevel.LevelMap.FileName;
                FillFormWithMapData();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveMapAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If there's a map save that first!!!!!!!!!!!!!!
            if (CurrentLevel.LevelMap != null)
            {
                toolStripMenuItem4_Click(sender, e);
            }

            //Save Level As

            CurrentLevel.Name = this.txtLevelName.Text;
            CurrentLevel.StartLocation = new Microsoft.Xna.Framework.Vector2((float)numX.Value, (float)numY.Value);
            CurrentLevel.StartOrientation = (float)numDegrees.Value;
            CurrentLevel.LevelMapFileName = this.lblmapfilename.Text;
            
            SaveFileDialog os = new SaveFileDialog();
            os.Title = "Save LEVEL as";
            DialogResult dr = os.ShowDialog();            

            if (dr == DialogResult.OK)
            {
                try
                {
                    CurrentLevel.SaveToFile(os.FileName, false);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message, "Error Saving Level, Save Aborted.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                lbllevelfilename.Text = CurrentLevel.FileName;                
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            //Save Map As

            CurrentLevel.LevelMap.Name = txtMapName.Text;

            //Set Grid data
            if (GridControl.Squares != null && CurrentLevel.LevelMap != null)
            {
                for (int i = 0; i < GridControl.Squares.Length; i++)
                {
                    CurrentLevel.LevelMap.SetSquareAt(i % CurrentLevel.LevelMap.Width, i / CurrentLevel.LevelMap.Width,GridControl.Squares[i]);
                }
            }

            SaveFileDialog os = new SaveFileDialog();
            os.Title = "Save MAP as";
            DialogResult dr = os.ShowDialog();

            if (dr == DialogResult.OK)
            {
                try
                {
                    CurrentLevel.LevelMap.SaveTofile(os.FileName);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message, "Error Saving Map, Save Aborted.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                lblmapfilename.Text = os.FileName;
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
