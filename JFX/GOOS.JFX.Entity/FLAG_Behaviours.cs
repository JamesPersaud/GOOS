using System;
using System.Collections.Generic;

namespace GOOS.JFX.Entity
{
    /// <summary>
    /// These are behaviour flags, they determine how an actor responds to certain conditions, they do not 
    /// specify what the actor is currently doing, for that use action flags.
    /// 
    /// NOTE: these enums are 32 bit ints - if we really want more than 32 available flags we should add enums i.e. FLAG_Behaviours_Extended 
    /// not change the data type.
    /// 
    /// </summary>
    [Flags] 
    public enum FLAG_Behaviours 
    {
        NONE = 0,
        MONSTER = 1     << 0,       // Monsters can be engaged in combat and can engage in combat
        AGGRESSIVE = 1  << 1,       // Aggressives will chase the player according to standard player chasing rules and awareness attributes.
        PATROLLER = 1   << 2,       // Patrollers - when under no other orders - will follow a circuitous list of waypoints 
        WANDERER = 1    << 3,       // Wanderers - when under no other orders - will wander around aimlessly.
		EVIL_GENIUS = 1 << 4,		// An evil genius is a monster that always knows where the player is and is therefore always alerted.
    };
}
