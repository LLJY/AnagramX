using AnagramX.Models;
using AnagramX.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnagramX.ViewModels
{
    class MainViewModel
    {
        /// <summary>
        /// MainViewModel is for business logic
        /// </summary>
        private readonly MainRepository _repository;
        // define random here to ensure that the random numbers picked have equal distribution of randomness
        private readonly Random _random = new Random();
        private List<WordData> _wordDatas;
        public MainViewModel(MainRepository repository)
        {
            _repository = repository;
        }
        /// <summary>
        /// Generate 10 random characters
        /// </summary>
        /// <returns></returns>
        public string GetRandomCharacters()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var returnString = "";
            // loop 10 times and get 10 random characters from the list
            for (int i = 0; i <= 10; i++)
            {
                returnString = $"{returnString}{chars[_random.Next(chars.Length)]}";
            }
            return returnString;
        }
        public async Task DownloadDictionary()
        {
            // asynchronously call getWordList
            _wordDatas = await Task.Run(_repository.GetWordList);
        }
        public List<string> GetAnagrams(string word)
        {
            var returnList = new List<string>();
            var compareWordData = _repository.ConvertWordToWordData(word);
            // compare words for the overflow case in parallel
            Parallel.ForEach(_wordDatas, (wordData) =>
            {
                // if it is a value that has overflowed, compare using bigint.
                if(compareWordData.WordValue == 0)
                {
                    // if the word has overflowed, compare both with bigint
                    if(wordData.WordValue == 0)
                    {
                        // if the word is completely divisible, add it to the list of anagrams
                        if(compareWordData.WordValueBig % wordData.WordValueBig == 0)
                        {
                            // get a lock on returnList to avoid a race condition
                            lock (returnList)
                            {
                                returnList.Add(wordData.Word);
                            }
                        }
                    }
                    else
                    {
                        // if the word is completely divisible, add it to the list of anagrams
                        if (compareWordData.WordValueBig % wordData.WordValue == 0)
                        {
                            // get a lock on returnList to avoid a race condition
                            lock (returnList)
                            {
                                returnList.Add(wordData.Word);
                            }
                        }
                    }
                }
                else
                {
                    if (wordData.WordValue == 0)
                    {
                        // if the word is completely divisible, add it to the list of anagrams
                        if (compareWordData.WordValue % wordData.WordValueBig == 0)
                        {
                            // get a lock on returnList to avoid a race condition
                            lock (returnList)
                            {
                                returnList.Add(wordData.Word);
                            }
                        }
                    }
                    else
                    {
                        // if the word is completely divisible, add it to the list of anagrams
                        if (compareWordData.WordValue % wordData.WordValue == 0)
                        {
                            // get a lock on returnList to avoid a race condition
                            lock (returnList)
                            {
                                returnList.Add(wordData.Word);
                            }
                        }
                    }
                }
            });
            // orderby the length of the words
            return returnList.OrderBy(x=>x.Length).AsParallel().ToList();
        }
    }
}
