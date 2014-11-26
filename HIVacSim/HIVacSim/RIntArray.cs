// ----------------------------------------------------------------------------
// <copyright file="RIntArray.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;
    using HIVacSim.Utility;

    /// <summary>
    /// Implements an one-dimension array of integers for random
    /// sampling individuals index.
    /// </summary>
    public class RIntArray
    {
        private int[] _data;
        private int _count;
        private int _index;
        private int _retval;
        private RandomDeviate _rnd;

        #region Constructor

        /// <summary>
        /// Initialise the random array with a given one-dimension array
        /// </summary>
        /// <param name="data">a one-dimension integer array</param>
        /// <param name="rnd"> The random number generator to be used </param>
        public RIntArray(int[] data, RandomDeviate rnd)
        {
            this._data = data;
            this._count = this._data.Length;
            this._rnd = rnd;
        }

        /// <summary>
        /// Initialise the sequential random array 
        /// </summary>
        /// <param name="size">The size of the array</param>
        /// <param name="rnd">The random number generator to be used</param>
        public RIntArray(int size, RandomDeviate rnd)
        {
            this._data = new int[size];
            for (this._index = 0; this._index < size; this._index++)
            {
                this._data[this._index] = this._index;
            }

            this._count = size;
            this._rnd = rnd;
        }
        #endregion

        #region public properties and methods
        /// <summary>
        /// Check if the array is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return this._count <= 0; }
        }

        /// <summary>
        /// Gets the current size of the array
        /// </summary>
        public int Count
        {
            get { return this._count; }
        }

        /// <summary>
        /// Gets a new random value from the array
        /// </summary>
        /// <returns>The random value</returns>
        public int Next()
        {
            if (!IsEmpty)
            {
                //Gets a new random index
                if (this.Count < 2)
                {
                    this._index = 0;
                }
                else
                {
                    this._index = (int)this._rnd.Uniform(0, this._count);
                }

                this._retval = this._data[this._index];

                //Resizes the array
                this._count--;
                this._data[this._index] = this._data[this._count];
                this._data[this._count] = -1;

                //return the random value
                return this._retval;
            }
            else
            {
                return int.MinValue;
            }
        }

        #endregion
    }
}
