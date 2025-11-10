using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using QuizLab3WPF.DataModels;

namespace QuizLab3WPF.Views
{
    public partial class EditView : UserControl
    {
        // Visningsmodell för listan (en rad per fråga)
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

        // Data i minnet
        private readonly ObservableCollection<Question> questions = new();
        private readonly ObservableCollection<QuestionRow> rows = new();

        public EditView()
        {
            InitializeComponent();
            ListQuestions.ItemsSource = rows;
        }

        // === Filväg: samma som PlayView/CreateView ===
        private static string GetQuizFilePath()
        {
            string appDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "QuizLab3WPF");
            Directory.CreateDirectory(appDir);
            return Path.Combine(appDir, "quiz.json");
        }

        // === Ladda från fil ===
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = GetQuizFilePath();
                if (!File.Exists(path))
                {
                    EditStatus.Text = "ℹ️ No file found.";
                    return;
                }

                string json = File.ReadAllText(path);
                var loaded = JsonSerializer.Deserialize<ObservableCollection<Question>>(json);

                if (loaded is null || loaded.Count == 0)
                {
                    EditStatus.Text = "ℹ️ File empty.";
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
                        Statement = q.Statement ?? "",
                        A = q.Answers != null && q.Answers.Length > 0 ? q.Answers[0] : "",
                        B = q.Answers != null && q.Answers.Length > 1 ? q.Answers[1] : "",
                        C = q.Answers != null && q.Answers.Length > 2 ? q.Answers[2] : "",
                        CorrectLetter = q.CorrectAnswer == 0 ? "A" : q.CorrectAnswer == 1 ? "B" : "C",
                        Category = q.Category ?? ""
                    });
                }

                ClearForm();
                EditStatus.Text = $"📥 Loaded {questions.Count} questions.";
            }
            catch (Exception ex)
            {
                EditStatus.Text = $"❌ Load failed: {ex.Message}";
            }
        }

        // === Spara till fil ===
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = GetQuizFilePath();
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(questions, options);
                File.WriteAllText(path, json);

                EditStatus.Text = $"💾 Saved {questions.Count} questions.";
            }
            catch (Exception ex)
            {
                EditStatus.Text = $"❌ Save failed: {ex.Message}";
            }
        }

        // === Välj fråga i listan => fyll formuläret ===
        private void ListQuestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListQuestions.SelectedIndex < 0 || ListQuestions.SelectedIndex >= questions.Count)
            {
                ClearForm();
                return;
            }

            var q = questions[ListQuestions.SelectedIndex];
            TextStatement.Text = q.Statement ?? "";
            AlternativA.Text = q.Answers != null && q.Answers.Length > 0 ? q.Answers[0] : "";
            AlternativB.Text = q.Answers != null && q.Answers.Length > 1 ? q.Answers[1] : "";
            AlternativC.Text = q.Answers != null && q.Answers.Length > 2 ? q.Answers[2] : "";
            CmbCorrect.SelectedIndex = Math.Clamp(q.CorrectAnswer, 0, 2);
            TextCatagory.Text = q.Category ?? "";
            TxtImagePath.Text = q.ImagePath ?? "";

            EditStatus.Text = "✏️ Edit mode: change fields then click Update.";
        }

        // === Uppdatera vald fråga ===
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            int i = ListQuestions.SelectedIndex;
            if (i < 0 || i >= questions.Count)
            {
                EditStatus.Text = "ℹ️ Select a question first.";
                return;
            }

            string statement = TextStatement.Text?.Trim() ?? "";
            string a = AlternativA.Text?.Trim() ?? "";
            string b = AlternativB.Text?.Trim() ?? "";
            string c = AlternativC.Text?.Trim() ?? "";
            int correctIndex = CmbCorrect.SelectedIndex;
            string category = TextCatagory.Text?.Trim() ?? "";
            string imagePath = TxtImagePath.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(statement) ||
                string.IsNullOrWhiteSpace(a) ||
                string.IsNullOrWhiteSpace(b) ||
                string.IsNullOrWhiteSpace(c) ||
                correctIndex < 0 || correctIndex > 2)
            {
                EditStatus.Text = "⚠️ Fill all fields (A/B/C) and choose correct answer.";
                return;
            }

            // Uppdatera data
            var q = questions[i];
            q.Statement = statement;
            q.CorrectAnswer = correctIndex;
            q.Answers = new[] { a, b, c };
            q.Category = string.IsNullOrWhiteSpace(category) ? null : category;
            q.ImagePath = string.IsNullOrWhiteSpace(imagePath) ? null : imagePath;

            // Uppdatera visningsraden
            rows[i] = new QuestionRow
            {
                Index = i + 1,
                Statement = statement,
                A = a,
                B = b,
                C = c,
                CorrectLetter = correctIndex == 0 ? "A" : correctIndex == 1 ? "B" : "C",
                Category = category
            };

            EditStatus.Text = "✅ Question updated. Don’t forget to Save.";
        }

        // === Ta bort vald fråga ===
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int i = ListQuestions.SelectedIndex;
            if (i < 0 || i >= questions.Count)
            {
                EditStatus.Text = "ℹ️ Select a question to delete.";
                return;
            }

            if (MessageBox.Show("Delete selected question?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            questions.RemoveAt(i);
            rows.RemoveAt(i);

            // Re-indexera
            for (int k = 0; k < rows.Count; k++) rows[k].Index = k + 1;

            ClearForm();
            EditStatus.Text = "🗑️ Question deleted. Save to keep changes.";
        }

        // === Rensa formuläret ===
        private void ClearButton_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            TextStatement.Text = "";
            AlternativA.Text = "";
            AlternativB.Text = "";
            AlternativC.Text = "";
            CmbCorrect.SelectedIndex = 0;
            TextCatagory.Text = "";
            TxtImagePath.Text = "";
            ListQuestions.SelectedIndex = -1;
        }
    }
}
