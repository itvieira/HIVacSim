// ----------------------------------------------------------------------------
// <copyright file="TriangArray.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Implements a triangular array, which represents a symmetric square 
    /// matrix in memory using only half of the normally required space.
    /// </summary>
    public class TriangArray
    {
        #region Local variables
        private int[] _data;
        private int _rows;
        private int _count;
        private int _index;
        #endregion //Local variables

        /// <summary>
        /// Triangular array constructor
        /// </summary>
        /// <param name="size">
        /// The size of the square matrix to be represented
        /// </param>
        public TriangArray(int size)
        {
            this._rows = size;
            this._count = (this._rows - 1) * (this._rows - 2) / 2 + (this._rows - 1);
            this._data = new int[this._count];
        }

        /// <summary>
        /// Gets or sets the array value
        /// </summary>
        public int this[int row, int col]
        {
            get
            {
                if (col == row)
                {
                    return 0;
                }
                else if (col > row)
                {
                    this._index = row;
                    row = col;
                    col = this._index;
                }

                this._index = row * (row - 1) / 2 + col;
                return this._data[this._index];
            }

            set
            {
                if (col == row)
                {
                    return;
                }
                else if (col > row)
                {
                    this._index = row;
                    row = col;
                    col = this._index;
                }

                this._index = row * (row - 1) / 2 + col;
                this._data[this._index] = value;
            }
        }

        /// <summary>
        /// Transform the array data in a square comma separated values string
        /// </summary>
        /// <returns>The array as a CSV square matrix</returns>
        public override string ToString()
        {
            int row, col;
            StringBuilder sb = new StringBuilder();
            for (row = 0; row < this._rows; row++)
            {
                for (col = 0; col < this._rows; col++)
                {
                    sb.AppendFormat("{0},", this[row, col]);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Writes the array data to comma separated values <see cref="Stream"/>
        /// </summary>
        public void ToCSV(StreamWriter sw)
        {
            int row, col;
            for (row = 0; row < this._rows; row++)
            {
                for (col = 0; col < this._rows; col++)
                {
                    sw.Write(this[row, col] + ",");
                }

                sw.Write(sw.NewLine);
            }
        }
    }
}
