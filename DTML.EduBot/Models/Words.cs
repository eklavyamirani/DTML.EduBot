using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTML.EduBot.Models
{
    public class Word
    {
        public string lan { get; set; }
        public string trans { get; set; }
        public string word { get; set; }
        public double complexity { get; set; }
        public string image { get; set; }
    }

    public class ExtendedWordCollection
    {
        public IEnumerable<Word> words;
        public double complexity = 1;
    }

    public class WordCollection
    {
        public IEnumerable<string> words;
        public double complexity = 1;
    }

    public class WordScore
    {
        public string source { get; set; }
        public string correct { get; set; }
        public double complexity { get; set; } = 1;
        public bool isCorrect { get; set; } = false;
    }
}