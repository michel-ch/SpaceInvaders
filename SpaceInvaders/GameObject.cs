using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SpaceInvaders
{
    /// <summary>
    /// This is the generic abstact base class for any entity in the game
    /// </summary>
    abstract class GameObject
    {
        #region Fields
        /// <summary>
        /// Side of the game object
        /// </summary>
        Side ObjectSide { get; set; }
        public GameObject(Side side)
        {
            ObjectSide = side;
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Simple constructor
        /// </summary>
        public GameObject()
        {
        }

        #endregion

        #region Methods
        /// <summary>
        /// Update the state of a game objet
        /// </summary>
        /// <param name="gameInstance">instance of the current game</param>
        /// <param name="deltaT">time ellapsed in seconds since last call to Update</param>
        public abstract void Update(Game gameInstance, double deltaT);

        /// <summary>
        /// Render the game object
        /// </summary>
        /// <param name="gameInstance">instance of the current game</param>
        /// <param name="graphics">graphic object where to perform rendering</param>
        public abstract void Draw(Game gameInstance, Graphics graphics);

        /// <summary>
        /// Determines if object is alive. If false, the object will be removed automatically.
        /// </summary>
        /// <returns>Am I alive ?</returns>
        public abstract bool IsAlive();

        /// <summary>
        /// Determines if the object and a missile collide, if so it removes lives accordingly.
        /// </summary>
        /// <param name="m">instance of the missile to verify the collision</param>
        public abstract void Collision(Missile m);

        #endregion

    }
    /// <summary>
    /// Enum to define the side of an element, this allows them to interact accordingly between them.
    /// </summary>
    enum Side
    {
        Ally,
        Enemy,
        Neutral
    }

}
