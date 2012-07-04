using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GOOS.JFX.Core;
using GOOS.JFX.Scripting;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Content;

namespace GOOS.JFX.Entity
{
    /// <summary>
	/// 
	/// TODO:
	/// 
	/// Actors need to be able to serialize themselves to XML in order to be load/saveable
	/// Actors need scripts - defined in files - to provide custom handling of events.
	/// Actors need to raise events during the update process - or does the stage manager do this?
	/// 
	/// OnInit
	/// 
	/// :::
	/// 
    /// The base class for all actor entities (monsters etc)
    /// </summary>	
    public class Actor : IScriptable,IEntity, IRenderable
    {
		#region IEntity Members

		public event EntityEventHandler Init;

		[ContentSerializerIgnore]
		public string TypeName
		{
			get{return mTypeName;}
			set{mTypeName = value;}
		}

		[ContentSerializerIgnore]
		public string InstanceName
		{
			get{return mInstanceName;}
			set{mInstanceName = value;}
		}

		[ContentSerializerIgnore]
		public string InstanceID
		{
			get{return mInstanceID;}
			set{mInstanceID = value;}
		}

		[ContentSerializerIgnore]
		public string AppearanceID
		{
			get{return mAppearanceID;}
			set{mAppearanceID = value;}
		}

		[ContentSerializerIgnore]
		public Dictionary<string,string> EventScripts
		{
			get { return mEventScripts; }
			set{mEventScripts = value;}
		}

		#endregion

		#region IScriptable Members

		[ContentSerializerIgnore]
		public string AssemblyName
		{
			get { return "GOOS.JFX.Entity"; }
		}

		[ContentSerializerIgnore]
		public string ClassName
		{
			get { return "GOOS.JFX.Entity.Actor"; }
		}

		[ContentSerializerIgnore]
		public Stack<KeyValuePair<string, string>> ScriptingFunctions
		{
			get { return new Stack<KeyValuePair<string, string>>(); }
		}

		[ContentSerializerIgnore]
		public string InitialScript
		{
			get{return string.Empty;}
			set{throw new NotImplementedException();}
		}

		#endregion

		#region IRenderable Members

		[ContentSerializerIgnore]
		public Matrix RenderWorldMatrix
		{
			get{return WorldMatrix;}
			set{WorldMatrix = value;}
		}

		[ContentSerializerIgnore]
		public Model DefaultModel
		{
			get { return mDefaultModel; }
			set { mDefaultModel = value; }			
		}

		[ContentSerializerIgnore]
		public Dictionary<string,Material> DefaultMaterials
		{
			get { return mDefaultMaterials; }
			set{mDefaultMaterials = value;}
		}

		//Private Members
		private Model mDefaultModel;

		[ContentSerializer]
		private Dictionary<string, Material> mDefaultMaterials;
		[ContentSerializer]
		private string DefaultModelName;

		public bool Render()
		{
			bool success = true;

			//TODO - Split rendering of meshes by material & mesh name
			DefaultMaterials["default"].DrawComplexModelWithMaterial(DefaultModel, ref WorldMatrix, string.Empty);

			return success;
		}

		public bool Render(QuakeCamera cam, float dist)
		{
			bool success = true;

			if (Vector3.Distance(CurrentLocation, cam.Position) <= dist)
			{
				BoundingFrustum frustum = new BoundingFrustum(cam.ViewMatrix * cam.ProjectionMatrix);
				if(frustum.Intersects(new BoundingSphere(CurrentLocation,32.0f)))
				{
					//TODO - Split rendering of meshes by material & mesh name
					DefaultMaterials["default"].DrawComplexModelWithMaterial(DefaultModel, ref WorldMatrix, string.Empty);
				}
			}

			return success;
		}

		#endregion

		//Scripts
		[ContentSerializer]
		private Dictionary<string, string> mEventScripts;

		//Stage management info.
		[ContentSerializer]
		public string mTypeName;
		[ContentSerializer]
		public string mInstanceName;
		[ContentSerializer]
		public string mInstanceID; // GUID     A unique id created when the actor is instantiated.
		[ContentSerializer]
		public string mAppearanceID; //GUID    A unique id re-created when the actor enters the stage.

        //TODO: make these properties.

		[ContentSerializer]
		public float ScalingFactor;

		[ContentSerializer]
		public int gridX;						//TODO: COMMENT or stop using
		[ContentSerializer]
		public int gridY;						//TODO: COMMENT or stop using

		[ContentSerializer]
		public int target_gridX;				// TODO: COMMENT or stop using
		[ContentSerializer]
		public int target_gridY;				// TODO: COMMENT or stop using

