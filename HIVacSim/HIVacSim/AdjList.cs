// ----------------------------------------------------------------------------
// <copyright file="AdjList.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;
    using System.Text;

    /// <summary>
    /// Adjacency list data structure class to hold the individuals 
    /// list of partners.
    /// </summary>
    [Serializable]
    public class AdjList
    {
        #region Internal Class
        /// <summary>
        /// Defines a element in the AdjList
        /// </summary>
        [Serializable]
        private class Node
        {
            #region Local variables
            /// <summary>
            /// The data stored in this element of the AdjList
            /// </summary>
            private Relation _data;

            /// <summary>
            /// Pointer to the next element in the AdjList
            /// </summary>
            private Node _next;

            /// <summary>
            /// Pointer to previous element in the AdjList
            /// </summary>
            private Node _previous;
            #endregion

            #region Public methods
            /// <summary>
            /// Initializes a element of the adjList
            /// </summary>
            /// <param name="data">The data to store in this element</param>
            /// <param name="next">Point to next element in the AdjList</param>
            /// <param name="previous">Point to previous element in the AdjList</param>
            public Node(Relation data, Node next, Node previous)
            {
                this._data = data;
                this._next = next;
                this._previous = previous;
            }

            /// <summary>
            /// Data in this AdjList element
            /// </summary>
            public Relation Data
            {
                get {return this._data;}
                set {this._data = value;}
            }
            
            /// <summary>
            /// Pointer to next element of the AdjList
            /// </summary>
            public Node Next
            {
                get {return this._next;}
                set {this._next=value;}
            }

            /// <summary>
            /// Pointer to previous element of the AdjList
            /// </summary>
            public Node Previous
            {
                get {return this._previous;}
                set {this._previous = value;}
            }
            #endregion
        }
        #endregion

        #region Local variables
        /// <summary>
        /// The first element of the AdjList
        /// </summary>
        private Node _head;

        /// <summary>
        /// The number of elements in the AdjList
        /// </summary>
        private int  _count;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the AdjList class
        /// </summary>
        public AdjList()
        {
            this._head			= new Node(null,null,null);
            this._head.Next		= this._head;
            this._head.Previous = this._head;
            this._count			= 0;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the number of elements contained in the AdjList
        /// </summary>
        public int Count
        {
            get{return this._count;}
        }
        
        /// <summary>
        /// Gets or sets the <see cref="Relation"/> at the specified index
        /// <paramref name="index"/>.
        /// </summary>
        public Relation this[int index] 
        {
            get {return FindAt(index).Data;} 
            set	{FindAt(index).Data = value;}
        }
        
        /// <summary>
        /// Gets or sets the <see cref="Relation"/> for a specified partner
        /// </summary>
        public Relation this[Person partner] 
        {
            get {return FindAt(IndexOf(partner)).Data;} 
            set	{FindAt(IndexOf(partner)).Data = value;}
        }
        
        #endregion

        #region Public methods
        /// <summary>
        /// Inserts an <see cref="Relation"/> into the <see cref="AdjList"/> 
        /// at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based <paramref name="index"/> at which 
        /// <paramref name="data"/> should be inserted.
        /// </param>
        /// <param name="data"> The <see cref="Relation"/> to insert. </param>
        /// <returns>True for successful addition, false otherwise</returns>
        public bool Insert(int index, Relation data)
        {
            if (this.Contains(data.ToPerson))
            {
                return false;
            }
            else
            {
                Node node;
                if (index == this._count)
                {
                    node = new Node(data, this._head, this._head.Previous);
                }
                else
                {
                    Node temp = FindAt(index);
                    node = new Node(data, temp, temp.Previous);
                }

                node.Previous.Next = node;
                node.Next.Previous = node;

                this._count++;
                return true;
            }
        }

        /// <summary>
        /// Adds an <see cref="Relation"/> to the end of the <see cref="AdjList"/>.
        /// </summary>
        /// <param name="data">
        /// The <see cref="Relation"/> to be added to the end of the <see cref="AdjList"/>
        /// </param>
        /// <returns>True for successful addition, false otherwise</returns>
        public bool Add(Relation data)
        {
            return Insert(this._count, data);
        }

        /// <summary>
        /// Removes the first occurrence of a specific <see cref="Relation"/> 
        /// from the <see cref="AdjList"/>.
        /// </summary>
        /// <param name="partner">
        /// The <see cref="Relation"/> to remove from the <see cref="AdjList"/>.
        /// </param>
        public bool Remove(Person partner)
        {	
            for (Node node = this._head.Next; node != this._head; node = node.Next)
            {
                if (node.Data.ToPerson == partner) 
                {
                    return Remove(node);
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the <see cref="Relation"/> at the specified 
        /// <paramref name="index"/> of the <see cref="AdjList"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based <paramref name="index"/> of the 
        /// <see cref="Relation"/> to remove.
        /// </param>
        public bool RemoveAt(int index)
        {
            return Remove(FindAt(index));
        }

        /// <summary>
        /// Determines whether an <see cref="Relation"/> is in the <see cref="AdjList"/>.
        /// </summary>
        /// <param name="partner">
        /// <br>The <see cref="Relation"/> to locate in the <see cref="AdjList"/>.</br> 
        /// The <see cref="Relation"/> to locate can be a <c>null</c> reference). 
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="partner"/> is found in the <see cref="AdjList"/>; otherwise, false.
        /// </returns>
        public bool Contains(Person partner)
        {
            return 0 <= IndexOf(partner);
        }

        /// <summary>
        /// Searches for the specified <see cref="Person"/> and returns the zero-based 
        /// index of the first occurrence within the entire <see cref="AdjList"/>.
        /// </summary>
        /// <param name="partner">
        /// The <see cref="Person"/> to locate in the <see cref="AdjList"/>.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="partner"/> within the entire 
        /// <see cref="AdjList"/>, if found; otherwise, -1.
        /// </returns>
        public int IndexOf(Person partner)
        {
            int currentIndex = 0;
            for (Node node =  this._head.Next; node != this._head; node = node.Next)
            {
                if (node.Data.ToPerson == partner)
                {
                    break;
                }

                currentIndex++;
            }

            if (this._count <= currentIndex)
            {
                currentIndex = -1;
            }

            return currentIndex;
        }

        /// <summary>
        /// Copies the entire <see cref="AdjList"/> to a one-dimensional <see cref="Array"/>.
        /// The <see cref="Array"/> must have zero-based indexing.
        /// </summary>
        /// <returns>A one-dimension array representing the BookList</returns>
        public Person[] ToArray()
        {
            if (this._count > 0)
            {
                int i = 0;
                Person[] data = new Person[this._count];
                for (Node node = this._head.Next; node != this._head; node = node.Next)
                {
                    data[i] = node.Data.ToPerson;
                    i++;
                }

                return data;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a list of person's unique identification separated by comma
        /// </summary>
        /// <returns>The list of persons in the list</returns>
        public string ToCSV()
        {
            StringBuilder list = new System.Text.StringBuilder();
            for (Node node = this._head.Next; node != this._head; node = node.Next)
            {
                list.Append(node.Data.ToPerson.Id.ToString() + ",");
            }

            //Remove last comma
            if (list.Length > 0)
            {
                list.Remove(list.Length - 1, 1);
                return list.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Removes all <see cref="Relation"/>s from the <see cref="AdjList"/>.
        /// </summary>
        public void Clear()
        {
            this._head.Next		= this._head;
            this._head.Previous	= this._head;
            this._count			= 0;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Returns the <see cref="Node"/> at the provided <paramref name="index"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based <paramref name="index"/> of the <see cref="Node"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/> at the provided <paramref name="index"/>.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If the <paramref name="index"/> is invalid.
        /// </exception>
        private Node FindAt(int index) 
        {
            if (index < 0 || this._count <= index)
            {
                throw new IndexOutOfRangeException(
                        "Attempted to access index " + index +
                        ", while the total count is " + this._count + ".");
            }

            Node node = this._head;
            if (index < (this._count / 2)) 
            {
                for (int i = 0; i <= index; i++)
                {
                    node = node.Next;
                }
            } 
            else 
            {
                for (int i = this._count; i > index; i--)
                {
                    node = node.Previous;
                }
            }

            return node;
        }

        /// <summary>
        /// Removes a <see cref="Node"/> from the <see cref="AdjList"/>.
        /// </summary>
        /// <param name="node">The node to be removed</param>
        /// <remarks>
        /// This removal adjusts the remaining <see cref="Node"/> accordingly.
        /// </remarks>
        private bool Remove(Node node)
        {
            if (node != this._head)
            {				
                node.Previous.Next = node.Next;
                node.Next.Previous = node.Previous;
                this._count--;
                return true;
            }

            return false;
        }
        #endregion
    }
}
