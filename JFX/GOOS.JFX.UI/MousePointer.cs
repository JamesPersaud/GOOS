using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GOOS.JFX.UI
{
	/// <summary>
	/// A class to represent the mouse pointer.
	/// </summary>
	public class MousePointer
	{
		#region Members

		private Effect mShader;
		private Point mHotSpot;
		private Point mLocation;
		private Texture2D mSkin;
		private Rectangle mCurrentDrawRectangle;
		private MousePointerState mState;
		private Dictionary<MousePointerState, Rectangle> mDrawRectangles;
		private Rectangle mScreenArea;	

		#endregion

		#region Properties

		/// <summary>
		/// False if Shader is null, true otherwise.
		/// </summary>
		public bool HasOwnShader
		{
			get { return (mShader != null); }
		}

		/// <summary>
		/// The effect file containing this mouse pointer's current pixel shader.
		/// </summary>
		public Effect Shader
		{
			get { return mShader; }
			set { mShader = value; }
		}

		/// <summary>
		/// The Point in screen coordinates currently being picked by the mouse.
		/// </summary>
		public Point PickedScreenCoordinate
		{
			get{return new Point(mLocation.X + mHotSpot.X, mLocation.Y + mHotSpot.Y);}
		}

		/// <summary>
		/// The hotspot for mouse clicks.
		/// </summary>
		public Point HotSpot
		{
			get { return mHotSpot; }
			set { mHotSpot = value; }
		}

		/// <summary>
		/// The area of screen coords to restrict the mouse to.
		/// </summary>
		public Rectangle ScreenArea
		{
			get { return mScreenArea; }
			set { mScreenArea = value; }
		}

		/// <summary>
		/// Get or Set what to draw for each mouse state.
		/// </summary>
		public Dictionary<MousePointerState, Rectangle> DrawRectangles
		{
			get { return mDrawRectangles; }
			set { mDrawRectangles = value; }
		}

		/// <summary>
		/// The current state of the mouse (normal, over an active area, loading, over a prohibited click area - etc)
		/// </summary>
		public MousePointerState State
		{
			get { return mState; }
			set 
			{ 
				mState = value;
				if (DrawRectangles.ContainsKey(mState))
					CurrentDrawRectangle = DrawRectangles[mState];
				else
					CurrentDrawRectangle = DrawRectangles[MousePointerState.Default];
			}
		}

		/// <summary>
		/// The source rectangle to be used in the next Draw.
		/// </summary>
		public Rectangle CurrentDrawRectangle
		{
			get { return mCurrentDrawRectangle; }
			set { mCurrentDrawRectangle = value; }
		}

		/// <summary>
		/// The texture from which to draw the mouse pointer.
		/// </summary>
		public Texture2D Skin
		{
			get { return mSkin; }
			set { mSkin = value; }
		}

		/// <summary>
		/// The location of the top left corner of the mouse in screen coordinates.
		/// </summary>
		public Point Location
		{
			get { return mLocation; }set{mLocation = value;}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public MousePointer()
		{
			mHotSpot = Point.Zero;
			mLocation = Point.Zero;
			mCurrentDrawRectangle = Rectangle.Empty;
			mState = MousePointerState.Default;
			mDrawRectangles = new Dictionary<MousePointerState, Rectangle>();
			mScreenArea = Rectangle.Empty;			
		}

		#endregion		

		#region Factory

		/// <summary>
		/// Creates a new Mouse pointer object from a Texture 2D
		/// </summary>
		/// <param name="tex">The Texture to use to render the mouse states</param>
		/// <param name="hotspot">The hotspot location in mouse texture coordinates</param>
		/// <param name="loc">The starting location of the mouse pointer</param>
		/// <param name="rectangles">A list of source rectangles to draw different mouse states</param>
		/// <param name="screen">The screen coordinate area that the mouse is limited to.</param>
		/// <returns>A new Mouse Pointer Object</returns>
		public static MousePointer CreateFromTexture2D(Texture2D tex, Point hotspot, Point loc, Dictionary<MousePointerState,Rectangle> rectangles,
			Rectangle screen)
		{
			MousePointer m = new MousePointer();
			m.SetSkin(tex, rectangles);
			m.HotSpot = hotspot;
			m.Location = loc;
			m.ScreenArea = screen;
			return m;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Assign a new texture to be the skin for the mouse pointer
		/// </summary>
		/// <param name="tex">The texture to use</param>
		/// <param name="dic">A list of source rectangles keyed by mouse state.</param>
		public void SetSkin(Texture2D tex, Dictionary<MousePointerState, Rectangle> rects)
		{
			mSkin = tex;

			if (rects == null)
				rects = new Dictionary<MousePointerState, Rectangle>();
			if (!rects.ContainsKey(MousePointerState.Default))
				rects.Add(MousePointerState.Default, new Rectangle(0, 0, 16, 16));

			mDrawRectangles = rects;
			mCurrentDrawRectangle = rects[MousePointerState.Default];
		}

		/// <summary>
		/// Mouse Update Method
		/// </summary>
		/// <param name="move">The amount by which to move the mouse location</param>
		/// <param name="state">The state to switch the mouse to.</param>
		public void Update(Point move, MousePointerState state)
		{
			if (move.X != 0)
			{
				mLocation.X += move.X;
				if (PickedScreenCoordinate.X < ScreenArea.Left)
					mLocation.X = ScreenArea.Left - mHotSpot.X;
				if (PickedScreenCoordinate.X > ScreenArea.Right)
					mLocation.X = ScreenArea.Right - mHotSpot.X;
			}

			if (move.Y != 0)
			{
				mLocation.Y += move.Y;
				if (PickedScreenCoordinate.Y < ScreenArea.Top)
					mLocation.Y = ScreenArea.Top - mHotSpot.Y;
				if (PickedScreenCoordinate.Y > ScreenArea.Bottom)
					mLocation.Y = ScreenArea.Bottom - mHotSpot.Y;
			}

			if (mState != state)			
				State = state;							
		}

		#endregion
	}
}
