using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using QuizLab3WPF.DataModels;

namespace QuizLab3WPF.Views
{
    public partial class Createview : UserControl
    {
        // Klass för ListView-rader (visar frågorna snyggt)
        private class QuestionRow
        {
            public int Index { get; set; }
            public string Statement { get; set; } = "";
            public string A { get; set; } = "";
            public string B { get; set; } = "";
            public string C { get; set; } = "";
            public string CorrectLetter { get; set; } = "";
            public string Category { get; set; } = "";
        }

        // Lista med riktiga frågor (för sparning)
        private readonly ObservableCollection<Question> questions = new();

        // Lista för visning i UI
        private readonly ObservableCollection<QuestionRow> rows = new();

        private readonly string savePath = "quizdata.json"; // Fil att spara i

        public Createview()
        {
            InitializeComponent();
            ListQuestions.ItemsSource = rows;
        }

        // När användaren trycker "Add Question"
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string statement = TextStatement.Text?.Trim() ?? "";
            string a = AlternativA.Text?.Trim() ?? "";
            string b = AlternativB.Text?.Trim() ?? "";
            string c = AlternativC.Text?.Trim() ?? "";
            string category = TextCatagory.Text?.Trim() ?? "";
            string imagePath = TxtImagePath.Text?.Trim() ?? "";
            int correctIndex = CmbCorrect.SelectedIndex;

            // Validering
            if (string.IsNullOrWhiteSpace(statement))
            {
                CreateStatus.Text = "⚠️ Write a question text!";
                TextStatement.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b) || string.IsNullOrWhiteSpace(c))
            {
                CreateStatus.Text = "⚠️ Enter all three answers (A, B, C).";
                return;
            }
            if (correctIndex < 0 || correctIndex > 2)
            {
                CreateStatus.Text = "⚠️ Choose a correct answer.";
                return;
            }

            // Skapa fråga
            var q = new Question(statement, correctIndex, a, b, c)
            {
                Category = string.IsNullOrWhiteSpace(category) ? null : category,
                ImagePath = string.IsNullOrWhiteSpace(imagePath) ? null : imagePath
            };

            // Lägg till listor
            questions.Add(q);
            rows.Add(new QuestionRow
            {
                Index = rows.Count + 1,
                Statement = statement,
                A = a,
                B = b,
                C = c,
                CorrectLetter = correctIndex == 0 ? "A" : correctIndex == 1 ? "B" : "C",
                Category = category
            });

            ClearInputs();
            CreateStatus.Text = "✅ The question has been added.";
        }

        // När användaren trycker "Clear"
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();
            CreateStatus.Text = "Cleared.";
        }

        private void ClearInputs()
        {
            TextStatement.Text = "";
            AlternativA.Text = "";
            AlternativB.Text = "";
            AlternativC.Text = "";
            CmbCorrect.SelectedIndex = 0;
            TextCatagory.Text = "";
            TxtImagePath.Text = "";
            TextStatement.Focus();
        }

        // --- SPARA till fil ---
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(questions, options);
                File.WriteAllText(savePath, json);

                CreateStatus.Text = $"💾 Saved {questions.Count} questions to file.";
            }
            catch (Exception ex)
            {
                CreateStatus.Text = $"❌ Error saving file: {ex.Message}";
            }
        }

        // --- LADDA från fil ---
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists(savePath))
                {
                    CreateStatus.Text = "⚠️ No saved file found.";
                    return;
                }

                string json = File.ReadAllText(savePath);
                var loaded = JsonSerializer.Deserialize<ObservableCollection<Question>>(json);

                if (loaded == null)
                {
                    CreateStatus.Text = "⚠️ File empty or invalid.";
                    return;
                }

                questions.Clear();
                rows.Clear();

                int i = 1;
                foreach (var q in loaded)
                {
                    questions.Add(q);
                    rows.Add(new QuestionRow
                    {
                        Index = i++,
                        Statement = q.Statement,
                        A = q.Answers[0],
                        B = q.Answers[1],
                        C = q.Answers[2],
                        CorrectLetter = q.CorrectAnswer == 0 ? "A" :
                                        q.CorrectAnswer == 1 ? "B" : "C",
                        Category = q.Category ?? ""
                    });
                }

                CreateStatus.Text = $"📂 Loaded {questions.Count} questions from file.";
            }
            catch (Exception ex)
            {
                CreateStatus.Text = $"❌ Error loading file: {ex.Message}";
            }
        }
    }
}
