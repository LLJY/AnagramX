using System;
using System.Collections.Generic;
using System.Net;
using AnagramX.Models;
using System.Threading.Tasks;
using System.Numerics;

namespace AnagramX.Repositories
{
    /// <summary>
    /// Repository is for getting information to the models
    /// </summary>
    internal class MainRepository
    {
        /// <summary>
        /// Gets a list of wordData from the github link.
        /// </summary>
        /// <returns></returns>
        public List<WordData> GetWordList()
        {
            var returnList = new List<WordData>();
            using var client = new WebClient();
            var contents = client
                .DownloadString("https://raw.githubusercontent.com/dwyl/english-words/master/words_alpha.txt")
                .Split('\n');
            // calculate the value of each word in the dictionary and do it in parallel, there are approximately 500k words
            Parallel.ForEach(contents, (word) =>
            {
                var wordData = ConvertWordToWordData(word);
                // it is important to gain a lock on something to prevent race conditions
                lock (returnList)
                {
                    returnList.Add(wordData);
                }
            });
            return returnList;
        }

        /// <summary>
        /// Converts a string into WordData
        /// </summary>
        /// <param name="word">
        /// String / Word to convert</param>
        /// <returns></returns>
        public WordData ConvertWordToWordData(string word)
        {
            // never assume the dataset is clean, so upper case all the characters and remove whitespace
            var capsWord = word.ToUpper().Trim();
            var overflow = false;
            ulong wordValue = 1L;
            // split up the word into individual characters so we can calculate the value of the word.
            foreach (var character in capsWord)
            {
                checked
                {
                    try
                    {
                        // checked to check if the word has overflown, throw an exception if that is true.
                        wordValue *= character switch
                        {
                            'A' => (ulong) 2,
                            'B' => 3,
                            'C' => 5,
                            'D' => 7,
                            'E' => 11,
                            'F' => 13,
                            'G' => 17,
                            'H' => 19,
                            'I' => 23,
                            'J' => 29,
                            'K' => 31,
                            'L' => 37,
                            'M' => 41,
                            'N' => 43,
                            'O' => 47,
                            'P' => 53,
                            'Q' => 59,
                            'R' => 61,
                            'S' => 67,
                            'T' => 71,
                            'U' => 73,
                            'V' => 79,
                            'W' => 83,
                            'X' => 89,
                            'Y' => 97,
                            'Z' => 101,
                            _ => 1,
                        };
                    }
                    catch (OverflowException e)
                    {
                        // if it has overflown, break out of the current loop and set overflow to true
                        overflow = true;
                        goto overflow_loop;
                    }
                }
            }

            overflow_loop: ;
            BigInteger wordvalueBig = 1;
            // if the previous loop overflowed, use biginteger for this word instead
            if (overflow)
            {
                foreach (var character in capsWord)
                {
                    wordvalueBig *= character switch
                    {
                        'A' => 2,
                        'B' => 3,
                        'C' => 5,
                        'D' => 7,
                        'E' => 11,
                        'F' => 13,
                        'G' => 17,
                        'H' => 19,
                        'I' => 23,
                        'J' => 29,
                        'K' => 31,
                        'L' => 37,
                        'M' => 41,
                        'N' => 43,
                        'O' => 47,
                        'P' => 53,
                        'Q' => 59,
                        'R' => 61,
                        'S' => 67,
                        'T' => 71,
                        'U' => 73,
                        'V' => 79,
                        'W' => 83,
                        'X' => 89,
                        'Y' => 97,
                        'Z' => 101
                    };
                }
            }

            // if it has overflowed, set wordValue to 0 and use big word value and vice versa
            return overflow ? new WordData(word, 0, wordvalueBig) : new WordData(word, wordValue, 0);
        }
    }
}