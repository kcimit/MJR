using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MJR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            vm = new ViewModel();
            vm.MaxJPEGSize = 20*1024;
            DataContext = vm;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            int fcount=0;
            if (!File.Exists(vm.InputFile))
            {
                MessageBox.Show("Cannot find input file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Directory.Exists(vm.OutputFolder))
            {
                var res = MessageBox.Show($"Directory {vm.OutputFolder} does not exists. Do you want to create it?", "Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Error);
                if (res == MessageBoxResult.Cancel) return;
                if (res == MessageBoxResult.No) return;
                try
                {
                    Directory.CreateDirectory(vm.OutputFolder);
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"Cannot create folder {vm.OutputFolder}:\r\n{exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (vm.MaxJPEGSize <= 0)
            {
                MessageBox.Show($"Invalid value of JPEG file size {vm.MaxJPEGSize}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            vm.InputEnabled = false;
            
            Task tsk = Task.Factory.StartNew(() =>
            {
                fcount=Extract.ProcessExtract(vm);
                return;
            });
            while (!tsk.IsCompleted)
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            vm.InputEnabled = true;

            if (fcount>0) MessageBox.Show($"{fcount} Files written.");
        }
        
        private void ButtonSelectFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".mov";
            dlg.Filter = "QT files (*.mov)|*.mov|DAT files (*.dat)|*.dat|All files (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            if (result==true)
                vm.InputFile = dlg.FileName;
        }

        private void ButtonOutputFolder(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(vm.OutputFolder)) dialog.SelectedPath = vm.OutputFolder;
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                    vm.OutputFolder = dialog.SelectedPath;
            }
        }
    }
}
