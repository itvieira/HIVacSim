// ----------------------------------------------------------------------------
// <copyright file="DataFunction.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim.Utility
{
    using System;

    /// <summary>
    /// Descriptive statistics functions definition.
    /// </summary>
    public sealed class DataFunction
    {
        /// <summary>
        /// Count the number of elements in an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>The number of data items</returns>
        public static double Count(double[] data)
        {
            return data.Length;
        }
        
        /// <summary>
        /// Sum of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>The sum of the data</returns>
        public static double Sum(double[] data)
        {
            int idx;
            double vsum = 0.0;
            for (idx = 0; idx < data.Length; idx++)
            {
                vsum += data[idx];
            }

            return vsum;
        }
        
        /// <summary>
        /// Minimum of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>The minimum of the data</returns>
        public static double Min(double[] data)
        {
            int idx;
            double vmin = double.MaxValue;
            for (idx = 0; idx < data.Length; idx++)
            {
                if (data[idx] < vmin)
                {
                    vmin = data[idx];
                }
            }

            return vmin;
        }
        
        /// <summary>
        /// Maximum of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>The maximum of the data</returns>
        public static double Max(double[] data)
        {
            int idx;
            double vmax = double.MinValue;
            for (idx = 0; idx < data.Length; idx++)
            {
                if (data[idx] > vmax)
                {
                    vmax = data[idx];
                }
            }

            return vmax;
        }

        /// <summary>
        /// Average or arithmetic mean of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>The arithmetic mean of the data</returns>
        public static double AMean(double[] data)
        {
            return Sum(data) / data.Length;
        }
        
        /// <summary>
        /// Harmonic mean of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>The arithmetic mean of the data</returns>
        public static double HMean(double[] data)
        {
            int idx;
            double sumi = 0.0;
            for (idx = 0; idx < data.Length; idx++)
            {
                sumi += 1.0 / data[idx];
            }

            return data.Length / sumi;
        }
        
        /// <summary>
        /// Geometric mean of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>The arithmetic mean of the data</returns>
        public static double GMean(double[] data)
        {
            int idx;
            double vsum = 1.0;

            // Validate data
            if (data.Length == 0)
            {
                throw new ArgumentException(
                    "The geometric mean requires at least one value in the data.");
            }

            for (idx = 0; idx < data.Length; idx++)
            {
                vsum *= data[idx];
            }

            return Math.Pow(vsum, 1.0 / data.Length);
        }
        
        /// <summary>
        /// Trimmed mean of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <param name="trim">The trimming value</param>
        /// <returns>The trimmed mean of the data</returns>
        public static double TMean(double[] data, double trim)
        {
            int split, idx, count;
            double vsum;

            split = (int)Math.Floor((double)(0.05 * (data.Length / 2.0)));
            if (split == 0)
            {
                return AMean(data);
            }
            else if (split == data.Length)
            {
                return Median(data);
            }

            QSort(data, 0, data.Length - 1, new RandomDeviate());
            vsum = 0.0;
            count = 0;
            for (idx = split; idx < (data.Length - split); idx++)
            {
                vsum += data[idx];
                count++;
            }

            return vsum / (double)count;
        }
        
        /// <summary>
        /// Median of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>The median of the data</returns>
        public static double Median(double[] data)
        {
            int count = data.Length;

            // Validates the data
            if (count < 2)
            {
                return 1.0;
            }

            // Sorts the data
            QSort(data, 0, count - 1, new RandomDeviate());

            // Calculates the median
            if (count % 2 != 0)
            {
                // Count is Odd
                return data[(count - 1) / 2];
            }
            else
            {
                count = count / 2;
                return (data[count - 1] + data[count]) / 2.0;
            }
        }
        
        /// <summary>
        /// Variance of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>
        /// The unbiased variance of the data (normalized by n-1)
        /// </returns>
        public static double Variance(double[] data)
        {
            return Variance(data, false);
        }
        
        /// <summary>
        /// Variance of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <param name="biased">
        /// If true, normalizes by n, otherwise normalizes by n-1.
        /// </param>
        /// <returns>The variance of the data</returns>
        public static double Variance(double[] data, bool biased)
        {
            int idx;
            double mean, sumSq;

            // Validate the number of data points
            if (data.Length < 2)
            {
                return 0.0;
            }

            // Calculates the mean
            mean = AMean(data);

            // Calculates the sum of the squares
            sumSq = 0.0;
            for (idx = 0; idx < data.Length; idx++)
            {
                sumSq += Math.Pow(data[idx] - mean, 2);
            }

            // Evaluates the type of variance to return
            if (biased)
            {
                return sumSq / data.Length;
            }
            else
            {
                return sumSq / (data.Length - 1);
            }
        }
        
        /// <summary>
        /// Standard deviation of an array of double 
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>
        /// The unbiased standard deviation of the data (normalized by n-1)
        /// </returns>
        public static double StdDev(double[] data)
        {
            return Math.Sqrt(Variance(data, false));
        }
        
        /// <summary>
        /// Standard deviation of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <param name="biased">
        /// If true, normalizes by n, otherwise normalizes by n-1.
        /// </param>
        /// <returns>The standard deviation of the data</returns>
        public static double StdDev(double[] data, bool biased)
        {
            return Math.Sqrt(Variance(data, biased));
        }
        
        /// <summary>
        /// Standard error of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>
        /// The unbiased standard error of the data (normalized by n-1)
        /// </returns>
        public static double StdError(double[] data)
        {
            return StdError(data, false);
        }

        /// <summary>
        /// Standard error of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <param name="biased">
        /// If true, normalizes by n, otherwise normalizes by n-1.
        /// </param>
        /// <returns>The standard error of the data</returns>
        public static double StdError(double[] data, bool biased)
        {
            // Validate data
            if (data.Length == 0)
            {
                throw new ArgumentException(
                    "The standard error requires at least one value in the data.");
            }

            return StdDev(data, biased) / Math.Sqrt(data.Length);
        }
        
        /// <summary>
        /// Quartile of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <param name="qtile">The i-th quartile to be evaluated [0-4]</param>
        /// <returns>The i-th quartile of the data</returns>
        public static double Quartile(double[] data, int qtile)
        {
            double split;
            int idx1, idx2;

            if ((qtile < 0) || (qtile > 4))
            {
                throw new ArgumentException(
                    "Quartile must be between zero and four.");
            }

            if (qtile == 0)
            {
                return Min(data);
            }

            if (qtile == 4)
            {
                return Max(data);
            }

            QSort(data, 0, data.Length - 1, new RandomDeviate());
            split = ((double)qtile / 4.0) * (double)(data.Length - 1);
            idx1 = (int)Math.Floor(split);
            idx2 = (int)Math.Ceiling(split);
            if (idx1 == idx2)
            {
                return data[idx1];
            }

            return data[idx1] + ((split - (double)idx1) * (data[idx2] - data[idx1]));
        }

        /// <summary>
        /// Percentile of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <param name="percent">
        /// The i-th percentile to be evaluated [0-1]
        /// </param>
        /// <returns>The i-th percentile of the data</returns>
        public static double Percentile(double[] data, double percent)
        {
            double split;
            int idx1, idx2;

            if ((percent < 0.0) || (percent > 1.0))
            {
                throw new ArgumentException(
                    "Percentile must be between zero and one.");
            }

            if (percent == 0.0)
            {
                return Min(data);
            }

            if (percent == 1.0)
            {
                return Max(data);
            }

            QSort(data, 0, data.Length - 1, new RandomDeviate());
            split = percent * (double)(data.Length - 1);
            idx1 = (int)Math.Floor(split);
            idx2 = (int)Math.Ceiling(split);
            if (idx1 == idx2)
            {
                return data[idx1];
            }

            return data[idx1] + ((split - (double)idx1) * (data[idx2] - data[idx1]));
        }

        /// <summary>
        /// The range of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>The range of the data</returns>
        public static double Range(double[] data)
        {
            return Max(data) - Min(data);
        }
        
        /// <summary>
        /// Skewness of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>
        /// The unbiased skewness of the data (normalized by n-1)
        /// </returns>
        public static double Skewness(double[] data)
        {
            return Skewness(data, false);
        }
        
        /// <summary>
        /// Skewness of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <param name="biased">
        /// If true, normalizes by n, otherwise normalizes by n-1.
        /// </param>
        /// <returns>The skewness of the data</returns>
        public static double Skewness(double[] data, bool biased)
        {
            int idx;
            double vtmp, vsum, vmean, vsumSq, vsumP3, len;

            if (data.Length < 3)
            {
                throw new ArgumentException(
                    "Skewness requires at least three elements.");
            }

            len = (double)data.Length;
            vsum = 0.0;
            for (idx = 0; idx < data.Length; idx++)
            {
                vsum += data[idx];
            }

            vmean = vsum / len;
            vsumSq = 0.0;
            vsumP3 = 0.0;
            for (idx = 0; idx < data.Length; idx++)
            {
                vtmp = data[idx] - vmean;
                vsumSq += vtmp * vtmp;
                vsumP3 += Math.Pow(vtmp, 3.0);
            }

            if (biased)
            {
                vtmp = vsumSq / len;
            }
            else
            {
                vtmp = vsumSq / (len - 1.0);
            }

            vtmp = vsumP3 / Math.Pow(Math.Sqrt(vtmp), 3.0);
            if (biased)
            {
                vtmp = vtmp / len;
            }
            else
            {
                vtmp = (len / ((len - 1.0) * (len - 2.0))) * vtmp;
            }

            return vtmp;
        }

        /// <summary>
        /// Skewness of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>
        /// The unbiased kurtosis of the data (normalized by n-1)
        /// </returns>
        public static double Kurtosis(double[] data)
        {
            return Kurtosis(data, false);
        }
        
        /// <summary>
        /// Skewness of an array of double
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <param name="biased">
        /// If true, normalizes by n, otherwise normalizes by n-1.
        /// </param>
        /// <returns>The kurtosis of the data</returns>
        public static double Kurtosis(double[] data, bool biased)
        {
            int idx;
            double vtmp, vmean, vsum, vsumSq, vsumP4, result;
            double len, lenSq, lenP3, lenP4;

            if (data.Length < 4)
            {
                throw new ArgumentException(
                    "Kurtosis requires at least four elements.");
            }

            len = (double)data.Length;
            vsum = 0.0;
            for (idx = 0; idx < data.Length; idx++)
            {
                vsum += data[idx];
            }

            vmean = vsum / len;
            vsumSq = 0.0;
            vsumP4 = 0.0;
            for (idx = 0; idx < data.Length; idx++)
            {
                vtmp = data[idx] - vmean;
                vsumSq += vtmp * vtmp;
                vsumP4 += Math.Pow(vtmp, 4.0);
            }

            if (biased)
            {
                vtmp = vsumSq / len;
            }
            else
            {
                vtmp = vsumSq / (len - 1.0);
            }

            vtmp = vsumP4 / (vtmp * vtmp);
            if (biased)
            {
                result = (vtmp / len) - 3.0;
            }
            else
            {
                lenSq = len * (len + 1.0);
                lenP3 = ((len - 1.0) * (len - 2.0)) * (len - 3.0);
                lenP4 = ((3.0 * (len - 1.0)) * (len - 1.0)) / ((len - 2.0) * (len - 3.0));
                result = ((lenSq / lenP3) * vtmp) - lenP4;
            }

            return result;
        }

        /// <summary>
        /// 95% Confidence interval for mean
        /// </summary>
        /// <param name="data">The data array to be used</param>
        /// <returns>The 95% confidence level (+-) of the data</returns>
        public static double Confidence(double[] data)
        {
            return 1.96 * StdError(data);
        }

        /// <summary>
        /// Descriptive Statistics
        /// </summary>
        /// <param name="data">The data to be used</param>
        /// <returns>
        /// The descriptive statistics of the data (unbiased)
        /// </returns>
        public static DataSummary Descriptive(double[] data)
        {
            return Descriptive(data, false);
        }

        /// <summary>
        /// Descriptives the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="biased">if set to <c>true</c> [biased].</param>
        /// <returns>
        /// The fully populated <see cref="DataSummary"/> summary
        /// </returns>
        public static DataSummary Descriptive(double[] data, bool biased)
        {
            int idx, tidx, tcount, split;
            double vtmp, vsum, vsumi, vsumg, vmin, vmax, vsumSq, vsumP3;
            double vsumP4, len, lenSq, lenP3, lenP4;
            DataSummary stat = new DataSummary();

            // Count
            stat.Count = data.Length;

            // Sum, Min, Max, AMean, HMean, GMean, Range
            vsum = 0.0;
            vsumi = 0.0;
            vsumg = 1.0;
            vmin = double.MaxValue;
            vmax = double.MinValue;
            for (idx = 0; idx < stat.Count; idx++)
            {
                vsum += data[idx];
                vsumi += 1.0 / data[idx];
                vsumg *= data[idx];
                if (data[idx] < vmin)
                {
                    vmin = data[idx];
                }

                if (data[idx] > vmax)
                {
                    vmax = data[idx];
                }
            }

            stat.Sum = vsum;
            stat.Min = vmin;
            stat.Max = vmax;
            stat.Range = stat.Max - stat.Min;
            stat.AMean = vsum / stat.Count;
            stat.HMean = stat.Count / vsumi;
            stat.GMean = Math.Pow(vsumg, 1.0 / stat.Count);

            // Sum of the Squares, Variance, StdDev, StdError, 95% CI Level
            vsumSq = 0.0;
            for (idx = 0; idx < stat.Count; idx++)
            {
                vsumSq += Math.Pow(data[idx] - stat.AMean, 2.0);
            }

            if (biased)
            {
                stat.Variance = vsumSq / stat.Count;
            }
            else
            {
                stat.Variance = vsumSq / (stat.Count - 1);
            }

            stat.StdDev = Math.Sqrt(stat.Variance);
            stat.StdError = stat.StdDev / Math.Sqrt(stat.Count);
            stat.CI95Level = 1.96 * stat.StdError;

            // Sorts the data
            QSort(data, 0, data.Length - 1, new RandomDeviate());

            // Median
            if (stat.Count < 2)
            {
                stat.Median = 1.0;
            }
            else
            {
                // Calculates the median
                if (stat.Count % 2 != 0)
                {
                    // Count is Odd
                    stat.Median = data[(stat.Count - 1) / 2];
                }
                else
                {
                    split = stat.Count / 2;
                    stat.Median = (data[split - 1] + data[split]) / 2.0;
                }
            }

            // Trimmed Mean
            split = (int)Math.Floor((double)(0.05 * (stat.Count / 2.0)));
            if (split == 0)
            {
                stat.TMean = stat.AMean;
            }
            else if (split == stat.Count)
            {
                stat.TMean = stat.Median;
            }
            else
            {
                vsum = 0.0;
                tcount = 0;
                for (idx = split; idx < (stat.Count - split); idx++)
                {
                    vsum += data[idx];
                    tcount++;
                }

                stat.TMean = vsum / (double)tcount;
            }

            // 1st and 3rd Quartiles
            vtmp = (1.0 / 4.0) * (double)(stat.Count - 1);
            idx = (int)Math.Floor(vtmp);
            tidx = (int)Math.Ceiling(vtmp);
            if (idx == tidx)
            {
                stat.Quartile1st = data[idx];
            }
            else
            {
                stat.Quartile1st = data[idx] + ((vtmp - (double)idx) * (data[tidx] - data[idx]));
            }

            vtmp = (3.0 / 4.0) * (double)(stat.Count - 1);
            idx = (int)Math.Floor(vtmp);
            tidx = (int)Math.Ceiling(vtmp);
            if (idx == tidx)
            {
                stat.Quartile3rd = data[idx];
            }
            else
            {
                stat.Quartile3rd = data[idx] + ((vtmp - (double)idx) * (data[tidx] - data[idx]));
            }

            // Temporary values for Skewness and Kurtosis calculation
            len = (double)data.Length;
            vsumSq = 0.0;
            vsumP3 = 0.0;
            vsumP4 = 0.0;

            // Sum of Squares and Pow of 3 and 4 
            for (idx = 0; idx < data.Length; idx++)
            {
                vtmp = data[idx] - stat.AMean;
                vsumSq += vtmp * vtmp;
                vsumP3 += Math.Pow(vtmp, 3.0); // Skewness
                vsumP4 += Math.Pow(vtmp, 4.0); // Kurtosis
            }

            // Skewness
            if (stat.Count < 3)
            {
                stat.Skewness = double.NaN;
            }
            else
            {
                if (biased)
                {
                    vtmp = vsumSq / len;
                }
                else
                {
                    vtmp = vsumSq / (len - 1.0);
                }

                vtmp = vsumP3 / Math.Pow(Math.Sqrt(vtmp), 3.0);
                if (biased)
                {
                    vtmp = vtmp / len;
                }
                else
                {
                    vtmp = (len / ((len - 1.0) * (len - 2.0))) * vtmp;
                }

                stat.Skewness = vtmp;
            }

            // Kurtosis
            if (stat.Count < 4)
            {
                stat.Kurtosis = double.NaN;
            }
            else
            {
                if (biased)
                {
                    vtmp = vsumSq / len;
                }
                else
                {
                    vtmp = vsumSq / (len - 1.0);
                }

                vtmp = vsumP4 / (vtmp * vtmp);
                if (biased)
                {
                    stat.Kurtosis = (vtmp / len) - 3.0;
                }
                else
                {
                    lenSq = len * (len + 1.0);
                    lenP3 = ((len - 1.0) * (len - 2.0)) * (len - 3.0);
                    lenP4 = ((3.0 * (len - 1.0)) * (len - 1.0)) / ((len - 2.0) * (len - 3.0));
                    stat.Kurtosis = ((lenSq / lenP3) * vtmp) - lenP4;
                }
            }

            return stat;
        }
        
        /// <summary>
        /// Quicksort algorithm for one-dimension array of double
        /// </summary>
        /// <param name="data">The array of double</param>
        /// <param name="min">The lower dimension</param>
        /// <param name="max">The upper dimension</param>
        /// <param name="rnd">The random number generator to be used</param>
        public static void QSort(double[] data, int min, int max, Random rnd)
        {
            int hi, lo, idx = 0;
            double med_value;

            // If the list has no more than 1 element, it's sorted.
            if (min >= max)
            {
                return;
            }

            // Pick a dividing item.
            idx = (int)(((max - min + 1) * rnd.NextDouble()) + min);
            med_value = data[idx];

            // Swap it to the front so we can find it easily.
            data[idx] = data[min];

            /* Move the items smaller than this into the left
             * half of the list. Move the others into the right.*/
            lo = min;
            hi = max;
            while (true)
            {
                // Look down from hi for a value < med_value.
                while (data[hi] >= med_value)
                {
                    hi = hi - 1;
                    if (hi <= lo)
                    {
                        break;
                    }
                }

                if (hi <= lo)
                {
                    data[lo] = med_value;
                    break;
                }

                // Swap the lo and hi values.
                data[lo] = data[hi];

                // Look up from lo for a value >= med_value.
                lo = lo + 1;
                while (data[lo] < med_value)
                {
                    lo = lo + 1;
                    if (lo >= hi)
                    {
                        break;
                    }
                }

                if (lo >= hi)
                {
                    lo = hi;
                    data[hi] = med_value;
                    break;
                }

                // Swap the lo and hi values.
                data[hi] = data[lo];
            }

            // Sort the two sub-lists
            QSort(data, min, lo - 1, rnd);
            QSort(data, lo + 1, max, rnd);
        }
    }
}
