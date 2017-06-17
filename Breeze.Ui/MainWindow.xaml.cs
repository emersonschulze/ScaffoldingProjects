using System;
using System.Diagnostics;
using System.Windows;
using Breeze.Engine;
using Breeze.Ui.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Breeze.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainWindowViewModel ViewModel;

        public MainWindow(string projectName, string entityName)
        {
            InitializeComponent();
            InitializeViewModel(projectName, entityName);
        }

        public MainWindow()
        {
            InitializeComponent();

            var projectName = Environment.GetCommandLineArgs().GetValue(1).ToString();
            var entityName = Environment.GetCommandLineArgs().GetValue(2).ToString();

            InitializeViewModel(projectName, entityName);

            if (Debugger.IsAttached)
                this.ShowInTaskbar = true;
        }



        private void InitializeViewModel(string projectName, string entityName)
        {
            this.ViewModel = new MainWindowViewModel(projectName, entityName)
            {
                CloseAction = WindowClose
            };
            this.DataContext = ViewModel;
        }

        private async void Generate(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.ClassMetadata.GenMapping)
            {
                this.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

                var result =
                    await this.ShowInputAsync(title: "Nome Tabela", message: "Qual o nome da Tabela a ser mapeada?");

                if (result == null) 
                    return;
                this.ViewModel.TableName = result;
            }

            this.ViewModel.GenerateCommand.Execute(null);
        }

        private void WindowClose()
        {
            this.Close();
        }

    }
}
