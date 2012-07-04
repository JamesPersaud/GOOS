#region File Description
//-----------------------------------------------------------------------------
// SmokePlumeParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace GOOS.JFX.Game.Components
{
    /// <summary>
    /// Custom particle system for creating a giant plume of long lasting smoke.
    /// </summary>
    public class SmokePlumeParticleSystem : ParticleSystem
    {
		public SmokePlumeParticleSystem(Microsoft.Xna.Framework.Game game, ContentManager content)
            : base(game, content)
        { }

		public SmokePlumeParticleSystem(Microsoft.Xna.Framework.Game game, ContentManager content, int density)
			: base(game, content)
		{ mDensity = density; }

		private int mDensity;
		public int Density
		{
			get { return mDensity; }
			set { mDensity = value; }
		}

        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "smoke";

			settings.MaxParticles = (int)(600 * (float)(mDensity / 100));
			if (settings.MaxParticles < 1)
				settings.MaxParticles = 1;

            settings.Duration = TimeSpan.FromSeconds(10);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 1.0f;

            settings.MinVerticalVelocity = 10;
            settings.MaxVerticalVelocity = 20;

            // Create a wind effect by tilting the gravity vector sideways.
            settings.Gravity = new Vector3(-0, -5, 0);

            settings.EndVelocity = 0.75f;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 100;
            settings.MaxStartSize = 150;

            settings.MinEndSize = 200;
            settings.MaxEndSize = 400;						
        }
    }
}
