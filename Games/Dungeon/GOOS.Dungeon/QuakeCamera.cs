using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace GOOS.Dungeon
{
#if XBOX
    //When compiled for the XBox, Mouse and MouseState are unknown.
    //Define dummy bodies for them, so a MouseState object
    //can be passed to the Update method of the camera.
    public class MouseState
    {
    }
    public static class Mouse
    {
        public static MouseState GetState()
        {
            return new MouseState();
        }
    }
#endif

    class QuakeCamera
    {

		public Ray PickingRay;

        Matrix viewMatrix;
        Matrix projectionMatrix;
        Viewport viewPort;

        public bool[] CollisionGrid;
        public int GridWidth;
        public int GridHeight;
        public int SquareWidth;
        public int SquareHeight;
        public int LastGridSquareX;
        public int LastGridSquareY;

        public float currentjump;

        public float leftrightRot;
        float updownRot;
        const float rotationSpeed = 0.005f;
        Vector3 cameraPosition;
        MouseState originalMouseState;

        public Vector3 LastTarget;        

        public QuakeCamera(Viewport viewPort) : this(viewPort, new Vector3(0, 1, 15), 0, 0)
        {
            //calls the constructor below with default startingPos and rotation values
        }

        public QuakeCamera(Viewport viewPort, Vector3 startingPos, float lrRot, float udRot)
        {
            this.LastTarget = new Vector3();
            this.leftrightRot = lrRot;
            this.updownRot = udRot;
            this.cameraPosition = startingPos;
            this.viewPort = viewPort;

            float viewAngle = MathHelper.PiOver4;
            float nearPlane = 0.5f;
            float farPlane = 1000.0f;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(viewAngle, viewPort.AspectRatio, nearPlane, farPlane);
            
            UpdateViewMatrix();
#if XBOX
#else
            Mouse.SetPosition(viewPort.Width/2, viewPort.Height/2);
            originalMouseState = Mouse.GetState();
#endif
        }        

        public void Update(MouseState currentMouseState, KeyboardState keyState, GamePadState gamePadState)
        {
#if XBOX            
            leftrightRot -= rotationSpeed * gamePadState.ThumbSticks.Left.X * 5.0f;
            updownRot += rotationSpeed * gamePadState.ThumbSticks.Left.Y * 5.0f;

            UpdateViewMatrix();

            float moveUp = gamePadState.Triggers.Right - gamePadState.Triggers.Left;
            AddToCameraPosition(new Vector3(gamePadState.ThumbSticks.Right.X, moveUp, -gamePadState.ThumbSticks.Right.Y));
#else
            if (currentMouseState != originalMouseState)
            {                
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                leftrightRot -= rotationSpeed * xDifference;
                updownRot -= rotationSpeed * yDifference;
                Mouse.SetPosition(viewPort.Width / 2, viewPort.Height / 2);
                UpdateViewMatrix();                
            }
            
            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))      //Forward
                AddToCameraPositionInc(new Vector3(0, 0, -0.921f));
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))    //Backward
                AddToCameraPositionInc(new Vector3(0, 0, 0.921f));
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))   //Right
                AddToCameraPositionInc(new Vector3(0.921f, 0, 0));
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))    //Left
                AddToCameraPositionInc(new Vector3(-0.921f, 0, 0));
            if (keyState.IsKeyDown(Keys.Q))                                     //Up
                AddToCameraPositionInc(new Vector3(0, 0.921f, 0));
            if (keyState.IsKeyDown(Keys.Z))                                     //Down
                AddToCameraPositionInc(new Vector3(0, -0.921f, 0));            
