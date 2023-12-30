using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


namespace SpaceInvaders
{
    abstract class SimpleObject : GameObject
    {
        #region Fields
        /// <summary>
        /// Position
        /// </summary>
        public Vecteur2D Position
        { get; set; }

        /// <summary>
        /// Lives
        /// </summary>
        public int Lives
        { get; set; }

        /// <summary>
        /// Image
        /// </summary>
        public Bitmap Image { get; set; }

        /// <summary>
        /// Object Side in the game
        /// </summary>
        public Side Side { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Simple constructor
        /// </summary>
        /// <param name="side">side of the object</param>
        public SimpleObject(Side side):base(side)
        {}

        /// <summary>
        /// Simple constructor
        /// </summary>
        public SimpleObject()
        {}

        #endregion

        #region Methods
        /// <summary>
        /// Implementation of the base function.
        /// </summary>
        /// <returns>Am I alive ?</returns>
        public override void Update(Game gameInstance, double deltaT)
        {}

        /// <summary>
        /// Implementation of the base function.
        /// </summary>
        /// <returns>Am I alive ?</returns>
        public override bool IsAlive() { return Lives>0; }

        /// <summary>
        /// Implementation of the base function.
        /// </summary>
        public override void Draw(Game gameInstance, Graphics graphics)
        {
            float positionX = (float)Position.X;
            float positionY = (float)Position.Y;
            graphics.DrawImage(Image, positionX, positionY, Image.Width, Image.Height);
        }

        /// <summary>
        /// Helper function to determine if a game object was intersected by a missil
        /// taking into account the whole game object image
        /// </summary>
        /// <returns>Does the whole game object image collides with the missil?</returns>
        public bool IsWholeImageInCollision(Missile m)
        {
            return !(m.Position.X + Image.Width < Position.X || m.Position.X > Position.X + Image.Width || m.Position.Y + Image.Height < Position.Y || m.Position.Y > m.Position.Y + Image.Height);
        }

        /// <summary>
        /// Helper function to determine if a game object was intersected by a missil
        /// taking into account the comparison pixel by pixel. If a black pixel gets hit
        /// by a missil, it turns to white.
        /// </summary>
        /// <returns>Number of pixels that got hit by a missil</returns>
        public int PixelsInCollision(Missile m)
        {
            // Definition of the variable to return 
            int pixelsInCollision = 0;
            /// Loop through the pixels of the y-axis of the missil
            for (int y = (int)m.Position.Y; y < (int)m.Position.Y + (int)m.Image.Height; y++)
            {
                /// Loop through the pixels of the x-axis of the missil
                for (int x = (int)m.Position.X; x < (int)m.Position.X + (int)m.Image.Width; x++)
                {
                    /// Definition of the position the loop is in and the Simple object position to determine an interval of collision
                    int imageX = x - (int)Position.X;
                    int imageY = y - (int)Position.Y;
                    // Verification of the reduced interval in SimpleObject
                    if (imageX >= 0 && imageX < (int)Image.Width && imageY >= 0 && imageY < (int)Image.Height &&
                        x >= (int)Position.X && x < (int)Position.X + (int)Image.Width &&
                        y >= (int)Position.Y && y < (int)Position.Y + (int)Image.Height)
                    {
                        /// If test to verify if a pixel in collision have Alpha > 0, and get its position
                        if (Image.GetPixel(imageX, imageY).A > 0 && m.Image.GetPixel(x - (int)m.Position.X, y - (int)m.Position.Y).A > 0)
                        {
                            // Change the pixel display from black to white
                            if(Object.ReferenceEquals(this.GetType(), typeof(Bunker))) { Image.SetPixel(imageX, imageY, Color.FromArgb(0, 255, 255, 255)); }
                            // Adds each pixel transformed into the return variable
                            pixelsInCollision++;
                        }
                    }
                }
            }
            /// Return he number of pixels transformed
            return pixelsInCollision;
        }

        /// <summary>
        /// Implementation of the collision function taking into account the helper functions
        /// </summary>
        public override void Collision(Missile m)
        {
            if(Side != m.Side)
           { if (IsWholeImageInCollision(m)) { int numberOfPixelsInCollision = PixelsInCollision(m); OnCollision(m, numberOfPixelsInCollision); } }
        }

        /// <summary>
        /// Function to reduce the amount of lives according to the object that got into the collision
        /// </summary>
        /// <param name="m">Missil that hits the object</param>
        /// <param name="m">Number of pixels that went into collision</param>
        protected abstract void OnCollision(Missile m, int numberOfPixelsInCollision);

        #endregion
    }
}
