using IniParser;
using IniParser.Model;
using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace Editor
{
    /// <summary>
    /// Interaction logic for setting.xaml
    /// </summary>
    public partial class setting : Window
    {
        string settingpath = "setting.ini";

        public setting()
        {
            InitializeComponent();
            LoadSetting();
        }

        private void LoadSetting()
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(settingpath);

            textBox_xsedebuggerpath.Text = data["path"]["xsedebugger"];
            textBox_xasmcompilerpath.Text = data["path"]["xasmcompiler"];
            textBox_xsscompilerpath.Text = data["path"]["xsscompiler"];
            if (data["compileoption"]["toxasm"].Equals("true"))
            {
                checkBox_toxasmsourcecode.IsChecked = true;
            }
            if (data["compileoption"]["toxse"].Equals("true"))
            {
                checkBox_toxsesourcecode.IsChecked = true;
            }
        }

        private void SaveSetting()
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(settingpath);
            data["path"]["xsedebugger"] = textBox_xsedebuggerpath.Text;
            data["path"]["xasmcompiler"] = textBox_xasmcompilerpath.Text;
            data["path"]["xsscompiler"] = textBox_xsscompilerpath.Text;
            data["compileoption"]["toxasm"] = checkBox_toxasmsourcecode.IsChecked.ToString().ToLower();
            data["compileoption"]["toxse"] = checkBox_toxsesourcecode.IsChecked.ToString().ToLower();
            parser.WriteFile(settingpath, data);
        }

        private void button_xsscompilepath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open the XSS compiler";
            openFileDialog.Filter = "Program (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                textBox_xsscompilerpath.Text = openFileDialog.FileName;
                SaveSetting();
            }
            else
            {
                MessageBox.Show("Can't not open file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void button_xasmcompilepath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open the XASM compiler";
            openFileDialog.Filter = "Program (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                textBox_xasmcompilerpath.Text = openFileDialog.FileName;
                SaveSetting();
            }
            else
            {
                MessageBox.Show("Can't not open file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void button_xsedebuggerpath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open the XSE debugger";
            openFileDialog.Filter = "Program (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                textBox_xsedebuggerpath.Text = openFileDialog.FileName;
                SaveSetting();
            }
            else
            {
                MessageBox.Show("Can't not open file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void button_savesetting_Click(object sender, RoutedEventArgs e)
        {
            SaveSetting();
            this.Close();
        }
    }
}
