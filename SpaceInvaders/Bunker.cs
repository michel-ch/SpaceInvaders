using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SpaceInvaders
{
    class Bunker: SimpleObject
    {
        #region Fields
        /// <summary>
        /// Store the number of Lives of the precedent image
        /// /// In order to not count the Lives if it is not a new Bunker Object
        /// </summary>
        private int precedentUsedImageLives;
        /// <summary>
        /// Store the precedent image
        /// In order to not reload the image if it is not a new Bunker Object
        /// </summary>
        private static Bitmap precedentUsedImage;
        #endregion

        #region Constructor
        /// <summary>
        /// Simple constructor
        /// </summary>
        /// <param name="position">start position</param>
        /// <param name="image">image of the bunker</param>
        public Bunker(Vecteur2D position, Bitmap image) :base(Side.Neutral)
        {
            if(precedentUsedImage != image)
            {
                precedentUsedImage = new Bitmap(image);
                precedentUsedImageLives = 0;
                // Count the number of pixels which are not empty and store it in precedentUsedImageLives ( it will be the number of Lives )
                for (int y = (int)position.Y;y < (int)position.Y+(int)image.Height; y++) {
                    for(int x = (int)position.X;x < (int)position.X+(int)image.Width; x++)
                    { if (image.GetPixel(x - (int)position.X, y - (int)position.Y).A > 0) { precedentUsedImageLives++; } }
                }
            } 
            Position = position;
            Lives = precedentUsedImageLives;
            Image = new Bitmap(precedentUsedImage);
            // Setting the Side to Neutral so it will collide with Side.Ally and Side.Enemy
            Side = Side.Neutral;
        }
        #endregion

        #region Methods
        public override void Update(Game gameInstance, double deltaT)
        {}

        /// <summary>
        /// Implementation of the collision function taking into account the helper functions
        /// </summary>
        /// <param name="m">missil to verify the collision</param>
        /// <param name="numberOfPixelsInCollision">number of pixels in the collision</param>
        protected override void OnCollision(Missile m, int numberOfPixelsInCollision)
        {
            if (m.Lives < numberOfPixelsInCollision) { m.Lives = 0; }
            else { m.Lives -= numberOfPixelsInCollision; }
            Lives -= numberOfPixelsInCollision;
        }
        #endregion
    }
}
