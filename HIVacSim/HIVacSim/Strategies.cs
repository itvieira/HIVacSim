// ----------------------------------------------------------------------------
// <copyright file="Strategies.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;

    /// <summary>
    /// Defines the intervention strategies container class.
    /// </summary>
    [Serializable]
    public class Strategies
    {
        #region Local variables
        private Strategy[] _strategies;	// Collections of intervention strategies
        private int _count;			    // The number of vaccines defined
        private int _selected;		    // Index of the selected vaccine
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Strategies()
        {
            this._count = 0;
            this._selected = -1;
        }
        #endregion

        #region Public properties

        /// <summary>
        /// The number of intervention strategies defined
        /// </summary>
        public int Count
        {
            get { return this._count; }
        }

        /// <summary>
        /// The index of the last selected vaccine
        /// </summary>
        public int Selected
        {
            get { return this._selected; }
        }

        /// <summary>
        /// Get or set an intervention strategy property.
        /// </summary>
        public Strategy this[int index]
        {
            get
            {
                if (index >= 0 && index < this._count)
                {
                    this._selected = index;
                    return this._strategies[index];
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
                    this._strategies[index] = value;
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
        /// Adds a new intervention strategy to the collection
        /// </summary>
        /// <param name="stgdef">The intervention strategy to be added</param>
        /// <returns>The index of the new intervention</returns>
        public int Add(Strategy stgdef)
        {
            int idx;

            // Initialise the container
            if (this._count == 0)
            {
                // Adds a new strategy
                idx = 0;
                this._strategies = new Strategy[1];
                this._strategies[idx] = stgdef;
                this._count = 1;
            }
            else
            {
                // Resize the array and copy the old data
                idx = this._count;
                Strategy[] tmpStg = this._strategies;
                this._strategies = new Strategy[this._count + 1];
                Array.Copy(tmpStg, this._strategies, tmpStg.Length);
                this._strategies[idx] = stgdef;
                this._count++;
            }

            //Update vaccine used
            this._strategies[idx].UseVaccine.UsedBy.Add(this._strategies[idx].Id);
            return this._count - 1;
        } 

        /// <summary>
        /// Removes an existing intervention strategy
        /// </summary>
        /// <param name="index">The index of the strategy to be removed</param>
        public void Remove(int index)
        {
            if (index >= 0 && index < this._count)
            {
                //Update vaccine used
                this._strategies[index].UseVaccine.UsedBy.RemoveId(this._strategies[index].Id);
                this._count--;

                //Check if population container is empty
                if (this._count == 0)
                {
                    this._strategies = null;
                    this._count = 0;
                    this._selected = -1;
                    Strategy.Reset();
                }
                else
                {
                    // Resize the array and copy the old data
                    Strategy[] tmpStg = this._strategies;
                    this._strategies = new Strategy[this._count];
                    for (int i = 0; i < this._count; i++)
                    {
                        if (i < index)
                        {
                            this._strategies[i] = tmpStg[i];
                        }
                        else
                        {
                            this._strategies[i] = tmpStg[i + 1];
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    "index < 0 or index > Count.",
                    "Invalid index argument on Strategies.RemoveStrategy.");
            }

            //Check selected index range
            if (this._selected >= this._count)
            {
                this._selected = this._count - 1;
            }
        }
        
        /// <summary>
        /// Removes an existing intervention strategy
        /// </summary>
        /// <param name="stgid">The intervention strategy to be removed</param>
        public void Remove(Strategy stgid)
        {
            int index = IndexOf(stgid);
            if (index >= 0)
            {
                this.Remove(index);
            }
        }
        
        /// <summary>
        /// Finds the index of an existing intervention strategy
        /// </summary>
        /// <param name="stgid">The strategy to be found</param>
        public int IndexOf(Strategy stgid)
        {
            if (this._count > 0)
            {
                for (int i = 0; i < this._count; i++)
                {
                    if (this._strategies[i] == stgid)
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
                    "Invalid intervention strategy, index < 0 or index > Count");
            }
        }
        
        /// <summary>
        /// Clears the intervention strategy container
        /// </summary>
        public void Clear()
        {
            this._strategies = null;
            this._count = 0;
            this._selected = -1;
            Strategy.Reset();
        }

        #endregion //Public methods
    }
}
