using Microsoft.VisualBasic.FileIO;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace File_manager
{
    public partial class Form1 : Form
    {
        TreeNode nod = new TreeNode();
        int havePas;
        Сustomization custom = new Сustomization();
        public string login;
        string pass;
        public Form1()
        {
            InitializeComponent();
            custom = Сustomization.GetSettings();
            GiveSetting();
            Login(havePas);
            this.WindowState = FormWindowState.Maximized;

            ToolStripMenuItem changeBackColor = new ToolStripMenuItem("Изменить задний фон");
            ToolStripMenuItem changeFont = new ToolStripMenuItem("Изменить шрифт");
            ToolStripMenuItem setPassword = new ToolStripMenuItem("Установить логин и пароль");
            ToolStripMenuItem removePassword = new ToolStripMenuItem("Убрать логин и пароль");

            textBox2.Text = "30";
            treeView1.Nodes.Add("Java");
            treeView1.Nodes.Add("JavaScript");
            treeView1.Nodes.Add("Kotlin");
            treeView1.Nodes.Add("C#");
            WindowState = FormWindowState.Normal;
        }

        // двойное нажатие по книге
        private void listView1_DoubleClick(object sender, EventArgs e) 
        {
            try
            {
                Process.Start(listView1.SelectedItems[0].SubItems[4].Text);
            }
            catch (Exception) { }
        }

        private void GiveSetting()
        {
            treeView1.BackColor = custom.backColor;
            listView1.BackColor = custom.backColor;
            treeView1.Font = custom.font;
            listView1.Font = custom.font;
            havePas = custom.havePas;
            login = custom.login;
            pass = custom.password;
        }
        private void Login(int HavePas)
        {
            if (HavePas == 1)
            {
                string loginCheck = Microsoft.VisualBasic.Interaction.InputBox("Логин");
                while (loginCheck != login)
                {
                    MessageBox.Show("Неверный логин");
                    loginCheck = Microsoft.VisualBasic.Interaction.InputBox("Логин");
                }
                string passwordCheck = Microsoft.VisualBasic.Interaction.InputBox("Пароль");
                while (passwordCheck != pass)
                {
                    MessageBox.Show("Неверный пароль");
                    passwordCheck = Microsoft.VisualBasic.Interaction.InputBox("Пароль");
                }
            }
        }

        //Получение всех книг
        private void WriteChild(TreeNode a)
        {
            
            try
            {
                List<Book> resultBooks = Parser.Parse(a.Text, Convert.ToInt32(textBox2.Text));
                listView1.Items.Clear();
                if (resultBooks != null)
                {
                    foreach (var book in resultBooks)
                        listView1.Items.Add(new ListViewItem(new[] { book.Name, book.Author, book.Rating, book.Price, book.Link }));
                }
            }
            catch (Exception) { }
        }

        // двойной клик по языку
        void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                TreeNode node = treeView1.SelectedNode;
                WriteChild(node);

            }
            catch (Exception) { }
        }

        // задний фон
        private void button1_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = treeView1.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            treeView1.BackColor = colorDialog1.Color;
            listView1.BackColor = colorDialog1.Color;

            custom.backColor = treeView1.BackColor;
            custom.Save();
        }

        // шрифт
        private void button3_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            treeView1.Font = fontDialog1.Font;
            listView1.Font = fontDialog1.Font;
            Console.WriteLine(fontDialog1.Font.GetType());

            custom.font = listView1.Font;
            custom.Save();
        }

        // логин и пароль
        private void button4_Click(object sender, EventArgs e)
        {
            havePas = 1;
            login = Microsoft.VisualBasic.Interaction.InputBox("Логин");
            pass = Microsoft.VisualBasic.Interaction.InputBox("Пароль");

            custom.havePas = havePas;
            custom.login = login;
            custom.password = pass;
            custom.Save();
        }

        // удалить логин и пароль
        private void button5_Click(object sender, EventArgs e)
        {
            havePas = 0;

            custom.havePas = havePas;
            custom.Save();
        }

        // окошко для количества книг
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {

            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != '\b')
            {
                e.Handled = true;
            }
        }

        // добавить язык
        private void button2_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Add(textBox1.Text);
        }
    }
}