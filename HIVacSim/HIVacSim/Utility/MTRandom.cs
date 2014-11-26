// ----------------------------------------------------------------------------
// <copyright file="MTRandom.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// ----------------------------------------------------------------------------

namespace HIVacSim.Utility
{
    using System;

    /// <summary>
    /// Implements the Mersenne Twister pseudo-random numbers generator algorithm.
    /// </summary>
    /// <remarks>
    /// <p> C# Version Copyright (C) 2001-2004 Akihilo Kramot (Takel). </p>
    /// C# porting from a C-program for MT19937, originaly coded by
    /// Takuji Nishimura, considering the suggestions by Topher Cooper and 
    /// Marc Rieffel in July-Aug. 1997. This library is free software under 
    /// the Artistic license.
    /// See <a href="http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html">
    /// http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html</a> for details
    /// on the algorithm.
    /// </remarks>
    public class MTRandom : Random
    {
        /* Period parameters */
        const int N = 624;                  // length of state vector
        const int M = 397;                  // period parameter
        const uint MATRIX_A = 0x9908b0df;   // constant vector a
        const uint UPPER_MASK = 0x80000000; // most significant w-r bits
        const uint LOWER_MASK = 0x7fffffff; // least significant r bits

        /* Tempering parameters */
        const uint TEMPERING_MASK_B = 0x9d2c5680;
        const uint TEMPERING_MASK_C = 0xefc60000;

        // Fields
        uint[] _mt = new uint[N]; // the array for the state vector
        short _mti;
        int _seed;

        static uint TEMPERING_SHIFT_U(uint y) { return y >> 11; }
        static uint TEMPERING_SHIFT_S(uint y) { return y << 7; }
        static uint TEMPERING_SHIFT_T(uint y) { return y << 15; }
        static uint TEMPERING_SHIFT_L(uint y) { return y >> 18; }
        static uint[] mag01 = { 0x0, MATRIX_A };

