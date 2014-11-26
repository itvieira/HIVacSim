// ----------------------------------------------------------------------------
// <copyright file="EnumsDef.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;

    /// <summary>
    /// Enumerates the types of partnership
    /// </summary>
    [Serializable]
    public enum EPartners : byte
    {
        /// <summary>
        /// Casual, short term partnership
        /// </summary>
        Casual = 0,

        /// <summary>
        /// Stable, long term partnership
        /// </summary>
        Stable = 1

    }


    /// <summary>
    /// Enumerates the possible STD infection status of the individuals
    /// </summary>
    [Serializable]
    public enum ESTDStatus : byte
    {
        /// <summary>
        /// Not infected, susceptible for STD infection
        /// </summary>
        Susceptable = 0,

        /// <summary>
        /// Infected with STD
        /// </summary>
        Infected = 1,

        /// <summary>
        /// Not infected, protected against STD infection
        /// </summary>
        Protected = 2
    }

    /// <summary>
    /// Enumerates the gender options
    /// </summary>
    [Serializable]
    public enum EGender : byte
    {
        /// <summary>
        /// The female gender
        /// </summary>
        Female = 0,

        /// <summary>
        /// The male gender
        /// </summary>
        Male = 1
    }

    /// <summary>
    /// Enumerates the possible partnership status
    /// </summary>
    [Serializable]
    public enum ERelation : byte
    {
        /// <summary>
        /// Stable or eventual partnership can be established
        /// </summary>
        Available = 0,

        /// <summary>
        /// If <p>concurrent</p> partnership is allowed, 
        /// one is available for eventual partnership. Otherwise, 
        /// no new partnership is allowed.
        /// </summary>
        Engaged = 1,

        /// <summary>
        /// Available only for eventual partnership
        /// </summary>
        Transitory = 2
    }

    /// <summary>
    /// Enumerates the population structure topologies
    /// </summary>
    [Serializable]
    public enum ETopology : byte
    {
        /// <summary>
        /// Scale free topology
        /// </summary>
        Free = 0,

        /// <summary>
        /// 2D lattice (circle) topology
        /// </summary>
        Circle = 1,

        /// <summary>
        /// 3D unit sphere topology
        /// </summary>
        Sphere = 2

    }

    /// <summary>
    /// Enumerates the simulation clock options
    /// </summary>
    [Serializable]
    public enum ESimClock : byte
    {
        /// <summary>
        /// Clock by month
        /// </summary>
        Month = 0,

        /// <summary>
        /// Clock by trimester
        /// </summary>
        Trimester = 1,

        /// <summary>
        /// Clock by semester
        /// </summary>
        Semester = 2,

        /// <summary>
        /// Clock by year
        /// </summary>
        Year = 3
    }

    /// <summary>
    /// Enumerates the run-time status of the simulation
    /// </summary>
    [Serializable]
    public enum ESimStatus : byte
    {
        /// <summary>
        /// The simulation is ready to start run
        /// </summary>
        Ready = 0,

        /// <summary>
        /// The simulation is running
        /// </summary>
        Running = 1,

        /// <summary>
        /// The simulation is stepping
        /// </summary>
        Stepping = 2,

        /// <summary>
        /// The simulation is paused
        /// </summary>
        Paused = 3,

        /// <summary>
        /// The simulation run has completed without resetting
        /// </summary>
        Completed = 4,

        /// <summary>
        /// The simulation is Resetting, cleaning data
        /// </summary>
        Resetting = 5
    }

    /// <summary>
    /// Enumerates the simulation events
    /// </summary>
    [Serializable]
    public enum ESimEvent : byte
    {
        /// <summary>
        /// //Warming up time
        /// </summary>
        WarmingUp = 0,

        /// <summary>
        /// //Update the UI simulation clock
        /// </summary>
        UpdateClock = 1,

        /// <summary>
        /// //Draw the current network to UI 
        /// </summary>
        DrawNetwork = 2,

        /// <summary>
        /// //Update persons colour according with HIV status to the UI
        /// </summary>
        UpdateHIVStatus = 4,

        /// <summary>
        /// //Update UI numbers for the current simulation run
        /// </summary>
        UpdateResults = 5, //Update UI numbers for the current simulation run

        /// <summary>
        /// //The simulation runs have finished without resetting
        /// </summary>
        Finished = 6,

        /// <summary>
        /// The simulation is resetting
        /// </summary>
        ResetNow = 7
    }

    /// <summary>
    /// Enumerates the warm-up options
    /// </summary>
    [Serializable]
    public enum EWarmup : byte
    {
        /// <summary>
        /// The warm-up runs up to a specified time period with 
        /// predefined HIV prevalence.
        /// </summary>
        Traditional = 0,

        /// <summary>
        /// The warm-up runs for a specified time period to
        /// define the HIV prevalence within the population.
        /// </summary>
        Temporal = 1,

        /// <summary>
        /// The warm-up runs while a the predefined condition is false
        /// </summary>
        Conditional = 2,

        /// <summary>
        /// No simulation warm-up takes place
        /// </summary>
        None = 3
    }

    /// <summary>
    /// Enumerates the simulation run-time exit conditions
    /// </summary>
    [Serializable]
    public enum ESimExit : byte
    {
        /// <summary>
        /// Simulation is running
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///Simulation completed the predefined running time
        /// </summary>
        Completed = 1,

        /// <summary>
        /// Simulation terminate because the population HIV prevalence is zero 
        /// </summary>
        HIVZero = 2,

        /// <summary>
        /// There is no population group defined population
        /// </summary>
        PopEmpty = 3
    }

    /// <summary>
    /// Enumerates the HIV testing results
    /// </summary>
    [Serializable]
    public enum EHIVTest : byte
    {
        /// <summary>
        /// The result is not important
        /// </summary>
        NotImportant = 0,

        /// <summary>
        /// Only HIV Negative
        /// </summary>
        HIVNegative = 1,

        /// <summary>
        /// Only HIV positive
        /// </summary>
        HIVPositive = 2
    }

    /// <summary>
    /// Enumerates the scope of the intervention strategies
    /// </summary>
    [Serializable]
    public enum EStrategy : byte
    {
        /// <summary>
        /// Intervention to be applied to all groups
        /// </summary>
        AllGroups = 0,

        /// <summary>
        /// Intervention to be applied to specific groups
        /// </summary>
        Custom = 1
    }
}
