using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using AutoIt;
using System.Diagnostics;

namespace AutoCheckComcast
{
    public partial class Form1 : Form
    {
        private int CountFile = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Filter = "txt files (*.txt)|*.txt";
            openfile.FilterIndex = 3;
            openfile.RestoreDirectory = true;
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                tbPath.Text = openfile.FileName;
                string[] accounts = File.ReadAllLines(tbPath.Text);
                List<string> list = new List<string>();
                for (int i = 0; i < accounts.Length; i++)
                {
                    if (i % 100 == 0 && i != 0)
                    {
                        CountFile++;
                        string[] temp = list.ToArray();
                        File.WriteAllLines($"account/account{CountFile}.txt", temp);
                        list.Clear();
                    }
                    else if (accounts.Length - i == 1)
                    {
                        list.Add(accounts[i]);
                        CountFile++;
                        string[] temp = list.ToArray();
                        File.WriteAllLines($"account/account{CountFile}.txt", temp);
                        list.Clear();
                        break;
                    }
                    list.Add(accounts[i]);

                }
            }
            
        }
        private void killAPP()
        {
            Process[] processesByName = Process.GetProcessesByName("CheckLiveComcast");
            foreach (Process process in processesByName)
            {
                process.Kill();
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            btnCheck.Enabled = false;
            AutoItX.WinActivate("Comcast");
            for (int i = 1; i <= CountFile; i++)
            {
                Process pr = new Process();
                pr.StartInfo.FileName = "CheckLiveComcast.exe";
                pr.Start();
                await Task.Delay(1000);
                AutoItX.WinActivate("Comcast");
                AutoItX.ControlSetText("Comcast", "", "[NAME:tbPath]", i.ToString());
                await Task.Delay(1000);
                AutoItX.ControlClick("Comcast", "", "[NAME:btnStart]");
                AutoItX.WinSetTitle("Comcast", "", i.ToString());
                if (i % 5 == 0 || i == CountFile) 
                {
                    await Task.Delay(20000);
                    killAPP();
                }
                await Task.Delay(1000);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            File.WriteAllText("data/Live.txt", null);
            MessageBox.Show("Clear success!");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] filePaths = Directory.GetFiles("account/");
            foreach (string filePath in filePaths)
                File.Delete(filePath);
        }
    }
}
