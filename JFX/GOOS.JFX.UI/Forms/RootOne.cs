using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GOOS.JFX.UI.Forms
{
	/// <summary>
	/// The most basic kind of root form is a form with no skin that simply collects other controls.
	/// </summary>
	public class RootOne : BaseGameForm
	{
		#region Constructors

		/// <summary>
		/// A new root with no skin that simply collects other controls.
		/// </summary>
		/// <param name="name">An optional name for this instance.</param>
		public RootOne(string name)
			: base()
		{ this.Name = name; }
		public RootOne()
			: this("RootOne")
		{}

		#endregion

		#region Event Handlers

		protected override void BaseGameForm_Init(IGameControl sender, GameControlTimedEventArgs args)
		{
			this.AltSkins = null;
			this.ChildForms = new Dictionary<string, IGameForm>();
			this.Controls = new Dictionary<string, IGameControl>();
			this.CurrentSkin = null;
			this.DefaultSkin = null;
			this.Enabled = true;
			this.Focus = true;
			this.Location = Vector3.Zero;			
			this.ParentForm = null;
			this.RenderColour = Vector4.One;
			this.RenderRectangle = Rectangle.Empty;
			this.Root = true;
			this.Shader = null;
			this.Visible = true;
			this.ElapsedTime = args.ElapsedTime;
			this.TotalTime = args.TotalTime;

			base.BaseGameForm_Init(sender, args);
		}		

		#endregion
		
	}
}
