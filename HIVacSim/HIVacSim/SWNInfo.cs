// ----------------------------------------------------------------------------
// <copyright file="SWNInfo.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;
    using System.Text;

    /// <summary>
    /// Defines a structure to hold the characteristics of a small world network.
    /// </summary>
    public struct SWNInfo
    {
        #region Public Properties
        /// <summary>
        /// The number of edges
        /// </summary>
        public double Edges;

        /// <summary>
        /// The number of vertices
        /// </summary>
        public double Vertices;

        /// <summary>
        /// Is the graph fully connected?
        /// </summary>
        public bool Connected;

        /// <summary>
        /// Average degree of the vertices
        /// </summary>
        public double Degree;

        /// <summary>
        /// The diameter of the graph (connected graph only)
        /// </summary>
        public double Diameter;

        /// <summary>
        /// The characteristic path length (L)
        /// </summary>
        public double PathLength;

        /// <summary>
        /// The clustering coefficient (C)
        /// </summary>
        public double Clustering;

        /// <summary>
        /// The connectivity length for the global network (L)
        /// </summary>
        public double GConnectivity;

        /// <summary>
        /// The connectivity length for the local network (1/C)
        /// </summary>
        public double LConnectivity;

        /// <summary>
        /// The global efficiency of the network (1/L)
        /// </summary>
        public double GEfficiency;

        /// <summary>
        /// The global variance of the network efficiency(1/L)
        /// </summary>
        public double GVEfficiency;

        /// <summary>
        /// The global standard error of the network efficiency(1/L)
        /// </summary>
        public double GSEfficiency;

        /// <summary>
        /// The local efficiency of the network (C)
        /// </summary>
        public double LEfficiency;

        /// <summary>
        /// The cost of the network
        /// </summary>
        public double Cost;

        /// <summary>
        /// Expected clustering coefficient for a regular network
        /// </summary>
        public double CRegular;

        /// <summary>
        /// Expected characteristic path length for a regular network
        /// </summary>
        public double LRegular;

        /// <summary>
        /// Expected clustering coefficient for a random network
        /// </summary>
        public double CRandom;

        /// <summary>
        /// Expected characteristic path length for a random network
        /// </summary>
        public double LRandom;

        #endregion //properties

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent 
        /// string representation
        /// </summary>
        /// <returns>
        /// The formatted string representation of the value of this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string nl = Environment.NewLine;
            sb.AppendFormat("Vertices      \t = {0}{1}", this.Vertices, nl);
            sb.AppendFormat("Edges         \t = {0}{1}", this.Edges, nl);
            sb.AppendFormat("Connected     \t = {0}{1}", this.Connected + nl);
            sb.AppendFormat("Degree        \t {0}{1}", this.Degree, nl);
            sb.AppendFormat("Diameter      \t {0}{1}", this.Diameter, nl);
            sb.AppendFormat("Path Length   \t {0}{1}", this.PathLength, nl);
            sb.AppendFormat("Clustering    \t {0}{1}", this.Clustering, nl);
            sb.AppendFormat("GConnectivity \t {0}{1}", this.GConnectivity, nl);
            sb.AppendFormat("LConnectivity \t {0}{1}", this.LConnectivity, nl);
            sb.AppendFormat("GEfficiency   \t {0}{1}", this.GEfficiency, nl);
            sb.AppendFormat("LEfficiency   \t {0}{1}", this.LEfficiency, nl);
            sb.AppendFormat("Network Cost  \t {0}{1}", this.Cost, nl);
            sb.AppendFormat("Regular L     \t {0}{1}", this.LRegular, nl);
            sb.AppendFormat("Regular C     \t {0}{1}", this.CRegular, nl);
            sb.AppendFormat("Random  L     \t {0}{1}", this.LRandom, nl);
            sb.AppendFormat("Random  C     \t {0}{1}", this.CRandom, nl);
            return sb.ToString();
        }

        /// <summary>
        /// Gets the names of the data structure variables as comma separated values
        /// </summary>
        public static string ToCSVTitle
        {
            get
            {
                return "Vertices,Edges,Connected,Degree,Diameter," +
                    "Path Length,Clustering,GConnectivity,LConnectivity," + 
                    "GEfficiency,LEfficiency,Network Cost,Regular L," +
                    "Regular C,Random L,Random C";
            }
        }

        /// <summary>
        /// Converts the numeric value of this instance to 
        /// its comma separated values (CSV) representation
        /// </summary>
        /// <param name="addTitle">If true, adds titles to output string</param>
        /// <returns>A CSV representation of the value of this instance</returns>
        public string ToCSV(bool addTitle)
        {
            StringBuilder sb = new StringBuilder();
            if (addTitle)
            {
                //Adds title
                sb.AppendLine(ToCSVTitle);
            }

            //Adds body
            sb.AppendFormat(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
                this.Vertices,
                this.Edges,
                this.Connected,
                this.Degree,
                this.Diameter,
                this.PathLength,
                this.Clustering,
                this.GConnectivity,
                this.LConnectivity,
                this.GEfficiency,
                this.LEfficiency,
                this.Cost,
                this.LRegular,
                this.CRegular,
                this.LRandom,
                this.CRandom);

            return sb.ToString();
        }//ToCSV
    }
}
