using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GOOS.JFX.UI
{
	/// <summary>
	/// All GameConrols inherit from the BaseGameControl, this control provides:
	/// 
	/// Implementation of the IGameControl interface
	/// Basic event handlers common to all controls and a method for the observer to raise events
	/// Generic control implementation
	/// 
	/// </summary>
	public class BaseGameControl : IGameControl
	{
		#region Members

		private Rectangle mFocusRectangle;
		private string mName;
		private Vector3 mLocation;
		private bool mVisible;
		private bool mEnabled;
		private bool mFocus;
		private Texture2D mDefaultSkin;
		private Dictionary<string, Texture2D> mAltSkins;
		private Texture2D mCurrentSkin;
		private Rectangle mRenderRectangle;
		private Vector4 mRenderColour;
		private Effect mShader;
		private float mTotalTime;
		private float mElapsedTime;
		private IGameForm mParentForm;
		private ContentManager mContent;
		private Dictionary<string, Rectangle> mSourceRectangles;

		#endregion		

		#region IGameControl Properties

		public Rectangle FocusRectangle
		{
			get { return mFocusRectangle; }
			set { mFocusRectangle = value; }
		}
		

		public Dictionary<string, Rectangle> SourceRectangles
		{
			get { return mSourceRectangles; }
			set { mSourceRectangles = value; }
		}
		/// <summary>
		/// Gets the position of the mouse relative to the top left corner of the control
		/// </summary>
		public Point RelativeMouseCoords
		{
			get 
			{
				return new Point(ParentForm.UI.CurrentMousePointer.Location.X - (int)Location.X,
					ParentForm.UI.CurrentMousePointer.Location.Y - (int)Location.Y);
			}
		}
		/// <summary>
		/// A Reference to the content manager.
		/// </summary>
		public ContentManager Content
		{
			get
			{
				return mContent;
			}
			set
			{
				mContent = value;
			}
		}
		/// <summary>
		/// A Reference to the parent form of this Game Control.
		/// </summary>
		public IGameForm ParentForm
		{
			get
			{
				return mParentForm;
			}
			set
			{
				mParentForm = value;
			}
		}
		/// <summary>
		/// The form level name of this GameControl 
		/// </summary>
		public string Name
		{
			get
			{
				return mName;
			}
			set
			{
				mName = value;
			}
		}
		/// <summary>
		/// The location of the GameControl relative to its parent control or form.
		/// </summary>
		public Microsoft.Xna.Framework.Vector3 Location
		{
			get
			{
				return mLocation;
			}
			set
			{
				mLocation = value;
			}
		}
		/// <summary>
		/// The visibility of this GameControl - set to false it will not render.
		/// </summary>
		public bool Visible
		{
			get
			{
				return mVisible;
			}
			set
			{
				mVisible = value;
			}
		}
		/// <summary>
		/// Should reflect whether or not this GameControl had the focus of the application at the last Update.
		/// Will raise a new ChangeFocus event when set.
		/// </summary>
		public bool Focus
		{
			get
			{
				return mFocus;
			}
			set
			{
				bool oldfocus = mFocus;
				mFocus = value;
				if(oldfocus != mFocus)
					ChangeFocus(this, new GameControlEventArgs());
			}
		}
		/// <summary>
		/// The availability of this control - Whether it can be focused or used in any way.
		/// </summary>
		public bool Enabled
		{
			get
			{
				return mEnabled;
			}
			set
			{
				mEnabled = value;
			}
		}
		/// <summary>
		/// The Default Skin of this control is the Texture that will normally be rendered for it.
		/// </summary>
		public Microsoft.Xna.Framework.Graphics.Texture2D DefaultSkin
		{
			get
			{
				return mDefaultSkin;
			}
			set
			{
				mDefaultSkin = value;
			}
		}
		/// <summary>
		/// Alternative skins for this control. Indexed by string (texture name)
		/// </summary>
		public Dictionary<string, Microsoft.Xna.Framework.Graphics.Texture2D> AltSkins
		{
			get
			{
				return mAltSkins;
			}
			set
			{
				mAltSkins = value;
			}
		}
		/// <summary>
		/// Determines the texture that should be used to render the GameControl on the next Render
		/// </summary>
		public Microsoft.Xna.Framework.Graphics.Texture2D CurrentSkin
		{
			get
			{
				return mCurrentSkin;
			}
			set
			{
				mCurrentSkin = value;
			}
		}
		/// <summary>
		/// The source rectangle to be used in rendering the current skin in the next render
		/// </summary>
		public Microsoft.Xna.Framework.Rectangle RenderRectangle
		{
			get
			{
				return mRenderRectangle;
			}
			set
			{
				mRenderRectangle = value;
			}
		}
		/// <summary>
		/// The colour r/g/b/a that should be used to render the GameControl on the next render
		/// </summary>
		public Microsoft.Xna.Framework.Vector4 RenderColour
		{
			get
			{
				return mRenderColour;
			}
			set
			{
				mRenderColour = value;
			}
		}
		/// <summary>
		/// The Pixel shader that should be used to render the GameControl on the next render.
		/// </summary>
		public Effect Shader
		{
			get
			{
				return mShader;
			}
			set
			{
				mShader = value;
			}
		}
		/// <summary>
		/// The amount of time (in milliseconds) elapsed since the last update
		/// </summary>
		public float ElapsedTime
		{
			get
			{
				return mElapsedTime;
			}
			set
			{
				mElapsedTime = value;
			}
		}
		/// <summary>
		/// The amount of time (in milliseconds) elapsed since the game was started
		/// </summary>
		public float TotalTime
		{
			get
			{
				return mTotalTime;
			}
			set
			{
				mTotalTime = value;
			}
		}
		#endregion

		#region IGameControl Events
		public event GameControlTimedEventHandler Init;

		public event GameControlTimedEventHandler Update;

		public event GameControlRenderEventHandler Render;

		public event GameControlEventHandler ChangeFocus;

		public event GameControlEventHandler Click;

		public event GameControlEventHandler MouseDown;

		public event GameControlEventHandler MouseHeld;

		public event GameControlEventHandler MouseUp;

		public event GameControlEventHandler MouseEnter;

		public event GameControlEventHandler MouseLeave;

		#endregion

		#region IGameControlMethods
		
		/// <summary>
		/// Gets this control's absolute location in screen coordinates.
		/// </summary>
		/// <returns>This control's absolute screen location.</returns>
		public Vector3 GetAbsoluteLocation()
		{
			Vector3 v = new Vector3();
			v = Location;
			AdjustAbsoluteVector(ParentForm, ref v);
			return v;
		}

		/// <summary>
		/// Recursively add to the location vector to get the absolute vector adjusting by the locations of the
		/// parent forms up through the hierarchy to the root form.
		/// </summary>
		/// <param name="control">The control to position</param>
		/// <param name="vector">The vector to hold the adjusted location</param>
		private void AdjustAbsoluteVector(IGameControl control, ref Vector3 vector)
		{
			if (control != null)
			{
				vector += control.Location;
				AdjustAbsoluteVector(control.ParentForm, ref vector);
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>		
		public BaseGameControl()
		{						
			WireUpEvents();
			Init(this, null);
			mFocusRectangle = Rectangle.Empty;
		}
		public BaseGameControl(float elapsedtime,float totaltime)
		{
			WireUpEvents();
			GameControlTimedEventArgs a = new GameControlTimedEventArgs(elapsedtime,totaltime);		
			Init(this, a);
		}		

		#endregion

		// Including the base event wiring method
		#region Private Methods

		/// <summary>
		/// Wire up the base events
		/// </summary>
		private void WireUpEvents()
		{
			Init += new GameControlTimedEventHandler(BaseGameControl_Init);
			Update += new GameControlTimedEventHandler(BaseGameControl_Update);
			Render += new GameControlRenderEventHandler(BaseGameControl_Render);
			Click += new GameControlEventHandler(BaseGameControl_Click);
			MouseDown += new GameControlEventHandler(BaseGameControl_MouseDown);
			MouseEnter += new GameControlEventHandler(BaseGameControl_MouseEnter);
			MouseLeave += new GameControlEventHandler(BaseGameControl_MouseLeave);
			MouseUp += new GameControlEventHandler(BaseGameControl_MouseUp);	
			ChangeFocus += new GameControlEventHandler(BaseGameControl_ChangeFocus);
			MouseHeld += new GameControlEventHandler(BaseGameControl_MouseHeld);
		}		

		#endregion

		// Handle the base events
		#region Protected Virtual Event Handlers							

		/// <summary>
		/// Dfault Render Event Handler
		/// </summary>
		protected virtual void BaseGameControl_Render(IGameControl sender, GameControlRenderEventArgs args)
		{
			//The basic render for a control is just to draw the skin texture at its location (relative to the form location
			Vector3 absolute = GetAbsoluteLocation();
			Vector2 screenloc = new Vector2(absolute.X, absolute.Y);
			float depth = absolute.Z;
			Color c = new Color(RenderColour);

			//Draw
			args.spriteBatch.Draw(CurrentSkin, screenloc, RenderRectangle, c, 0, Vector2.Zero,1.0f, SpriteEffects.None, depth);
			FocusRectangle = new Rectangle((int)screenloc.X, (int)screenloc.Y, RenderRectangle.Width, RenderRectangle.Height);

		}
		/// <summary>
		/// Default Update Event Handler
		/// </summary>	
		protected virtual void BaseGameControl_Update(IGameControl sender, GameControlTimedEventArgs args)
		{			
			//Set focus
			Focus = (ParentForm.UI.ControlWithFocus == this);		
		}
		/// <summary>
		/// Default Init Event Handler
		/// </summary>
		protected virtual void BaseGameControl_Init(IGameControl sender, GameControlTimedEventArgs args)
		{
			mSourceRectangles = new Dictionary<string, Rectangle>();
			if (args != null)
			{
				ElapsedTime = args.ElapsedTime;
				TotalTime = args.TotalTime;
			}
		}
		/// <summary>
		/// Default Click Event Handler
		/// </summary>
		protected virtual void BaseGameControl_Click(IGameControl sender, GameControlEventArgs args)
		{
			
		}
		/// <summary>
		/// Default Mouse Up Event Handler
		/// </summary>
		protected virtual void BaseGameControl_MouseUp(IGameControl sender, GameControlEventArgs args)
		{
			
		}
		/// <summary>
		/// Default Mounse Leave Event Handler
		/// </summary>
		protected virtual void BaseGameControl_MouseLeave(IGameControl sender, GameControlEventArgs args)
		{
			
		}
		/// <summary>
		/// Default mouse Enter Event Handler
		/// </summary>
		protected virtual void BaseGameControl_MouseEnter(IGameControl sender, GameControlEventArgs args)
		{
			
		}
		/// <summary>
		/// Default Mouse Down Event Handler
		/// </summary>
		protected virtual void BaseGameControl_MouseDown(IGameControl sender, GameControlEventArgs args)
		{
			
		}
		/// <summary>
		/// Default Mouse Down Event Handler
		/// </summary>
		protected virtual void BaseGameControl_MouseHeld(IGameControl sender, GameControlEventArgs args)
		{

		}
		/// <summary>
		/// Default Change focus Event Handler
		/// </summary>
		protected virtual void BaseGameControl_ChangeFocus(IGameControl sender, GameControlEventArgs args)
		{

		}		

		#endregion   

		// For the observer to raise events
		#region Public Event Raisers

		/// <summary>
		/// Use this method to raise a base GameControl event. (typically called by the observer.)
		/// </summary>
		/// <param name="e">The event to raise</param>
		/// <param name="sender">The GameControl sending the event</param>
		/// <param name="args">The event arguments</param>
		public void RaiseEvent(BaseControlEvents e, IGameControl sender, GameControlEventArgs args)
		{
			switch (e)
			{
				case BaseControlEvents.ChangeFocus: ChangeFocus(sender, args); break;
				case BaseControlEvents.Click: Click(sender, args); break;
				case BaseControlEvents.Init: Init(sender, (GameControlTimedEventArgs)args); break;
				case BaseControlEvents.MouseDown: MouseDown(sender, args); break;
				case BaseControlEvents.MouseEnter: MouseEnter(sender, args); break;
				case BaseControlEvents.MouseLeave: MouseLeave(sender, args); break;
				case BaseControlEvents.MouseUp: MouseUp(sender, args); break;
				case BaseControlEvents.Render: Render(sender, (GameControlRenderEventArgs)args); break;
				case BaseControlEvents.Update: Update(sender, (GameControlTimedEventArgs)args); break;
				case BaseControlEvents.MouseHeld: MouseHeld(sender, args); break;
			}
		}

		#endregion
	}
}
