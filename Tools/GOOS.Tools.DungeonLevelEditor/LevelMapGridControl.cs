using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GOOS.JFX.Level;

namespace GOOS.Tools.DungeonLevelEditor
{
    public partial class LevelMapGridControl : UserControl
    {
        public string[] Buttons;
        public LevelMapSquare[] Squares;
        private Control[] buttoncontrols;

        public LevelMapGridControl()
        {
            InitializeComponent();
        }

        private void LevelMapGridControl_Load(object sender, EventArgs e)
        {

        }

        public void InitGrid(GOOS.JFX.Level.LevelMapData map)
        {
            this.Visible = false;
            buttoncontrols = new Control[map.Height * map.Width];

            Squares = new GOOS.JFX.Level.LevelMapSquare[map.Height * map.Width];
            for (int q = 0; q < Squares.Length; q++)
            {
                Squares[q] = map.GetSquareAt(q % map.Width, q / map.Width);
            }

            this.Controls.Clear();

            Buttons = new string[map.Height * map.Width];

            Button b;
            Label l;

            int x,y;

            for (int i = 0; i < Buttons.Length; i++)
            {            
                x=(i % map.Width);
                y=(i /map.Width);

                if (y == 0)
                {
                    l = new Label();
                    l.Text = x.ToString();                    
                    l.Width = 20;
                    l.Height = 20;
                    l.Top = 50;
                    l.Left = x * 20 + 21;
                    this.Controls.Add(l);
                }

                if (x == 0)
                {
                    l = new Label();
                    l.Text = y.ToString();                    
                    l.Width = 20;
                    l.Height = 20;
                    l.Top = y * 20 + 71; 
                    l.Left = 0;
                    this.Controls.Add(l);
                }

                b = new Button();
                b.ImageKey = i.ToString();
                b.Text = "";                
                b.Click += new EventHandler(b_Click);
                b.MouseClick += new MouseEventHandler(b_MouseClick);             
                b.Name = "Grid " + x.ToString() + "," + y.ToString() + " index " + i.ToString();                

                b.Left = x * 20 + 21;
                b.Top = y * 20 + 71; 
                b.Width = 20;
                b.Height = 20;               

                b.FlatStyle = FlatStyle.Flat;

                if (map.GetSquareAt(x, y).type == GOOS.JFX.Level.MapSquareType.Closed)
                {
                    b.ForeColor = Color.Black;
                    b.BackColor = Color.Gray;
                }
                else if (map.GetSquareAt(x, y).type == GOOS.JFX.Level.MapSquareType.Open)
                {
                    b.ForeColor = Color.Black;
                    b.BackColor = Color.White;
                }

                //this.Controls.Add(b);
                buttoncontrols[i] = b;
                Buttons[i] = b.Name;
            }
            
            for (int add = 0; add < buttoncontrols.Length; add++)
            {                
                this.Controls.Add(buttoncontrols[add]);
            }
            this.Visible = true;
        }

        void b_MouseLeave(object sender, EventArgs e)
        {
           
        }

        void b_MouseEnter(object sender, EventArgs e)
        {
            
        }

        void b_MouseHover(object sender, EventArgs e)
        {
           
        }

        void b_MouseClick(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            if (b.BackColor == Color.Gray)
            {
                b.BackColor = Color.White;
                Squares[Convert.ToInt32(b.ImageKey)].type = GOOS.JFX.Level.MapSquareType.Open;
            }
            else
            {
                b.BackColor = Color.Gray;
                Squares[Convert.ToInt32(b.ImageKey)].type = GOOS.JFX.Level.MapSquareType.Closed;
            }
        }

        void b_Click(object sender, EventArgs e)
        {
           
        }
    }
}
