using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GOOS.JFX.Core;

namespace GOOS.JFX.Entity
{
	/// <summary>
	/// Provides an interface by which entities can be accessed for rendering
	/// </summary>
	public interface IRenderable
	{
		Matrix RenderWorldMatrix { get; set; }						//World matrix to use when rendering this entity
		Model DefaultModel { get; set; }							//The default model for the renderable entity.
		Dictionary<string, Material> DefaultMaterials { get; set; } //Materials keyed by mesh name	
		bool Render();												//The main render method - returns success.
	}
}
