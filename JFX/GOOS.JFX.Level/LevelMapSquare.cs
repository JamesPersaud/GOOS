using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOOS.JFX.Level
{
    /// <summary>
    /// Represents one square of MapData
    /// </summary>
    [Serializable]
    public struct LevelMapSquare
    {
        public MapSquareType type;

        public LevelMapSquare(MapSquareType t)
        {
            type = t;
        }
    }
}
