// ----------------------------------------------------------------------------
// <copyright file="PQueue.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;

    /// <summary>
    /// Implements a FIFO queue to hold individuals using a linked list.
    /// </summary>
    public class PQueue
    {
        //Local variables
        private Node first;        // beginning of queue
        private Node last;         // end of queue

        /// <summary>
        /// Private class defining a node within the PQueue's linked list
        /// </summary>
        private class Node
        {
            public Person item;
            public Node next;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PQueue()
        {
            this.first = null;
            this.last = this.first;
        }

        /// <summary>
        /// Indicates whether the queue is empty.
        /// </summary>
        /// <returns>True if the queue is empty, False otherwise.</returns>
        public bool IsEmpty()
        {
            return (first == null);
        }

        /// <summary>
        /// Adds a new person to queue
        /// </summary>
        /// <param name="prs">The person to add to queue</param>
        public void Enqueue(Person prs)
        {
            Node x = new Node();
            x.item = prs;
            x.next = null;
            if (IsEmpty())
            {
                this.first = x;
            }
            else
            {
                last.next = x;
            }

            last = x;
        }

        /// <summary>
        /// Remove the least recently added person from queue
        /// </summary>
        /// <returns>The least recently person added</returns>
        public Person Dequeue()
        {
            Person prs = first.item;
            first = first.next;
            return prs;
        }
    }
}
