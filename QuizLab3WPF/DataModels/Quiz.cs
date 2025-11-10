using System;
using System.Collections.Generic;
using System.Linq;

namespace QuizLab3WPF.DataModels
{
    internal class Quiz
    {
        private readonly List<Question> _questionList = new();
        private string _title = string.Empty;

        public IEnumerable<Question> Questions => _questionList;
        public string Title => _title;

        public Quiz()
        {
            _questionList = new List<Question>();
        }

        public Question GetRandomQuestion()
        {
            // throw new NotImplementedException("A random Question needs to be returned here!");
            if (_questionList.Count == 0)
                throw new InvalidOperationException("No questions in quiz!");

            Random rnd = new Random();
            int i = rnd.Next(_questionList.Count);
            return _questionList[i];
        }

        public void AddQuestion(string statement, int correctAnswer, params string[] answers)
        {
            // throw new NotImplementedException("Question need to be instantiated and added to list of questions here!");
            var q = new Question(statement, correctAnswer, answers);
            _questionList.Add(q);
        }

        public void RemoveQuestion(int index)
        {
            // throw new NotImplementedException("Question at requested index need to be removed here!");
            if (index >= 0 && index < _questionList.Count)
                _questionList.RemoveAt(index);
        }
    }
}
