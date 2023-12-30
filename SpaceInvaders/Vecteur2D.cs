using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    internal class Vecteur2D
    {
        #region Fields
        /// <summary>
        /// X position
        /// </summary>
        public double X
        { get; set; }

        /// <summary>
        /// Y position
        /// </summary>
        public double Y
        { get; set; }

        /// <summary>
        /// Norm of the vector
        /// </summary>
        public double Norme
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Simple constructor
        /// </summary>
        /// <param name="x">x start position</param>
        /// <param name="y">y start position</param>
        public Vecteur2D(double x = 0, double y = 0)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Add two vectors
        /// </summary>
        /// <param name="v1">first vector</param>
        /// <param name="v2">vector to add</param>
        public static Vecteur2D operator +(Vecteur2D v1, Vecteur2D v2)
        { return new Vecteur2D(v1.X + v2.X, v1.Y + v2.Y); }

        /// <summary>
        /// Subtract two vectors
        /// </summary>
        /// <param name="v1">first vector</param>
        /// <param name="v2">vector to subtract</param>
        public static Vecteur2D operator -(Vecteur2D v1, Vecteur2D v2)
        { return new Vecteur2D(v1.X - v2.X, v1.Y - v2.Y); }

        /// <summary>
        /// Negative vector
        /// </summary>
        /// <param name="v1">vector</param>
        public static Vecteur2D operator -(Vecteur2D v1)
        { return new Vecteur2D(-v1.X, -v1.Y); }

        /// <summary>
        /// Multiply a vector by a scalar, order matters: first vector and then scalar
        /// </summary>
        /// <param name="v1">base vector</param>
        /// <param name="scalar">scalar</param>
        public static Vecteur2D operator *(Vecteur2D v1, double scalar)
        { return new Vecteur2D(v1.X * scalar, v1.Y * scalar); }

        /// <summary>
        /// Multiply a vector by a scalar, order matters: first scalar and then vector
        /// </summary>
        /// <param name="scalar">scalar</param>
        /// <param name="v1">base vector</param>
        public static Vecteur2D operator *(double scalar, Vecteur2D v1)
        { return v1 * scalar; }

        #endregion
    }
}
