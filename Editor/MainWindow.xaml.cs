using System.Windows;
using AurelienRibon.Ui.SyntaxHighlightBox;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text;
using System;
using IniParser;
using IniParser.Model;
using System.Windows.Input;


namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string settingpath = "setting.ini";
        string xss_compilerPath = "XSS_Compiler.exe";
        string xasm_compilerPath = "XASM_Compiler.exe";
        string xse_debuggerPath = "";
        string currFilePath = null;
        public static RoutedCommand MyCommand = new RoutedCommand();
        bool compileToXASM = false;
        bool compileToXSE = false;

        public MainWindow()
        {
            InitializeComponent();
            box.CurrentHighlighter = HighlighterManager.Instance.Highlighters["XSS"];
            InitPath();
            MyCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
        }

        private void InitPath()
        {
            LoadSetting();
            if (string.IsNullOrEmpty(xse_debuggerPath))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Open the XSE debugger";
                openFileDialog.Filter = "Program (*.exe)|*.exe|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    xse_debuggerPath = openFileDialog.FileName;
                    SaveSetting();
                }
                else
                {
                    MessageBox.Show("Can't not open file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadSetting()
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(settingpath);

            xse_debuggerPath = data["path"]["xsedebugger"];
            xasm_compilerPath = data["path"]["xasmcompiler"];
            xss_compilerPath = data["path"]["xsscompiler"];
            compileToXASM = string.Equals(data["compileoption"]["toxasm"],"true");
            compileToXSE = string.Equals(data["compileoption"]["toxse"], "true");
        }

        private void SaveSetting()
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(settingpath);
            data["path"]["xsedebugger"] = xse_debuggerPath;
            data["path"]["xasmcompiler"] = xasm_compilerPath;
            data["path"]["xsscompiler"] = xss_compilerPath;
            data["compileoption"]["toxasm"] = compileToXASM.ToString().ToLower();
            data["compileoption"]["toxse"] = compileToXSE.ToString().ToLower();
            parser.WriteFile(settingpath, data);
        }

        private void btnsav_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        public void save()
        {
            if (currFilePath != null)
            {
                StreamWriter strw = new StreamWriter(currFilePath);
                strw.Write(box.Text);
                strw.Close();
                MessageBox.Show("saved");
            }
            else
            {
                MessageBox.Show("Can't not save file" + '\n' + "There is no currently open file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnopn_Click(object sender, RoutedEventArgs e)
        {
            open();
        }

        public void open()
        {
            bool isFileOpenSuccess = true;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XSS source (*.xss)|*.xss|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string temp = openFileDialog.FileName;
                if (!temp.Contains(" "))
                {
                    currFilePath = temp;
                }
                else
                {
                    MessageBox.Show("Can't not open file" + '\n' + "There is space in the file path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    isFileOpenSuccess = false;
                }
            }
            else
            {
                MessageBox.Show("Can't not open file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                isFileOpenSuccess = false;
            }
            if (isFileOpenSuccess)
            {
                box.Text = File.ReadAllText(currFilePath);
            }
        }

        private void btncpl_Click(object sender, RoutedEventArgs e)
        {
            if (xss_compilerPath == null || !File.Exists(xss_compilerPath))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Program (*.exe)|*.exe|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                    xss_compilerPath = openFileDialog.FileName;
                else
                {
                    MessageBox.Show("Can't not open file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                Process compiler = new Process();
                compiler.StartInfo.FileName = xss_compilerPath;
                compiler.StartInfo.Arguments = Construct_arg(currFilePath, " -N");
                compiler.Start();
                compiler.WaitForExit();
                int code = compiler.ExitCode;
                switch (code)
                {
                    case 0:
                        MessageBox.Show("Compile failed");
                        break;
                    case 1:
                        MessageBox.Show("Compile success");

                        XASMcompile();
                        break;
                    default:
                        MessageBox.Show("Compile failed");
                        break;
                }
            }
        }

        private void XASMcompile()
        {
            string temparg = Path.ChangeExtension(currFilePath, ".xasm");
            Process compiler = new Process();
            compiler.StartInfo.FileName = xasm_compilerPath;
            compiler.StartInfo.Arguments = temparg;
            compiler.Start();
            compiler.WaitForExit();
            int code = compiler.ExitCode;
            switch (code)
            {
                case 0:
                    MessageBox.Show("Assemble failed");
                    break;
                case 1:
                    MessageBox.Show("Assemble success");
                    break;
                default:
                    MessageBox.Show("Assemble failed");
                    break;
            }
        }

        private void btnrun_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currFilePath))
            {
                string temparg = Path.ChangeExtension(currFilePath, ".xse");
                Process debugger = new Process();
                debugger.StartInfo.FileName = xse_debuggerPath;
                temparg = Construct_arg(" -source ", temparg);
                debugger.StartInfo.Arguments = temparg;
                debugger.Start();
            }
            else
            {
                //MessageBox.Show("")
            }
        }

        private void btnsetting_Click(object sender, RoutedEventArgs e)
        {
            Editor.setting setting = new Editor.setting();
            setting.ShowDialog();
            LoadSetting();
        }

        private void btnhelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Halping!");
        }

        private static string GetFileNameWithExtension(string path)
        {
            return Path.GetFileName(path);
        }

        private static string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        private static string GetFileNameNoExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        private static string GetDirectory(string path)
        {
            return Construct_arg(Path.GetDirectoryName(path),Path.DirectorySeparatorChar.ToString());
        }

        private static string Construct_arg(string modifier1, string modifier2)
        {
            StringBuilder arg = new StringBuilder();

            arg.Append(modifier1);
            arg.Append(modifier2);

            return arg.ToString();
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            save();
        }
        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            open();
        }
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand
                (
                        "Exit",
                        "Exit",
                        typeof(CustomCommands),
                        new InputGestureCollection()
                        {
                                        new KeyGesture(Key.F4, ModifierKeys.Alt)
                        }
                );

        //Define more commands here, just like the one above
    }

}