		[ContentSerializer]
		public bool gridCloseed;				// TODO: COMMENT or stop using
		[ContentSerializer]
        public Vector3 CurrentLocation;			// The actor's current location in world space
		[ContentSerializer]
		public float CurrentFacing;				// The angle that the actor is currently facing - in radians
		[ContentSerializer]
        public Matrix WorldMatrix;				// The actor's world matrix for rendering (see IRenderable implementation)
		[ContentSerializer]
        public float FacingOffset;				// An offset for the actor's facing angle to ensure its front always faces CurrentFacing.

		[ContentSerializer]
        public float turnspeed;					// Speed at which the actor can turn on the y axis (to face a point)

		[ContentSerializer]
        public float movespeed;					// Speed at which the actor can move about the XZ plane
		[ContentSerializer]
        public float difference;				// TODO: COMMENT or stop using

		[ContentSerializer]
        public FLAG_Behaviours Behaviours;		// BEHAVIOUR flags drive the actor's actions
		[ContentSerializer]
        public FLAG_Actions Actions;			// ACTION flags describe the actor's current actions.

		[ContentSerializer]
        public bool Alive;						// The actor (if mortal) is alive.
		[ContentSerializer]
        public bool InPlay;						// The actor is in play (deprecated by the stage manager?)

        //test
		[ContentSerializer]
        public Order CurrentOrder;				// Current order if the Actor is under an order
		[ContentSerializer]
        public Queue<Order> OrderQueue;			// Queue of multiple orders
		[ContentSerializer]
        public Queue<Vector2> SimpleMoveQueue;	// Pathfinding queue
		[ContentSerializer]
        public List<Vector2> SimplePatrolList;  // Pathfinding list 
		[ContentSerializer]
        public Vector2 CurrentTarget;			// Grid location of current movement target

        //enemy = player... usually.
		[ContentSerializer]
        public Vector3 EnemyLocation;			// Location of current combat target/enemy in 3D space

		[ContentSerializer]
        public int CurrentPatrolIndex;
                
		[ContentSerializer]
        public Vector3 WanderVector;			//Used when the actor is wandering aimlessly.
		[ContentSerializer]
		public int WanderFacing;				//Used when the actor is wandering aimlessly
		[ContentSerializer]
        public bool[] CollisionGrid;			//boolean array for simplified grid collision detection.
		[ContentSerializer]
        public int GridWidth;					//Width of the current grid.
		[ContentSerializer]
        public int GridHeight;					//Height of the current grid.		
		[ContentSerializer]
        Random rand;							//Random number sequence.
		[ContentSerializer]
        float WanderTimer;						//A timer used to change direction when wandering.

		[ContentSerializer]
        public float FightRange;            // The range at which the actor can engage opponents in melee combat.

        //Awareness/intelligence settings.
		[ContentSerializer]
        public float FOV;                   // Actor's field of view in degrees.
		[ContentSerializer]
        public float HearingRadius;         // Not the actual range of the actor's hearing but the range at which it will be aware of its enemy's location when alerted.
		[ContentSerializer]
        public float SightRadius;           // How far the actor can see. Useful, e.g. if the player comes within a monster's FOV, LOS and sight radius it might become alerted
		[ContentSerializer]
        public float SenseRadius;           // Ever felt like you're being watched? Even if the actor can't see something, he might sense it's near and
                                            // turn to investigate. e.g. Some monsters can be easier to sneak up on than others!       

        /// <summary>
        /// Default constructor
        /// </summary>
        public Actor()
        {
			//Shadow values
            CurrentLocation = Vector3.Zero;
            WorldMatrix = Matrix.Identity;
            CurrentFacing = 0.0f;
            FacingOffset = 0.0f;
            FOV = 45.0f;
            movespeed = 0.2f;
            CurrentTarget = Vector2.Zero;
            OrderQueue = new Queue<Order>();
            rand = new Random();
            WanderVector = Vector3.Zero;
            EnemyLocation = Vector3.Zero;
            CurrentPatrolIndex = 0;

			ScalingFactor = 1.0f;

            FightRange = 4;
            HearingRadius = 100;
            SightRadius = 64;
            SenseRadius = 16;

            turnspeed = 3;

			//IEntity support
			InstanceID = System.Guid.NewGuid().ToString();

			mDefaultMaterials = new Dictionary<string, Material>();
        }
            
        public void Update()
        {                       
            Update(Vector3.Zero,null,false);
        }

