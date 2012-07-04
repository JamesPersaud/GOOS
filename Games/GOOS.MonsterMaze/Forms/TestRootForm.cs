using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GOOS.JFX.UI.Forms;
using GOOS.JFX.UI.Controls;
using Microsoft.Xna.Framework.Content;
using GOOS.JFX.UI;

namespace GOOS.MonsterMaze.Forms
{
	/// <summary>
	/// UI Test's root form.
	/// </summary>
	public class TestRootForm : BaseGameForm
	{
		public UIScreenEnum ScreenState;

		#region Constructors

		/// <summary>
		/// A new root with no skin that simply collects other controls.
		/// </summary>
		/// <param name="name">An optional name for this instance.</param>
		public TestRootForm(string name)
			: base()
		{ this.Name = name; }
		public TestRootForm()
			: this("RootOne")
		{
			ScreenState = UIScreenEnum.MainMenu;
		}

		#endregion		

		/// <summary>
		/// Build up the controls on this form.
		/// </summary>
		public void AddControls()
		{
			//New game
			ImageButton ib = ImageButton.CreateNew("Button_NewGame", Content.Load<Texture2D>("button_new"), new Vector4(1.0f,1.0f,1.0f,0.4f));
			ib.Location = new Vector3(((RenderRectangle.Right / 2) - ib.RenderRectangle.Width / 2), 228, 0.4f);
			AddControl(ib);

			//Options
			ib = ImageButton.CreateNew("Button_Options", Content.Load<Texture2D>("button_options"), new Vector4(1.0f, 1.0f, 1.0f, 0.4f));
			ib.Location = new Vector3(((RenderRectangle.Right / 2) - ib.RenderRectangle.Width / 2), 276, 0.4f);
			AddControl(ib);

			//Hi-Scores
			ib = ImageButton.CreateNew("Button_HiScore", Content.Load<Texture2D>("button_hiscores"), new Vector4(1.0f, 1.0f, 1.0f, 0.4f));
			ib.Location = new Vector3(((RenderRectangle.Right / 2) - ib.RenderRectangle.Width / 2), 324, 0.4f);
			AddControl(ib);

			//Help
			ib = ImageButton.CreateNew("Button_Help", Content.Load<Texture2D>("button_help"), new Vector4(1.0f, 1.0f, 1.0f, 0.4f));
			ib.Location = new Vector3(((RenderRectangle.Right / 2) - ib.RenderRectangle.Width / 2), 372, 0.4f);
			AddControl(ib);

			//Quit
			ib = ImageButton.CreateNew("Button_Quit", Content.Load<Texture2D>("button_quit"), new Vector4(1.0f, 1.0f, 1.0f, 0.4f));
			ib.Location = new Vector3(((RenderRectangle.Right / 2) - ib.RenderRectangle.Width / 2), 420, 0.4f);
			AddControl(ib);

			//OK Button (for returning from other screens)
			ib = ImageButton.CreateNew("Button_Ok", Content.Load<Texture2D>("button_ok"), new Vector4(1.0f, 1.0f, 1.0f, 0.4f));
			ib.Location = new Vector3(((RenderRectangle.Right / 2) - ib.RenderRectangle.Width / 2 + ib.RenderRectangle.Width + 16),420,0.4f);
			AddControl(ib);
			ib.Visible = false;

			//Big banner
			Image i = Image.CreateNew("BigBanner", Content.Load<Texture2D>("bigbanner"), Vector4.One);
			i.Location = new Vector3(((RenderRectangle.Right / 2) - i.RenderRectangle.Width / 2), 32, 0.4f);
			i.Enabled = false;
			AddControl(i);

			//Test credit fader
			CreditFader cf = CreditFader.CreateNew("thefader", Content.Load<SpriteFont>("testfont"));
			cf.AddCredit(new GameCredit("Programming and Design", "\r\n    James Persaud", Point.Zero, Point.Zero, 12000, new Vector4(1, 0, 0, 1), 1.0f));
			cf.AddCredit(new GameCredit("JFX Engine", "\r\n    James Persaud", Point.Zero, Point.Zero, 8000, new Vector4(0, 1, 0, 1), 1.0f));			
			cf.AddCredit(new GameCredit("Graphics", "\r\n    James Persaud", Point.Zero, Point.Zero, 8000, new Vector4(0, 0.1f, 1, 1), 1.0f));
			cf.AddCredit(new GameCredit("Music", "\r\n    Angus Arnold", Point.Zero, Point.Zero, 8000, new Vector4(0, 1, 0, 1), 1.0f));
			cf.AddCredit(new GameCredit("Additional Textures", "\r\n    Benjamin Röhling\r\n    Herbert Fahrnholz", Point.Zero, Point.Zero, 10000, new Vector4(1,0,0, 1), 1.0f));			
			cf.AddCredit(new GameCredit("Sound Effects", "\r\n    Partners In Rhyme", Point.Zero, Point.Zero, 8000, new Vector4(0, 0.1f, 1, 1), 1.0f));
			cf.AddCredit(new GameCredit("Original Concept", "\r\n    Malcolm Evans\r\n    3D Monster Maze (©1981)", Point.Zero, Point.Zero, 8000, new Vector4(1, 0, 0, 1), 1.0f));
			cf.Location = new Vector3(16, RenderRectangle.Bottom-100, 0.4f);
			AddControl(cf);

			// Help Screen
			string helptext;
			helptext = "3D creature cave is a 1st person perspective 3D game in which the objective\r\nis to escape the creature (an unnamable terror from the nethermost depths.)\r\n\r\nUse the directional keys WASD to move around and the mouse to look. You may\r\njump using the space bar. The objective of each level is to find the magic\r\nportal that will teleport you to the next level. The quicker you reach the\r\nteleporter the more you will score. The further you get, the faster the creature\r\nwill become. If the creature touches you, you die.\r\n\r\nYour compass will always show you in which direction the creature (red dot)\r\nand the portal (blue dot) lie. Other visual and audio clues will warn you of\r\nimminent danger.\r\n\r\nGood Luck!";			
			Label lab = Label.CreateNew("Label_HelpText", helptext, new Vector4(0.0f, 1.0f, 0.0f,1.0f), Content.Load<SpriteFont>("helpfont"));
			lab.Location = new Vector3(((RenderRectangle.Right / 2) - 275), 180, 0.4f);
			lab.Visible = false;
			AddControl(lab);					

			// HiScore Screen
			HiScoreTableControl his = HiScoreTableControl.CreateNew("HiScoreTable", Content.Load<SpriteFont>("helpfont"), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f), null, 10);
			his.Location = new Vector3(((RenderRectangle.Right / 2) - 275), 180, 0.4f);			
			his.Visible = false;
			AddControl(his);

