using System;
using System.Collections.Generic;
using System.IO;
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
using RamMachine;
namespace BRamMachine
{
    public partial class MainWindow : Window
    {
        private static readonly string BasePath = $"{System.IO.Path.Combine(System.IO.Path.GetTempPath(), "BramMachineLast")}";
        private static readonly string Path = $" {System.IO.Path.Combine(BasePath,"save.txt")}";
        private RamMachineRuntime last = null;
        private string lineTemplate;
        public MainWindow()
        {
            InitializeComponent();
            lineTemplate = LineCounterLabel.Content.ToString();
            Load();
            Saving();
            CodeTextBox.Focus();
            RefreshLineCounter();
        }
        public void Load()
        {
            if (File.Exists(Path))
                CodeTextBox.Text = File.ReadAllText(Path);
        }
        private async Task Saving()
        {
            Directory.CreateDirectory(BasePath);
            while(true)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                Save();
            }
        }
        public void Save()
        {
          
            using(StreamWriter writer= new StreamWriter(new FileStream(Path,FileMode.Create)))
            {
                writer.Write(CodeTextBox.Text);
            }
        }
        public void WriteConsole(string text,Color color)
        {
            Paragraph para = new Paragraph();
            para.Margin = new Thickness(0);
            para.Foreground = new SolidColorBrush(color);
            para.Inlines.Add(new Run($"[{DateTime.Now:HH:mm:ss}] {text}"));
            ConsoleTextBox.Document.Blocks.Add(para);
            ConsoleTextBox.ScrollToEnd();
        }
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
           
            RamMachineRuntime runtime = null;
            try
            {
                var parsed = new ParsedRamCode(CodeTextBox.Text);
                if(!parsed.Commands.Any())
                {
                    WriteConsole("The code is empty", ConsoleStyles.Error);
                    return;
                }
                runtime = new RamMachineRuntime(parsed);
                
            }
            catch (RamMachineParserException exception)
            {
                WriteConsole($"Parser exception \"{exception.Message}\"",ConsoleStyles.Error);
                return;
            }
          
            try
            {
                if (!string.IsNullOrEmpty(InputTextBox.Text))
                    runtime.Input(InputTextBox.Text.Split(',').Select(item => long.Parse(item.Trim())).ToArray());
            }
            catch(Exception exception)
            {
                WriteConsole($"There's a problem while loading input \"{exception.Message}\"", ConsoleStyles.Error);
            }
          
            try
            {
                runtime.DoUntillEnd();
            }
            catch (RamMachineRuntimeException r)
            {
                WriteConsole($"Runtime error has been thrown \"{r} ({(r.InnerException?.Message ?? "")})\"", ConsoleStyles.Error);
                return;
            }

           
            string raw = runtime.GetOutput().Aggregate(new StringBuilder(), (b, v) => b.Append($",{v}")).ToString();
            if (raw.Length > 0)
                OutputTextBox.Text = raw.Substring(1);
            else
                OutputTextBox.Text = string.Empty;
            last = runtime;
            WriteConsole($"Finished {(OutputTextBox.Text.Equals(string.Empty)?"no output":$" result = ({OutputTextBox.Text})") }", ConsoleStyles.Finish);



        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Save();
        }

        
       

     
        private void RefreshLineCounter()
        {
            LineCounterLabel.Content = string.Format(lineTemplate, CodeTextBox.Text
                .Substring(0, CodeTextBox.CaretIndex).Count(item => item == '\n')+1);
        }

        private void ShowMemoryButton_Click(object sender, RoutedEventArgs e)
        {
           
            if(last!=null)
            {
                var memory = last.Memory;
                int index = 0;
                WriteConsole(last.Memory.Aggregate(new StringBuilder($"Last memory's result:"), (b, v) =>
                {
                    return b.Append($"\n[{index}]=({memory[index++]})");
                   
                }).ToString(), ConsoleStyles.Information);

            }
            else
            {
                WriteConsole("There's no last successed runtime", Colors.Red);
            }
        }

        private void CodeTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            RefreshLineCounter();
        }
    }
}
