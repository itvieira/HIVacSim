// ----------------------------------------------------------------------------
// <copyright file="RPrsArray.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;
    using HIVacSim.Utility;

    /// <summary>
    /// Implements an one-dimension array of persons for random sampling
    /// </summary>
    public class RPrsArray
    {
        private Person[] _data;
        private int _count;
        private int _index;
        private Person _prs;
        private RandomDeviate _rnd;

        #region Constructor
        /// <summary>
        /// Initialise the random array with a given one-dimension array
        /// </summary>
        /// <param name="data">a one-dimension integer array</param>
        /// <param name="rnd"> the random number generator to be used</param>
        public RPrsArray(Person[] data, RandomDeviate rnd)
        {
            this._data = data;
            this._count = this._data.Length;
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
        /// Gets a new random person from the array
        /// </summary>
        /// <returns>The random value</returns>
        public Person Next()
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

                this._prs = this._data[this._index];

                //Resizes the array
                this._count--;
                this._data[this._index] = this._data[this._count];
                this._data[this._count] = null;

                //return the random value
                return this._prs;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
