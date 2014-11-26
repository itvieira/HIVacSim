// ----------------------------------------------------------------------------
// <copyright file="DataSummary.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim.Utility
{
    using System;
    using System.Text;

    /// <summary>
    /// Descriptive statistics structure
    /// </summary>
    public struct DataSummary
    {
        /// <summary>
        /// Length of the data
        /// </summary>
        public int Count;

        /// <summary>
        /// Sum of the data
        /// </summary>
        public double Sum;

        /// <summary>
        /// Minimum of the data
        /// </summary>
        public double Min;

        /// <summary>
        /// Maximum of the data
        /// </summary>
        public double Max;

        /// <summary>
        /// Range of the data
        /// </summary>
        public double Range;
        
        /// <summary>
        /// Arithmetic Mean of the data
        /// </summary>
        public double AMean;

        /// <summary>
        /// Harmonic Mean of the data
        /// </summary>
        public double HMean;

        /// <summary>
        /// Harmonic Mean of the data
        /// </summary>
        public double GMean;

        /// <summary>
        /// Trimmed Mean of the data (default 5%)
        /// </summary>
        public double TMean;

        /// <summary>
        /// Median of the data
        /// </summary>
        public double Median;

        /// <summary>
        /// Median of the data (default unbiased)
        /// </summary>
        public double Variance;

        /// <summary>
        /// Standard Deviation of the data (default unbiased)
        /// </summary>
        public double StdDev;

        /// <summary>
        /// Standard Error of the data (default unbiased)
        /// </summary>
        public double StdError;

        /// <summary>
        /// 1st Quantile of the data
        /// </summary>
        public double Quartile1st;

        /// <summary>
        /// 3rd Quantile of the data
        /// </summary>
        public double Quartile3rd;

        /// <summary>
        /// Skewness of the data
        /// </summary>
        public double Skewness;

        /// <summary>
        /// Kurtosis of the data
        /// </summary>
        public double Kurtosis;

        /// <summary>
        /// 95% Confidence Level of the mean
        /// </summary>
        public double CI95Level;

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent
        /// string representation.
        /// </summary>
        /// <returns>
        /// The string representation of the value of this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string nl = Environment.NewLine;
            sb.AppendFormat("Count    \t= {0}{1}", this.Count, nl);
            sb.AppendFormat("Sum      \t= {0}{1}", this.Sum, nl);
            sb.AppendFormat("Min      \t= {0}{1}", this.Min, nl);
            sb.AppendFormat("Max      \t= {0}{1}", this.Max, nl);
            sb.AppendFormat("Range    \t= {0}{1}", this.Range, nl);
            sb.AppendFormat("AMean    \t= {0}{1}", this.AMean, nl);
            sb.AppendFormat("HMean    \t= {0}{1}", this.HMean, nl);
            sb.AppendFormat("GMean    \t= {0}{1}", this.GMean, nl);
            sb.AppendFormat("5% TMean \t= {0}{1}", this.TMean, nl);
            sb.AppendFormat("Median   \t= {0}{1}", this.Median, nl);
            sb.AppendFormat("Variance \t= {0}{1}", this.Variance, nl);
            sb.AppendFormat("StdDev   \t= {0}{1}", this.StdDev, nl);
            sb.AppendFormat("StdError \t= {0}{1}", this.StdError, nl);
            sb.AppendFormat("1st Quartile \t= {0}{1}", this.Quartile1st, nl);
            sb.AppendFormat("3rd Quartile \t= {0}{1}", this.Quartile3rd, nl);
            sb.AppendFormat("Skewness \t= {0}{1}", this.Skewness, nl);
            sb.AppendFormat("Kurtosis \t= {0}{1}", this.Kurtosis, nl);
            sb.AppendFormat("95% CI   \t= {0}{1}", this.CI95Level, nl);
            return sb.ToString();
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent
        /// comma separated values (csv) representation.
        /// </summary>
        /// <param name="title">If true, adds the title of each Column</param>
        /// <returns>
        /// The csv representation of the value of this instance.
        /// </returns>
        public string ToCSV(bool title)
        {
            StringBuilder sb = new StringBuilder();
            string nl = Environment.NewLine;
            if (title)
            {
                sb.AppendLine(
                    "Count,Sum,Min,Max,Range,AMean,HMean,GMean,5% TMean," + 
                    "Median,Variance,StdDev,StdError,1st Quartile," + 
                    "3rd Quartile,Skewness,Kurtosis,95% CI");
            }

            sb.AppendFormat(
                            "{0},{1},{2},{3},{4},{5},{6},{7},{8},",
                            this.Count, 
                            this.Sum, 
                            this.Min, 
                            this.Max, 
                            this.Range,
                            this.AMean, 
                            this.HMean, 
                            this.GMean, 
                            this.TMean);

            sb.AppendFormat(
                            "{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                            this.Median,
                            this.Variance,
                            this.StdDev, 
                            this.StdError, 
                            this.Quartile1st, 
                            this.Quartile3rd, 
                            this.Skewness, 
                            this.Kurtosis, 
                            this.CI95Level);

            return sb.ToString();
        }
    }   
}
