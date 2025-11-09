using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizLab3WPF.DataModels
{
    internal class Question
    {
        public string Statement { get; set; }
        public string[] Answers { get; set; }
        public int CorrectAnswer { get; set; }
        public string ImagePath { get; set; }
        public string Category { get; set; }
    public Question() 
        {
        }
        public Question(string statement, int correctAnswer, params string[] answers ) 
        {
            Statement = statement;
            Answers = answers;
            CorrectAnswer = correctAnswer;
         }

        public bool IsCorrect(int index)
        { return index == CorrectAnswer; }

    }
}