			//HiScore Name Label
			Label scorelab = Label.CreateNew("HiScoreLabel", "You have suffered an unmentionable fate at the hands of an unnamable horror.\r\nThe good news is, you have a high score. Please enter your name below.", new Vector4(0.0f, 1.0f, 0.0f, 1.0f), Content.Load<SpriteFont>("helpfont"));
			scorelab.Location = new Vector3(((RenderRectangle.Right / 2) - 225), 205, 0.3f);
			scorelab.Visible = false;
			AddControl(scorelab);

			//HiScore Name TextBox
			TextBox keylab = TextBox.CreateNew("KeyboardTestLabel", "", new Vector4(0.0f, 1.0f, 0.0f, 1.0f), Content.Load<SpriteFont>("helpfont"));
			keylab.Location = new Vector3(((RenderRectangle.Right / 2) - 225), 265, 0.3f);
			keylab.Visible = false;
			AddControl(keylab);

			// Options Screen
			Label optionslab = Label.CreateNew("OptionsMainLabel", "Turning down these options can improve game performance on systems with integrated graphics.\r\nFor best image quality set everything to maximum and use your monitor's native display mode.\r\nChanges to some settings will not take effect until you restart the game.", new Vector4(0.0f, 1.0f, 1.0f, 1.0f), Content.Load<SpriteFont>("helpfont"));
			optionslab.Location = new Vector3(((RenderRectangle.Right / 2) - 300), 150, 0.3f);
			optionslab.Visible = false;
			AddControl(optionslab);

			// display mode		(slider) fullscreen	(checkbox)
			// 
			// view distance		(slider) 1-100
			// bloom intensity		(slider) 1-100 off  (checkbox)
			// particle density	(slider) 1-100	
			// texture Filtering	checklist (none, point, linear, anisotropic)
			// 
			// music volume		(slider) 1-100 mute (checkbox)
			// sfx volume			(slider) 1-100 mute (checkbox)

			// Options Labels
			Label optionslabs = Label.CreateNew("OptionsLabels", "Display Mode\r\n\r\nView Distance\r\nBloom Intensity\r\n"+
				"Particle Density\r\nTexture Sampling\r\n\r\nMusic Volume\r\nSFX Volume", new Vector4(0.0f, 1.0f, 1.0f, 1.0f), Content.Load<SpriteFont>("helpfont"));
			optionslabs.Location = new Vector3(((RenderRectangle.Right / 2) - 300), 225, 0.3f);
			optionslabs.Visible = false;
			AddControl(optionslabs);

			SpriteFont font = Content.Load<SpriteFont>("helpfont");
			Texture2D checktex = Content.Load<Texture2D>("checkbox");
			float lineheight = 0.0f;
			lineheight = font.LineSpacing; //3,7,8

