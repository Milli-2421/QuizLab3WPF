using QuizLab3WPF.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuizLab3WPF
{
   
    public partial class MainWindow : Window
    {
        //  My views

        private readonly PlayView playview = new PlayView();
        private readonly Createview createview = new Createview();
        private readonly EditView editview = new EditView();
      
      


        public MainWindow()
        {
            // When user visit ......

            InitializeComponent();
            MainContent.Content = playview;
         
        }
       
        // Play view
        private void PlayButton_Click_1(object sender, RoutedEventArgs e)
        {
            MainContent.Content = playview;
        }

        // create view
        private void CreatButton_Click_1(object sender, RoutedEventArgs e)
        {
            MainContent.Content = createview;
        }
        // Edit view
        private void EditButton_Click_1(object sender, RoutedEventArgs e)
        {
            MainContent.Content = editview;
        }
    }
}

    
