using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GOOS.JFX.Scripting;

namespace GOOS.Tools.DungeonConfig
{
	public partial class Form1 : Form
	{
		public GOOS.JFX.Scripting.GeneralConfig ConfigFile;

		public Form1()
		{
			InitializeComponent();
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void cboResolution_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			ConfigFile = new GeneralConfig();

			//put data into config file
			ConfigFile.Ambient = (float)this.numAmbientLight.Value;
			ConfigFile.DefaultLevel = this.txtLevel.Text;
			ConfigFile.Fullscreen = this.chkFullscreen.Checked;
			string hw = cboResolution.SelectedItem.ToString();
			string[] s = hw.Split("x".ToCharArray());
			ConfigFile.width = Convert.ToInt32(s[0]);
			ConfigFile.Height = Convert.ToInt32(s[1]);
			ConfigFile.TorchAttenuation = (float)this.numTorchAttenuation.Value;
			ConfigFile.TorchRange = (float)this.numtorchRange.Value;
			ConfigFile.WallSpecularIntensity = (float)this.numWallIntens.Value;
			ConfigFile.WallSpecularPower = (float)this.numWallPower.Value;


			SaveFileDialog os = new SaveFileDialog();
			os.Title = "Save Config file as";
			DialogResult dr = os.ShowDialog();

			if (dr == DialogResult.OK)
			{
				try
				{
					ConfigFile.SaveTofile(os.FileName);
				}
				catch (Exception Ex)
				{
					MessageBox.Show(Ex.Message, "Error Saving Config, Save Aborted.", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{
			// Load map ONLY
			OpenFileDialog od = new OpenFileDialog();
			od.Title = "Choose a config file to open";
			DialogResult dr = od.ShowDialog();

			if (dr == DialogResult.OK)
			{
				try
				{
					ConfigFile = GeneralConfig.LoadFromFile(od.FileName);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error Loading Config, Load Aborted.", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				FillForm();
			}
		}

		private void FillForm()
		{
			this.numAmbientLight.Value = (decimal)ConfigFile.Ambient;
			this.txtLevel.Text = ConfigFile.DefaultLevel;
			this.chkFullscreen.Checked = ConfigFile.Fullscreen;
			string hw = ConfigFile.width + "x" + ConfigFile.Height;
			cboResolution.SelectedItem = hw;
			this.numTorchAttenuation.Value = (decimal)ConfigFile.TorchAttenuation;
			this.numtorchRange.Value = (decimal)ConfigFile.TorchRange;
			this.numWallIntens.Value = (decimal)ConfigFile.WallSpecularIntensity;
			this.numWallPower.Value = (decimal)ConfigFile.WallSpecularPower;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			// btnLoad_Click(btnLoad, new EventArgs());
		}

		private void btnDefaults_Click(object sender, EventArgs e)
		{
			this.numAmbientLight.Value = (decimal)0.085;
			this.txtLevel.Text = "<none>";
			this.chkFullscreen.Checked = false;
			cboResolution.SelectedIndex = 0;
			this.numTorchAttenuation.Value = (decimal)0.9;
			this.numtorchRange.Value = 50;
			this.numWallIntens.Value = 2;
			this.numWallPower.Value = 6;
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{

		}
	}
}
