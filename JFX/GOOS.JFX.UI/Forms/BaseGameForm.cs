using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GOOS.JFX.UI.Forms
{
	/// <summary>
	/// Basic implementation of a Game Form. All forms inherit from this one.
	/// 
	/// IGameForm inherits IGameControl so all classes that inherit BaseGameForm implement both interfaces.
	/// 
	/// </summary>
	public class BaseGameForm : IGameForm
	{		
		#region Members

		//Control
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

		//Form
		private bool mRoot;
		private Dictionary<string, IGameControl> mControls;
		private Dictionary<string, IGameForm> mForms;

		//Root forms should have this
		private UIComponent mUI;

		#endregion

		#region IGameForm Properties					

		/// <summary>
		/// A handle back to the UI Component
		/// </summary>
		public UIComponent UI
		{
			get { return mUI; }
			set { mUI = value; }
		}

		/// <summary>
		/// True if this form is at root level
		/// </summary>
		public bool Root
		{
			get
			{
				return mRoot;
			}
			set
			{
				mRoot = value;
			}
		}

		/// <summary>
		/// The collection of controls at form level for this form.
		/// </summary>
		public Dictionary<string, IGameControl> Controls
		{
			get
			{
				return mControls;
			}
			set
			{
				mControls = value;
			}
		}		

		/// <summary>
		/// The collection of Child Forms for this form.
		/// </summary>
		public Dictionary<string, IGameForm> ChildForms
		{
			get
			{
				return mForms;
			}
			set
			{
				mForms = value;
			}
		}		

		#endregion

		#region IGameForm Methods				

		/// <summary>
		/// Add a new root level control to this form.
		/// </summary>
		/// <param name="control">The control to add</param>
		public void AddControl(IGameControl control)
		{
			Controls.Add(control.Name, control);
			control.ParentForm = this;
		}

		/// <summary>
		/// Remove a Control from the root level of this form.
		/// </summary>
		/// <param name="name">The name of the control to remove</param>
		public void RemoveControl(string name)
		{
			Controls.Remove(name);			
		}

		/// <summary>
		/// Return a Control in the collection.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public IGameControl GetControl(string name)
		{
			return Controls[name];
		}

		/// <summary>
		/// Add a Child form to this form
		/// </summary>
		/// <param name="form">The form to add</param>
		public void AddForm(IGameForm form)
		{
			ChildForms.Add(form.Name, form);
			form.ParentForm = this;
			form.Content = this.Content;
			form.UI = this.UI;
		}

		/// <summary>
		/// Remove a form from the child forms
		/// </summary>
		/// <param name="name">The name of the form to remove</param>
		public void RemoveForm(string name)
		{
			ChildForms.Remove(name);
		}

		/// <summary>
		/// Returns a form from the child forms collection by its name.
		/// </summary>
		/// <param name="name">The name of the form to get</param>
		/// <returns></returns>
		public IGameForm GetForm(string name)
		{
			return ChildForms[name];
		}

		#endregion

		#region IGameControl Properties

		public Rectangle FocusRectangle
		{
			get { return mFocusRectangle; }
			set { mFocusRectangle = value; }
		}

		/// <summary>
		/// Collection of source rectangles.
		/// </summary>
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
				mFocus = value;
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

		public event GameControlEventHandler MouseUp;

		public event GameControlEventHandler MouseEnter;

		public event GameControlEventHandler MouseLeave;

		public event GameControlEventHandler MouseHeld;

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
		public BaseGameForm()
		{
			Controls = new Dictionary<string, IGameControl>();
			ChildForms = new Dictionary<string, IGameForm>();
			WireUpEvents();
			Init(this, null);
			mFocusRectangle = Rectangle.Empty;
		}

		#endregion

		// Including the base event wiring method
		#region Private Methods

		/// <summary>
		/// Wire up the base events
		/// </summary>
		private void WireUpEvents()
		{
			Init += new GameControlTimedEventHandler(BaseGameForm_Init);
			Update += new GameControlTimedEventHandler(BaseGameForm_Update);
			Render += new GameControlRenderEventHandler(BaseGameForm_Render);
			Click += new GameControlEventHandler(BaseGameForm_Click);
			MouseDown += new GameControlEventHandler(BaseGameForm_MouseDown);
			MouseEnter += new GameControlEventHandler(BaseGameForm_MouseEnter);
			MouseLeave += new GameControlEventHandler(BaseGameForm_MouseLeave);
			MouseUp += new GameControlEventHandler(BaseGameForm_MouseUp);
			ChangeFocus += new GameControlEventHandler(BaseGameForm_ChangeFocus);
		}

		#endregion

		// Handle the base events
		#region Protected Virtual Event Handlers

		/// <summary>
		/// Dfault Render Event Handler
		/// </summary>
		protected virtual void BaseGameForm_Render(IGameControl sender, GameControlRenderEventArgs args)
		{
			ElapsedTime = args.ElapsedTime;
			TotalTime = args.TotalTime;

			//Render all controls at root level
			foreach (IGameControl control in Controls.Values)
				if(control.Visible)
					control.RaiseEvent(BaseControlEvents.Render, control, args);

			//Render all child forms 
			foreach (IGameForm form in ChildForms.Values)
				if(form.Visible)
					form.RaiseEvent(BaseControlEvents.Render,form,args);
		}
		/// <summary>
		/// Default Update Event Handler
		/// </summary>	
		protected virtual void BaseGameForm_Update(IGameControl sender, GameControlTimedEventArgs args)
		{
			//Focus
			this.Focus = (UI.FormWithFocus == this);				

			ElapsedTime = args.ElapsedTime;
			TotalTime = args.TotalTime;

			//Update all controls at root level
			foreach (IGameControl control in Controls.Values)
				control.RaiseEvent(BaseControlEvents.Update, control, args);

			//update all child forms 
			foreach (IGameForm form in ChildForms.Values)
				form.RaiseEvent(BaseControlEvents.Update, form, args);
		}
		/// <summary>
		/// Default Init Event Handler
		/// </summary>
		protected virtual void BaseGameForm_Init(IGameControl sender, GameControlTimedEventArgs args)
		{
			this.AltSkins = null;
			this.ChildForms = new Dictionary<string, IGameForm>();
			this.Controls = new Dictionary<string, IGameControl>();
			this.SourceRectangles = new Dictionary<string, Rectangle>();
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
			if (args != null)
			{
				this.ElapsedTime = args.ElapsedTime;
				this.TotalTime = args.TotalTime;
			}			
		}
		/// <summary>
		/// Default Click Event Handler
		/// </summary>
		protected virtual void BaseGameForm_Click(IGameControl sender, GameControlEventArgs args)
		{

		}
		/// <summary>
		/// Default Mouse Up Event Handler
		/// </summary>
		protected virtual void BaseGameForm_MouseUp(IGameControl sender, GameControlEventArgs args)
		{

		}
		/// <summary>
		/// Default Mounse Leave Event Handler
		/// </summary>
		protected virtual void BaseGameForm_MouseLeave(IGameControl sender, GameControlEventArgs args)
		{

		}
		/// <summary>
		/// Default mouse Enter Event Handler
		/// </summary>
		protected virtual void BaseGameForm_MouseEnter(IGameControl sender, GameControlEventArgs args)
		{

		}
		/// <summary>
		/// Default Mouse Down Event Handler
		/// </summary>
		protected virtual void BaseGameForm_MouseDown(IGameControl sender, GameControlEventArgs args)
		{

		}

		/// <summary>
		/// Default Mouse Down Event Handler
		/// </summary>
		protected virtual void BaseGameForm_MouseHeld(IGameControl sender, GameControlEventArgs args)
		{

		}

		/// <summary>
		/// Default Change focus Event Handler
		/// </summary>
		protected virtual void BaseGameForm_ChangeFocus(IGameControl sender, GameControlEventArgs args)
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
			}
		}

		#endregion
	}
}
