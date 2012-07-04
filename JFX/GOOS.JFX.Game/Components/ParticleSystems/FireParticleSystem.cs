#region File Description
//-----------------------------------------------------------------------------
// FireParticleSystem.cs
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
    /// Custom particle system for creating a flame effect.
    /// </summary>
    public class FireParticleSystem : ParticleSystem
    {
		public FireParticleSystem(Microsoft.Xna.Framework.Game game, ContentManager content, int density)
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
            settings.TextureName = "fire";

			settings.MaxParticles = (int)(1200 * (float)(mDensity / 100));
			if (settings.MaxParticles < 1)
				settings.MaxParticles = 1;

            settings.Duration = TimeSpan.FromSeconds(2);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 10;
            settings.MaxHorizontalVelocity = 50;

            settings.MinVerticalVelocity = -10;
            settings.MaxVerticalVelocity = 10;

            // Set gravity upside down, so the flames will 'fall' upward.
            settings.Gravity = new Vector3(0, 12, 0);

            settings.MinColor = new Color(255, 255, 255, 10);
            settings.MaxColor = new Color(255, 255, 255, 40);

            settings.MinStartSize = 5;
            settings.MaxStartSize = 10;

            settings.MinEndSize = 10;
            settings.MaxEndSize = 40;

            // Use additive blending.
            settings.SourceBlend = Blend.SourceAlpha;
            settings.DestinationBlend = Blend.One;
        }
    }
}
