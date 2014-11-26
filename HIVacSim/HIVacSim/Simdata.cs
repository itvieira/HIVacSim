// ----------------------------------------------------------------------------
// <copyright file="SimData.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;

    /// <summary>
    /// Defines a structure to hold the simulation output data.
    /// </summary>
    [Serializable]
    public struct SimData
    {
        /// <summary>
        /// Simulation clock in world time
        /// </summary>
        public DateTime Date;

        //===========================================
        //Population
        //===========================================
        /// <summary>
        /// Total population's size
        /// </summary>
        public int Size;

        /// <summary>
        /// Number of females
        /// </summary>
        public int Female;

        /// <summary>
        /// Number of males
        /// </summary>
        public int Male;

        /// <summary>
        /// Number of homosexuals
        /// </summary>
        public int Homosexual;

        //===========================================
        //Friendships
        //===========================================
        /// <summary>
        /// Number of friendships
        /// </summary>
        public int Friends;

        /// <summary>
        /// Number of friendships started within friends 
        /// </summary>
        public int FromFriends;

        //===========================================
        //Partnerships
        //===========================================

        /// <summary>
        /// Total number of partnerships
        /// </summary>
        public int Partners;

        /// <summary>
        /// Number of stable partnerships
        /// </summary>
        public int Stable;

        /// <summary>
        /// Number of stable partners from acquaintances
        /// </summary>
        public int StbFromList;

        /// <summary>
        /// Number of casual partnerships
        /// </summary>
        public int Casual;

        /// <summary>
        /// Number of internal partnerships
        /// </summary>
        public int Internal;

        /// <summary>
        /// Number of external partnerships
        /// </summary>
        public int External;

        /// <summary>
        /// Number of concurrent partnerships
        /// </summary>
        public int Concurrent;

        //===========================================
        //STD Infections counters
        //===========================================

        /// <summary>
        /// STD prevalence
        /// </summary>
        public double STDPrevalence;

        /// <summary>
        /// STD incidence
        /// </summary>
        public int STDIncidence;

        /// <summary>
        /// Female incidence
        /// </summary>
        public int STDFemale;

        /// <summary>
        /// Male incidence
        /// </summary>
        public int STDMale;

        /// <summary>
        /// Homosexual incidence
        /// </summary>
        public int STDHomosexual;

        /// <summary>
        /// Male to female incidence
        /// </summary>
        public int STDMale2Female;

        /// <summary>
        /// Female to male incidence
        /// </summary>
        public int STDFemale2Male;

        /// <summary>
        /// Male to male incidence
        /// </summary>
        public int STDMale2Male;

        /// <summary>
        /// Stable partnership incidence
        /// </summary>
        public int STDStable;

        /// <summary>
        /// Casual partnership incidence
        /// </summary>
        public int STDCasual;

        /// <summary>
        /// Internal partnership incidence
        /// </summary>
        public int STDInternal;

        /// <summary>
        /// External partnership incidence
        /// </summary>
        public int STDExternal;

        /// <summary>
        /// STD infection recovering
        /// </summary>
        public int STDRecovered;

        /// <summary>
        /// STD infection protected
        /// </summary>
        public int STDProtected;

        //===========================================
        //Deaths
        //===========================================
        /// <summary>
        /// Total number of deaths in the population  
        /// </summary>
        public int Deaths;

        /// <summary>
        /// NUmber of deaths caused by STD infection in the population
        /// </summary>
        public int STDDeaths;

        //===========================================
        //Custom data field
        //===========================================
        /// <summary>
        /// User defined data, this field is used for experiments
        /// </summary>
        public string UserData;
    }
}