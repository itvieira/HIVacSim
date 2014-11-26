// ----------------------------------------------------------------------------
// <copyright file="BookList.cs" company="HIVacSim">
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
    /// list of acquaintances.
    /// </summary>
    [Serializable]
    public class BookList
    {
        #region Internal Class
        /// <summary>
        /// Defines a element in the BookList
        /// </summary>
        [Serializable]
        private class Node
        {
            #region Local variables
            /// <summary>
            /// The data stored in this element of the BookList
            /// </summary>
            private Person _data;

            /// <summary>
            /// Pointer to the next element in the BookList
            /// </summary>
            private Node _next;

            /// <summary>
            /// Pointer to previous element in the BookList
            /// </summary>
            private Node _previous;
            #endregion

            #region Public methods
            /// <summary>
            /// Initializes a element of the BookList
            /// </summary>
            /// <param name="data">The data to store in this element</param>
            /// <param name="next">Point to next element in the BookList</param>
            /// <param name="previous">Point to previous element in the BookList</param>
            public Node(Person data, Node next, Node previous)
            {
                this._data = data;
                this._next = next;
                this._previous = previous;
            }

            /// <summary>
            /// Data in this BookList element
            /// </summary>
            public Person Data
            {
                get {return this._data;}
                set {this._data = value;}
            }
            
            /// <summary>
            /// Pointer to next element of the BookList
            /// </summary>
            public Node Next
            {
                get {return this._next;}
                set {this._next=value;}
            }

            /// <summary>
            /// Pointer to previous element of the BookList
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
        /// The first element of the BookList
        /// </summary>
        private Node _head;
        private Node _current;

        /// <summary>
        /// The number of elements in the BookList
        /// </summary>
        private int  _count;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the BookList class
        /// </summary>
        public BookList()
        {
            this._head			= new Node(null,null,null);
            this._head.Next		= this._head;
            this._head.Previous = this._head;
            this._current		= this._head;
            this._count			= 0;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the number of elements contained in the BookList
        /// </summary>
        public int Count
        {
            get{return this._count;}
        }

        /// <summary>
        /// Gets or sets the <see cref="Person"/> at the specified index
        /// </summary>
        public Person this[int index] 
        {
            get {return FindAt(index).Data;} 
            set	{FindAt(index).Data = value;}
        }

        /// <summary>
        /// Returns the current person in the friends list
        /// </summary>
        public Person Current
        {
            get	{return this._current.Data;}
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Resets the current person to beginning of the list
        /// </summary>
        public void Reset()
        {
            this._current = this._head;
        }


        /// <summary>
        /// Move the current selection to the next person in the list 
        /// </summary>
        /// <returns>
        /// True if the next person has been selected, false otherwise
        /// </returns>
        public bool MoveNext()
        {
            this._current = this._current.Next;
            if (this._current != this._head)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Inserts an <see cref="Person"/> into the <see cref="BookList"/> 
        /// at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based <paramref name="index"/> at which 
        /// <paramref name="data"/> should be inserted.</param>
        /// <param name="data"> The <see cref="Person"/> to insert. </param>
        /// <returns>True for successful addition, false otherwise.</returns>
        public bool Insert(int index, Person data)
        {
            if (this.Contains(data))
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
        /// Adds an <see cref="Person"/> to the end of the <see cref="BookList"/>.
        /// </summary>
        /// <param name="data">
        /// The <see cref="Person"/> to be added to the end of the <see cref="BookList"/>
        /// </param>
        /// <returns>
        /// True for successful addition, false otherwise.
        /// </returns>
        public bool Add(Person data)
        {
            return Insert(this._count, data);
        }


        /// <summary>
        /// Removes the first occurrence of a specific <see cref="Relation"/> 
        /// from the <see cref="BookList"/>.
        /// </summary>
        /// <param name="friend">
        /// The <see cref="Relation"/> to remove from the <see cref="BookList"/>.
        /// </param>
        public bool Remove(Person friend)
        {	
            for (Node node = this._head.Next; node != this._head; node = node.Next)
            {
                if (node.Data == friend) 
                {
                    return Remove(node);
                }
            }

            return false;
        }


        /// <summary>
        /// Removes the <see cref="Relation"/> at the specified 
        /// <paramref name="index"/> of the <see cref="BookList"/>.
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
        /// Determines whether an <see cref="Person"/> is in the <see cref="BookList"/>.
        /// </summary>
        /// <param name="friend">
        /// <br>The <see cref="Person"/> to locate in the <see cref="BookList"/>.</br> 
        /// The <see cref="Person"/> to locate can be a <c>null</c> reference). 
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="friend"/> is found in the 
        /// <see cref="BookList"/>; otherwise, false.
        /// </returns>
        public bool Contains(Person friend)
        {
            return (0 <= IndexOf(friend));
        }


        /// <summary>
        /// Searches for the specified <see cref="Person"/> and returns 
        /// the zero-based index of the first occurrence within the 
        /// entire <see cref="BookList"/>.
        /// </summary>
        /// <param name="friend">
        /// The <see cref="Person"/> to locate in the <see cref="BookList"/>.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of 
        /// <paramref name="friend"/> within the entire 
        /// <see cref="BookList"/>, if found; otherwise, -1.
        /// </returns>
        public int IndexOf(Person friend)
        {
            int currentIndex = 0;
            for (Node node =  this._head.Next; node != this._head; node = node.Next)
            {
                if (node.Data == friend)
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
        /// Copies the entire <see cref="BookList"/> to a one-dimensional <see cref="Array"/>.
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
                    data[i] = node.Data;
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
                list.Append(node.Data.Id.ToString() + ",");
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
        /// Removes all <see cref="Person"/>s from the <see cref="BookList"/>.
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
        /// Removes a <see cref="Node"/> from the <see cref="BookList"/>.
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
