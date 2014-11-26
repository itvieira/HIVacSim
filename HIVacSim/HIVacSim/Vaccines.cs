// ----------------------------------------------------------------------------
// <copyright file="Vaccines.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Defines a container to hold the preventive vaccines.
    /// </summary>
    public class Vaccines
    {
        #region Local variables
        private Vaccine[] _vaccines;    // Collections of vaccines
        private int _count;			    // The number of vaccines defined
        private int _selected;		    // Index of the selected vaccine
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Vaccines()
        {
            this._count = 0;
            this._selected = -1;
        }
        #endregion

        #region Public properties

        /// <summary>
        /// The number of vaccines defined
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
        /// Get or set a vaccine property.
        /// </summary>
        public Vaccine this[int index]
        {
            get
            {
                if (index >= 0 && index < this._count)
                {
                    this._selected = index;
                    return this._vaccines[index];
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
                    this._vaccines[index] = value;
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
        /// Adds a new vaccine to the collection
        /// </summary>
        /// <param name="vacdef">The vaccine to be added</param>
        /// <returns>The index of the new vaccine</returns>
        public int Add(Vaccine vacdef)
        {
            //Initialise the container
            if (this._count == 0)
            {
                //Adds a new vaccine
                this._vaccines = new Vaccine[1];
                this._vaccines[0] = vacdef;
                this._count = 1;
            }
            else
            {
                // Resize the array and copy the old data
                Vaccine[] tmpVac = this._vaccines;
                this._vaccines = new Vaccine[this._count + 1];
                Array.Copy(tmpVac, this._vaccines, tmpVac.Length);
                this._vaccines[this._count] = vacdef;
                this._count++;
            }

            return this._count - 1;
        }

        /// <summary>
        /// Removes an existing vaccine
        /// </summary>
        /// <param name="index">The index of the vaccine to be removed</param>
        public void Remove(int index)
        {
            if (index >= 0 && index < this._count)
            {
                //Data integrity check
                if (this._vaccines[index].UsedBy.Count > 0)
                {
                    throw new SimulationException(
                        "Vaccine [" + this._vaccines[index].Name +
                        "] is currently in use by one or more intervention strategies.");
                }

                this._count--;

                //Check if population container is empty
                if (this._count == 0)
                {
                    this._vaccines = null;
                    this._count = 0;
                    this._selected = -1;
                    Vaccine.Reset();
                }
                else
                {
                    // Resize the array and copy the old data
                    Vaccine[] tmpVac = this._vaccines;
                    this._vaccines = new Vaccine[this._count];
                    for (int i = 0; i < this._count; i++)
                    {
                        if (i < index)
                        {
                            this._vaccines[i] = tmpVac[i];
                        }
                        else
                        {
                            this._vaccines[i] = tmpVac[i + 1];
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    "index < 0 or index > Count.",
                    "Invalid index argument on Vaccines.RemoveVaccine.");
            }

            //Check selected index range
            if (this._selected >= this._count)
            {
                this._selected = this._count - 1;
            }
        }

        /// <summary>
        /// Removes an existing vaccine
        /// </summary>
        /// <param name="vacid">The vaccine to be removed</param>
        public void Remove(Vaccine vacid)
        {
            int index = IndexOf(vacid);
            if (index >= 0)
            {
                this.Remove(index);
            }
        }

        /// <summary>
        /// Finds the index of an existing vaccine
        /// </summary>
        /// <param name="vacid">The vaccine to be found</param>
        public int IndexOf(Vaccine vacid)
        {
            if (this._count > 0)
            {
                for (int i = 0; i < this._count; i++)
                {
                    if (this._vaccines[i] == vacid)
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
                                "Invalid Vaccine, index < 0 or index > Count");
            }
        }

        /// <summary>
        /// Finds the index of an existing vaccine
        /// </summary>
        /// <param name="vacid">The ID of the vaccine to be found</param>
        public int IndexOf(int vacid)
        {
            if (this._count > 0)
            {
                for (int i = 0; i < this._count; i++)
                {
                    if (this._vaccines[i].Id == vacid)
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
                                "Invalid Vaccine, index < 0 or index > Count");
            }
        }

        /// <summary>
        /// Clear the vaccine container
        /// </summary>
        public void Clear()
        {
            this._vaccines = null;
            this._count = 0;
            this._selected = -1;
            Vaccine.Reset();
        }

        #endregion //Public methods
    }
}
