using System;
using System.Collections.Generic;
using System.Linq;

namespace QuizLab3WPF.DataModels
{
    internal class Quiz
    {
        private IEnumerable<Question> _questions;
        private string _title = string.Empty;
        public IEnumerable<Question> Questions => _questions;
        public string Title => _title;

        public Quiz()
        {
            _questions = new List<Question>();
        }

        public Question GetRandomQuestion()
        {
            var list = (_questions as List<Question>) ?? new List<Question>(_questions);

            if (list.Count == 0)
                throw new InvalidOperationException("No questions available.");

            var rnd = new Random();
            int i = rnd.Next(list.Count);
            return list[i];
        }

        public void AddQuestion(string statement, int correctAnswer, params string[] answers)
        {
            if (string.IsNullOrWhiteSpace(statement))
                throw new ArgumentException("Statement cannot be empty.", nameof(statement));

            if (answers == null || answers.Length == 0)
                throw new ArgumentException("Provide at least one answer.", nameof(answers));

            var q = new Question(statement, correctAnswer, answers);

            if (_questions is List<Question> list)
            {
                list.Add(q);
            }
            else
            {
                _questions = new List<Question>(_questions) { q };
            }
        }

        public void RemoveQuestion(int index)
        {
            var list = (_questions as List<Question>) ?? new List<Question>(_questions);

            if (index < 0 || index >= list.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Invalid question index.");

            list.RemoveAt(index);
            _questions = list;
        }
    }
}
