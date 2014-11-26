// ----------------------------------------------------------------------------
// <copyright file="Stochastic.cs" company="HIVacSim">
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
    /// Implements the stochastic data structure representing a
    /// probability distribution.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(StochasticParse))]
    public struct Stochastic
    {
        #region Local variables
        private DeviateFunction _dist;
        private double _first;
        private double _second;
        private double _third;
        private double _fouth;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="Stochastic"/> struct.
        /// </summary>
        /// <param name="dist">The name of the distribution</param>
        /// <param name="first">The first parameter of the distribution</param>
        /// <param name="second">The second parameter of the distribution</param>
        /// <param name="third">The third parameter of the distribution</param>
        /// <param name="fouth">The fourth parameter of the distribution</param>
        public Stochastic(DeviateFunction dist, double first, double second, double third, double fouth)
        {
            this._dist = dist;
            this._first = first;
            this._second = second;
            this._third = third;
            this._fouth = fouth;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Stochastic"/> struct.
        /// </summary>
        /// <param name="dist">The name of the distribution</param>
        /// <param name="first">The first parameter of the distribution</param>
        /// <param name="second">The second parameter of the distribution</param>
        /// <param name="third">The third parameter of the distribution</param>
        public Stochastic(DeviateFunction dist, double first, double second, double third) :
            this(dist, first, second, third, 0.0)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Stochastic"/> struct.
        /// </summary>
        /// <param name="dist">The name of the distribution</param>
        /// <param name="first">The first parameter of the distribution</param>
        /// <param name="second">The second parameter of the distribution</param>
        public Stochastic(DeviateFunction dist, double first, double second) :
            this(dist, first, second, 0.0, 0.0)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Stochastic"/> struct.
        /// </summary>
        /// <param name="dist">The name of the distribution</param>
        /// <param name="first">The first parameter of the distribution</param>
        public Stochastic(DeviateFunction dist, double first) :
            this(dist, first, 0.0, 0.0, 0.0)
        {
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the provability distribution's name
        /// </summary>
        [DefaultValueAttribute(DeviateFunction.Average),
        DescriptionAttribute("The name of the probability distribution.")]
        public DeviateFunction Distribution
        {
            get { return this._dist; }
            set { this._dist = value; }
        }

        /// <summary>
        /// Gets or sets the first parameter of the distribution 
        /// (location a, scale b, shape c, xMin, degree of freedom df, mu, n, s, v)
        /// </summary>
        [DefaultValueAttribute(0.00),
        DescriptionAttribute("The first parameter of the distribution (" +
            "location a, scale b, shape c, xMin, degree of freedom df, mu, n, s, v)")]
        public double Parameter1
        {
            get { return this._first; }
            set { this._first = value; }
        }

        /// <summary>
        /// Gets or sets the second parameter of the distribution 
        /// (scale b, shape c, xMin, xMax, degree of freedom df, d, p, v, w)
        /// </summary>
        [DefaultValueAttribute(0.00),
        DescriptionAttribute("The second parameter of the distribution. (" +
            "scale b, shape c, xMin, xMax, degree of freedom df, d, p, v, w)")]
        public double Parameter2
        {
            get { return this._second; }
            set { this._second = value; }
        }
        
        /// <summary>
        /// Gets or sets the third parameter of the distribution (shape c, xMin, xMax, k, v)
        /// </summary>
        [DefaultValueAttribute(0.00),
        DescriptionAttribute("The third parameter of the distribution (" +
            "shape c, xMin, xMax, k, v)")]
        public double Parameter3
        {
            get { return this._third; }
            set { this._third = value; }
        }

        /// <summary>
        /// Gets or sets the fourth parameter of the distribution (xMax, w)
        /// </summary>
        [DefaultValueAttribute(0.00),
        DescriptionAttribute("The fourth parameter of the distribution (xMax, w)")]
        public double Parameter4
        {
            get { return this._fouth; }
            set { this._fouth = value; }
        }
        #endregion

        /// <summary>
        /// Description of the parameters of available distribution function
        /// </summary>
        /// <param name="dist">The distribution to get the parameters</param>
        /// <returns>The description of the parameters</returns>
        public static string Parameters(DeviateFunction dist)
        {
            string retdata;
            string[,] data = new string[11, 2];
            data[0, 0] = "Average"; 
            data[0, 1] = "Value:";

            data[1, 0] = "Beta"; 
            data[1, 1] = "Shape:,Shape:,Lower bound:,Upper bound:";

            data[2, 0] = "Exponential"; 
            data[2, 1] = "Location:,Scale:";

            data[3, 0] = "Gamma"; 
            data[3, 1] = "Location:,Scale:,Shape:";

            data[4, 0] = "InvNormal"; 
            data[4, 1] = "Location:,Scale:,Shape:";

            data[5, 0] = "LogNormal"; 
            data[5, 1] = "Location:,Scale:,Shape:";

            data[6, 0] = "Normal"; 
            data[6, 1] = "Location:,Scale:";

            data[7, 0] = "Poisson"; 
            data[7, 1] = "No. of successes:";

            data[8, 0] = "Triangular"; 
            data[8, 1] = "Location:,Lower bound:,Upper bound:";

            data[9, 0] = "Uniform"; 
            data[9, 1] = "Minimum:,Maximum:";

            data[10, 0] = "Weibull"; 
            data[10, 1] = "Location:,Scale:,Shape:";

            int dimCount = data.GetUpperBound(0);
            for (int i = 0; i < dimCount; i++)
            {
                if (data[i, 0] == dist.ToString())
                {
                    return retdata = data[i, 1];
                }
            }

            return string.Empty; // Not found
        }
    }

    /// <summary>
    /// Parse the Stochastic data structure
    /// </summary>
    [Serializable]
    internal class StochasticParse : ExpandableObjectConverter
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
        /// Converts the given string to the Stochastic
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
        /// </param>
        /// <param name="info">
        /// The <see cref="CultureInfo"/> to use as the current culture
        /// </param>
        /// <param name="value">The <see cref="String"/> to convert.</param>
        /// <returns>
        /// An <see cref="Stochastic"/> that represents the converted value.
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
                    double pOne, pTwo, pThree, pFour;
                    int split, split2;
                    DeviateFunction dist;

                    // Parse the format "Distribution(ParamOne,ParamTwo,ParamThree,ParamFour)"
                    // Gets the distribution
                    split = s.IndexOf('(');
                    dist = (DeviateFunction)Convert.ToByte(s.Substring(0, split));

                    // pick up the first parameter
                    split2 = s.IndexOf(',', split + 1);
                    pOne = double.Parse(s.Substring(split + 1, split2 - split - 1));

                    // pick up the second parameter
                    split = s.IndexOf(',', split2 + 1);
                    pTwo = double.Parse(s.Substring(split2 + 1, split - split2 - 1));

                    // pick up the third parameter
                    split2 = s.IndexOf(',', split + 1);
                    pThree = double.Parse(s.Substring(split + 1, split2 - split - 1));

                    // pick up the third parameter
                    split = s.LastIndexOf(')');
                    pFour = double.Parse(s.Substring(split2 + 1, s.Length - split2 - 1));

                    Stochastic udtData = new Stochastic();
                    udtData.Distribution = dist;
                    udtData.Parameter1 = pOne;
                    udtData.Parameter2 = pTwo;
                    udtData.Parameter3 = pThree;
                    udtData.Parameter4 = pFour;
                    return udtData;
                }
                catch 
                { 
                }

                // if we got this far, complain that we couldn't parse the string
                throw new ArgumentException(
                    "Can not convert '" + (string)value + "' to type Stochastic");
            }

            return base.ConvertFrom(context, info, value);
        }

        /// <summary>
        /// Converts a <see cref="Stochastic"/> data structure to string.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
        /// <param name="culture">A <see cref="CultureInfo"/> object. </param>
        /// <param name="value">The <see cref="Stochastic"/> to convert.</param>
        /// <param name="destType">The Type to convert the value parameter to.</param>
        /// <returns>An <see cref="String"/> that represents the converted value.</returns>
        public override object ConvertTo(
                                        ITypeDescriptorContext context,
                                        CultureInfo culture, 
                                        object value, 
                                        Type destType)
        {
            if (destType == typeof(string) && value is Stochastic)
            {
                Stochastic udtData = (Stochastic)value;

                // Builds up the stochastic variable structure
                // Distribution(ParamOne, ParamTwo, ParamThree,ParamFour)
                return udtData.Distribution + "(" + udtData.Parameter1 + "," +
                        udtData.Parameter2 + "," + udtData.Parameter3 + "," +
                        udtData.Parameter4 + ")";
            }

            return base.ConvertTo(context, culture, value, destType);
        }
    }
}
