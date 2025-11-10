using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using QuizLab3WPF.DataModels;

namespace QuizLab3WPF.Views
{
    public partial class PlayView : UserControl
    {
        // Fält
        private List<Question> questions;
        private Question current;
        private int index;
        private int correct = 0;
        private int asked = 0;
        private bool answeredThisQuestion = false;

        public PlayView()
        {
            InitializeComponent();

            // 1) Ladda frågor från fil (fallback till exempel)
            if (!TryLoadQuestionsFromFile())
            {
                questions = new List<Question>
                {
                    new Question("Vilken färg har himlen?", 0, "Blå", "Röd", "Grön")
                    { Category = "Natur", ImagePath = "/Images/ABC.png" },

                    new Question("2 + 2 = ?", 1, "3", "4", "5")
                    { Category = "Matematik", ImagePath = "/Images/BCD.png" },

                    new Question("Huvudstad i Sverige?", 0, "Stockholm", "Oslo", "Köpenhamn")
                    { Category = "Geografi", ImagePath = "/Images/CDE.png" }
                };
            }

            // 2) Slumpa ordningen
            var random = new Random();
            questions = questions.OrderBy(q => random.Next()).ToList();

            // 3) Starta
            index = 0;
            ShowQuestion();
        }

        // === Filhjälp ===
        private static string GetQuizFilePath()
        {
            string appDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "QuizLab3WPF");
            Directory.CreateDirectory(appDir);
            return Path.Combine(appDir, "quiz.json");
        }

        private bool TryLoadQuestionsFromFile()
        {
            try
            {
                string path = GetQuizFilePath();
                if (!File.Exists(path)) return false;

                string json = File.ReadAllText(path);
                var loaded = JsonSerializer.Deserialize<List<Question>>(json);
                if (loaded == null || loaded.Count == 0) return false;

                questions = loaded;
                return true;
            }
            catch
            {
                return false;
            }
        }

        // === Visa fråga ===
        private void ShowQuestion()
        {
            current = questions[index];
            this.DataContext = current;

            answeredThisQuestion = false;
            if (StatusText != null) StatusText.Text = string.Empty;

            SetButtonsEnabled(true);
            NetQizutn.IsEnabled = true;

            UpdateScoreText();
        }

        // === Hjälpmetoder ===
        private void SetButtonsEnabled(bool enabled)
        {
            ButtonA.IsEnabled = enabled;
            ButtonB.IsEnabled = enabled;
            ButtonC.IsEnabled = enabled;
        }

        private void UpdateScoreText()
        {
            int percent = asked == 0 ? 0 : (int)Math.Round(100.0 * correct / asked);
            if (ScoreText != null)
                ScoreText.Text = $"Rätt: {correct}/{asked} ({percent}%)";
        }

        // === Svarshantering ===
        private void Answer(int chosenIndex)
        {
            if (answeredThisQuestion) return;
            if (current == null || current.Answers == null || current.Answers.Length == 0) return;

            asked++;
            answeredThisQuestion = true;
            SetButtonsEnabled(false);

            if (current.IsCorrect(chosenIndex))
            {
                correct++;
                StatusText.Text = "Rätt! 👍";
            }
            else
            {
                string right = current.Answers[current.CorrectAnswer];
                StatusText.Text = $"Fel. Rätt svar: {right}";
            }

            UpdateScoreText();
        }

        // Klick-händelser (måste matcha XAML)
        private void ButtonA_Click(object sender, RoutedEventArgs e) => Answer(0);
        private void ButtonB_Click(object sender, RoutedEventArgs e) => Answer(1);
        private void ButtonC_Click(object sender, RoutedEventArgs e) => Answer(2);

        // === Next ===
        private void NetQizutn_Click(object sender, RoutedEventArgs e)
        {
            index++;

            if (index >= questions.Count)
            {
                int percent = asked == 0 ? 0 : (int)Math.Round(100.0 * correct / asked);
                string msg = $"🎉 Du är klar med quizet!\n\nPoäng: {correct}/{asked}\nProcent: {percent}%";
                MessageBox.Show(msg, "Resultat", MessageBoxButton.OK, MessageBoxImage.Information);

                StatusText.Text = $"🎉 Du är klar med quizet! Poäng: {correct}/{asked} ({percent}%)";
                NetQizutn.IsEnabled = false;
                SetButtonsEnabled(false);
                return;
            }

            ShowQuestion();
        }

        // === Restart ===
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            ResetQuiz();
        }

        private void ResetQuiz()
        {
            correct = 0;
            asked = 0;
            index = 0;

            // blanda om igen vid omstart
            var random = new Random();
            questions = questions.OrderBy(q => random.Next()).ToList();

            NetQizutn.IsEnabled = true;
            SetButtonsEnabled(true);
            if (StatusText != null) StatusText.Text = string.Empty;

            ShowQuestion();
        }
    }
}
