using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GOOS.JFX.Entity
{
    /// <summary>
    /// Documentation to follow
    /// </summary>
    public class Order
    {
        public ENUM_OrderType Type;
        public Dictionary<string, int> IntParams;
        public Dictionary<string, float> FloatParams;
        public Dictionary<string, Vector2> Vector2Params;
        public Dictionary<string, Vector3> Vector3Params;
        public Dictionary<string, object> ObjectParams;
        public Dictionary<string, bool> BoolParams;
        public Dictionary<string, string> StringParams;

        public Queue<Vector3> Vector3Queue;
        public Queue<Vector2> Vector2Queue;        

        /// <summary>
        /// Default
        /// </summary>
        public Order()
        {

        }
    }
}
