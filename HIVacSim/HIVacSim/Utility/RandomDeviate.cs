// ----------------------------------------------------------------------------
// <copyright file="RandomDeviate.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim.Utility
{
    using System;

    /// <summary>
    /// Implements algorithms to generate random deviates from 
    /// several probability distribution functions
    /// </summary>
    public class RandomDeviate : MTRandom
    {
        #region Field
        /// <summary>
        /// Default numerical precision when comparing floating point numbers
        /// </summary>
        public static double DefaultPrecision = 1E-06;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="RandomDeviate"/> class, 
        /// using the specified seed value.
        /// </summary>
        /// <param name="seed">A number used to calculate a starting value for the 
        /// pseudo-random number sequence. If a negative number is specified, 
        /// the absolute value of the number is used. </param>
        public RandomDeviate(int seed)
            : base(seed)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="RandomDeviate"/> class, 
        /// using a time-dependent default seed value.
        /// </summary>
        public RandomDeviate()
            : base()
        {
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Compares two float-point numbers for equality within a given precision
        /// </summary>
        /// <param name="a">The first number to compare</param>
        /// <param name="b">The second number to compare</param>
        /// <returns><b>true</b> if the relative difference between <c>a</c> and <c>b</c>
        /// is less than the default numerical precision, otherwise <b>false</b></returns>
        public static bool Equals(double a, double b)
        {
            return Equals(a, b, DefaultPrecision);
        }

        /// <summary>
        /// Compares two float-point numbers for equality within a given precision
        /// </summary>
        /// <param name="a">The first number to compare</param>
        /// <param name="b">The second number to compare</param>
        /// <param name="precision">The numerical precision to be considered</param>
        /// <returns><b>true</b> if the relative difference between <c>a</c> and <c>b</c>
        /// is less than precision, otherwise <b>false</b></returns>
        public static bool Equals(double a, double b, double precision)
        {
            double norm = Math.Max(Math.Abs(a), Math.Abs(b));
            return norm < precision || Math.Abs(a - b) < precision * norm;
        }
        #endregion

        #region Sample Functions

        /// <summary>
        /// Sample from a uniform distribution with probability of success
        /// </summary>
        /// <remarks>
        /// This function is the same as a Bernoulli trial. 
        /// </remarks>
        /// <param name="probability">Probability of success</param>
        /// <returns>True for success or False for failure</returns>
        public bool Trial(double probability)
        {
            return this.Bernoulli(probability);
        }

        /// <summary>
        /// Sample from a probability distribution defined within a 
        /// Stochastic data structure.
        /// </summary>
        /// <remarks>
        /// The distribution names are tested by priority (more used first)
        /// </remarks>
        /// <param name="dist">The stochastic structure to sample from</param>
        /// <returns>A sample from the selected PDF</returns>
        public double Sample(Stochastic dist)
        {
            switch (dist.Distribution)
            {
                case DeviateFunction.Average:
                    {
                        return dist.Parameter1;
                    }

                case DeviateFunction.Gamma:
                    {
                        if (dist.Parameter3 == 0.0)
                            return this.Gamma(dist.Parameter1, dist.Parameter2);
                        else
                            return this.Gamma(dist.Parameter1, dist.Parameter2, dist.Parameter3);
                    }

                case DeviateFunction.Weibull:
                    {
                        if (dist.Parameter3 == 0.0)
                            return this.Weibull(dist.Parameter1, dist.Parameter2);
                        else
                            return this.Weibull(dist.Parameter1, dist.Parameter2, dist.Parameter3);
                    }

                case DeviateFunction.LogNormal:
                    {
                        if (dist.Parameter3 == 0.0)
                            return this.LogNormal(dist.Parameter1, dist.Parameter2);
                        else
                            return this.LogNormal(dist.Parameter1, dist.Parameter2, dist.Parameter3);
                    }

                case DeviateFunction.InvNormal:
                    {
                        if (dist.Parameter3 == 0.0)
                            return this.InvNormal(dist.Parameter1, dist.Parameter2);
                        else
                            return this.InvNormal(dist.Parameter1, dist.Parameter2, dist.Parameter3);
                    }

                case DeviateFunction.Exponential:
                    {
                        if (dist.Parameter2 == 0.0)
                            return this.Exponential(dist.Parameter1);
                        else
                            return this.Exponential(dist.Parameter1, dist.Parameter2);
                    }

                case DeviateFunction.Normal:
                    {
                        return this.Normal(dist.Parameter1, dist.Parameter2);
                    }

                case DeviateFunction.Beta:
                    {
                        if (dist.Parameter3 == 0.0 && dist.Parameter4 == 0.0)
                            return this.Beta(dist.Parameter1, dist.Parameter2);
                        else
                            return this.Beta(dist.Parameter1, dist.Parameter2, dist.Parameter3, dist.Parameter4);
                    }

                case DeviateFunction.Triangular:
                    {
                        return this.Triangular(dist.Parameter1, dist.Parameter2, dist.Parameter3);
                    }

                case DeviateFunction.Uniform:
                    {
                        return this.Uniform(dist.Parameter1, dist.Parameter2);
                    }

                case DeviateFunction.Poisson:
                    {
                        return this.Poisson(dist.Parameter1);
                    }

                default:
                    {
                        throw new ArgumentException(
                              "Invalid distribution function in argument.",
                              "RandomDeviate.Sample dist");
                    }
            }
        }

        #endregion

        #region Continuous Distributions

        /// <summary>
        /// Generates a random deviate from a generalized Beta distribution
        /// </summary>
        /// <param name="v">Continuous shape parameter of the PDF</param>
        /// <param name="w">Continuous shape parameter of the PDF</param>
        /// <param name="xMin">Continuous lower bound parameter of the PDF</param>
        /// <param name="xMax">Continuous upper bound parameter of the PDF</param>
        /// <returns>A sample from the Beta PDF</returns>
        public double Beta(double v, double w, double xMin, double xMax)
        {
            if (v > 0.0 && w > 0.0)
            {
                double y1, y2;
                if (v < w)
                {
                    return xMax - (xMax - xMin) * this.Beta(w, v);
                }

                y1 = this.Gamma(0.0, 1.0, v);
                y2 = this.Gamma(0.0, 1.0, w);
                return xMin + (xMax - xMin) * y1 / (y1 + y2);
            }
            else
            {
                throw new ArgumentException(
                    "Invalid Beta distribution argument.",
                    "shape v <=0.0 or shape w <=0.0");
            }
        }

        /// <summary>
        /// Generates a random deviate from a generalized Beta distribution
        /// </summary>
        /// <param name="v">Continuous shape parameter of the PDF</param>
        /// <param name="w">Continuous shape parameter of the PDF</param>
        /// <returns>A sample from the Beta PDF</returns>
        public double Beta(double v, double w)
        {
            return this.Beta(v, w, 0.0, 1.0);
        }

        /// <summary>
        /// Generates a random deviate from an Exponential distribution
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">scale parameter b &lt;= zero</exception>
        /// <param name="a">Continuous location parameter of the PDF</param>
        /// <param name="b">Continuous scale parameter of the PDF</param>
        /// <returns>A sample from the Exponential PDF</returns>
        public double Exponential(double a, double b)
        {
            if (b > 0.0)
            {
                return a - b * Math.Log(this.NextDouble());
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    "Scale b <= 0.0.",
                    "Invalid Exponential distribution argument.");
            }
        }

        /// <summary>
        /// Generates a random deviate from an Exponential distribution
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">scale parameter b &lt;= zero</exception>
        /// <param name="b">Continuous scale parameter of the PDF</param>
        /// <returns>A sample from the Exponential PDF</returns>
        public double Exponential(double b)
        {
            return this.Exponential(0.0, b);
        }

        /// <summary>
        /// Generates a random deviate from a standard Exponential distribution
        /// </summary>
        /// <returns>A sample from the standard Exponential PDF</returns>
        public double Exponential()
        {
            return this.Exponential(1.0);
        }

        /// <summary>
        /// Generates a random deviate from a Gamma distribution
        /// </summary>
        /// <param name="a">Continuous location parameter of the PDF</param>
        /// <param name="b">Continuous scale parameter of the PDF</param>
        /// <param name="c">Continuous shape parameter of the PDF</param>
        /// <returns>A sample from the Gamma PDF</returns>
        public double Gamma(double a, double b, double c)
        {
            if (b > 0.0 && c > 0.0)
            {
                // Set up variables
                double A = 1.0 / Math.Sqrt(2.0 * c - 1.0);
                double B = c - Math.Log(4.0);
                double Q = c + 1.0 / A;
                double T = 4.5;
                double D = 1.0 + Math.Log(T);
                double C = 1.0 + c / Math.E;

                // Local variables
                double p, p1, p2, v, y, z, w;

                if (c < 1.0)
                {
                    while (true)
                    {
                        p = C * this.NextDouble();
                        if (p > 1.0)
                        {
                            y = -Math.Log((C - p) / c);
                            if (this.NextDouble() <= Math.Pow(y, c - 1.0))
                            {
                                return a + b * y;
                            }
                        }
                        else
                        {
                            y = Math.Pow(p, 1.0 / c);
                            if (this.NextDouble() <= Math.Exp(-y))
                            {
                                return a + b * y;
                            }
                        }
                    }
                }
                else if (c == 1)
                {
                    return this.Exponential(a, b);
                }
                else  
                {
                    // c > 1
                    while (true)
                    {
                        p1 = this.NextDouble();
                        p2 = this.NextDouble();
                        v = A * Math.Log(p1 / (1.0 - p1));
                        y = c * Math.Exp(v);
                        z = p1 * p1 * p2;
                        w = B + Q * v - y;
                        if ((w + D - T * z) >= 0.0 || w >= Math.Log(z))
                        {
                            return a + b * y;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException(
                    "Invalid Gamma distribution argument.",
                    "scale b <= 0.0 or shape c <= 0.0");
            }
        }

        /// <summary>
        /// Generates a random deviate from a Gamma distribution
        /// </summary>
        /// <param name="b">Continuous scale parameter of the PDF</param>
        /// <param name="c">Continuous shape parameter of the PDF</param>
        /// <returns>A sample from the Gamma PDF</returns>
        public double Gamma(double b, double c)
        {
            return this.Gamma(0.0, b, c);
        }

        /// <summary>
        /// Generates a random deviate from an inverse Normal distribution,
        /// also known as Inverse Gaussian 
        /// </summary>
        /// <param name="a">Continuous location parameter of the PDF</param>
        /// <param name="b">Continuous scale parameter (mean) of the PDF</param>
        /// <param name="c">Continuous shape parameter (std. deviation) of the PDF</param>
        /// <returns>A sample from the inverse Normal PDF</returns>
        public double InvNormal(double a, double b, double c)
        {
            if (b > 0.0 && c > 0.0)
            {
                double stdNormal, stdNormalSq, phi, frac, u;

                phi = c / b;
                stdNormal = this.Normal(0.0, 1.0);
                stdNormalSq = stdNormal * stdNormal;

                frac = 1.0 + (stdNormalSq - Math.Sqrt(4.0 * phi * stdNormalSq +
                    (stdNormalSq * stdNormalSq))) / (2.0 * phi);

                u = this.NextDouble();
                if (u * (1.0 + frac) <= 1.0)
                {
                    return a + (b * frac);
                }
                else
                {
                    return a + (b / frac);
                }
            }
            else
            {
                throw new ArgumentException(
                    "Invalid Inverse Normal (Gaussian) distribution argument.",
                    "scale (mean) b <= 0.0 or shape (std. deviation) c <= 0.0");
            }
        }

        /// <summary>
        /// Generates a random deviate from an inverse Normal distribution,
        /// also known as Inverse Gaussian 
        /// </summary>
        /// <param name="b">Continuous scale parameter (mean) of the PDF</param>
        /// <param name="c">Continuous shape parameter (std. deviation) of the PDF</param>
        /// <returns>A sample from the inverse Normal PDF</returns>
        public double InvNormal(double b, double c)
        {
            return this.InvNormal(0.0, b, c);
        }

        /// <summary>
        /// Generates a random deviate from a Log Normal distribution
        /// </summary>
        /// <remarks>
        /// The parameters mean and variance are from a log normal distribution,
        /// therefore this function recalculates the correct mean and 
        /// std. deviation parameters before generate the random deviate.
        /// </remarks>
        /// <param name="a">Continuous location parameter of the PDF</param>
        /// <param name="b">Continuous scale parameter (mean) of the PDF</param>
        /// <param name="c">Continuous shape parameter (variance) of the PDF</param>
        /// <returns>A sample from the Log Normal PDF</returns>
        public double LogNormal(double a, double b, double c)
        {
            if (b > 0.0 && c > 0.0)
            {
                double mu, sd, sdSq;
                mu = Math.Log((b * b) / Math.Sqrt((c * c) + (b * b)));
                sdSq = Math.Log(((c * c) + (b * b)) / (b * b));
                sd = Math.Sqrt(sdSq);
                return a + Math.Exp(this.Normal(mu, sd));
            }
            else
            {
                throw new ArgumentException(
                    "Invalid Log Normal distribution argument.",
                    "scale (mean) b <= 0.0 or shape (std. deviation) c <= 0.0");
            }
        }

        /// <summary>
        /// Generates a random deviate from a Log Normal distribution
        /// </summary>
        /// The parameters mean and variance are from a log normal distribution,
        /// therefore this function recalculates the correct mean and 
        /// std. deviation parameters before generate the random deviate.
        /// <param name="b">Continuous scale parameter (mean) of the PDF</param>
        /// <param name="c">Continuous shape parameter (variance) of the PDF</param>
        /// <returns>A sample from the Log Normal PDF</returns>
        public double LogNormal(double b, double c)
        {
            return this.LogNormal(0.0, b, c);
        }

        /// <summary>
        /// Generates a random deviate from a Normal distribution
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">scale parameter b &lt;= zero</exception>
        /// <param name="a">Continuous location parameter (mean) of the PDF</param>
        /// <param name="b">Continuous scale parameter (std. deviation) of the PDF</param>
        /// <returns>A sample from the Normal PDF</returns>
        public double Normal(double a, double b)
        {
            if (b > 0.0)
            {
                double fac, rsq, v1, v2;
                do
                {
                    v1 = 2.0 * this.NextDouble() - 1.0;
                    v2 = 2.0 * this.NextDouble() - 1.0;
                    rsq = v1 * v1 + v2 * v2;
                } while (rsq >= 1.0 || rsq == 0.0);

                fac = Math.Sqrt((-2.0 * Math.Log(rsq)) / rsq);

                // Box-Muller transformation
                return (v1 * fac * b) + a;
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                                    "Scale (std. deviation) b <= 0.0.",
                                    "Invalid Normal distribution argument.");
            }
        }

        /// <summary>
        /// Generates a random deviate from a standard Normal distribution
        /// </summary>
        /// <returns>A sample from the Normal PDF</returns>
        public double Normal()
        {
            return this.Normal(0.0, 1.0);
        }

        /// <summary>
        /// Generates a random deviate from a Triangular distribution
        /// </summary>
        /// <param name="c">Continuous location of mode parameter of the PDF</param>
        /// <param name="xMin">Continuous lower bound parameter of the PDF</param>
        /// <param name="xMax">Continuous upper bound parameter of the PDF</param>
        /// <returns>A sample from the Triangular PDF</returns>
        public double Triangular(double c, double xMin, double xMax)
        {
            if (xMin < xMax && xMin <= c && c <= xMax)
            {
                double p, q;
                p = this.NextDouble();
                q = 1.0 - p;

                if (p <= ((c - xMin) / (xMax - xMin)))
                {
                    return xMin + Math.Sqrt((xMax - xMin) * (c - xMin) * p);
                }
                else
                {
                    return xMax - Math.Sqrt((xMax - xMin) * (xMax - c) * q);
                }
            }
            else
            {
                throw new ArgumentException(
                                "Invalid Triangular distribution argument.", 
                                "xMin > xMax or xMin > c or  c > xMax");
            }
        }

        /// <summary>
        /// Generates a random deviate from an uniform distribution 
        /// between the Min and Max values 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">minimum &gt;= maximum</exception>
        /// <param name="xMin">The minimum value</param>
        /// <param name="xMax">The maximum value</param>
        /// <returns>A sample from the Uniform PDF</returns>
        public double Uniform(double xMin, double xMax)
        {
            if (xMin < xMax)
            {
                return xMin + (this.NextDouble() * (xMax - xMin));
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                                    "Minimum xMin >= maximum xMax.",
                                    "Invalid Uniform distribution argument.");
            }
        }

        /// <summary>
        /// Generates a random deviate from a Weibull distribution
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">scale b or shape c 
        /// parameters &lt;= zero</exception>
        /// <param name="a">Continuous location parameter of the PDF</param>
        /// <param name="b">Continuous scale parameter of the PDF</param>
        /// <param name="c">Continuous shape parameter of the PDF</param>
        /// <returns>A sample from the Weibull PDF</returns>
        public double Weibull(double a, double b, double c)
        {
            if (b > 0.0 && c > 0.0)
            {
                // Weibull return value
                return a + b * Math.Pow(-Math.Log(1.0 - this.NextDouble()), 1.0 / c);
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                                    "Scale b <= 0.0 or shape c <= 0.0.",
                                    "Invalid Weibull distribution argument.");
            }
        }

        /// <summary>
        /// Generates a random deviate from a Weibull distribution
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">scale b or shape c 
        /// parameters &lt;= zero</exception>
        /// <param name="b">Continuous scale parameter of the PDF</param>
        /// <param name="c">Continuous shape parameter of the PDF</param>
        /// <returns>A sample from the Weibull PDF</returns>
        public double Weibull(double b, double c)
        {
            return this.Weibull(0.0, b, c);
        }

        #endregion

        #region Discrete Distribution

        /// <summary>
        /// Perform a Bernoulli trial with p probability of success and
        /// (1 - p) of failure.
        /// </summary>
        /// <param name="p">Probability of success in a single trial</param>
        /// <returns>True for success outcome or False for failure.</returns>
        public bool Bernoulli(double p)
        {
            if (p >= 0.0 && p <= 1.0)
            {
                return this.NextDouble() <= p;
            }
            else
            {
                throw new ArgumentException(
                                "Invalid Bernoulli distribution argument.", 
                                "probability of success p < 0.0 or p > 1.0");
            }
        }

        /// <summary>
        /// Perform a Poisson trial, where the probability of k successes when
        /// the probability of success in each trial is small and the rate of 
        /// occurrence mu is constant.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The parameter mu &lt;= zero
        /// </exception>
        /// <param name="mu">The mean number of successes</param>
        /// <returns>The number of successes.</returns>
        public int Poisson(double mu)
        {
            double a, p;
            int x;

            if (mu > 0.0)
            {
                if (mu <= 20.0)
                {
                    // Use the direct method
                    x = -1;
                    a = Math.Exp(-mu);
                    p = 1.0;
                    while (p > a)
                    {
                        p *= this.NextDouble();
                        x++;
                    }

                    return x;
                }
                else
                {
                    // Use normal approximation
                    a = Math.Pow(mu, 0.5);
                    p = this.Normal(0.0, 1.0);
                    x = Math.Max(0, (int)Math.Floor(0.5 + mu + a * p));
                    return x;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                                "Mean number of successes mu <= 0.",
                                "Invalid Poisson distribution argument.");
            }
        }

        #endregion

        #region Bivariate Distributions

        /// <summary>
        /// Generate uniformly distributed points on the surface of the 
        /// unit (3-dimensional) sphere
        /// </summary>
        /// <returns>The point on the unit sphere</returns>
        public Point3d Sphere3d()
        {
            double u, v, s, a;
            do
            {
                u = this.Uniform(-1, 1);
                v = this.Uniform(-1, 1);
                s = (u * u) + (v * v);
            } while (s > 1.0);

            a = 2 * Math.Sqrt(1.0 - s);
            return new Point3d(a * u, a * v, 2 * s - 1);
        }

        /// <summary>
        /// Generate uniformly distributed points on the surface of the 
        /// unit sphere in n-dimensions.
        /// </summary>
        /// <remarks>
        /// When n = 1, this returns {-1, +1}.
        /// When n = 2, it generates coordinates of points on the unit circle.
        /// When n = 3, it generates coordinates of points on the unit 3-sphere.
        /// </remarks>
        /// <param name="n">The number of dimensions</param>
        /// <returns>
        /// The n-dimensions point on the surface of the unit sphere.
        /// </returns>
        public double[] Spherical(int n)
        {
            double[] x;
            double r2, norm;
            int i;

            if (n < 1)
            {
                return null;
            }

            // Generate a point inside the unit n-sphere by normal polar method
            x = new double[n];
            r2 = 0.0;
            for (i = 0; i < n; i++)
            {
                x[i] = this.Normal(0.0, 1.0);
                r2 += x[i] * x[i];
            }

            // Project the point onto the surface of the n-sphere by scaling
            norm = 1.0 / Math.Sqrt(r2);
            for (i = 0; i < n; i++)
            {
                x[i] *= norm;
            }

            return x;
        }

        #endregion
    }
}
