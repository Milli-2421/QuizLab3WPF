
using QuizLab3WPF.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuizLab3WPF.Views
{

    public partial class PlayView : UserControl
    {
      
        private List<Question> questions;
        private int index; 

        private Question current;
        public PlayView()
        {
            InitializeComponent();
            current = new Question("What color is the sky?", 0, "Blue", "Red", "Green")
            {
                Category = "Nature",
                ImagePath = "/Images/ABC.png"
            };

            this.DataContext = current;
            if (StatusText!= null) StatusText.Text = string.Empty;
        }
        private void Answer(int index)
        { 
            if (current==null || current.Answers== null || current.Answers.Length==0) 
                return;
            if (current.IsCorrect(index))
            {
                StatusText.Text = "Correct";
            }
            else 
            {
                string right = current.Answers[current.CorrectAnswer];
                StatusText.Text = $"Fel. Correct Answer:{right}";
            }



        }

        private void ButtonA_Click(object sender, RoutedEventArgs e) => Answer(0);
        private void ButtonB_Click(object sender, RoutedEventArgs e)=> Answer(1);               
        private void ButtonC_Click(object sender, RoutedEventArgs e) => Answer(2);

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NetQizutn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}




