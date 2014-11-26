// ----------------------------------------------------------------------------
// <copyright file="RPairArray.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;
    using System.Collections;
    using HIVacSim.Utility;

    /// <summary>
    /// Implements a structure to hold a pair of persons within RPairArray
    /// </summary>
    public struct RPair
    {
        /// <summary>
        /// The first person's index
        /// </summary>
        public int Source;

        /// <summary>
        /// The second person’s index
        /// </summary>
        public int Target;

        /// <summary>
        /// Optional constructor
        /// </summary>
        /// <param name="source">The first index</param>
        /// <param name="target">The second index</param>
        public RPair(int source, int target)
        {
            this.Source = source;
            this.Target = target;
        }

        /// <summary>
        /// Invalidate the pair of index (set both indexes to int.MinValue)
        /// </summary>
        public void Invalidate()
        {
            this.Source = int.MinValue;
            this.Target = int.MinValue;
        }

        /// <summary>
        /// Validate the pair of index
        /// </summary>
        /// <returns>True if the pair is valid, false otherwise</returns>
        public bool IsValid()
        {
            return (this.Source != int.MinValue && this.Target != int.MinValue);
        }

        /// <summary>
        /// Create a simple hash-code to be used with an one-dimension array of size n [0,n-1]
        /// </summary>
        /// <param name="n">The length of the one-dimension array</param>
        /// <returns>
        /// An integer number representing the hash-code of the pair, the index of the array.
        /// </returns>
        public int HashIdx(int n)
        {
            string x;
            if (this.Source > this.Target)
            {
                x = this.Target.ToString() + this.Source.ToString();
            }
            else
            {
                x = this.Source.ToString() + this.Target.ToString();
            }

            return int.Parse(x) % n;
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation
        /// </summary>
        /// <returns>The formatted string representation of this instance</returns>
        public override string ToString()
        {
            return this.Source.ToString() + "," + this.Target.ToString();
        }
    }

    /// <summary>
    /// Holds an one-dimension array of RPair for random sampling
    /// </summary>
    public class RPairArray
    {
        #region Local variables

        private int[] _data;
        private BitArray _used;
        private int _count;
        private int _size;
        private int _index;
        private RandomDeviate _rnd;

        #endregion

        #region Constructor
        /// <summary>
        /// Initialise the random array with a given one-dimension array
        /// </summary>
        /// <param name="data">The one-dimension integer array</param>
        /// <param name="size">The size of the sample to be taken</param>
        /// <param name="rnd">The random number to be used</param>
        public RPairArray(int[] data, int size, RandomDeviate rnd)
        {
            this._count = this._data.Length;
            this._size = size;
            RPairArray.ValidateSample(this._count, this._size); // Validate size
            this._rnd = rnd;
            this._used = new BitArray(RPairArray.MaximumPairs(this._count));
            this._data = data;
        }

        /// <summary>
        /// Initialise the sequential random array 
        /// </summary>
        /// <param name="count">The number of items in the array (population size)</param>
        /// <param name="size">The size of the sample to be taken</param>
        /// <param name="rnd">The random number to be used</param>
        public RPairArray(int count, int size, RandomDeviate rnd)
        {
            //Validates input
            RPairArray.ValidateSample(count, size);
            this._count = count;
            this._size = size;
            this._rnd = rnd;

            //Creates the temporary hash table
            this._used = new BitArray(RPairArray.MaximumPairs(this._count));

            //Create temporary data arrays
            this._data = new int[this._count];
            for (this._index = 0; this._index < this._count; this._index++)
            {
                this._data[this._index] = this._index;
            }
        }

        #endregion

        #region Public properties and methods
        /// <summary>
        /// Check if the array is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return this._count <= 0; }
        }

        /// <summary>
        /// Gets the surrent size of the array
        /// </summary>
        public int Count
        {
            get { return this._count; }
        }

        /// <summary>
        /// Gets the ramaining number of random samples (pair) for this instance 
        /// </summary>
        public int Remaining
        {
            get { return this._size; }
        }

        /// <summary>
        /// Gets a new random pair of indexes
        /// </summary>
        /// <returns>The random value</returns>
        public RPair Next()
        {
            RPair pair = new RPair();
            if (!IsEmpty && this._size > 0)
            {
                //Gets a random index
                do
                {
                    //Validate the sampled pair
                    do
                    {
                        pair.Source = (int)this._rnd.Uniform(0, this._count);
                        pair.Target = (int)this._rnd.Uniform(0, this._count);
                    }
                    while (pair.Source == pair.Target);

                    //Gets the array index
                    this._index = pair.HashIdx(this._used.Length);

                }
                while (this._used.Get(this._index));

                //A new pair has been generated, update hashtable
                this._used.Set(this._index, true);
                this._size--;

                //return the random pair
                return pair;
            }
            else
            {
                pair.Invalidate();
                return pair;
            }
        }

        #endregion

        #region Static Methods
        /// <summary>
        /// Validate the size of the sample
        /// </summary>
        /// <exception cref="SimulationException"> is throw if an invalid 
        /// population or sample size is found.</exception>
        /// <param name="n">The size of the dataset</param>
        /// <param name="size">
        /// The number of pair to be taken from the data
        /// </param>
        private static void ValidateSample(int n, int size)
        {
            if ((n <= 2) || (size > RPairArray.MaximumSamples(n)))
            {
                throw new SimulationException(
                    "Invalid sample size [size > N(N-1)/4] or dataset size <= 2.");
            }
        }
        
        /// <summary>
        /// Calculates the maximum number of random pairs, given a population size n
        /// </summary>
        /// <param name="n">The population size</param>
        /// <returns>
        /// The maximum number of random samples (pair) for a population of size n
        /// </returns>
        public static int MaximumSamples(int n)
        {
            return n * (n - 1) / 4;
        }

        /// <summary>
        /// Calculates the maximum number of random pairs, given a population size n
        /// </summary>
        /// <returns>The maximum number of pairs for a population</returns>
        public static int MaximumPairs(int n)
        {
            return n * (n - 1) / 2;
        }

        #endregion
    }
}
