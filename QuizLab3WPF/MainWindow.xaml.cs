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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PlayView playview = new PlayView();
        private readonly Createview createview = new Createview();
        private readonly EditView editview = new EditView();


        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = playview;
        }


        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    MainContent.Content = playview;
        //}

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = playview;
        }

        private void CreatButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = createview; 
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = editview;
        }
    }
}

    
