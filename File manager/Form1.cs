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
        int flag = 0;
        int flagMove = 0;
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
            changeBackColor.Click += changeBackColor_Click;
            changeFont.Click += changeFont_Click;
            setPassword.Click += setPassword_Click;
            removePassword.Click += removePassword_Click;

            textBox2.Text = "30";
            ToolStripMenuItem searchNewBooks = new ToolStripMenuItem("Искать новые книги");
            contextMenuStrip1.Items.AddRange(new[] { searchNewBooks });
            treeView1.ContextMenuStrip = contextMenuStrip1;
            changeBackColor.Click += changeBackColor_Click;
            changeFont.Click += changeFont_Click;
            setPassword.Click += setPassword_Click;
            removePassword.Click += removePassword_Click;

            treeView1.Nodes.Add("Java");
            treeView1.Nodes.Add("JavaScript");
            treeView1.Nodes.Add("Kotlin");
            treeView1.Nodes.Add("C#");

            /*ToolStripMenuItem goToBook = new ToolStripMenuItem("Открыть в браузере");
            contextMenuStrip3.Items.AddRange(new[] { goToBook });
            listView1.ContextMenuStrip = contextMenuStrip3;
            goToBook.Click += goToBook_Click;*/

            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start(listView1.SelectedItems[0].SubItems[4].Text);
            }
            catch (Exception) { }
        }
        private void searchRegex_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode node = treeView1.SelectedNode;
                string regex = Microsoft.VisualBasic.Interaction.InputBox("Введите выражение");
                Regex newRegex = new Regex(@regex);
                if (File.Exists(node.Name))
                {
                    using (StreamReader sr = new StreamReader(node.Name))
                    {
                        foreach (Match mch in newRegex.Matches(sr.ReadToEnd()))
                        {
                        }
                    }
                }
                else if (Directory.Exists(node.Name))
                {
                    string[] files = Directory.GetFiles(node.Name);
                    Parallel.ForEach(files, currentFile =>
                    {
                        if (Path.GetExtension(currentFile) == ".txt")
                        {
                            using (StreamReader sr = new StreamReader(currentFile))
                            {
                                Match[] matches = Regex.Matches(sr.ReadToEnd(), regex)
                                        .Cast<Match>()
                                        .ToArray();
                                this.Invoke((Action)delegate
                                {
                                }
                                );
                            }
                        }
                    }
                    );
                }
            }
            catch (Exception)
            {
            }
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

        private void WriteChild(TreeNode a) //Получение всех папок и файлов и заненсение их в дерево
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
        void treeView1_MouseDown(object sender, MouseEventArgs e)// Обработка двойного клика по файлу
        {
            try
            {
                TreeNode node = treeView1.SelectedNode;
                WriteChild(node);

            }
            catch (Exception) { }
        }

        void createDirMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode node = treeView1.SelectedNode;
                string newName = Microsoft.VisualBasic.Interaction.InputBox("название папки");

                Directory.CreateDirectory($"{node.Name}\\{newName}");
                TreeNode newNode = new TreeNode($"{node.Name}\\{newName}");
                newNode.Text = $"{newName}";
                newNode.Name = $"{node.Name}\\{newName}";
                node.Nodes.Add(newNode);
            }
            catch (Exception) { }

        }

        //Переименование файла или папки
        void renameMenuItem_Click(object sender, EventArgs e)
        {
            try {
                TreeNode node = treeView1.SelectedNode;
                string newName = Microsoft.VisualBasic.Interaction.InputBox("название файла");
                if (File.Exists(node.Name))
                {
                    FileSystem.RenameFile(node.Name, newName + Path.GetExtension(node.Name));
                    node.Text = newName + Path.GetExtension(node.Name);
                    node.Name = node.Name.Substring(0, node.Name.LastIndexOf('\\') + 1) + newName + Path.GetExtension(node.Name);
                }

                else if (Directory.Exists(node.Name))
                {
                    FileSystem.RenameDirectory(node.Name, newName);
                    node.Text = newName;
                    node.Name = node.Name.Substring(0, node.Name.LastIndexOf('\\') + 1) + newName;
                }
            }
            catch (Exception) { }
        }

        //Удаление папок
        void deleteMenuItem_Click(object sender, EventArgs e)
        {
            try {
                TreeNode node = treeView1.SelectedNode;
                if (File.Exists(node.Name))
                {
                    File.Delete(node.Name);
                }
                else if (Directory.Exists(node.Name))
                {
                    Directory.Delete(node.Name);

                }
                treeView1.Nodes.Remove(node);
            }
            catch (Exception) { }

        }

        //Архивирование папок
        void archiveMenuItem_Click(object sender, EventArgs e)
        {
            try {
                TreeNode node = treeView1.SelectedNode;
                if (File.Exists(node.Name))
                {
                    Compress(node.Name, node.Name.Substring(0, node.Name.LastIndexOf('.')) + ".gz");
                    TreeNode archiveFile = new TreeNode(node.Name.Substring(0, node.Name.LastIndexOf('.')) + ".gz");
                    archiveFile.Name = archiveFile.Text;
                    archiveFile.Text = archiveFile.Text.Substring(archiveFile.Text.LastIndexOf('\\') + 1);
                    archiveFile.ImageIndex = 2;
                    node.Parent.Nodes.Add(archiveFile);
                }
                else if (Directory.Exists(node.Name))
                {
                    ZipFile.CreateFromDirectory(node.Name, node.Name + ".zip");
                    TreeNode archiveDirectory = new TreeNode(node.Name + ".zip");
                    archiveDirectory.Name = archiveDirectory.Text;
                    archiveDirectory.Text = archiveDirectory.Text.Substring(archiveDirectory.Text.LastIndexOf('\\') + 1);
                    archiveDirectory.ImageIndex = 2;
                    node.Parent.Nodes.Add(archiveDirectory);

                }
            }
            catch (Exception) { }
        }

        //Разархивировния, реализовано только для файлов
        void antiArchiveMenuItem_Click(object sender, EventArgs e)
        {
            try {
                TreeNode node = treeView1.SelectedNode;
                ZipFile.ExtractToDirectory(node.Name, node.Name.Substring(0, node.Name.LastIndexOf('\\')));
                string[] files = Directory.GetFiles(node.Name.Substring(0, node.Name.LastIndexOf(".")));
                foreach (string s in files)
                {
                    TreeNode newNode = new TreeNode(s);
                    newNode.Name = newNode.Text;
                    newNode.Text = newNode.Name.Substring(newNode.Name.LastIndexOf('\\') + 1);
                    node.Parent.Nodes.Add(newNode);
                }
            }
            catch (Exception) { }
        }

        // для архивации
        public static void Compress(string sourceFile, string compressedFile)
        {
            try {
                using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
                using (FileStream targetStream = File.Create(compressedFile))
                using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    sourceStream.CopyTo(compressionStream);
            }
            catch (Exception) { }
        }
        void moveFromMenuItem_Click(object sender, EventArgs e)
        {
            nod = treeView1.SelectedNode;
            flagMove = 1;
        }

        //Перемещение папок
        void moveMenuItem_Click(object sender, EventArgs e)
        {
            try {
                if (flagMove == 1)
                {
                    TreeNode node = treeView1.SelectedNode;
                    if (File.Exists(nod.Name))
                    {
                        FileInfo fileInf = new FileInfo(nod.Name);
                        fileInf.MoveTo(node.Name + "\\" + nod.Name.Substring(nod.Name.LastIndexOf('\\') + 1));
                        treeView1.Nodes.Remove(nod);
                        nod.Name = node.Name + "\\" + nod.Name.Substring(nod.Name.LastIndexOf('\\') + 1);
                        node.Nodes.Add(nod);


                    }
                    else if (Directory.Exists(nod.Name))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(nod.Name);
                        dirInfo.MoveTo(node.Name + '\\' + nod.Text);
                        treeView1.Nodes.Remove(nod);
                        nod.Name = node.Name + '\\' + nod.Text;
                        node.Nodes.Add(nod);

                    }
                    flagMove = 0;
                }
            }
            catch (Exception) { }

        }
        void copyMenuItem_Click(object sender, EventArgs e)
        {
            try {
                nod = treeView1.SelectedNode;
                flag = 1;
                MessageBox.Show("Скопировано");
            }
            catch (Exception) { }
        }

        // Информация по клику
        void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            try {
                TreeNode a = treeView1.SelectedNode;
                listView1.Items.Clear();

                string[] dirs = Directory.GetDirectories(a.Name);
                foreach (string dir in dirs)
                {
                    TreeNode newNode = new TreeNode(dir);
                    newNode.Name = newNode.Text;
                    newNode.Text = dir.Remove(0, dir.LastIndexOf('\\') + 1);
                    listView1.Items.Add(new ListViewItem(new[] { newNode.Name, "<DIR>", "<DIR>", File.GetCreationTime(newNode.Name).ToString() }));
                }

                string[] files = Directory.GetFiles(a.Name);
                foreach (string s in files)
                {
                    TreeNode newNode = new TreeNode(s);
                    newNode.Name = s.Substring(s.IndexOf('\\') + 1);
                    newNode.ImageIndex = 1;
                    newNode.Name = newNode.Text;
                    newNode.Text = s.Remove(0, s.LastIndexOf('\\') + 1);
                    listView1.Items.Add(new ListViewItem(new[] { newNode.Name, newNode.Name.Substring(newNode.Name.LastIndexOf('.') + 1), (new FileInfo(newNode.Name).Length).ToString(), File.GetCreationTime(newNode.Name).ToString() }));

                }
            }
            catch (Exception) { }
        }

        private void Form1_FormClosing(object sender, EventArgs e)
        {

            Close();

        }

        // Вставить папку
        private void pasteMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (flag == 1)
                {
                    TreeNode node = treeView1.SelectedNode;
                    if (File.Exists(nod.Name))
                    {
                        FileInfo fileInf = new FileInfo(nod.Name);
                        TreeNode newNode = new TreeNode(nod.Text);
                        newNode.ImageIndex = 1;
                        newNode.Name = node.Name + "\\" + nod.Name.Substring(nod.Name.LastIndexOf('\\') + 1);
                        fileInf.CopyTo(node.Name + "\\" + nod.Name.Substring(nod.Name.LastIndexOf('\\') + 1));
                        node.Nodes.Add(newNode);
                        flag = 0;


                    }
                    else if (Directory.Exists(nod.Name))
                    {
                        TreeNode newDir = new TreeNode(nod.Text);
                        newDir.Name = node.Name + '\\' + nod.Text;
                        node.Nodes.Add(newDir);
                        Directory.CreateDirectory(node.Name + '\\' + nod.Text);
                        string[] files = Directory.GetFiles(nod.Name);
                        foreach (string s in files)
                        {
                            FileInfo fileInf = new FileInfo(s);
                            TreeNode newNode = new TreeNode(s.Substring(s.LastIndexOf('\\') + 1));
                            newNode.ImageIndex = 1;
                            newNode.Name = newDir.Name + "\\" + s.Substring(s.LastIndexOf('\\') + 1);
                            fileInf.CopyTo(newNode.Name);
                            newDir.Nodes.Add(newNode);
                        }
                        flag = 0;
                    }
                }
            }
            catch (Exception) { }

        }
        private void openMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            Process.Start(node.Name);
        }

        private void button1_Click(object sender, EventArgs e) // задний фон
        {
            colorDialog1.Color = treeView1.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            treeView1.BackColor = colorDialog1.Color;
            listView1.BackColor = colorDialog1.Color;

            custom.backColor = treeView1.BackColor;
            custom.Save();
        }

        private void button3_Click(object sender, EventArgs e) // шрифт
        {
            if (fontDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            treeView1.Font = fontDialog1.Font;
            listView1.Font = fontDialog1.Font;
            Console.WriteLine(fontDialog1.Font.GetType());

            custom.font = listView1.Font;
            custom.Save();
        }

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

        private void button5_Click(object sender, EventArgs e)
        {
            havePas = 0;

            custom.havePas = havePas;
            custom.Save();
        }
        private void changeBackColor_Click(object sender, EventArgs e)
        {

        }
        private void changeFont_Click(object sender, EventArgs e)
        {

        }
        private void setPassword_Click(object sender, EventArgs e)
        {

        }
        private void removePassword_Click(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
        private void button2_Click_1(object sender, EventArgs e) // задний фон
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {

            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != '\b')
            {
                e.Handled = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Add(textBox1.Text);
        }
    }
}