#endif            

            
        }

        private void AddToCameraPositionInc(Vector3 vectorToAdd)
        {
          //for (int i = 0; i < 5; i++)
          //{
            AddToCameraPosition(vectorToAdd);
          //}
        }

        private void AddToCameraPosition(Vector3 vectorToAdd)
        {           
            float moveSpeed = 0.5f;
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);            
            Vector3 addvec = (moveSpeed * rotatedVector);
            addvec.Y = 0;

            Vector3 checkvec = cameraPosition + addvec * 1/moveSpeed;

            //Is vectortoadd going to cause collision with a closed grid?            
            //bool blocked = Grimwood.Bane.Core.Collision.CollideVector3WithGridSquare(ref checkvec, ref CollisionGrid, GridWidth, GridHeight, SquareWidth, SquareHeight, ref LastGridSquareX, ref LastGridSquareY);

            //if (!blocked)
            //{
            //    cameraPosition += addvec;
            //}
            //else
            //{                                          
                Vector3 xcomponent = new Vector3(addvec.X, 0.0f, 0.0f);
                Vector3 zcomponent = new Vector3(0.0f, 0.0f, addvec.Z);

                checkvec = cameraPosition + xcomponent * 1 / moveSpeed;

                // X movement was responsible
                if(GOOS.JFX.Core.Collision.CollideVector3WithGridSquare(ref checkvec, ref CollisionGrid, GridWidth, GridHeight, SquareWidth, SquareHeight, ref LastGridSquareX, ref LastGridSquareY))                                
                    addvec.X = 0.0f;

                checkvec = cameraPosition + zcomponent * 1 / moveSpeed;

                // Z movement was responsible
                if(GOOS.JFX.Core.Collision.CollideVector3WithGridSquare(ref checkvec, ref CollisionGrid, GridWidth, GridHeight, SquareWidth, SquareHeight, ref LastGridSquareX, ref LastGridSquareY))                         
                    addvec.Z = 0.0f;
                               

                cameraPosition += addvec;
            //}
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);

            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = cameraPosition + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);
            Vector3 cameraFinalUpVector = cameraPosition + cameraRotatedUpVector;

            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraFinalTarget, cameraRotatedUpVector);

            LastTarget = cameraFinalTarget;

			////Project picking ray
			//Vector3 RayDirectionVector = LastTarget;
			//RayDirectionVector -= Position;
			//RayDirectionVector.Normalize();

			//PickingRay = new Ray(Position, RayDirectionVector);	
        }

        public float UpDownRot
        {
            get { return updownRot; }
            set { updownRot = value; }
        }

        public float LeftRightRot
        {
            get { return leftrightRot; }
            set { leftrightRot = value; }
        }

        public Matrix ProjectionMatrix
        {
            get { return projectionMatrix; }
        }

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }
        public Vector3 Position
        {
            get { return cameraPosition; }
            set { 
                cameraPosition = value;
                UpdateViewMatrix();
            }
        }
        public Vector3 TargetPosition
        {
            get 
            {
                Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
                Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
                Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
                Vector3 cameraFinalTarget = cameraPosition + cameraRotatedTarget;
                return cameraFinalTarget;
            }
        }
        public Vector3 Forward
        {
            get
            {
                Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
                Vector3 cameraForward = new Vector3(0, 0, -1);
                Vector3 cameraRotatedForward = Vector3.Transform(cameraForward, cameraRotation);
                return cameraRotatedForward;
            }
        }
        public Vector3 SideVector
        {
            get
            {
                Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
                Vector3 cameraOriginalSide = new Vector3(1, 0, 0);
                Vector3 cameraRotatedSide = Vector3.Transform(cameraOriginalSide, cameraRotation);
                return cameraRotatedSide;
            }
        }
        public Vector3 UpVector
        {
            get
            {
                Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
                Vector3 cameraOriginalUp = new Vector3(0, 1, 0);
                Vector3 cameraRotatedUp = Vector3.Transform(cameraOriginalUp, cameraRotation);
                return cameraRotatedUp;
            }
        }

        public void Setjump(float j)
        {
            this.cameraPosition.Y = j;
            UpdateViewMatrix();
        }

        public float GetFacingdegrees()
        {            
            return Math.Abs(MathHelper.ToDegrees(this.leftrightRot)) % 360;
        }

        public void SetFacingDegrees(float degrees)
        {
            this.leftrightRot = MathHelper.ToRadians(degrees);
        }
    }
}
