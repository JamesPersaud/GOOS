using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GOOS.JFX.UI
{
	/// <summary>
	/// This is the component which handles the updating and rendering of all forms and controls in the game.
	/// </summary>
	public class UIComponent : DrawableGameComponent
	{
		#region Members

		private bool mActive;

		private ContentManager Content;

		private IGameForm mRootForm;
		private MousePointer mMousePointer;
		private SpriteBatch mGameSpriteBatch;

		private GraphicsDeviceManager GameGraphics;
		private MouseState mouseState;
		private int MouseHomeX;
		private int MouseHomeY;

		//focus
		private IGameControl mControlWithFocus;
		private IGameControl mControlWithInputFocus;
		private IGameForm mFormWithFocus;
		private List<IGameControl> mPickedControls;

		//mouse clicking.
		private List<MouseButtonStates> mMouseButtonHistory;

		//Key state
		private KeyboardState LastKeystate;
		private Queue<Keys> mKeyQueue;
		private bool mKeyboardActive;

		#endregion

		#region Properties

		/// <summary>
		/// Get or Set the control which currently has focus for text input.
		/// </summary>
		public IGameControl ControlWithInputFocus
		{
			get { return mControlWithInputFocus; }
			set { mControlWithInputFocus = value; }
		}

		/// <summary>
		/// Is the keyboard active for UI input.
		/// </summary>
		public bool KeyboardActive
		{
			get { return mKeyboardActive; }
			set { this.KeyQueue = new Queue<Keys>(); mKeyboardActive = value; }
		}

		/// <summary>
		/// The current keyboard input queue.
		/// </summary>
		public Queue<Keys> KeyQueue
		{
			get { return mKeyQueue; }
			set { mKeyQueue = value; }
		}

		/// <summary>
		/// Gets or sets whether the UI is active (and in control of the mouse)
		/// </summary>
		public bool Active
		{
			get { return mActive; }
			set { mActive = value; }
		}

		/// <summary>
		/// A history of mouse button activity.
		/// </summary>
		public List<MouseButtonStates> MouseButtonHistory
		{
			get {return mMouseButtonHistory;}
			set { mMouseButtonHistory = value; }
		}

		/// <summary>
		/// A reference to the form that currently has the focus.
		/// </summary>
		public IGameForm FormWithFocus
		{
			get { return mFormWithFocus; }
			set { mFormWithFocus = value; }
		}
		/// <summary>
		/// A reference to the control that currently has the focus.
		/// </summary>
		public IGameControl ControlWithFocus
		{
			get { return mControlWithFocus; }
			set { mControlWithFocus = value; }
		}

		/// <summary>
		/// The current list of visible and enabled controls overlapping the mouse pointer location.
		/// </summary>
		public List<IGameControl> PickedControls
		{
			get { return mPickedControls; }
		}

		/// <summary>
		/// Get or Set a reference to the game's sprite batch.
		/// </summary>
		public SpriteBatch GameSpriteBatch
		{
			get { return mGameSpriteBatch; }
			set { mGameSpriteBatch = value; }
		}

		/// <summary>
		/// The current mouse pointer.
		/// </summary>
		public MousePointer CurrentMousePointer
		{
			get { return mMousePointer; }
			set { mMousePointer = value; }
		}

		/// <summary>
		/// The root level form.
		/// </summary>
		public IGameForm RootForm
		{
			get
			{
				return mRootForm;
			}
			set
			{
				mRootForm = value;
				mRootForm.UI = this;
				mRootForm.Content = Content;
			}
		}

		#endregion

		#region constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="game">A reference to the the game.</param>
		public UIComponent(Microsoft.Xna.Framework.Game game, GraphicsDeviceManager graphics): base(game)
		{
			GameGraphics = graphics;
			Content = game.Content;
			MouseHomeX = GameGraphics.PreferredBackBufferWidth / 2;
			MouseHomeY = GameGraphics.PreferredBackBufferHeight / 2;

			mPickedControls = new List<IGameControl>();
			MouseButtonHistory = new List<MouseButtonStates>();

			Active = true;
			KeyboardActive = true;
		}		

		#endregion		

		#region Picking		

		/// <summary>
		/// Pick the top control that falls under the mouse picked coords.
		/// </summary>
		public IGameControl PickTopControl(bool focus,bool click)
		{
			IGameControl topcontrol = null;
			foreach (IGameControl control in mPickedControls)
				if (topcontrol == null || control.Location.Z <= topcontrol.Location.Z)
					topcontrol = control;

			if(topcontrol != null)
			{
				if (focus)
				{
					//Focus picked control
					ControlWithFocus = topcontrol;
					FormWithFocus = topcontrol.ParentForm;
				}
				if (click)
				{
					topcontrol.RaiseEvent(BaseControlEvents.Click, topcontrol, new GameControlEventArgs());
					topcontrol.ParentForm.RaiseEvent(BaseControlEvents.Click, topcontrol, new GameControlEventArgs());
				}
			}

			return topcontrol;
		}

		/// <summary>
		/// Recursively build a list of controls overlapping a screen coord.
		/// </summary>
		/// <param name="control">The IGameControl to check (and check all children of)</param>
		/// <param name="list">The list to add controls to</param>
		/// <param name="visible">Whether to pick only visible controls.</param>
		/// <param name="enabled">Whether to pick only enabled controls.</param>
		public void PickControlList(Point coords, IGameControl control, ref List<IGameControl> list,bool visible,bool enabled)
		{
			//control is a form
			if (control != null && control is IGameForm)
			{
				foreach (IGameControl childcontrol in ((IGameForm)control).Controls.Values)
					PickControlList(coords, childcontrol, ref list, visible, enabled);
				foreach (IGameForm childform in ((IGameForm)control).ChildForms.Values)
					PickControlList(coords, childform, ref list, visible, enabled);				
			}
			//control is a control
			else
			{
				if (control != null && control.Visible == visible && control.Enabled == enabled)
				{
					//Vector3 v = control.GetAbsoluteLocation();
					Point topleft = new Point(control.FocusRectangle.X, control.FocusRectangle.Y);
					Point bottomright = new Point(control.FocusRectangle.Width + topleft.X,control.FocusRectangle.Height + topleft.Y);

					if (coords.X >= topleft.X && coords.X <= bottomright.X &&
						coords.Y >= topleft.Y && coords.Y <= bottomright.Y)
					{
						list.Add(control);
					}
				}
			}
		}

		#endregion

		#region Mouse Button Activity

		/// <summary>
		/// Manage mouse button history
		/// </summary>
		/// <param name="currentState">The current mouse state</param>
		private void ManageMouseButtonHistory(MouseState newMouseState, int time)
		{
			//mouseState is the last mouse state
			//newMouseState is the current state			

			//Is this a mouse held event?
			if (newMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Pressed && ControlWithFocus != null)
				ControlWithFocus.RaiseEvent(BaseControlEvents.MouseHeld, ControlWithFocus, new GameControlEventArgs());

			//If there is a change in buttons, add to the button history.
			if (newMouseState.LeftButton != mouseState.LeftButton ||
				newMouseState.MiddleButton != mouseState.MiddleButton ||
				newMouseState.RightButton != mouseState.RightButton)
			{
				//If the last button history was more than a second ago, clear the history
				if (MouseButtonHistory.Count > 0 && (time - MouseButtonHistory[MouseButtonHistory.Count - 1].Time) > 500)
					MouseButtonHistory.Clear();

				//Add this button state to the history
				string controlname = string.Empty;
				if(ControlWithFocus != null)
					controlname = ControlWithFocus.Name;				

				//Is this a mouse down event?
				if (newMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released && ControlWithFocus != null)
					ControlWithFocus.RaiseEvent(BaseControlEvents.MouseDown, ControlWithFocus, new GameControlEventArgs());

				//Is this a mouse up event?
				if (newMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed && ControlWithFocus != null)
				{
					ControlWithFocus.RaiseEvent(BaseControlEvents.MouseUp, ControlWithFocus, new GameControlEventArgs());
					//Is this mouse up event also a mouse click event?
					//It is if the last recorded button change was a left button down on the focused control.
					if (MouseButtonHistory.Count > 0 && MouseButtonHistory[MouseButtonHistory.Count - 1].Left == ButtonState.Pressed)
						ControlWithFocus.RaiseEvent(BaseControlEvents.Click, ControlWithFocus, new GameControlEventArgs());
				}				

				MouseButtonHistory.Add(new MouseButtonStates(newMouseState,time,controlname));
			}

			//Update the mouse state
			mouseState = Mouse.GetState();
		}

		#endregion

		#region Update

		/// <summary>
		/// Do all updating for this component (all forms and controls)
		/// </summary>
		/// <param name="gameTime">XNA's timer class</param>
		public override void Update(GameTime gameTime)
		{
			if (Active)
			{
				//Let forms and controls update themselves
				if (RootForm != null && RootForm.Enabled)
					RootForm.RaiseEvent(BaseControlEvents.Update, RootForm, new GameControlTimedEventArgs((float)gameTime.ElapsedGameTime.TotalMilliseconds, (float)gameTime.TotalGameTime.TotalMilliseconds));

				//Manage mouse clicks
				ManageMouseButtonHistory(Mouse.GetState(), (int)gameTime.TotalGameTime.TotalMilliseconds);

				int mousemoxex = mouseState.X - MouseHomeX;
				int mousemovey = mouseState.Y - MouseHomeY;

				Mouse.SetPosition(MouseHomeX, MouseHomeY);

				if (mousemovey != 0 || mousemoxex != 0)
				{
					mPickedControls = new List<IGameControl>();
					PickControlList(CurrentMousePointer.PickedScreenCoordinate, RootForm, ref mPickedControls, true, true);
				}

				//By default the mouse is in default state.
				MousePointerState mousestateenum = MousePointerState.Default;

				IGameControl topcontrol = null;
				if (mPickedControls != null && mPickedControls.Count > 0)
				{
					//Focus the top control (if there is one)
					topcontrol = PickTopControl(true, false);
					//Is a control is focused display the focusing mouse
					if (CurrentMousePointer.DrawRectangles.Keys.Contains(MousePointerState.ActiveArea))
						mousestateenum = MousePointerState.ActiveArea;
				}
				else if (mPickedControls.Count == 0)
					ControlWithFocus = null;

				if (mousemovey != 0 || mousemoxex != 0)
					CurrentMousePointer.Update(new Point(mousemoxex, mousemovey), mousestateenum);

				if (LastKeystate == null)				
					LastKeystate = Keyboard.GetState();				
				else if(KeyboardActive)
				{
					if (KeyQueue == null)
						KeyQueue = new Queue<Keys>();
					//Keyboard handler/listener
					KeyboardState keystate = Keyboard.GetState();
					//These keys are currently help down.
					Keys[] keys = keystate.GetPressedKeys();
					//Any keys that weren't already being help down, get sent to the keypress buffer.
					foreach (Keys k in keys)					
						if (!LastKeystate.IsKeyDown(k))						
							KeyQueue.Enqueue(k);										

					LastKeystate = keystate;
				}


				base.Update(gameTime);
			}
		}

		#endregion

		#region Draw

		/// <summary>
		/// Do all drawing for this component (all forms and controls)
		/// </summary>
		/// <param name="gameTime">XNA's timer class</param>
		public override void Draw(GameTime gameTime)
		{
			if (Active)
			{
				GameSpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);

				//Let forms and controls draw themselves
				if (RootForm != null && RootForm.Visible)
					RootForm.RaiseEvent(BaseControlEvents.Render, RootForm, new GameControlRenderEventArgs((float)gameTime.ElapsedGameTime.TotalMilliseconds, (float)gameTime.TotalGameTime.TotalMilliseconds, GameSpriteBatch));

				//Draw the mouse pointer
				GameSpriteBatch.Draw(CurrentMousePointer.Skin,
					new Vector2((float)CurrentMousePointer.Location.X,
					(float)CurrentMousePointer.Location.Y),
					CurrentMousePointer.CurrentDrawRectangle,
					Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.01f);

				GameSpriteBatch.End();

				base.Draw(gameTime);
			}
		}

		#endregion
	}
}
