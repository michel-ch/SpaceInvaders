using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{    internal class Missile : SimpleObject
    {
        #region Fields
        /// <summary>
        /// Speed of the missil
        /// </summary>
        private double speedPixelPerSecond = 800;
        #endregion

        #region Constructor
        /// <summary>
        /// Simple constructor
        /// </summary>
        /// <param name="position">start vector position</param>
        /// <param name="lives">start number of lives</param>
        /// <param name="image">image of the missil</param>
        /// <param name="side">side of the object who shot</param>
        public Missile(Vecteur2D position, int lives, Bitmap img, Side side) : base(side)
        {
            Position = position;
            Lives = lives;
            Image = img;
            Side = side;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Override the base function to update the object display and state
        /// </summary>
        /// <param name="gameInstance">game instance</param>
        /// <param name="deltaT">time delta</param>
        public override void Update(Game gameInstance, double deltaT)
        {
            // If the missile is out of the window, set Lives of it to 0 ( in order to delete it after )
            if (Position.Y < 0 || Position.Y > gameInstance.gameSize.Height) { Lives = 0; }
            // If the object is an Enemy, move it downward
            if (Side == Side.Enemy) { Position.Y += deltaT * speedPixelPerSecond; }
            // If the object is an Ally, move it upward
            if (Side == Side.Ally) { Position.Y -= deltaT * speedPixelPerSecond; }
            // Check for collisions with the current game object through all game object in GameInstance
            foreach (GameObject gameobject in gameInstance.gameObjects) { gameobject.Collision(this); }
        }
        /// <summary>
        /// Override base function to reduce the amount of lives according to the object that got into the collision
        /// </summary>
        /// <param name="m">Missil that hits the object</param>
        /// <param name="m">Number of pixels that went into collision</param>
        protected override void OnCollision(Missile m, int numberOfPixelsInCollision)
        {
            if(this != m) { Lives = 0; m.Lives = 0; }
        }
        #endregion
    }
}
