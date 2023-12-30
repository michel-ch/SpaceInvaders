using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace SpaceInvaders
{
    internal class EnemyBlock : GameObject
    {
        #region Fields
        /// <summary>
        /// Hashset of the enemyShips ( type Spaceship )
        /// </summary>
        public HashSet<Spaceship> enemyShips = new HashSet<Spaceship>();
        /// <summary>
        /// Define the max width of the movable space
        /// </summary>
        private int baseWidth;
        public Vecteur2D Position { get; set; }
        /// <summary>
        /// Storing an image with Bitmap
        /// </summary>
        public Bitmap Image { get; set; }
        /// <summary>
        /// Numbers of pixels per horizontal shift
        /// </summary>
        public double speedPixelPerSecond = 200;
        /// <summary>
        /// Number of pixels per forward shift
        /// </summary>
        public double forwardMove = 15;
        /// <summary>
        /// Boolean variable which store if the ship have to go to it's left or not
        /// </summary>
        public bool left = false;
        /// <summary>
        /// Probability of shooting missile per second
        /// </summary>
        private double randomShootProbability = 0.4;
        /// <summary>
        /// Max interation for shooting enemy test
        /// </summary>
        private int maxInteration = 1;
        /// <summary>
        /// Count number of steps forward already done
        /// </summary>
        private int countLeft = 0;
        /// <summary>
        /// Random initialisation
        /// </summary>
        Random random = new Random();
        /// <summary>
        /// Store the ship to explose
        /// </summary>
        public Spaceship shipToExplose = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the enemyBlock that contains the enemy ships
        /// </summary>
        /// <param name="position">Position of the enemyBlock</param>
        /// <param name="size">Size of the enemyBlock</param>
        public EnemyBlock(Vecteur2D position, Size size)
        {
            Position = position;
            baseWidth = size.Width;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draw the ships in enemyShips
        /// </summary>
        /// <param name="gameInstance">Instance of game</param>
        /// <param name="graphics">Graphics to draw in</param>
        public override void Draw(Game gameInstance, Graphics graphics)
        {
            // For each Spaceship in enemyShips, we draw them if there are alive
            // if not we draw an explosion
            foreach (Spaceship s in enemyShips)
            {
                if (s.IsAlive()) { graphics.DrawImage(s.Image, (float)s.Position.X, (float)s.Position.Y, s.Image.Width, s.Image.Height); }
                if (shipToExplose !=null && !shipToExplose.IsAlive()) {
                    Bitmap explosionImage = SpaceInvaders.Properties.Resources.enemyExplosion;
                    float positionX = (float)(shipToExplose.Position.X + ((shipToExplose.Image.Width - explosionImage.Width)/2));
                    float positionY = (float)(shipToExplose.Position.Y + ((shipToExplose.Image.Height - explosionImage.Height) / 2));
                    graphics.DrawImage(explosionImage, positionX, positionY, explosionImage.Width, explosionImage.Height);
                    shipToExplose = null;
                }
                
            }
        }
        /// <summary>
        /// Determines if at least one enemy of the block is still alive.
        /// </summary>
        /// <returns>Am I alive ?</returns>
        public override bool IsAlive()
        {
            bool blockAlive = false;
            foreach (Spaceship s in enemyShips) { if (s.Lives > 0) { blockAlive = true; break; } }
            return blockAlive;
        }
        /// <summary>
        /// Update the state of the enemy block, shoots aleatory
        /// </summary>
        /// <param name="gameInstance">Instance of the current game</param>
        /// <param name="deltaT">Time of program ( deltaT )</param>
        public override void Update(Game gameInstance, double deltaT)
        {
            for(int interation = 0; interation < maxInteration; interation++) {
                // Random shoot for 2 ships included in enemyShips
                for (int i = 0; i < 2; i++) { if(random.NextDouble() <= randomShootProbability * deltaT)
                    {   
                        int selectionRandomEnemyInEnemyship = random.Next(0, enemyShips.Count);
                        for (int position = 0; position < enemyShips.Count; position++)
                        {  if (position == selectionRandomEnemyInEnemyship) { Spaceship ship = enemyShips.ElementAt(position); ship.Shoot(gameInstance); } }
                    }
                }
            }
            // Check if the enemyBloc is at a X of 10 or less than 10, if so go to right (!left)
            if (this.Position.X <= 10)
            {   
                left = false;
                foreach(Spaceship s in enemyShips) { s.Position.Y += forwardMove; }
                forwardMove += 5;
                speedPixelPerSecond += 10;
                // If randomShootProbability does not exceed 0.9, if not we can add 0.1 to it
                randomShootProbability += (randomShootProbability < 0.9) ? 0.1 : 0;
                countLeft++;
                if (countLeft == 4) { maxInteration++; countLeft = 0; }
            }
            // Check if the first ship of enemyShips is at a Y bigger or equal than the size of the gameInstance, if so go to left for all enemyShips and the enemyBlock itself
            if (Position.X+baseWidth >= (gameInstance.gameSize.Width)) { left = true; }
            if (left) { foreach (Spaceship s in enemyShips) { s.Position.X -= deltaT * speedPixelPerSecond; } Position.X -= deltaT * speedPixelPerSecond; }
            else { foreach (Spaceship s in enemyShips) { s.Position.X += deltaT * speedPixelPerSecond; } Position.X += deltaT * speedPixelPerSecond; }
        }
        /// <summary>
        /// Add line of spaceships
        /// number of Spaceship, number of live, image 
        /// </summary>
        /// <param name="nbShips">Number of Spaceship, number of live of each Spaceship, image used for each Spaceship</param>
        /// <param name="nbLives">Number of live of each Spaceship</param>
        /// <param name="shipImage">Image used for each Spaceship</param>
        public void AddLine(int nbShips, int nbLives, Bitmap shipImage)
        {
            int totalWidth = shipImage.Width * nbShips;
            if (totalWidth > baseWidth) { baseWidth = totalWidth + (nbShips * 10); }
            int widthPadding = (int)((baseWidth - (shipImage.Width * nbShips)) / nbShips);
            int heightPadding = (enemyShips.Count == 0) ? 0 : (int)enemyShips.Last().Position.Y + 25;
            for (int i = 0; i < nbShips; i++)
            {
                for (int j = (widthPadding/2); j < baseWidth; j += widthPadding + shipImage.Width)
                {
                    Vecteur2D enemyPosition = new Vecteur2D(j + Position.X, Position.Y + heightPadding);
                    Bitmap newShipImage = new Bitmap(shipImage);
                    Spaceship enemy = new Spaceship(enemyPosition, nbLives, newShipImage, Side.Enemy);
                    enemyShips.Add(enemy);
                }
            }
        }
        /// <summary>
        /// Return boolean showing if the selected Missile was intersected with the selected Spaceship
        /// taking into account the whole game object image
        /// </summary>
        /// <param name="s">Spaceship to compare with</param>
        /// <param name="m">Missile to compare with</param>
        /// <returns>Does the whole game object image collides with the missil?</returns>
        public bool IsWholeImageInCollision(Spaceship s,Missile m)
        {
            if (m.Image == null) return false;
            return s.IsWholeImageInCollision(m);
        }
        /// <summary>
        /// Return the number of pixel in the collision between the selected Missile clash with the selected Spaceship
        /// taking into account the comparison pixel by pixel. If a black pixel gets hit
        /// by a missil, it turns to white.
        /// </summary>
        /// <param name="s">Spaceship to compare with</param>
        /// <param name="m">Missile to compare with</param>
        /// <returns>Number of pixels that got hit by a missil</returns>
        public int PixelsInCollision(Spaceship s,Missile m)
        {
            // Definition of the variable to return 
            int collision = 0;
            if (m.Image == null || s.Position == null) { return collision; }
            collision = s.PixelsInCollision(m);
            return collision;
            
            
        }
        /// <summary>
        /// Check for each Spaceship in enemyShips if it collides with the selected Missile
        /// </summary>
        /// <param name="m">Missile to verify the collision with</param>
        public override void Collision(Missile m)
        {
            // for each Spaceship in enemyShips if the current Spaceship and Missile is not an enemy, we try the collision using IsWholeImageInCollision
            // if it worked, we decrease the pixels which collide to the Spaceship.Lives but if Spaceship.Lives inferior to the number of pixel which collide, we set the Spaceship.Lives to 0
            foreach (Spaceship s in enemyShips) { if(Side.Enemy != s.Side && Side.Enemy != m.Side)
                {   if (IsWholeImageInCollision(s, m))
                    {   
                        int nmpixels = PixelsInCollision(s, m);
                        if(0< nmpixels) { s.Lives -= nmpixels; m.Lives = 0;
                            if(s.Lives <= 0) { shipToExplose = s; }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