			//fullscreen
			Checkbox newcheck = Checkbox.CreateNew("OptionsCheckFullscreen",
				new Vector3((RenderRectangle.Right / 2)+200, 225 , 0.4f), "Fullscreen", CheckboxAlignment.Left,
				checktex, font, new Rectangle(11, 0, 12, 12), new Rectangle(0, 0, 12, 12), 0, Vector4.One, new Vector4(0.0f, 1.0f, 1.0f, 1.0f), 12);
			newcheck.Visible = false;
			AddControl(newcheck);
			//bloom on
			newcheck = Checkbox.CreateNew("OptionsCheckBloom",
				new Vector3((RenderRectangle.Right / 2) + 200, 225 + 3 * lineheight, 0.4f), "Bloom On", CheckboxAlignment.Left,
				checktex, font, new Rectangle(11, 0, 12, 12), new Rectangle(0, 0, 12, 12), 0, Vector4.One, new Vector4(0.0f, 1.0f, 1.0f, 1.0f), 12);
			newcheck.Visible = false;
			AddControl(newcheck);
			//music mute
			newcheck = Checkbox.CreateNew("OptionsCheckMusicMute",
				new Vector3((RenderRectangle.Right / 2) + 200, 225 + 7 * lineheight, 0.4f), "Mute", CheckboxAlignment.Left,
				checktex, font, new Rectangle(11, 0, 12, 12), new Rectangle(0, 0, 12, 12), 0, Vector4.One, new Vector4(0.0f, 1.0f, 1.0f, 1.0f), 12);
			newcheck.Visible = false;
			AddControl(newcheck);
			//sfx mute
			newcheck = Checkbox.CreateNew("OptionsCheckSFXMute",
				new Vector3((RenderRectangle.Right / 2) + 200, 225 + 8 * lineheight, 0.4f), "Mute", CheckboxAlignment.Left,
				checktex, font, new Rectangle(11, 0, 12, 12), new Rectangle(0, 0, 12, 12), 0, Vector4.One, new Vector4(0.0f, 1.0f, 1.0f, 1.0f), 12);
			newcheck.Visible = false;
			AddControl(newcheck);
			
			//Sliders
			Texture2D slidertex = Content.Load<Texture2D>("slider");
			Rectangle GR = new Rectangle(0, 0, 200, 24);
			Rectangle BR = new Rectangle(202, 0, 12, 24);
			SliderStepMode stepmode = SliderStepMode.Snap;
			//display mode.
            
			Slider newslider = Slider.CreateNew("SliderDisplayMode", slidertex, GR, BR, 0, 100, 10, SliderAlignment.Horizontal, stepmode);
			newslider.Location = new Vector3((RenderRectangle.Right / 2) - 100, 225, 0.4f);
			newslider.Visible = false;
			AddControl(newslider);
			//sampling
			newslider = Slider.CreateNew("SliderSampling", slidertex, GR, BR, 0, 100, 33, SliderAlignment.Horizontal, stepmode);
			newslider.Location = new Vector3((RenderRectangle.Right / 2) - 100, 225 + (5 * lineheight), 0.4f);
			newslider.Visible = false;
			AddControl(newslider);
			//view distance.
			newslider = Slider.CreateNew("SliderViewDist", slidertex, GR, BR, 0, 100, 1, SliderAlignment.Horizontal, stepmode);
			newslider.Location = new Vector3((RenderRectangle.Right / 2)-100, 225 + (2 * lineheight), 0.4f);
			newslider.Visible = false;
			AddControl(newslider);
			//bloom.
			newslider = Slider.CreateNew("SliderBloom", slidertex, GR, BR, 0, 100, 1, SliderAlignment.Horizontal, stepmode);
			newslider.Location = new Vector3((RenderRectangle.Right / 2) - 100, 225 + (3 * lineheight), 0.4f);
			newslider.Visible = false;
			AddControl(newslider);
			//particles.
			newslider = Slider.CreateNew("SliderParticles", slidertex, GR, BR, 0, 100, 1, SliderAlignment.Horizontal, stepmode);
			newslider.Location = new Vector3((RenderRectangle.Right / 2) - 100, 225 + (4 * lineheight), 0.4f);
			newslider.Visible = false;
			AddControl(newslider);
			//music.
			newslider = Slider.CreateNew("SliderMusic", slidertex, GR, BR, 0, 100, 1, SliderAlignment.Horizontal, stepmode);
			newslider.Location = new Vector3((RenderRectangle.Right / 2) - 100, 225 + (7 * lineheight), 0.4f);
			newslider.Visible = false;
			AddControl(newslider);
			//sfx.
			newslider = Slider.CreateNew("SliderSFX", slidertex, GR, BR, 0, 100, 1, SliderAlignment.Horizontal, stepmode);
			newslider.Location = new Vector3((RenderRectangle.Right / 2) - 100, 225 + (8 * lineheight), 0.4f);
			newslider.Visible = false;
			AddControl(newslider);

			//Slider values label
			Label sliderlabel = Label.CreateNew("sliderlabel", "", new Vector4(0.0f, 1.0f, 1.0f, 1.0f), Content.Load<SpriteFont>("helpfont"));
			sliderlabel.Location = new Vector3((RenderRectangle.Right / 2) +104, 225, 0.4f);
			sliderlabel.Visible = false;
			AddControl(sliderlabel);

			// Mouse position test label
			Label mouselabel = Label.CreateNew("mouselabel", "", new Vector4(0.0f, 1.0f, 1.0f, 1.0f), Content.Load<SpriteFont>("helpfont"));
			mouselabel.Location = new Vector3(0.0f, 50.0f, 0.4f);
			mouselabel.Visible = false;
			AddControl(mouselabel);
		}					
	}	
}
