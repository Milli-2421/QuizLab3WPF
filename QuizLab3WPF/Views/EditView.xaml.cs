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
    public partial class EditView : UserControl
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

        public EditView()
        {
            InitializeComponent();
            ListQuestions.ItemsSource = _rows;


            SetEditButtonsEnabled(false);
        }

  
        private static string GetQuizFilePath()
        {
            string appDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "QuizLab3WPF");
            Directory.CreateDirectory(appDir);
            return Path.Combine(appDir, "quiz.json");
        }

      
        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = GetQuizFilePath();
                if (!File.Exists(path))
                {
                    EditStatus.Text = "No file found";
                    return;
                }

                EditStatus.Text = "Loading.";
                await using var fs = File.OpenRead(path);
                var loaded = await JsonSerializer.DeserializeAsync<ObservableCollection<Question>>(fs);

                _questions.Clear();
                _rows.Clear();

                if (loaded is null || loaded.Count == 0)
                {
                    EditStatus.Text = "File Empty.";
                    return;
                }

                int i = 1;
                foreach (var q in loaded)
                {
                    // säkra tomma fält
                    var a0 = q.Answers != null && q.Answers.Length > 0 ? q.Answers[0] : "";
                    var a1 = q.Answers != null && q.Answers.Length > 1 ? q.Answers[1] : "";
                    var a2 = q.Answers != null && q.Answers.Length > 2 ? q.Answers[2] : "";

                    _questions.Add(q);
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

                ClearForm();
                EditStatus.Text = $" Loaded {_questions.Count} questions.";
            }
            catch (Exception ex)
            {
                EditStatus.Text = $"Load Faild: {ex.Message}";
            }
        }

 
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = GetQuizFilePath();
                var options = new JsonSerializerOptions { WriteIndented = true };

                EditStatus.Text = "Saving.";
                await using var fs = File.Create(path);
                await JsonSerializer.SerializeAsync(fs, _questions, options);

                EditStatus.Text = $" Saved {_questions.Count} questions.";
            }
            catch (Exception ex)
            {
                EditStatus.Text = $" Save Failed: {ex.Message}";
            }
        }

       
        private void ListQuestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListQuestions.SelectedIndex < 0 || ListQuestions.SelectedIndex >= _questions.Count)
            {
                ClearForm();
                SetEditButtonsEnabled(false);
                return;
            }

            var q = _questions[ListQuestions.SelectedIndex];
            TextStatement.Text = q.Statement ?? "";
            AlternativA.Text = q.Answers != null && q.Answers.Length > 0 ? q.Answers[0] : "";
            AlternativB.Text = q.Answers != null && q.Answers.Length > 1 ? q.Answers[1] : "";
            AlternativC.Text = q.Answers != null && q.Answers.Length > 2 ? q.Answers[2] : "";
            CmbCorrect.SelectedIndex = Math.Clamp(q.CorrectAnswer, 0, 2);
            TextCatagory.Text = q.Category ?? "";
            TxtImagePath.Text = q.ImagePath ?? "";

            SetEditButtonsEnabled(true);
            EditStatus.Text = " Edit mode: change fields, then click Update.";
        }

 
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            int i = ListQuestions.SelectedIndex;
            if (i < 0 || i >= _questions.Count)
            {
                EditStatus.Text = "Select a question first.";
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
                EditStatus.Text = "Fill all fildes and choose the corect answer";
                return;
            }

            var q = _questions[i];
            q.Statement = statement;
            q.CorrectAnswer = correctIndex;
            q.Answers = new[] { a, b, c };
            q.Category = string.IsNullOrWhiteSpace(category) ? null : category;
            q.ImagePath = string.IsNullOrWhiteSpace(imagePath) ? null : imagePath;

            _rows[i] = new QuestionRow
            {
                Index = i + 1,
                Statement = statement,
                A = a,
                B = b,
                C = c,
                CorrectLetter = correctIndex == 0 ? "A" : correctIndex == 1 ? "B" : "C",
                Category = category
            };

            EditStatus.Text = "Question Updated (Don't forget to save) ";
        }

 
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int i = ListQuestions.SelectedIndex;
            if (i < 0 || i >= _questions.Count)
            {
                EditStatus.Text = "Select a question  be  deleted.";
                return;
            }

            if (MessageBox.Show("Do you want to delete selected questions?", "Confrim",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            _questions.RemoveAt(i);
            _rows.RemoveAt(i);

            for (int k = 0; k < _rows.Count; k++) _rows[k].Index = k + 1;

            ClearForm();
            SetEditButtonsEnabled(false);
            EditStatus.Text = "Question Delted (Don't forget to save).";
        }

      
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            SetEditButtonsEnabled(ListQuestions.SelectedIndex >= 0);
        }

        private void ClearForm()
        {
            TextStatement.Text = "";
            AlternativA.Text = "";
            AlternativB.Text = "";
            AlternativC.Text = "";
            CmbCorrect.SelectedIndex = 0;
            TextCatagory.Text = "";
            TxtImagePath.Text = "";
        }

        private void SetEditButtonsEnabled(bool enabled)
        {
            UpdateButton.IsEnabled = enabled;
            DeleteButton.IsEnabled = enabled;
        }
    }
}
