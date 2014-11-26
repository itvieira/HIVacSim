// ----------------------------------------------------------------------------
// <copyright file="ListOfIds.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;

    /// <summary>
    /// Implements a dynamic list of integers (unique ids) class.
    /// </summary>
    public class ListOfIds
    {
        #region Local variables
        private int[] _data;	//Array of data item
        private int _count;	//The number of items in the list
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ListOfIds()
        {
            this._count = 0;
        }
        #endregion

        #region Public properties

        /// <summary>
        /// The number of elements in the list
        /// </summary>
        public int Count
        {
            get { return this._count; }
        }

        /// <summary>
        /// Verifies if the container is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return this._count == 0; }
        }

        /// <summary>
        /// Get or set an element's property.
        /// </summary>
        public int this[int index]
        {
            get
            {
                if (index >= 0 && index < this._count)
                {
                    return this._data[index];
                }
                else
                {
                    throw new ArgumentOutOfRangeException(
                                                "index < 0 or index > Count",
                                                "Invalid index argument.");
                }
            }
            set
            {
                if (index >= 0 && index < this._count)
                {
                    this._data[index] = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(
                                                "index < 0 or index > Count",
                                                "Invalid index argument.");
                }
            }
        }

        #endregion //Properties

        #region Public Methods

        /// <summary>
        /// Adds a new element to list
        /// </summary>
        /// <param name="item">The element to be added</param>
        /// <returns>The index of the new element</returns>
        public int Add(int item)
        {
            //Initialise the container
            if (this._count == 0)
            {
                //Adds a new element
                this._data = new int[1];
                this._data[0] = item;
                this._count = 1;
            }
            else
            {
                //Avoids duplicated
                if (this.IndexOf(item) != -1)
                {
                    return -1;
                }

                // Resize the array and copy the old data
                int[] tmpdat = this._data;
                this._data = new int[this._count + 1];
                Array.Copy(tmpdat, this._data, tmpdat.Length);
                this._data[this._count] = item;
                this._count++;

            }

            return this._count - 1;
        }

        /// <summary>
        /// Adds new elements from a comma separated string
        /// </summary>
        /// <param name="csv">The string (csv) of elements to be added</param>
        /// <returns>The number of new elements added to the list</returns>
        public int Add(string csv)
        {
            string[] tmp;
            int i, c = 0;
            if (csv.Length <= 0)
            {
                return -1;
            }

            tmp = csv.Split((char)',');
            for (i = 0; i < tmp.Length; i++)
            {
                if (this.Add(int.Parse(tmp[i])) != -1)
                {
                    c++;
                }
            }

            return c;
        }

        /// <summary>
        /// Removes an existing element
        /// </summary>
        /// <param name="index">The index of the element to be removed</param>
        public void Remove(int index)
        {
            if (index >= 0 && index < this._count)
            {
                this._count--;

                //Check if population container is empty
                if (this._count == 0)
                {
                    this._data = null;
                }
                else
                {
                    // Resize the array and copy the old data
                    int[] tmpdat = this._data;
                    this._data = new int[this._count];
                    for (int i = 0; i < this._count; i++)
                    {
                        if (i < index)
                        {
                            this._data[i] = tmpdat[i];
                        }
                        else
                        {
                            this._data[i] = tmpdat[i + 1];
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    "index < 0 or index > Count.",
                    "Invalid index argument on ListOfIds.Remove.");
            }
        }

        /// <summary>
        /// Removes an existing element by value
        /// </summary>
        /// <param name="objid">The element to be removed</param>
        public void RemoveId(int objid)
        {
            int index = IndexOf(objid);
            if (index >= 0)
            {
                this.Remove(index);
            }
        }

        /// <summary>
        /// Finds the index of an existing element
        /// </summary>
        /// <param name="objid">The element to be found</param>
        public int IndexOf(int objid)
        {
            if (this._count > 0)
            {
                for (int i = 0; i < this._count; i++)
                {
                    if (this._data[i] == objid)
                    {
                        return i;
                    }
                }

                return -1;
            }
            else
            {
                throw new ArgumentException(
                            "The population container is empty.",
                            "Invalid element, index < 0 or index > Count");
            }
        }

        /// <summary>
        /// Clears container
        /// </summary>
        public void Clear()
        {
            this._data = null;
            this._count = 0;
            System.GC.Collect();
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent 
        /// string representation.
        /// </summary>
        /// <returns>A string representation of the list</returns>
        public override string ToString()
        {
            string csv = string.Empty;
            if (!this.IsEmpty)
            {
                for (int i = 0; i < this._count; i++)
                {
                    csv += this._data[i] + ",";
                }

                csv = csv.Remove(csv.Length - 1, 1);
            }

            return csv;
        }

        #endregion //Public methods
    }
}
