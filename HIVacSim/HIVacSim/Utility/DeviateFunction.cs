// ----------------------------------------------------------------------------
// <copyright file="DeviateFunction.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim.Utility
{
    using System;

    /// <summary>
    /// Enumerates the names of available probability distributions
    /// </summary>
    [Serializable]
    public enum DeviateFunction
    {
        /// <summary>
        /// A fixed, average value is to be used
        /// </summary>
        Average,

        /// <summary>
        /// Beta distribution
        /// </summary>
        Beta,

        /// <summary>
        /// Exponential distribution
        /// </summary>
        Exponential,

        /// <summary>
        /// Gamma distribution
        /// </summary>
        Gamma,

        /// <summary>
        /// Inverse Normal (Gaussian) distribution
        /// </summary>
        InvNormal,

        /// <summary>
        /// Log Normal distribution
        /// </summary>
        LogNormal,

        /// <summary>
        /// Normal distribution
        /// </summary>
        Normal,

        /// <summary>
        /// Poisson distribution
        /// </summary>
        Poisson,

        /// <summary>
        /// Triangular distribution
        /// </summary>
        Triangular,

        /// <summary>
        /// Uniform distribution
        /// </summary>
        Uniform,

        /// <summary>
        /// Weibull distribution
        /// </summary>
        Weibull
    }
}
