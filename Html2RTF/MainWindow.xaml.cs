using HtmlToWord.Core;
using HtmlToWord.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Html2RTF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly ILogger _logger;
        private readonly IWordApplication _word;
        private int htmlCount = 0;
        private int htmlCountProcessed = 0;
        private string inputPath = "";
        private string otputPath = "";
        public MainWindow()
        {
            InitializeComponent();
            this._logger = new Logger();
            this._word = new WordApplication(this._logger);
            this._word.SetDocumentSize(414, 650);
            
        }

        private void folderView_OnFileOpen1(object sender, FileListView.FolderView.FileOpenEventArgs e)
        {
            System.Console.WriteLine(123);
        }
        private void folderView_OnFileOpen2(object sender, FileListView.FolderView.FileOpenEventArgs e)
        {
            System.Console.WriteLine(321);
        }

        private void lockView()
        {
            this.convertButton.IsEnabled = false;
            this.inputFolder.IsEnabled = false;
            this.outputFolder.IsEnabled = false;
        }

        private void unlockView()
        {
            this.convertButton.IsEnabled = true;
            this.inputFolder.IsEnabled = true;
            this.outputFolder.IsEnabled = true;
        }

        private void convertButton_Click(object sender, RoutedEventArgs e)
        {
            this.progressBar.IsIndeterminate = true;
            this.lockView();
            this.inputPath = this.inputFolder.CurrentFolder;
            this.otputPath = this.outputFolder.CurrentFolder;
            List<string> inputHtmlList = new List<string>();
            string inputCss = "";
            var item = ((ListView)((DockPanel)this.inputFolder.Content).Children[2]).Items;
            foreach (var i in item){
                var contentType = i.ToString().Split('.').Last();
                switch (contentType) {
                    case "css":
                        inputCss = i.ToString();
                        break;
                    case "html":
                        inputHtmlList.Add(i.ToString());
                        break;
                    default:
                        break;
                }
            }
            this.htmlCount = inputHtmlList.Count;
            this.htmlCountProcessed = 0;
            this.progressValue.Text = "0/" + this.htmlCount;
            Thread thread = new Thread(ConvertMisssion);
            thread.Start(inputHtmlList);

        }

        private void ReloadListView()
        {
            var tempOutValue = ((ComboBox)((DockPanel)this.outputFolder.Content).Children[0]).Text;
            ((ComboBox)((DockPanel)this.outputFolder.Content).Children[0]).Text = "";
            ((ComboBox)((DockPanel)this.outputFolder.Content).Children[0]).Text = tempOutValue;
        }
        public void ConvertMisssion(object inputHtmlList)
        {
            foreach (string htmlFilePath in (List<string>)inputHtmlList)
            {
                string docName = htmlFilePath.Split('\\').Last().Split('.').First() + ".rtf";
                var exportFilePath = System.IO.Path.Combine(this.otputPath, docName);
                var inputFileInfo = new FileInfo(htmlFilePath);
                var exportFileInfo = new FileInfo(exportFilePath);
                this._word.ConvertToRTF(inputFileInfo, exportFileInfo, out var message);
                this.Dispatcher.Invoke(() => {
                    this.ReloadListView();
                    this.htmlCountProcessed++;
                    this.progressValue.Text = this.htmlCountProcessed + "/" + this.htmlCount;
                });
            }
            this.Dispatcher.Invoke(() => {
                    this.unlockView();
                    this.progressBar.IsIndeterminate = false;
            });
        }

        // pandoc -s MANUAL.txt -o example7.rtf
        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    new Thread(DoSomething).Start();
        //}
        //public void DoSomething()
        //{
        //    for (int i = 0; i < 100000000; i++)
        //    {
        //        this.Dispatcher.Invoke(() => {
        //            textbox.Text = i.ToString();
        //        });
        //    }
        //}

    }
}
