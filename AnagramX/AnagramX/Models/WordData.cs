using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AnagramX.Models
{
    class WordData
    {
        /// <summary>
        /// The word itself.
        /// </summary>
        public string Word { get; set; }
        /// <summary>
        /// The value of the word calculated using primes
        /// </summary>
        public ulong WordValue { get; set; }
        /// <summary>
        /// Large words have a tendency to exceed even the 64bit unsigned long limit, add a BigInteger to store the larger words.
        /// </summary>
        public BigInteger WordValueBig{get;set;}

        public WordData(string word, ulong wordValue, BigInteger wordValueBig)
        {
            Word = word;
            WordValue = wordValue;
            WordValueBig = wordValueBig;
        }
    }
}
