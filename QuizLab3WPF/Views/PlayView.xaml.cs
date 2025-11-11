using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using QuizLab3WPF.DataModels;

namespace QuizLab3WPF.Views
{
    public partial class PlayView : UserControl
    {
     
        private List<Question> _allQuestions = new List<Question>();    
        private List<Question> _questions = new List<Question>();
        private Question _current;
        private int _index = 0;
        private int _correct = 0;
        private int _asked = 0;
        private bool _answeredThisQuestion = false;

        public PlayView()
        {
            InitializeComponent();
            this.Loaded += PlayView_Loaded;
            SetButtonsEnabled(false);
            NetQizutn.IsEnabled = false;
        }

       
        private async void PlayView_Loaded(object sender, RoutedEventArgs e)
        {
      
            await LoadQuestionsAsync();
            FillCategoryCombo();    
            if (CategoryCombo.Items.Count > 0)
                CategoryCombo.SelectedIndex = 0;
            StatusText.Text = "Choose category.";        }

    
        private async Task LoadQuestionsAsync()
        {
            try
            {
                string path = GetQuizFilePath();
                if (!File.Exists(path))
                {
                    
                    _allQuestions = new List<Question>
                    {
                        new Question("Which coloor has sky", 0, "Blue", "Red", "Green")
                        { Category = "Nature", ImagePath = "/Images/sky.jpg" },

                        new Question("2 + 2 = ?", 1, "3", "4", "5")
                        { Category = "Math", ImagePath = "/Images/math.jpg" },

                        new Question("What is Capital city of sweden?", 0, "Stockholm", "Oslo", "Paris")
                        { Category = "Geography", ImagePath = "/Images/sthlm.jpg" }
                    };
                    return;
                }

                await using var fs = File.OpenRead(path);
                var loaded = await JsonSerializer.DeserializeAsync<List<Question>>(fs);

                _allQuestions = loaded ?? new List<Question>();
                if (_allQuestions.Count == 0)
                {
                    StatusText.Text = "There is not questions in this file.";
                    _allQuestions = new List<Question>
                    {
                           new Question("Which coloor has sky", 0, "Blue", "Red", "Green")
                        { Category = "Nature", ImagePath = "/Images/sky.png" },

                         new Question("2 + 2 = ?", 1, "3", "4", "5")
                        { Category = "Math", ImagePath = "/Images/math.png" },
                    };
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Could not read the file ({ex.Message})";
                _allQuestions = new List<Question>
                {
                    new Question("Exempel question: 1+1?", 1, "1", "2", "3") { Category = "Demo" }
                };
            }
        }

        private void FillCategoryCombo()
        {
            var items = new List<string> { "All" };

            var cats = _allQuestions
                .Select(q => q?.Category ?? "")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            items.AddRange(cats);

            CategoryCombo.ItemsSource = items;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string selected = CategoryCombo.SelectedItem as string ?? "All";

    
            if (selected == "All")
                _questions = _allQuestions.ToList();
            else
                _questions = _allQuestions.Where(q => string.Equals(q.Category, selected, StringComparison.OrdinalIgnoreCase)).ToList();

            if (_questions.Count == 0)
            {
                StatusText.Text = "There is no questions on this categotry.";
                SetButtonsEnabled(false);
                NetQizutn.IsEnabled = false;
                this.DataContext = null;
                return;
            }

            var rnd = new Random();
            _questions = _questions.OrderBy(_ => rnd.Next()).ToList();

            _index = 0;
            _correct = 0;
            _asked = 0;

            ShowQuestion();
        }

  
        private void ShowQuestion()
        {
            _current = _questions[_index];
            this.DataContext = _current;

            _answeredThisQuestion = false;

     
            NetQizutn.IsEnabled = false;


            SetButtonsEnabled(true);


            StatusText.Text = "";
            UpdateScoreText();
        }

        private void SetButtonsEnabled(bool enabled)
        {
            ButtonA.IsEnabled = enabled;
            ButtonB.IsEnabled = enabled;
            ButtonC.IsEnabled = enabled;
        }

        private void UpdateScoreText()
        {
            int percent = _asked == 0 ? 0 : (int)Math.Round(100.0 * _correct / _asked);
            ScoreText.Text = $"Right: {_correct}/{_asked} ({percent}%)";
        }

  
        private void Answer(int chosenIndex)
        {
            if (_answeredThisQuestion) return;
            if (_current == null || _current.Answers == null || _current.Answers.Length < 3) return;

            _asked++;
            _answeredThisQuestion = true;

            SetButtonsEnabled(false);

            if (_current.IsCorrect(chosenIndex))
            {
                _correct++;
                StatusText.Text = "Right!!!!!!";
            }
            else
            {
                string right = _current.Answers[_current.CorrectAnswer];
                StatusText.Text = $"Wrong. Right Answer: {right}";
            }

            UpdateScoreText();

  
            NetQizutn.IsEnabled = true;
        }

        private void ButtonA_Click(object sender, RoutedEventArgs e) => Answer(0);
        private void ButtonB_Click(object sender, RoutedEventArgs e) => Answer(1);
        private void ButtonC_Click(object sender, RoutedEventArgs e) => Answer(2);

        private void NetQizutn_Click(object sender, RoutedEventArgs e)
        {
            _index++;

            if (_index >= _questions.Count)
            {
                int percent = _asked == 0 ? 0 : (int)Math.Round(100.0 * _correct / _asked);
                string msg = $"You are done!\n\npoints: {_correct}/{_asked}\nPercent: {percent}%";
                MessageBox.Show(msg, "Result", MessageBoxButton.OK, MessageBoxImage.Information);

                
                NetQizutn.IsEnabled = false;
                SetButtonsEnabled(false);
                return;
            }

            ShowQuestion();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_questions.Count == 0)
            {
                StatusText.Text = "Chose category and start the quiz.";
                return;
            }

         
            var rnd = new Random();
            _questions = _questions.OrderBy(_ => rnd.Next()).ToList();

            _index = 0;
            _correct = 0;
            _asked = 0;

            ShowQuestion();
        }

 
        private static string GetQuizFilePath()
        {
            string appDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "QuizLab3WPF");
            Directory.CreateDirectory(appDir);
            return Path.Combine(appDir, "quiz.json");
        }

        
    }
}