        public void Update(Vector3 enemy, Queue<Vector2> q, bool LOS)
        {
			//An evil geinus is always alerted.
			if ((Behaviours & FLAG_Behaviours.EVIL_GENIUS) != 0)
				if ((Actions & FLAG_Actions.ALERTED) != FLAG_Actions.ALERTED)
					Actions ^= FLAG_Actions.ALERTED;

			if ((Actions & FLAG_Actions.ALERTED) != 0)
			{
				if (LOS)
				{
					q = new Queue<Vector2>();
					SimpleMoveQueue = q;
				}
			}

			gridCloseed = GOOS.JFX.Core.Collision.CollideVector3WithGridSquare(ref CurrentLocation, ref CollisionGrid, GridWidth, GridHeight, 16, 16, ref gridX, ref gridY);

			Vector3 tv;
			tv = new Vector3(CurrentTarget.X+8, CurrentLocation.Y, CurrentTarget.Y+8);

			GOOS.JFX.Core.Collision.CollideVector3WithGridSquare(ref tv, ref CollisionGrid, GridWidth, GridHeight, 16, 16, ref target_gridX, ref target_gridY);

            if (enemy != Vector3.Zero)            
                EnemyLocation = enemy;            

            // am I under an order?
            if (CurrentOrder != null)
            {
                if ((Actions & FLAG_Actions.ACTING_BY_INSTINCT) == FLAG_Actions.ACTING_BY_INSTINCT)Actions ^= FLAG_Actions.ACTING_BY_INSTINCT;
                if((Actions & FLAG_Actions.ACTING_UNDER_ORDERS) != FLAG_Actions.ACTING_UNDER_ORDERS)Actions ^= FLAG_Actions.ACTING_UNDER_ORDERS;
                // execute order
                ExecuteCurrentOrder();
            }
            else
            {
                if ((Actions & FLAG_Actions.ACTING_UNDER_ORDERS) == FLAG_Actions.ACTING_UNDER_ORDERS)Actions ^= FLAG_Actions.ACTING_UNDER_ORDERS;
                //Are there any more orders in the queue?
                if (OrderQueue.Count > 0)
                {
                    //Get next order
                    CurrentOrder = OrderQueue.Dequeue();
                    InitOrder();
                }
                else
                {
                    if ((Actions & FLAG_Actions.ACTING_BY_INSTINCT) != FLAG_Actions.ACTING_BY_INSTINCT)Actions ^= FLAG_Actions.ACTING_BY_INSTINCT;
                    //Behave by instinct.    
                    if ((Behaviours & FLAG_Behaviours.WANDERER) != 0)
                    {
                        if ((Actions & FLAG_Actions.MOVING_TO_ATTACK) == FLAG_Actions.MOVING_TO_ATTACK) Actions ^= FLAG_Actions.MOVING_TO_ATTACK;
                        if ((Actions & FLAG_Actions.MOVING_TO_NODE) == FLAG_Actions.MOVING_TO_NODE) Actions ^= FLAG_Actions.MOVING_TO_NODE;
                        if ((Actions & FLAG_Actions.WANDERING_AIMLESSLY) != FLAG_Actions.WANDERING_AIMLESSLY) Actions ^= FLAG_Actions.WANDERING_AIMLESSLY;
                        WanderAimlessly();
                    }
                    if ((Behaviours & FLAG_Behaviours.PATROLLER) != 0)
                    {
                        if ((Actions & FLAG_Actions.MOVING_TO_ATTACK) == FLAG_Actions.MOVING_TO_ATTACK) Actions ^= FLAG_Actions.MOVING_TO_ATTACK;
                        if ((Actions & FLAG_Actions.MOVING_TO_NODE) != FLAG_Actions.MOVING_TO_NODE) Actions ^= FLAG_Actions.MOVING_TO_NODE;
                        Patrol();
                    }
					//Aggressive monster here.
                    if ((Behaviours & FLAG_Behaviours.AGGRESSIVE) != 0)
                    {
						//Alerted behaviour
                        if ((Actions & FLAG_Actions.ALERTED) != 0)
                        {
                            if (IsPointWithinRadius(EnemyLocation, FightRange, 4))
                            {
                                // Fight!
                                CurrentTarget = Vector2.Zero;
                                if ((Actions & FLAG_Actions.FIGHTING) != FLAG_Actions.FIGHTING)Actions ^= FLAG_Actions.FIGHTING;
                                if ((Actions & FLAG_Actions.MOVING_TO_ATTACK) == FLAG_Actions.MOVING_TO_ATTACK) Actions ^= FLAG_Actions.MOVING_TO_ATTACK;
                                TurnTowardsVetor3(EnemyLocation);
                            }
                            else
                            {
                                if ((Actions & FLAG_Actions.FIGHTING) == FLAG_Actions.FIGHTING)Actions ^= FLAG_Actions.FIGHTING;
                                if (IsPointWithinRadius(EnemyLocation, HearingRadius, 4))
                                {
                                    if ((Actions & FLAG_Actions.MOVING_TO_ATTACK) != FLAG_Actions.MOVING_TO_ATTACK) Actions ^= FLAG_Actions.MOVING_TO_ATTACK;
                                    if (q != null)
                                    {
                                        SimpleMoveQueue = q;
                                        CurrentTarget = Vector2.Zero;

                                        if (LOS && (Actions & FLAG_Actions.ALERTED) != 0)
                                        {
                                            CurrentTarget = new Vector2(EnemyLocation.X - 8, EnemyLocation.Z - 8);
											SimpleMoveQueue.Clear();
                                        }
                                        
                                        ExecuteMoveQueue();
                                    }
                                    else
                                    {
                                        if (LOS && (Actions & FLAG_Actions.ALERTED) != 0)
                                        {
                                            CurrentTarget = new Vector2(EnemyLocation.X - 8, EnemyLocation.Z - 8);                                        
                                        }

                                        ExecuteMoveQueue();
                                    }
                                }
                                else
                                {
                                    // Enemy got away.
                                    if ((Actions & FLAG_Actions.MOVING_TO_ATTACK) == FLAG_Actions.MOVING_TO_ATTACK) Actions ^= FLAG_Actions.MOVING_TO_ATTACK;
                                    Actions ^= FLAG_Actions.ALERTED;
                                    SimpleMoveQueue.Clear();
                                    enemy = Vector3.Zero;
                                }
                            }                                                                                                       
                        }
                        else
                        {
                            if ((Actions & FLAG_Actions.MOVING_TO_ATTACK) == FLAG_Actions.MOVING_TO_ATTACK) Actions ^= FLAG_Actions.MOVING_TO_ATTACK;
                            if (IsPointWithinRadius(EnemyLocation, SenseRadius, 4))
                            {
                                // If not alerted but within sense radius
                                Actions ^= FLAG_Actions.ALERTED;
                            }
                            else
                            {
                                if (IsPointWithinRadius(EnemyLocation, SightRadius, 4))
                                {
                                    if (IsPointInFOV(EnemyLocation))
                                    {
                                        if (LOS)
                                        {
                                            //If in sight
                                            Actions ^= FLAG_Actions.ALERTED;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }            
        }

        #region Update Methods

        /// <summary>
        /// Sets things up for the new order.
        /// </summary>
        private void InitOrder()
        {
            if (CurrentOrder.Type == ENUM_OrderType.MOVE_TO)
            {
                //We rely on this being populated by the level before it's needed by the actor.
                SimpleMoveQueue = CurrentOrder.Vector2Queue;                
            }
        }

        /// <summary>
        /// Begins or continues to follow an order.
        /// </summary>
        private void ExecuteCurrentOrder()
        {
            if (CurrentOrder.Type == ENUM_OrderType.MOVE_TO)
            {
                if ((Actions & FLAG_Actions.MOVING_TO_NODE) != FLAG_Actions.MOVING_TO_NODE) Actions ^= FLAG_Actions.MOVING_TO_NODE;
                try
                {                    
                    if (CurrentTarget != Vector2.Zero)
                    {
                        if (SimpleMoveQueue.Count > 0)
                        {
                            Vector3 ctv = new Vector3(SimpleMoveQueue.Peek().X + 8, CurrentLocation.Y, SimpleMoveQueue.Peek().Y + 8);

                            if (Vector3.Distance(CurrentLocation, ctv) > 1)
                                TurnTowardsVetor3(ctv);
                        }
                        else
                        {
                            TurnTowardsVetor3(new Vector3(CurrentTarget.X + 8, CurrentLocation.Y, CurrentTarget.Y + 8));
                        }

                        bool xaligned = (CurrentLocation.X <= (CurrentTarget.X + 8) + 2 &&
                                        CurrentLocation.X >= (CurrentTarget.X + 8) - 2);

                        bool yaligned = (CurrentLocation.Z >= (CurrentTarget.Y + 8) - 2 &&
                                        CurrentLocation.Z <= (CurrentTarget.Y + 8) + 2);

                        if (Vector3.Distance(CurrentLocation, new Vector3(CurrentTarget.X+8, CurrentLocation.Y, CurrentTarget.Y+8)) < 6)
                        {
                            CurrentTarget = Vector2.Zero;
                        }
                        else
                        {
                            // BEGIN ACTUAL MOVEMENT

                            //Get direction vector
                            Vector3 addvec = new Vector3(CurrentTarget.X + 8, CurrentLocation.Y, CurrentTarget.Y + 8);
                            addvec -= CurrentLocation;
                            addvec.Normalize();
                            addvec *= movespeed;

                            //Check sanity of direction vector.

                            Vector3 xcomponent = new Vector3(addvec.X, 0.0f, 0.0f);
                            Vector3 zcomponent = new Vector3(0.0f, 0.0f, addvec.Z);

                            Vector3 checkvec = CurrentLocation + xcomponent * 1 / movespeed;

                            int x = 0, y = 0;

                            // X movement is not allowed
                            if (GOOS.JFX.Core.Collision.CollideVector3WithGridSquare(ref checkvec, ref CollisionGrid, GridWidth, GridHeight, 16, 16, ref x, ref y))
                                addvec.X = 0.0f;

                            checkvec = CurrentLocation + zcomponent * 1 / movespeed;

                            // Z movement is not allowed
                            if (GOOS.JFX.Core.Collision.CollideVector3WithGridSquare(ref checkvec, ref CollisionGrid, GridWidth, GridHeight, 16, 16, ref x, ref y))
                                addvec.Z = 0.0f;

                            //Deal with jumping here
                            addvec.Y = 0;

                            // If vector is now slower than movement speed, extend it!  (stops corners from slowing monsters down!!!)
                            if (addvec.Length() < movespeed)
                            {
                                addvec.Normalize();
                                addvec *= movespeed;
                            }

                            CurrentLocation += addvec;

                            // END ACTUAL MOVEMENT
                            //float dx, dy;

                            //if (!xaligned)
                            //{
                            //    dx = CurrentTarget.X + 8 - CurrentLocation.X;
                            //    if (dx > 0)
                            //        CurrentLocation.X += movespeed;
                            //    if (dx < 0)
                            //        CurrentLocation.X -= movespeed;
                            //}

                            //if (!yaligned)
                            //{
                            //    dy = CurrentTarget.Y + 8 - CurrentLocation.Z;
                            //    if (dy > 0)
                            //        CurrentLocation.Z += movespeed;
                            //    if (dy < 0)
                            //        CurrentLocation.Z -= movespeed;
                            //}
                        }

                        UpdateWorldMatrix();
                    }
                    else
                    {
                        if (SimpleMoveQueue.Count > 0)
                            CurrentTarget = SimpleMoveQueue.Dequeue();
                        else
                        {
                            CurrentOrder = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        
        /// <summary>
        /// Use for attack movement
        /// </summary>
        private void ExecuteMoveQueue()
        {          
            try
            {
                if (CurrentTarget != Vector2.Zero)
                {                                       
                    if (Vector3.Distance(CurrentLocation, new Vector3(CurrentTarget.X + 8, CurrentLocation.Y, CurrentTarget.Y + 8)) < 6)
                    {
                        CurrentTarget = Vector2.Zero;  
                        // If we haven't moved, the move queue is empty,
                        // the distance to the enemy is less than 3/4 of a square but we're not in combat - move towards enemy

                        if(SimpleMoveQueue.Count ==0 && (Actions & FLAG_Actions.MOVING_TO_ATTACK) == FLAG_Actions.MOVING_TO_ATTACK
                            && (Actions & FLAG_Actions.FIGHTING) != FLAG_Actions.FIGHTING)
                        {
                            if (Vector3.Distance(EnemyLocation, CurrentLocation) < 12)
                            {
                                Vector2 targ = new Vector2(EnemyLocation.X, EnemyLocation.Z);
                                CurrentTarget = targ;
								SimpleMoveQueue.Clear();
                            }                           
                        }                        
                    }
                    else
                    {
                        // BEGIN ACTUAL MOVEMENT

                        //Get direction vector
                        Vector3 addvec = new Vector3(CurrentTarget.X + 8, CurrentLocation.Y, CurrentTarget.Y + 8);
                        addvec -= CurrentLocation;
                        addvec.Normalize();
                        addvec *= movespeed;

                        //Check sanity of direction vector.

                        Vector3 xcomponent = new Vector3(addvec.X, 0.0f, 0.0f);
                        Vector3 zcomponent = new Vector3(0.0f, 0.0f, addvec.Z);

                        Vector3 checkvec = CurrentLocation + xcomponent * 1 / movespeed;

                        int x = 0, y = 0;

                        // X movement is not allowed
                        if (GOOS.JFX.Core.Collision.CollideVector3WithGridSquare(ref checkvec, ref CollisionGrid, GridWidth, GridHeight, 16, 16, ref x, ref y))
                            addvec.X = 0.0f;

                        checkvec = CurrentLocation + zcomponent * 1 / movespeed;

                        // Z movement is not allowed
                        if (GOOS.JFX.Core.Collision.CollideVector3WithGridSquare(ref checkvec, ref CollisionGrid, GridWidth, GridHeight, 16, 16, ref x, ref y))
                            addvec.Z = 0.0f;


                        //Deal with jumping here
                        addvec.Y = 0;

						Vector3 safevector = addvec;

                        // If vector is now slower than movement speed, extend it!  (stops corners from slowing monsters down!!!)
                        if (addvec.Length() < movespeed)
                        {
                            addvec.Normalize();
                            addvec *= movespeed;

							checkvec = CurrentLocation + addvec;
							if (GOOS.JFX.Core.Collision.CollideVector3WithGridSquare(ref checkvec, ref CollisionGrid, GridWidth, GridHeight, 16, 16, ref x, ref y))
								addvec = safevector;
                        }

                        Vector3 lastlocation = CurrentLocation;

                        CurrentLocation += addvec;

                        if (float.IsNaN(CurrentLocation.X))
                        {
                            //throw new Exception("NAN! " + lastlocation.ToString() + " " + addvec.ToString() );
							this.CurrentTarget = Vector2.Zero;
							CurrentLocation = lastlocation;							
                        }

						TurnTowardsVetor3(EnemyLocation);                       
                    }

                    UpdateWorldMatrix();
                }
                else
                {
                    if (SimpleMoveQueue != null && SimpleMoveQueue.Count > 0)
                        CurrentTarget = SimpleMoveQueue.Dequeue();                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Patrol the actor through their patrol path.
        /// 
        /// TODO - ability to find a path back to the patrol path if patrol is interrupted.
        /// 
        /// </summary>
        private void Patrol()
        {
            try
            {
                CurrentTarget = SimplePatrolList[CurrentPatrolIndex];

                if (SimplePatrolList.Count > 0)
                {
                    Vector3 ctv = new Vector3(SimplePatrolList[(CurrentPatrolIndex + 1) % SimplePatrolList.Count].X + 8, CurrentLocation.Y, 
                                                SimplePatrolList[(CurrentPatrolIndex + 1) % SimplePatrolList.Count].Y + 8);

                    if (Vector3.Distance(CurrentLocation, ctv) > 1)
                        TurnTowardsVetor3(ctv);
                }
                else
                {
                    TurnTowardsVetor3(new Vector3(CurrentTarget.X + 8, CurrentLocation.Y, CurrentTarget.Y + 8));
                }

                bool xaligned = (CurrentLocation.X <= (CurrentTarget.X + 8) + 2 &&
                                CurrentLocation.X >= (CurrentTarget.X + 8) - 2);

                bool yaligned = (CurrentLocation.Z >= (CurrentTarget.Y + 8) - 2 &&
                                CurrentLocation.Z <= (CurrentTarget.Y + 8) + 2);

                if (Vector3.Distance(CurrentLocation, new Vector3(CurrentTarget.X + 8, CurrentLocation.Y, CurrentTarget.Y + 8)) < 6)
                {
                    CurrentPatrolIndex = (CurrentPatrolIndex + 1) % SimplePatrolList.Count;
                }
                else
                {
                    // BEGIN ACTUAL MOVEMENT

                    //Get direction vector
                    Vector3 addvec = new Vector3(CurrentTarget.X+8, CurrentLocation.Y, CurrentTarget.Y+8);
                    addvec -= CurrentLocation;
                    addvec.Normalize();
                    addvec *= movespeed;                                        

                    //Check sanity of direction vector.

                    Vector3 xcomponent = new Vector3(addvec.X, 0.0f, 0.0f);
                    Vector3 zcomponent = new Vector3(0.0f, 0.0f, addvec.Z);

                    Vector3 checkvec = CurrentLocation + xcomponent * 1 / movespeed;

                    int x = 0, y = 0;

                    // X movement is not allowed
                    if (GOOS.JFX.Core.Collision.CollideVector3WithGridSquare(ref checkvec, ref CollisionGrid, GridWidth, GridHeight, 16, 16, ref x, ref y))
                        addvec.X = 0.0f;

                    checkvec = CurrentLocation + zcomponent * 1 / movespeed;

                    // Z movement is not allowed
                    if (GOOS.JFX.Core.Collision.CollideVector3WithGridSquare(ref checkvec, ref CollisionGrid, GridWidth, GridHeight, 16, 16, ref x, ref y))
                        addvec.Z = 0.0f;

                    //Deal with jumping here
                    addvec.Y = 0;                    

                    // If vector is now slower than movement speed, extend it!  (stops corners from slowing monsters down!!!)
                    if (addvec.Length() < movespeed)
                    {
                        addvec.Normalize();
                        addvec *= movespeed;
                    }

                    CurrentLocation += addvec;

                    //float dx, dy;

                    //dx = addvec.X;
                    //dy = addvec.Z;

                    ////If obstruction???????????????     
                    //if (xaligned != yaligned && dx == 0 && dy == 0)
                    //{
                    //    if (xaligned && zcomponent.Z < 0)
                    //        dx += movespeed;
                    //    if (xaligned && zcomponent.Z > 0)
                    //        dx -= movespeed;  
                    //}

                    //if (!xaligned)
                    //{                                            
                    //    CurrentLocation.X += dx;                   
                    //}

                    //if (!yaligned)
                    //{                                               
                    //    CurrentLocation.Z += dy;                      
                    //}
                                 

                    // END ACTUAL MOVEMENT
                }

                UpdateWorldMatrix();                           
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Causes the actor to wander aimlessly, each update presents a percentage chance to change wander vector by a random angle       
        /// Actor will not choose intelligent paths in this mode and will most likely hug the walls and only rarely leave a semi-enclosed space
        /// </summary>
        private void WanderAimlessly()
        {                       

            //Get a forward vector.
            if (WanderVector == Vector3.Zero)
            {
                WanderTimer = 0;
                //initialise and randomise vector
                WanderVector = Vector3.Forward;
                Matrix m = Matrix.Identity;
                WanderFacing += rand.Next(1, 359);
                m = m * Matrix.CreateRotationY(MathHelper.ToRadians(WanderFacing));
                WanderVector = Vector3.Transform(WanderVector, m);

                return;
            }
            else
            {
                WanderTimer++;

                if (WanderTimer > 60 * 10)
                {
                    if (rand.Next(1, 100) > 98)
                    {
                        //randomise vector
                        Matrix m = Matrix.Identity;
                        WanderFacing = rand.Next(1, 359);
                        m = m * Matrix.CreateRotationY(MathHelper.ToRadians(WanderFacing));
                        WanderVector = Vector3.Transform(WanderVector, m);
                        WanderTimer = 0;
                        return;
                    }
                }
            }            

            //Move towards vector sensitive to walls.

            Vector3 addvec = (movespeed * WanderVector);
            addvec.Y = 0;           
                                                
            Vector3 xcomponent = new Vector3(addvec.X, 0.0f, 0.0f);
            Vector3 zcomponent = new Vector3(0.0f, 0.0f, addvec.Z);

            Vector3 checkvec = CurrentLocation + xcomponent * 1 / movespeed;

            int x=0, y=0;

            // X movement was responsible
            if (GOOS.JFX.Core.Collision.CollideVector3WithGridSquare(ref checkvec, ref CollisionGrid, GridWidth, GridHeight, 16, 16, ref x, ref y))
                addvec.X = 0.0f;

            checkvec = CurrentLocation + zcomponent * 1 / movespeed;

            // Z movement was responsible
            if (GOOS.JFX.Core.Collision.CollideVector3WithGridSquare(ref checkvec, ref CollisionGrid, GridWidth, GridHeight, 16, 16, ref x, ref y))
                addvec.Z = 0.0f;

            TurnTowardsVetor3(CurrentLocation + addvec / 2);
            CurrentLocation += addvec/2;

            UpdateWorldMatrix();

            if (addvec.X == 0 || addvec.Z == 0)
            {
                Matrix m = Matrix.Identity;
                WanderFacing = rand.Next(1, 359);
                m = m * Matrix.CreateRotationY(MathHelper.ToRadians(WanderFacing));
                WanderVector = Vector3.Transform(WanderVector, m);
                WanderTimer = 0;
            }
        }

        #endregion

        #region Helpers        

        /// <summary>
        /// Check if an area around the vector v intersects with the given radius 
        /// </summary>   
        public bool IsPointWithinRadius(Vector3 v,float radius, float vradius)
        {
            BoundingSphere s1 = new BoundingSphere(CurrentLocation, radius);
            BoundingSphere s2 = new BoundingSphere(v, vradius);
            return (s1.Intersects(s2));
        }

        /// <summary>
        /// Gradually turn the actor to face a vector
        /// </summary>
        /// <param name="location">The vector to face</param>       
        public void TurnTowardsVetor3(Vector3 location)
        {
            float y = CurrentLocation.Y;
            CurrentLocation.Y = 0;

            float locationz = location.Z;
            location.Y = 0.0f;

            //turn actor to face a vector.
            //make the actor's position the origin            
            location -= CurrentLocation;

            //Get the angle between the translated vector and the x axis (x axis unit vector is 1.0,0.0,0.0
            float costheta;
            location.Normalize();

            if (locationz <= CurrentLocation.Z)
                costheta = Vector3.Dot(location, Vector3.Right);
            else
                costheta = Vector3.Dot(location, Vector3.Left);

            double theta = Math.Acos(costheta);

            if (locationz > CurrentLocation.Z)
                theta += MathHelper.ToRadians(180);

            //TODO - STOP USING DEGREES HERE IT'S CONFUSING THINGS - the diff between 10 and 360 is 10 not 350!

            float diff,facing;

            facing = GetFacingDegrees();
            diff = CurrentFacing - (float)theta;

            //difference = diff;

            // AAARGH! It works now, I knew why at the time, and I never want to know why again.
            if (MathHelper.ToDegrees(diff) > turnspeed*2 || MathHelper.ToDegrees(diff) < -turnspeed*2)
            {
                if (diff < 0 && MathHelper.ToDegrees(diff) > -180)
                    facing += turnspeed;
                if (diff > 0 && MathHelper.ToDegrees(diff) <180)
                    facing -= turnspeed;

                if (diff < 0 && MathHelper.ToDegrees(diff) <= -180)
                    facing -= turnspeed;
                if (diff > 0 && MathHelper.ToDegrees(diff) >= 180)
                    facing += turnspeed;

                if (facing <= 0)
                    facing = 360 + facing;
                else
                    facing = facing % 360;

                this.CurrentFacing = MathHelper.ToRadians(facing);
            }
            else
                this.CurrentFacing = (float)theta;                                     
               

            CurrentLocation.Y = y;

            UpdateWorldMatrix();
        }

        /// <summary>
        /// Instantly turn the actor to face a vector
        /// </summary>
        /// <param name="location">The vector to face</param>       
        public void SetFacingVetor3(Vector3 location)
        {            
            float y = CurrentLocation.Y;
            float locationz = location.Z;
            location.Y = 0.0f;

            //turn actor to face a vector.
            //make the actor's position the origin            
            location -= CurrentLocation;

            //Get the angle between the translated vector and the x axis (x axis unit vector is 1.0,0.0,0.0
            float costheta;
            location.Normalize();

            if (locationz <= CurrentLocation.Z)
                costheta = Vector3.Dot(location, Vector3.Right);
            else
                costheta = Vector3.Dot(location, Vector3.Left);

            double theta = Math.Acos(costheta);

            if (locationz > CurrentLocation.Z)
                theta += MathHelper.ToRadians(180);

            this.CurrentFacing = (float)theta;

            CurrentLocation.Y = y;

            UpdateWorldMatrix();
        }

        /// <summary>
        /// Check to see whether a given point is in the actor's FOV (field of vision)
        /// </summary>
        /// <param name="location">The vector to check</param>
        /// <returns>True if the point is in FOV</returns>
        public bool IsPointInFOV(Vector3 location)
        {
            //turn actor to face a vector.
            //make the actor's position the origin   
            float locationz = location.Z;
            location -= CurrentLocation;

            //Get the angle between the translated vector and the x axis (x axis unit vector is 1.0,0.0,0.0
            float costheta;
            location.Normalize();

            if (locationz <= CurrentLocation.Z)
                costheta = Vector3.Dot(location, Vector3.Right);
            else
                costheta = Vector3.Dot(location, Vector3.Left);

            double theta = Math.Acos(costheta);

            if (locationz > CurrentLocation.Z)
                theta += MathHelper.ToRadians(180);

            float diff = CurrentFacing - (float)theta;

            // diff is the difference between the current facing angle and the angle at which the location is found.                   
                            
            if (MathHelper.ToDegrees(diff) > 180)            
                diff = MathHelper.ToRadians(360) - diff;     

            if (MathHelper.ToDegrees(diff) < -180)            
                diff = MathHelper.ToRadians(360) + diff;                   

            return (MathHelper.ToDegrees(diff) < FOV && MathHelper.ToDegrees(diff) > -FOV);            
        }

        public float GetFacingDegrees()
        {
            return Math.Abs(MathHelper.ToDegrees(this.CurrentFacing)) % 360;
        }

        public void SetFacingDegrees(float d)
        {
            CurrentFacing = MathHelper.ToRadians(d);
            UpdateWorldMatrix();
        }

        public void SetLoction(Vector3 v)
        {
            CurrentLocation = v;
            UpdateWorldMatrix();
        }

        public void SetLocation(float x, float y, float z)
        {
            CurrentLocation = new Vector3(x, y, z);
            UpdateWorldMatrix();
        }

        public void UpdateWorldMatrix()
        {
            WorldMatrix = Matrix.Identity;
			WorldMatrix *= Matrix.CreateScale(ScalingFactor);
            WorldMatrix *= Matrix.CreateRotationY(CurrentFacing + FacingOffset);
            WorldMatrix *= Matrix.CreateTranslation(CurrentLocation);
        }

        #endregion		
					
	}
}
