using System.Windows;
using System.Windows.Controls;

namespace Breeze.Ui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Page
    {
        private readonly string projectName;
        private readonly string entityName;

        public App(string projectName, string entityName)
        {
            MainWindow window = new MainWindow(projectName, entityName);
            window.Show();

            this.projectName = projectName;
            this.entityName = entityName;
            InitializeComponent();
        }

        public void Show()
        {
            
        }
}
}
