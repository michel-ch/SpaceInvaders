using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using System.Media;
using System.IO;
using System.Threading.Tasks;
using System.Timers;

namespace SpaceInvaders
{
    class Spaceship : SimpleObject
    {
        #region Fields
        /// <summary>
        /// Speed of the spaceship
        /// </summary>
        public double speedPixelPerSecond = 600;
        /// <summary>
        /// Missil object that the spaceship can shoot
        /// </summary>
        public Missile missile = null;
        /// <summary>
        /// Sound of enemy when shooting
        /// </summary>
        public SoundPlayer enemyShoot = new SoundPlayer(SpaceInvaders.Properties.Resources.enemyShoot);
        #endregion

        #region Constructor
        /// <summary>
        /// Simple constructor
        /// </summary>
        /// <param name="position">start vector position</param>
        /// <param name="lives">start number of lives</param>
        /// <param name="image">image of the spaceship</param>
        /// <param name="side">side of the object</param>
        public Spaceship(Vecteur2D position, int lives, Bitmap image, Side side) : base(side)
        {
            Position = position;
            Lives = lives;
            Image = image;
        }
        #endregion

        #region Methods
        public override void Update(Game gameInstance, double deltaT)
        {}
        /// <summary>
        /// Base function to shoot missils
        /// </summary>
        /// <param name="gameInstance">game instance</param>
        public virtual void Shoot(Game gameInstance)
        {
            Bitmap imagemissile = SpaceInvaders.Properties.Resources.shoot1;
            Vecteur2D pos_missile = new Vecteur2D(Position.X + Image.Width / 2, Position.Y);
            missile = new Missile(pos_missile, 1, imagemissile, Side.Enemy);
            gameInstance.AddNewGameObject(missile);
            // Try to play the enemyShoot sound
            try { enemyShoot.Play(); }
            catch (Exception ex) { Console.WriteLine("enemyShoot sound not found"); Console.WriteLine(ex.ToString()); }
        }

        /// <summary>
        /// Function that reduces the number of lives according to the number of pixels in collision
        /// </summary>
        /// <param name="m">Missil that generated the collision</param>
        /// <param name="numberOfPixelsInCollision">Number of pixels in collision</param>
        protected override void OnCollision(Missile m, int numberOfPixelsInCollision)
        {
            if(this.GetType() != typeof(Spaceship)) {
                if (!Object.ReferenceEquals(missile, m)) {
                    int min = Math.Min(Lives, m.Lives);
                    Lives -= min;
                    m.Lives -= min;
                }
            }
        }
        #endregion
    }

    class PlayerSpaceship : Spaceship
    {
        #region Fields
        /// <summary>
        /// Sound of player when shooting
        /// </summary>
        public SoundPlayer playerShoot = new SoundPlayer(SpaceInvaders.Properties.Resources.playerShoot);
        /// <summary>
        /// Store if the playerShip can shoot
        /// </summary>
        private bool canShoot = true;
        /// <summary>
        /// Timer for the playerShip shooting
        /// </summary>
        private System.Timers.Timer spaceKeyTimer = new System.Timers.Timer(250);
        #endregion

        #region Constructor
        /// <summary>
        /// Simple constructor
        /// </summary>
        /// <param name="position">start vector position</param>
        /// <param name="lives">start number of lives</param>
        /// <param name="image">image of the spaceship</param>
        // Set the side of PlayerSpaceship to Side.Ally
        public PlayerSpaceship(Vecteur2D position, int lives, Bitmap image) : base(position, lives, image, Side.Ally)
        { }
        #endregion

        #region Methods
        /// <summary>
        /// Override the base function to determine the player side
        /// </summary>
        /// <param name="gameInstance">game instance</param>
        public override void Shoot(Game gameInstance)
        {
            Bitmap imagemissile = SpaceInvaders.Properties.Resources.shoot1;
            Vecteur2D pos_missile = new Vecteur2D(Position.X + Image.Width / 2, Position.Y);
            missile = new Missile(pos_missile, 1, imagemissile, Side.Ally);
            gameInstance.AddNewGameObject(missile);
            // Try to play the playerShoot sound
            try { playerShoot.Play(); }
            catch (Exception ex) { Console.WriteLine("playerShoot sound not found"); Console.WriteLine(ex.ToString()); }
        }
        

        /// <summary>
        /// Override the base function to display the number of lives
        /// </summary>
        /// <param name="gameInstance">game instance</param>
        public override void Draw(Game gameInstance, Graphics graphics)
        {
            // Draw the Draw method of mother class
            base.Draw(gameInstance, graphics);
            Font drawFont = new Font("Courier New", 13);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            string LivesShow = Lives.ToString();
            // Verify if LivesShow contains 2 numbers if not add "0" to it
            while (LivesShow.Length < 2) { LivesShow = "0" + LivesShow; }
            // Draw the number of lifes
            graphics.DrawString(LivesShow, drawFont, drawBrush, gameInstance.gameSize.Width - 55, 10);
            // Get et draw the heart image
            Bitmap heartImage = SpaceInvaders.Properties.Resources.life;
            graphics.DrawImage(heartImage, gameInstance.gameSize.Width - 30, 12);
        }
        /// <summary>
        /// Override the base function to update the object display and state
        /// </summary>
        /// <param name="gameInstance">game instance</param>
        /// <param name="deltaT">time delta</param>
        public override void Update(Game gameInstance, double deltaT)
        {
            // Stop the timer and allow
            // by stopping the timer and setting canShoot to true to allow shooting
            spaceKeyTimer.Elapsed += (s, e) => { spaceKeyTimer.Stop(); canShoot = true; };
            // If left key pressed, we add speedPixelPerSecond to playerShip.Position.Y
            Position.X -= (gameInstance.keyPressed.Contains(Keys.Left) && Position.X > Image.Width / 2) ? speedPixelPerSecond * deltaT : 0;
            // If right key pressed, we add speedPixelPerSecond to playerShip.Position.X
            Position.X += (gameInstance.keyPressed.Contains(Keys.Right) && Position.X + (Image.Width * 3 / 2) < gameInstance.gameSize.Width) ? speedPixelPerSecond * deltaT : 0;
            
            if (gameInstance.keyPressed.Contains(Keys.Space) && canShoot) {
                gameInstance.playerShip.Shoot(gameInstance);
                gameInstance.ReleaseKey(Keys.Space);
                spaceKeyTimer.Start();
                canShoot = false;
            }
            if (missile != null && (missile.Position.Y < 0 || missile.Lives <= 0)) { missile.Lives = 0; missile = null; }
        }
        #endregion
    }
}
