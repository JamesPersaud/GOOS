using System;
using System.Collections.Generic;

namespace GOOS.JFX.Entity
{
    /// <summary>
    /// These are action flags, they determine what an actor is currently doing 
    /// do not use these to determine how an actor will react under certain conditions, for that use behaviour flags.
    /// 
    /// NOTE: these enums are 32 bit ints - if we really want more than 32 available flags we should add enums i.e. FLAG_Behaviours_Extended 
    /// not change the data type.
    /// 
    /// </summary>
    [Flags]
    public enum FLAG_Actions : uint
    {
        NONE = 0,        
        MOVING_TO_NODE = 1      << 0,		// The actor is moving to a node, either by order or by instinct.
        MOVING_TO_ATTACK = 1    << 1,		// The actor is moving to attack a target either by order or by instinct.
        FIGHTING =1             << 2,       // The actor is fighting its target.
        ACTING_UNDER_ORDERS = 1 << 3,		// The actor is doing whatever he is doing because he has been ordered to.
        ACTING_BY_INSTINCT = 1  << 4,		// The actor is doing whatever he is doing purely by instinct.
        ALERTED =1              << 5,		// The actor is alerted to the presence of its target.
        WANDERING_AIMLESSLY =1  << 6		// The actor is wandering aimlessly without any apparent intellicence or sense.
    };
}
