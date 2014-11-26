// ----------------------------------------------------------------------------
// <copyright file="Population.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;

    /// <summary>
    /// Defines a population container to hold the groups as a 
    /// weighted and directed graph.
    /// </summary>
    public class Population
    {
        #region Local variables
        private Group[] _groups;		//Population groups
        private double[,] _adjMatrix;	//Digraph adjacency matrix
        private int _count;			    //The number of groups define within the population
        private int _selected;		    //Index of the selected group
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Population()
        {
            this._count = -1;
            this._selected = -1;
        }
        #endregion

        #region Public properties

        /// <summary>
        /// The number of population groups defined
        /// </summary>
        public int Count
        {
            get { return this._count; }
        }

        /// <summary>
        /// The index of the last selected group
        /// </summary>
        public int Selected
        {
            get { return this._selected; }
        }

        /// <summary>
        /// The size of the total population
        /// </summary>
        public long Size
        {
            get
            {
                if (this._count > 0)
                {
                    int i;
                    long sum = 0;
                    for (i = 0; i < this._count; i++)
                    {
                        sum += this._groups[i].Size;
                    }

                    return sum;
                }
                else
                {
                    return this._count;
                }
            }
        }

        /// <summary>
        /// Get or set a population group property.
        /// </summary>
        public Group this[int index]
        {
            get
            {
                if (index >= 0 && index < this._count)
                {
                    this._selected = index;
                    return this._groups[index];
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
                    this._groups[index] = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(
                                                "index < 0 or index > Count",
                                                "Invalid index argument.");
                }
            }
        }

        #endregion

        #region Public Digraph Methods

        /// <summary>
        /// Adds a new population group to the population
        /// </summary>
        /// <param name="popdef">The population group to be added</param>
        /// <returns>The index of the new group</returns>
        public int AddGroup(Group popdef)
        {
            //Initialise the group and AdjMatrix
            if (this._count == -1)
            {
                //Adds a new population group
                this._groups = new Group[1];
                this._groups[0] = popdef;
                this._count = 1;
                this._selected = 0;

                // Resize the adjacency matrix
                this._adjMatrix = new double[this._count, this._count];
            }
            else
            {
                // Resize the array and copy the old data
                Group[] tmpGroup = this._groups;
                this._groups = new Group[this._count + 1];
                Array.Copy(tmpGroup, this._groups, tmpGroup.Length);
                this._groups[this._count] = popdef;

                // Resize the adjacency matrix and copy the old data
                double[,] tmpMatrix = this._adjMatrix;
                this._adjMatrix = new double[this._groups.Length, this._groups.Length];
                for (int r = 0; r < this._count; r++)
                {
                    for (int c = 0; c < this._count; c++)
                    {
                        this._adjMatrix[r, c] = tmpMatrix[r, c];
                    }
                }

                this._count++;
            }

            return this._count - 1;
        }

        /// <summary>
        /// Removes an existing population group
        /// </summary>
        /// <param name="index">The index of the group to be removed</param>
        public void RemoveGroup(int index)
        {
            int i, j;
            if (index >= 0 && index < this._count)
            {
                this._count--;

                //Check if population container is empty
                if (this._count == 0)
                {
                    this._groups = null;
                    this._adjMatrix = null;
                    this._count = -1;
                    this._selected = -1;
                    Group.Reset();
                }
                else
                {
                    // Resize the array and copy the old data
                    Group[] tmpGroup = this._groups;
                    this._groups = new Group[this._count];
                    for (i = 0; i < this._count; i++)
                    {
                        if (i < index)
                        {
                            this._groups[i] = tmpGroup[i];
                        }
                        else
                        {
                            this._groups[i] = tmpGroup[i + 1];
                        }
                    }

                    // Resize the adjacency matrix and copy the old data
                    double[,] tmpMatrix = this._adjMatrix;
                    this._adjMatrix = new double[this._count, this._count];

                    // Copy the first part of the matrix
                    for (i = 0; i < this._count; i++)
                    {
                        for (j = 0; j < this._count; j++)
                        {
                            if (i < index && j < index)
                                this._adjMatrix[i, j] = tmpMatrix[i, j];
                            else if (i < index && j >= index)
                                this._adjMatrix[i, j] = tmpMatrix[i, j + 1];
                            else if (i >= index && j < index)
                                this._adjMatrix[i, j] = tmpMatrix[i + 1, j];
                            else
                                this._adjMatrix[i, j] = tmpMatrix[i + 1, j + 1];
                        }
                    }

                    //Check selected index range
                    if (this._selected >= this._count)
                    {
                        this._selected = this._count - 1;
                    }
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                        "index < 0 or index > Count.",
                        "Invalid index argument on Population.RemoveGroup.");
            }
        }

        /// <summary>
        /// Removes an existing population group
        /// </summary>
        /// <param name="popid">The group to be removed</param>
        public void RemoveGroup(Group popid)
        {
            int index = IndexOf(popid);
            if (index >= 0)
            {
                this.RemoveGroup(index);
            }
        }

        /// <summary>
        /// Finds the index of an existing population group
        /// </summary>
        /// <param name="popid">The group to be found</param>
        public int IndexOf(Group popid)
        {
            if (this._count > 0)
            {
                for (int i = 0; i < this._count; i++)
                {
                    if (this._groups[i] == popid)
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
                                    "Group popid, index < 0 or index > Count");
            }
        }

        /// <summary>
        /// Clear the population container
        /// </summary>
        public void Clear()
        {
            this._adjMatrix = null;
            for (int i = 0; i < this._count; i++)
            {
                this._groups[0].ClearPopulation();
            }

            this._groups = null;
            this._count = -1;
            this._selected = -1;
            Group.Reset();
        }

        /// <summary>
        /// Gets the digraph adjacency matrix
        /// </summary>
        public double[,] AdjMatrix
        {
            get { return this._adjMatrix; }
        }

        #endregion //Public methods

        #region Utility public methods

        /// <summary>
        /// Save original HIV prevalence
        /// </summary>
        public void SavePrevalence()
        {
            if (this._groups == null)
            {
                return;
            }
            else
            {
                for (int g = 0; g < this._count; g++)
                {
                    this._groups[g].InitPrevalence = this._groups[g].STDPrevalence;
                }
            }
        }

        /// <summary>
        /// Restore original HIV prevalence
        /// </summary>
        public void RestorePrevalence()
        {
            if (this._groups == null)
            {
                return;
            }
            else
            {
                for (int g = 0; g < this._count; g++)
                {
                    this._groups[g].STDPrevalence = this._groups[g].InitPrevalence;
                }
            }
        }

        /// <summary>
        /// Save the original concurrency settings
        /// </summary>
        /// <param name="wmaxConcurrent">Maximum number of concurrent partners during warm-up</param>
        /// <param name="wprConcurrent">Probability of concurrent partners during warm-up</param>
        public void SaveConcurrency(int wmaxConcurrent, double wprConcurrent)
        {
            if (this._groups == null)
            {
                return;
            }
            else
            {
                for (int g = 0; g < this._count; g++)
                {
                    this._groups[g].InitMaxConcurrent = this._groups[g].MaxConcurrent;
                    this._groups[g].InitPrConcurrent = this._groups[g].PrConcurrent;

                    //Update maximum number of concurrent partnerships
                    if (this._groups[g].MaxConcurrent < wmaxConcurrent)
                    {
                        this._groups[g].MaxConcurrent = wmaxConcurrent;
                    }

                    //Update probability of concurrent partnerships
                    if (this._groups[g].PrConcurrent < wprConcurrent)
                    {
                        this._groups[g].PrConcurrent = wprConcurrent;
                    }
                }
            }
        }

        /// <summary>
        /// Restore the original concurrency settings
        /// </summary>
        public void RestoreConcurrency()
        {
            if (this._groups == null)
            {
                return;
            }
            else
            {
                for (int g = 0; g < this._count; g++)
                {
                    this._groups[g].MaxConcurrent = this._groups[g].InitMaxConcurrent;
                    this._groups[g].PrConcurrent = this._groups[g].InitPrConcurrent;
                }
            }
        }

        #endregion
    }
}
