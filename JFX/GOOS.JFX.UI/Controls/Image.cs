using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GOOS.JFX.UI.Controls
{
	/// <summary>
	/// The static image control is probably the most basic control implementation
	/// </summary>
	public class Image : BaseGameControl
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		private Image() : base()
		{
		}

		#endregion		

		#region Factory

		/// <summary>
		/// Create a new image control from a texture
		/// </summary>
		/// <param name="name">The form level name of the control.</param>
		/// <param name="imageTexture">The texture to display as the image.</param>
		/// <param name="imageTexture">The colour with which to draw the image.</param>
		/// <returns>A new image control.</returns>
		public static Image CreateNew(string name, Texture2D imageTexture, Vector4 colour)
		{
			Image i = new Image();
			i.Name = name;
			i.DefaultSkin = imageTexture;
			i.CurrentSkin = i.DefaultSkin;
			i.RenderColour = colour;
			i.Visible = true;
			i.Enabled = true;
			i.RenderRectangle = new Rectangle(0, 0, imageTexture.Width, imageTexture.Height);

			return i;
		}
		public static Image CreateNew(string name, Texture2D imageTexture)
		{
			Vector4 c = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
			return CreateNew(name, imageTexture, c);
		}

		#endregion
	}
}
