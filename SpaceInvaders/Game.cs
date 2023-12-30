using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace SpaceInvaders
{
    /// <summary>
    /// This class represents the entire game, it implements the singleton pattern
    /// </summary>
    class Game
    {
        #region GameObjects management
        /// <summary>
        /// Set of all game objects currently in the game
        /// </summary>
        public HashSet<GameObject> gameObjects = new HashSet<GameObject>();
        /// <summary>
        /// Set of new game objects scheduled for addition to the game
        /// </summary>
        private HashSet<GameObject> pendingNewGameObjects = new HashSet<GameObject>();

        /// <summary>
        /// Schedule a new object for addition in the game.
        /// The new object will be added at the beginning of the next update loop
        /// </summary>
        /// <param name="gameObject">object to add</param>
        public void AddNewGameObject(GameObject gameObject)
        {
            pendingNewGameObjects.Add(gameObject);
        }
        /// <summary>
        /// Ship of the player
        /// </summary>
        public Spaceship playerShip;
        /// <summary>
        /// EnemyBlock containing the current list of enemies
        /// </summary>
        private EnemyBlock enemies;
        /// <summary>
        /// Current state of the game
        /// </summary>
        public GameState state;
        
        #endregion

        #region game technical elements
        /// <summary>
        /// Size of the game area
        /// </summary>
        public Size gameSize;

        /// <summary>
        /// State of the keyboard
        /// </summary>
        public HashSet<Keys> keyPressed = new HashSet<Keys>();
        #endregion

        #region static fields (helpers)

        /// <summary>
        /// Singleton for easy access
        /// </summary>
        public static Game game { get; private set; }
        #endregion


        #region constructors
        /// <summary>
        /// Initializing the game
        /// </summary>
        /// <param name="gameSize">Size of the game area</param>
        public static Game CreateGame(Size gameSize)
        {
            if (game == null)
                game = new Game(gameSize);
            game.state = GameState.Menu;
            return game;
        }
        /// <summary>
        /// Restart the game
        /// </summary>
        /// <param name="gameSize">Size of the game area</param>
        public static Game RestartGame(Size gameSize)
        {
            if (game.state == GameState.Lost || game.state == GameState.Win)
                RestartedGame(gameSize,game);
            game.state = GameState.Menu;
            return game;
        }

        /// <summary>
        /// Function to start the game
        /// </summary>
        /// <param name="gameSize">Size of the game area</param>
        private Game(Size gameSize)
        {
            RestartedGame(gameSize, this); 
        }
        /// <summary>
        /// Function to restart the game after a win or a lose
        /// </summary>
        /// <param name="gameSize">Size of the game area</param>
        /// <param name="game">Game session to restart</param>
        public static void RestartedGame(Size gameSize, Game game)
        {
            game.gameSize = gameSize;
            Vecteur2D playerPosition = new Vecteur2D(gameSize.Width / 2, gameSize.Height - 50);
            Bitmap playerImage = SpaceInvaders.Properties.Resources.ship3;
            game.playerShip = new PlayerSpaceship(playerPosition, 5, playerImage);
            game.AddNewGameObject(game.playerShip);

            Size size = new Size(300, 100);
            Vecteur2D position = new Vecteur2D(10, 10);
            game.enemies = new EnemyBlock(position, size);
            Bitmap enemy1Image = SpaceInvaders.Properties.Resources.ship1;
            game.enemies.AddLine(5, 1, enemy1Image);

            Bitmap enemy2Image = SpaceInvaders.Properties.Resources.ship2;
            game.enemies.AddLine(5, 1, enemy2Image);
            game.AddNewGameObject(game.enemies);

            Bitmap enemy3Image = SpaceInvaders.Properties.Resources.ship3;
            game.enemies.AddLine(5, 1, enemy3Image);
            game.AddNewGameObject(game.enemies);

            Bitmap enemy4Image = SpaceInvaders.Properties.Resources.ship4;
            game.enemies.AddLine(5, 1, enemy4Image);
            game.AddNewGameObject(game.enemies);

            Bitmap enemy5Image = SpaceInvaders.Properties.Resources.ship5;
            game.enemies.AddLine(2 , 1, enemy5Image);
            game.AddNewGameObject(game.enemies);

            Bitmap bunkerImage = SpaceInvaders.Properties.Resources.bunker;
            double distance = (gameSize.Width - 3 * bunkerImage.Width) / 4;
            for (int i = 0; i < 3; i++)
            {
                Vecteur2D bunkerPos = new Vecteur2D(i * bunkerImage.Width + (i + 1) * distance, gameSize.Height - 5 * playerImage.Width);
                Bunker b = new Bunker(bunkerPos, bunkerImage);
                game.AddNewGameObject(b);
            }
        }
        #endregion

        #region methods

        /// <summary>
        /// Force a given key to be ignored in following updates until the user
        /// explicitily retype it or the system autofires it again.
        /// </summary>
        /// <param name="key">key to ignore</param>
        public void ReleaseKey(Keys key)
        {
            keyPressed.Remove(key);
        }

        /// <summary>
        /// Draw the whole game
        /// </summary>
        /// <param name="g">Graphics to draw in</param>
        public void Draw(Graphics g)
        {
            foreach (GameObject gameObject in gameObjects) gameObject.Draw(this, g);
            Font drawFont = new Font("Courier New", 13);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            // Variable to store to text to measure
            string textToMeasure = "";
            // Size of the prediction by using textToMeasure
            SizeF textSize;
            // Draw the menu page
            if (state == GameState.Menu) {
                Font drawFontMenu = new Font("Courier New", 20);
                // Measure and draw for "SPACE" text at the center of the window
                textToMeasure = "SPACE";
                textSize = g.MeasureString(textToMeasure, drawFontMenu);
                g.DrawString("SPACE", drawFontMenu, drawBrush, (game.gameSize.Width-textSize.Width)/2, gameSize.Height/3);
                // Measure and draw for "INVADERS" text at the center of the window
                textToMeasure = "INVADERS";
                textSize = g.MeasureString(textToMeasure, drawFontMenu);
                g.DrawString("INVADERS", drawFontMenu, drawBrush, (game.gameSize.Width - textSize.Width)/2, (gameSize.Height/3)+ drawFontMenu.Height*2);
                // Measure and draw for "INVADERS" text at the center of the window
                textToMeasure = "PRESS P TO PLAY";
                textSize = g.MeasureString(textToMeasure, drawFontMenu);
                g.DrawString("PRESS P TO PLAY", drawFontMenu, drawBrush, (game.gameSize.Width - textSize.Width) / 2, (gameSize.Height / 3) + drawFontMenu.Height * 4);
            }
            // Draw "PAUSE" at position (10,10)
            if (state == GameState.Pause) { g.DrawString("PAUSE",drawFont,drawBrush,10,10); }
            // Draw "EN COURS" at position (10,10)
            if (state== GameState.Play) { g.DrawString("EN COURS", drawFont, drawBrush, 10, 10); }
            if (state == GameState.Win) { textToMeasure = "YOU WON"; drawBrush = new SolidBrush(Color.Green); }
            if (state == GameState.Lost) { textToMeasure = "YOU LOST"; drawBrush = new SolidBrush(Color.Red); }
            if (state == GameState.Win || state == GameState.Lost)
            {
                // Measure and draw for "WIN" or "LOST" text at the center of the window
                textSize = g.MeasureString(textToMeasure, drawFont);
                g.DrawString(textToMeasure, drawFont, drawBrush, (game.gameSize.Width - textSize.Width) / 2, game.gameSize.Height / 2);
                // Measure and draw of "PRESS SPACEBAR TO RESTART THE GAME" text at the center of the window
                textToMeasure = "PRESS SPACEBAR TO RESTART THE GAME";
                textSize = g.MeasureString(textToMeasure, drawFont);
                g.DrawString("PRESS SPACEBAR TO RESTART THE GAME", drawFont, drawBrush, (game.gameSize.Width - textSize.Width)/2, (game.gameSize.Height/2)+drawFont.Height*2);
                textToMeasure = "PRESS P TO GO TO MENU";
                textSize = g.MeasureString(textToMeasure, drawFont);
                g.DrawString("PRESS P TO GO TO MENU", drawFont, drawBrush, (game.gameSize.Width - textSize.Width) / 2, (game.gameSize.Height / 2) + drawFont.Height * 4);
            }
        }


        /// <summary>
        /// Update the game
        /// </summary>
        /// <param name="deltaT">Time of program ( deltaT )</param>
        public void Update(double deltaT)
        {
            if(state==GameState.Play) 
            {
                // add new game objects
                gameObjects.UnionWith(pendingNewGameObjects);
                pendingNewGameObjects.Clear();
                // update each game object
                foreach (GameObject gameObject in gameObjects) { gameObject.Update(this, deltaT); }
                // remove dead objects
                gameObjects.RemoveWhere(gameObject => !gameObject.IsAlive());
                // remove dead enemy ships
                foreach(GameObject gameObject in gameObjects) if (gameObject.GetType() == typeof(EnemyBlock)) ((EnemyBlock)gameObject).enemyShips.RemoveWhere(enemy => !enemy.IsAlive());
            }
            // Set the state of the GameState to Menu if Key pressed is "P" and Gamestate is GameState.Win or GameState.Lost
            if (keyPressed.Contains(Keys.P) && (game.state == GameState.Win || game.state == GameState.Lost)) { RestartGame(gameSize); game.state = GameState.Menu; ReleaseKey(Keys.P); }
            // Set the state of the GameState to Pause if Key pressed is "P" and Gamestate is GameState.Play if not GameState.Pause
            else if (keyPressed.Contains(Keys.P)) { state = state == GameState.Play ? GameState.Pause : GameState.Play; ReleaseKey(Keys.P); }
            // If player has no live, set the GameState to lost
            if (playerShip.Lives == 0) { state = GameState.Lost; }
            // If the game is won or lost
            // Emptying the HashSet of Objects of the current Game
            if (state == GameState.Lost || state == GameState.Win) { gameObjects.Clear(); pendingNewGameObjects.Clear(); }
            // If the player wins or loses and then presses the spacebar, tha game restarts
            if (keyPressed.Contains(Keys.Space) && (state == GameState.Lost|| state == GameState.Win) ) { RestartGame(gameSize); game.state = GameState.Play; ReleaseKey(Keys.Space); }
            // For each gameObjects check if type is EnemyBlock if so we do a break and check if last gameObject type is not a EnemyBlock and the current gameObject is the last one
            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.GetType() == typeof(EnemyBlock))
                { 
                    if(((EnemyBlock)gameObject).enemyShips.Last().Position.Y >= playerShip.Position.Y) state = GameState.Lost;
                    break;
                }
                if (gameObjects.Last().GetType() != typeof(EnemyBlock) && gameObject == gameObjects.Last()) { state = GameState.Win; }
            }
        }
        #endregion
    }
    /// <summary>
    /// Shows the state of the current game
    /// </summary>
    enum GameState 
    { 
        Play,
        Pause,
        Win,
        Lost,
        Menu
    }

}
