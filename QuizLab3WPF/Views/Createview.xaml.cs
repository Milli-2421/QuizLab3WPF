using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using QuizLab3WPF.DataModels;

namespace QuizLab3WPF.Views
{
    public partial class Createview : UserControl
    {
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

        private readonly ObservableCollection<Question> _questions = new();
        private readonly ObservableCollection<QuestionRow> _rows = new();

        public Createview()
        {
            InitializeComponent();
            ListQuestions.ItemsSource = _rows;
        }

        private static string GetQuizFilePath()
        {
            string appDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "QuizLab3WPF");
            Directory.CreateDirectory(appDir);
            return Path.Combine(appDir, "quiz.json");
        }

   
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string statement = TextStatement.Text?.Trim() ?? "";
            string a = AlternativA.Text?.Trim() ?? "";
            string b = AlternativB.Text?.Trim() ?? "";
            string c = AlternativC.Text?.Trim() ?? "";
            string category = TextCatagory.Text?.Trim() ?? "";
            string imagePath = TxtImagePath.Text?.Trim() ?? "";
            int correctIndex = CmbCorrect.SelectedIndex;

            if (string.IsNullOrWhiteSpace(statement))
            {
                CreateStatus.Text = "Please write a question.";
                TextStatement.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b) || string.IsNullOrWhiteSpace(c))
            {
                CreateStatus.Text = "You must fill 3 answers";
                return;
            }

            if (correctIndex < 0 || correctIndex > 2)
            {
                CreateStatus.Text = "Choose whicj´h answer is correct";
                return;
            }

     
            var q = new Question(statement, correctIndex, a, b, c)
            {
                Category = string.IsNullOrWhiteSpace(category) ? null : category,
                ImagePath = string.IsNullOrWhiteSpace(imagePath) ? null : imagePath
            };

            _questions.Add(q);

       
            _rows.Add(new QuestionRow
            {
                Index = _rows.Count + 1,
                Statement = statement,
                A = a,
                B = b,
                C = c,
                CorrectLetter = correctIndex == 0 ? "A" : correctIndex == 1 ? "B" : "C",
                Category = category
            });

            ClearInputs();
            CreateStatus.Text = "Question added to the list.";
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();
            CreateStatus.Text = "Cleared";
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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = GetQuizFilePath();
                var options = new JsonSerializerOptions { WriteIndented = true };

                CreateStatus.Text = "Saving.";
                await using var fs = File.Create(path);
                await JsonSerializer.SerializeAsync(fs, _questions, options);

                CreateStatus.Text = $"Saved  {_questions.Count} questions.";
            }
            catch (Exception ex)
            {
                CreateStatus.Text = $"save failed {ex.Message}";
            }
        }

     
        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = GetQuizFilePath();
                if (!File.Exists(path))
                {
                    CreateStatus.Text = "ℹ️ No file found.";
                    return;
                }

                CreateStatus.Text = "📥 Loading...";
                await using var fs = File.OpenRead(path);
                var loaded = await JsonSerializer.DeserializeAsync<ObservableCollection<Question>>(fs);

                _questions.Clear();
                _rows.Clear();

                if (loaded == null || loaded.Count == 0)
                {
                    CreateStatus.Text = "No question founded in the fail.";
                    return;
                }

                int i = 1;
                foreach (var q in loaded)
                {
                    _questions.Add(q);
                    var a0 = q.Answers?.Length > 0 ? q.Answers[0] : "";
                    var a1 = q.Answers?.Length > 1 ? q.Answers[1] : "";
                    var a2 = q.Answers?.Length > 2 ? q.Answers[2] : "";

                    _rows.Add(new QuestionRow
                    {
                        Index = i++,
                        Statement = q.Statement ?? "",
                        A = a0,
                        B = a1,
                        C = a2,
                        CorrectLetter = q.CorrectAnswer == 0 ? "A" : q.CorrectAnswer == 1 ? "B" : "C",
                        Category = q.Category ?? ""
                    });
                }

                CreateStatus.Text = $" Loaded  {_questions.Count} questions.";
            }
            catch (Exception ex)
            {
                CreateStatus.Text = $" Load failed: {ex.Message}";
            }
        }
    }
}
