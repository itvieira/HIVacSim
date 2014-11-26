// ----------------------------------------------------------------------------
// <copyright file="Point3d.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim.Utility
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// Defines a structure to represent a point in the three dimensional space.
    /// </summary>
    [TypeConverter(typeof(Point3dParse))]
    [Serializable]
    public struct Point3d
    {    
        /// <summary>
        /// An uninitialized Point3d Structure.
        /// </summary>
        public static readonly Point3d Empty;

        #region Local variables
        private double _cx; // X coordinate
        private double _cy; // Y coordinate
        private double _cz; // Z coordinate
        #endregion

        #region Public Constructor

        /// <summary>
        /// Initialises a new instance of the <see cref="Point3d"/> struct.
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <param name="z">The z coordinate</param>
        public Point3d(double x, double y, double z)
        {
            this._cx = x;
            this._cy = y;
            this._cz = z;
        }

        #endregion

        #region Properties

        /// <summary>
        ///	Gets a value indicating whether X, Y and Z are zero.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (this._cx == 0.0) &&
                       (this._cy == 0.0) &&
                       (this._cz == 0.0);
            }
        }

        /// <summary>
        ///	Gets or sets the X coordinate of the Point3d.
        /// </summary>
        public double X
        {
            get { return this._cx; }
            set { this._cx = value; }
        }

        /// <summary>
        ///	Gets or sets the Y coordinate of the Point3d.
        /// </summary>
        public double Y
        {
            get { return this._cy; }
            set { this._cy = value; }
        }

        /// <summary>
        ///	Gets or sets the Z coordinate of the Point3d.
        /// </summary>
        public double Z
        {
            get { return this._cz; }
            set { this._cz = value; }
        }

        #endregion

        #region Public Shared Members

        /// <summary>
        /// Addition Operator
        /// </summary>
        public static Point3d operator +(Point3d pt, Point3d sz)
        {
            return new Point3d(pt.X + sz.X, pt.Y + sz.Y, pt.Z + sz.Z);
        }

        /// <summary>
        ///	Equality Operator
        /// </summary>
        /// <remarks>
        ///	Compares two Point3d objects. The return value is based on 
        ///	the equivalence of the X, Y and Z properties of the two points.
        /// </remarks>
        public static bool operator ==(Point3d a, Point3d b)
        {
            return (a.X == b.X) && (a.Y == b.Y) && (a.Z == b.Z);
        }

        /// <summary>
        ///	Inequality Operator
        /// </summary>
        /// <remarks>
        ///	Compares two Point3d objects. The return value is based on 
        ///	the equivalence of the X, Y and Z properties of the two points.
        /// </remarks>
        public static bool operator !=(Point3d a, Point3d b)
        {
            return (a.X != b.X) || (a.Y != b.Y) || (a.Z != b.Z);
        }

        /// <summary>
        ///	Subtraction Operator
        /// </summary>
        public static Point3d operator -(Point3d pt, Point3d sz)
        {
            return new Point3d(pt.X - sz.X, pt.Y - sz.Y, pt.Z - sz.Z);
        }

        /// <summary>
        /// Scalar product (dot) of two 3-dimensional vectors (Point3d)
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>The scalar product of the two vectors</returns>
        public static double Dot3d(Point3d a, Point3d b)
        {
            return Dot3d(a, b, true);
        }

        /// <summary>
        /// Scalar product (dot) of two 3-dimensional vectors (Point3d)
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <param name="unit">Is the vector length = 1.0?</param>
        /// <returns>The scalar product of the two vectors</returns>
        public static double Dot3d(Point3d a, Point3d b, bool unit)
        {
            if (unit)
            {
                return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
            }
            else
            {
                return (a.X * b.X + a.Y * b.Y + a.Z * b.Z) / (Mag3d(a) * Mag3d(b));
            }
        }

        /// <summary>
        /// Magnitude of a 3-dimensional vectors (Point3d)
        /// </summary>
        /// <param name="a">The vector</param>
        /// <returns>The magnitude of a vectors</returns>
        public static double Mag3d(Point3d a)
        {
            return Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
        }

        #endregion

        #region Public Instance Methods

        /// <summary>
        ///	Checks equivalence of this Point3d and another object.
        /// </summary>
        public override bool Equals(object o)
        {
            if (!(o is Point3d))
            {
                return false;
            }
            else
            {
                return this == (Point3d)o;
            }
        }

        /// <summary>
        ///	Calculates a hashing value.
        /// </summary>
        public override int GetHashCode()
        {
            return (int)(this._cx + this._cy) ^ (int)this._cz;
        }

        /// <summary>
        ///	Formats the Point3d as a string in coordinate notation.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{{X={0}, Y={1}, Y={2}}}", this._cx, this._cy, this._cz);
        }

        /// <summary>
        ///	Formats the Point3d as a comma separated values string.
        /// </summary>
        public string ToCSV()
        {
            return string.Format("{0},{1},{2}", this._cx, this._cy, this._cz);
        }

        #endregion
    }

    /// <summary>
    /// Parse the Point3d data structure
    /// </summary>
    [Serializable]
    internal class Point3dParse : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type t)
        {
            if (t == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, t);
        }

        /// <summary>
        /// Converts the given string to the Point3d
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
        /// </param>
        /// <param name="info">
        /// The <see cref="CultureInfo"/> to use as the current culture
        /// </param>
        /// <param name="value">The <see cref="String"/> to convert.</param>
        /// <returns>
        /// A <see cref="Point3d"/> that represents the converted value.
        /// </returns>
        public override object ConvertFrom(
                                            ITypeDescriptorContext context,
                                            CultureInfo info,
                                            object value)
        {
            if (value is string)
            {
                try
                {
                    string s = (string)value;
                    double x, y, z;
                    int split, split2;

                    // Gets x
                    split = s.IndexOf(',');
                    x = double.Parse(s.Substring(1, split));

                    // Gets y
                    split2 = s.IndexOf(',', split + 1);
                    y = double.Parse(s.Substring(split + 1, split2 - split - 1));

                    // Gets z
                    split = s.LastIndexOf(')');
                    z = double.Parse(s.Substring(split2 + 1, s.Length - split2 - 1));

                    return new Point3d(x, y, z);
                }
                catch
                {
                    // Do not throw parsing exception
                }

                // if we got this far, complain that we couldn't parse the string
                throw new ArgumentException(
                    "Can not convert '" + (string)value + "' to type Point3d");
            }

            return base.ConvertFrom(context, info, value);
        }
        
        /// <summary>
        /// Converts a <see cref="Point3d"/> data structure to string.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext"/> that provides a format context. 
        /// </param>
        /// <param name="culture">
        /// A <see cref="CultureInfo"/> object. 
        /// </param>
        /// <param name="value">
        /// The <see cref="Point3d"/> to convert.
        /// </param>
        /// <param name="destType">
        /// The Type to convert the value parameter to.
        /// </param>
        /// <returns>
        /// An <see cref="String"/> that represents the converted value.
        /// </returns>
        public override object ConvertTo(
                                        ITypeDescriptorContext context,
                                        CultureInfo culture,
                                        object value,
                                        Type destType)
        {
            if (destType == typeof(string) && value is Point3d)
            {
                Point3d data = (Point3d)value;

                // Builds up the Point3d variable structure (x,y,z)
                return "(" + data.X + "," + data.Y + "," + data.Z + ")";
            }

            return base.ConvertTo(context, culture, value, destType);
        }
    }
}
