namespace GOOS.Tools.DungeonConfig
{
	partial class Form1
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.btnQuit = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.btnDefaults = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.cboResolution = new System.Windows.Forms.ComboBox();
			this.chkFullscreen = new System.Windows.Forms.CheckBox();
			this.txtLevel = new System.Windows.Forms.TextBox();
			this.numWallPower = new System.Windows.Forms.NumericUpDown();
			this.numWallIntens = new System.Windows.Forms.NumericUpDown();
			this.numAmbientLight = new System.Windows.Forms.NumericUpDown();
			this.numtorchRange = new System.Windows.Forms.NumericUpDown();
			this.numTorchAttenuation = new System.Windows.Forms.NumericUpDown();
			this.btnLoad = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numWallPower)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numWallIntens)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numAmbientLight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numtorchRange)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numTorchAttenuation)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(0, 13);
			this.label1.TabIndex = 0;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(12, 12);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(489, 41);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "This tool allow you to configure Bane to run at a resolution supported by your ha" +
				"rdware and also allows you to tweak some ingame variables. This program is desig" +
				"ned for use with the XNA demo only.";
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 110);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Graphics Mode";
			// 
			// textBox2
			// 
			this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBox2.Location = new System.Drawing.Point(12, 245);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.Size = new System.Drawing.Size(489, 51);
			this.textBox2.TabIndex = 3;
			this.textBox2.Text = resources.GetString("textBox2.Text");
			this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
			// 
			// btnQuit
			// 
			this.btnQuit.Location = new System.Drawing.Point(426, 302);
			this.btnQuit.Name = "btnQuit";
			this.btnQuit.Size = new System.Drawing.Size(75, 23);
			this.btnQuit.TabIndex = 4;
			this.btnQuit.Text = "Quit";
			this.btnQuit.UseVisualStyleBackColor = true;
			this.btnQuit.Click += new System.EventHandler(this.button1_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(345, 302);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 5;
			this.btnSave.Text = "Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(258, 110);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(106, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Wall Specular Power";
			// 
			// btnDefaults
			// 
			this.btnDefaults.Location = new System.Drawing.Point(15, 300);
			this.btnDefaults.Name = "btnDefaults";
			this.btnDefaults.Size = new System.Drawing.Size(113, 23);
			this.btnDefaults.TabIndex = 8;
			this.btnDefaults.Text = "Restore Defaults";
			this.btnDefaults.UseVisualStyleBackColor = true;
			this.btnDefaults.Click += new System.EventHandler(this.btnDefaults_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 159);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(78, 13);
			this.label5.TabIndex = 9;
			this.label5.Text = "Level Filename";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(258, 134);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(115, 13);
			this.label6.TabIndex = 10;
			this.label6.Text = "Wall Specular Intensity";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(258, 158);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(100, 13);
			this.label8.TabIndex = 12;
			this.label8.Text = "Ambient Light Level";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(258, 182);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(96, 13);
			this.label9.TabIndex = 13;
			this.label9.Text = "Torch Light Range";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(258, 206);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(118, 13);
			this.label10.TabIndex = 14;
			this.label10.Text = "Torch Light Attenuation";
			// 
			// cboResolution
			// 
			this.cboResolution.FormattingEnabled = true;
			this.cboResolution.Items.AddRange(new object[] {
            "800x600",
            "960x600",
            "1024x768",
            "1152x864",
            "1280x720",
            "1280x768",
            "1280x800",
            "1280x960",
            "1280x1024",
            "1360x768",
            "1440x900",
            "1600x900",
            "1600x1200",
            "1680x1050",
            "1920x1200",
            "1920x1440",
            "2048x1536",
            "2560x1600"});
			this.cboResolution.Location = new System.Drawing.Point(94, 107);
			this.cboResolution.Name = "cboResolution";
			this.cboResolution.Size = new System.Drawing.Size(121, 21);
			this.cboResolution.TabIndex = 15;
			this.cboResolution.SelectedIndexChanged += new System.EventHandler(this.cboResolution_SelectedIndexChanged);
			// 
			// chkFullscreen
			// 
			this.chkFullscreen.AutoSize = true;
			this.chkFullscreen.Location = new System.Drawing.Point(93, 134);
			this.chkFullscreen.Name = "chkFullscreen";
			this.chkFullscreen.Size = new System.Drawing.Size(80, 17);
			this.chkFullscreen.TabIndex = 16;
			this.chkFullscreen.Text = "Fullscreen?";
			this.chkFullscreen.UseVisualStyleBackColor = true;
			// 
			// txtLevel
			// 
			this.txtLevel.Location = new System.Drawing.Point(94, 158);
			this.txtLevel.Name = "txtLevel";
			this.txtLevel.Size = new System.Drawing.Size(121, 20);
			this.txtLevel.TabIndex = 17;
			// 
			// numWallPower
			// 
			this.numWallPower.DecimalPlaces = 3;
			this.numWallPower.Location = new System.Drawing.Point(381, 107);
			this.numWallPower.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numWallPower.Name = "numWallPower";
			this.numWallPower.Size = new System.Drawing.Size(120, 20);
			this.numWallPower.TabIndex = 19;
			// 
			// numWallIntens
			// 
			this.numWallIntens.DecimalPlaces = 3;
			this.numWallIntens.Location = new System.Drawing.Point(381, 131);
			this.numWallIntens.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numWallIntens.Name = "numWallIntens";
			this.numWallIntens.Size = new System.Drawing.Size(120, 20);
			this.numWallIntens.TabIndex = 20;
			// 
			// numAmbientLight
			// 
			this.numAmbientLight.DecimalPlaces = 3;
			this.numAmbientLight.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numAmbientLight.Location = new System.Drawing.Point(381, 157);
			this.numAmbientLight.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numAmbientLight.Name = "numAmbientLight";
			this.numAmbientLight.Size = new System.Drawing.Size(120, 20);
			this.numAmbientLight.TabIndex = 21;
			// 
			// numtorchRange
			// 
			this.numtorchRange.DecimalPlaces = 3;
			this.numtorchRange.Location = new System.Drawing.Point(381, 180);
			this.numtorchRange.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numtorchRange.Name = "numtorchRange";
			this.numtorchRange.Size = new System.Drawing.Size(120, 20);
			this.numtorchRange.TabIndex = 22;
			// 
			// numTorchAttenuation
			// 
			this.numTorchAttenuation.DecimalPlaces = 3;
			this.numTorchAttenuation.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numTorchAttenuation.Location = new System.Drawing.Point(381, 204);
			this.numTorchAttenuation.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numTorchAttenuation.Name = "numTorchAttenuation";
			this.numTorchAttenuation.Size = new System.Drawing.Size(120, 20);
			this.numTorchAttenuation.TabIndex = 23;
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(264, 302);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(75, 23);
			this.btnLoad.TabIndex = 24;
			this.btnLoad.Text = "Load";
			this.btnLoad.UseVisualStyleBackColor = true;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(513, 335);
			this.Controls.Add(this.btnLoad);
			this.Controls.Add(this.numTorchAttenuation);
			this.Controls.Add(this.numtorchRange);
			this.Controls.Add(this.numAmbientLight);
			this.Controls.Add(this.numWallIntens);
			this.Controls.Add(this.numWallPower);
			this.Controls.Add(this.txtLevel);
			this.Controls.Add(this.chkFullscreen);
			this.Controls.Add(this.cboResolution);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.btnDefaults);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnQuit);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "Bane Config Tool";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.numWallPower)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numWallIntens)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numAmbientLight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numtorchRange)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numTorchAttenuation)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button btnQuit;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnDefaults;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ComboBox cboResolution;
		private System.Windows.Forms.CheckBox chkFullscreen;
		private System.Windows.Forms.TextBox txtLevel;
		private System.Windows.Forms.NumericUpDown numWallPower;
		private System.Windows.Forms.NumericUpDown numWallIntens;
		private System.Windows.Forms.NumericUpDown numAmbientLight;
		private System.Windows.Forms.NumericUpDown numtorchRange;
		private System.Windows.Forms.NumericUpDown numTorchAttenuation;
		private System.Windows.Forms.Button btnLoad;
	}
}