        /// <summary>
        /// Initializes a new instance of the <see cref="MTRandom"/> class, 
        /// using a time-dependent default seed value.
        /// </summary>
        public MTRandom()
            : this(Environment.TickCount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MTRandom"/> class, 
        /// using the specified seed value.
        /// </summary>
        /// <param name="seed">A number used to calculate a starting value for the 
        /// pseudo-random number sequence. If a negative number is specified, 
        /// the absolute value of the number is used. </param>
        public MTRandom(int seed)
        {
            this.ReSeed(seed);
        }

        /// <summary>
        /// Gets the seed used to initialise this pseudo-random number sequence
        /// </summary>
        public int Seed
        {
            get { return this._seed; }
        }

        /// <summary>
        /// Re-seeds the pseudo-random generator with a new seed value
        /// </summary>
        /// <param name="seed">A number used to calculate a starting value for the 
        /// pseudo-random number sequence. If a negative number is specified, 
        /// the absolute value of the number is used. </param>
        public void ReSeed(int seed)
        {
            if (seed < 0)
            {
                seed = Math.Abs(seed);
            }

            this.Initialise((uint)seed);
            this._seed = seed;
        }

        /// <summary>
        /// Returns a non-negative random number.
        /// </summary>
        /// <returns>A 32-bit signed integer greater than or equal to zero and 
        /// less than <see cref="int.MaxValue"/>.
        /// </returns>
        public override int Next()
        {
            return this.Next(int.MaxValue);
        }

        /// <summary>
        /// Returns a non-negative random number less than the specified maximum.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// maxValue is less than zero.
        /// </exception>
        /// <param name="maxValue">The exclusive upper bound of the random number 
        /// to be generated. maxValue must be greater than or equal to zero. </param>
        /// <returns>A 32-bit signed integer greater than or equal to zero, and less 
        /// than maxValue. If maxValue equals zero, maxValue is returned.
        /// </returns>
        public override int Next(int maxValue)
        {
            if (maxValue <= 1)
            {
                if (maxValue < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return 0;
            }

            return (int)(this.Sample() * maxValue);
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// minValue is greater than maxValue.
        /// </exception>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">he exclusive upper bound of the random number returned. 
        /// maxValue must be greater than or equal to minValue.</param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to minValue and less than maxValue. 
        /// If minValue equals maxValue, minValue is returned.
        /// </returns>
        public override int Next(int minValue, int maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (maxValue == minValue)
            {
                return minValue;
            }
            else
            {
                return this.Next(maxValue - minValue) + minValue;
            }
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <exception cref="ArgumentNullException">buffer is null.</exception>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        public override void NextBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            int bufLen = buffer.Length;
            for (int idx = 0; idx < bufLen; ++idx)
            {
                buffer[idx] = (byte)this.Next(256);
            }
        }

        /// <summary>
        /// Generates a random number on [0,1) with 53-bit resolution
        /// </summary>
        /// <returns>A double-precision floating point number greater 
        /// than or equal to 0.0, and less than 1.0. 
        /// </returns>
        public double NextDouble53()
        {
            uint a = this.InternalSample() >> 5;
            uint b = this.InternalSample() >> 6;
            return (a * 67108864.0 + b) * (1.0 / 9007199254740992.0);
        }

        /// <summary>
        /// Returns a random number on [0,1) with 32-bit resolution
        /// </summary>
        /// <returns>A double-precision floating point number greater 
        /// than or equal to 0.0, and less than 1.0.</returns>
        protected override double Sample()
        {
            return (double)this.InternalSample() / ((ulong)uint.MaxValue + 1);
        }

        /// <summary>
        /// Generates a new pseudo-random <see cref="uint"/> from the sequence.
        /// </summary>
        /// <returns>A pseudo-random <see cref="uint"/></returns>
        protected uint InternalSample()
        {
            uint y;

            // mag01[x] = x * MATRIX_A  for x=0,1
            if (this._mti >= N)
            {
                // Generate N words at one time
                short kk = 0;

                for (; kk < N - M; ++kk)
                {
                    y = (this._mt[kk] & UPPER_MASK) | (this._mt[kk + 1] & LOWER_MASK);
                    this._mt[kk] = this._mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1];
                }

                for (; kk < N - 1; ++kk)
                {
                    y = (this._mt[kk] & UPPER_MASK) | (this._mt[kk + 1] & LOWER_MASK);
                    this._mt[kk] = this._mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1];
                }

                y = (this._mt[N - 1] & UPPER_MASK) | (this._mt[0] & LOWER_MASK);
                this._mt[N - 1] = this._mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1];

                this._mti = 0;
            }

            y = this._mt[this._mti++];
            y ^= TEMPERING_SHIFT_U(y);
            y ^= TEMPERING_SHIFT_S(y) & TEMPERING_MASK_B;
            y ^= TEMPERING_SHIFT_T(y) & TEMPERING_MASK_C;
            y ^= TEMPERING_SHIFT_L(y);

            return y;
        }

        /// <summary>
        /// Initialise the pseudo-random number sequence
        /// </summary>
        /// <param name="seed">A number to be used as seed to the 
        /// pseudo-random number sequence</param>
        private void Initialise(uint seed)
        {
            this._mt[0] = seed & 0xffffffffU;
            for (this._mti = 1; this._mti < N; this._mti++)
            {
                this._mt[this._mti] = (uint)(1812433253U * (this._mt[this._mti - 1] ^ (this._mt[this._mti - 1] >> 30)) + this._mti);
                /* See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier. */
                /* In the previous versions, MSBs of the seed affect   */
                /* only MSBs of the array mt[].                        */
                /* 2002/01/09 modified by Makoto Matsumoto             */
                this._mt[this._mti] &= 0xffffffffU;
                /* for >32 bit machines */
            }
        }
    }
}
