using Microsoft.VisualBasic.FileIO;
using System;
using System.Runtime;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading.Tasks;

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
        string password;
        List<WebClient> webClients;
        public Form1()
        {
            InitializeComponent();
            custom = Сustomization.GetSettings();
            GiveSetting();
            Login(havePas);
            this.WindowState = FormWindowState.Maximized;
            TakeDisk();
            webClients = new List<WebClient>();
            WindowState = FormWindowState.Normal;
            listView1.Sorting = System.Windows.Forms.SortOrder.None; //Ascending Descending

            // правая кнопка мыши по файлу/папке
            #region right click
            ToolStripMenuItem createDirMenuItem = new ToolStripMenuItem("Создать папку");
            ToolStripMenuItem renameMenuItem = new ToolStripMenuItem("Переименовать");
            ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("Удалить");
            ToolStripMenuItem archiveMenuItem = new ToolStripMenuItem("Архивировать");
            ToolStripMenuItem antiArchiveMenuItem = new ToolStripMenuItem("Разархивировать");
            ToolStripMenuItem moveFromMenuItem = new ToolStripMenuItem("Переместить что");
            ToolStripMenuItem moveMenuItem = new ToolStripMenuItem("Переместить куда");
            ToolStripMenuItem copyMenuItem = new ToolStripMenuItem("Скопировать");
            ToolStripMenuItem pasteMenuItem = new ToolStripMenuItem("Вставить");
            ToolStripMenuItem openMenuItem = new ToolStripMenuItem("Открыть");
            ToolStripMenuItem searchRegex = new ToolStripMenuItem("Поиск по регулярному выражению");
            ToolStripMenuItem downoloadFile = new ToolStripMenuItem("Скачать файл сюда");
            ToolStripMenuItem canselDownoload = new ToolStripMenuItem("Отмениит скачивание файла");

            contextMenuStrip1.Items.AddRange(new[] { createDirMenuItem, renameMenuItem, deleteMenuItem, archiveMenuItem, antiArchiveMenuItem, moveFromMenuItem, moveMenuItem, copyMenuItem, pasteMenuItem, openMenuItem, searchRegex, downoloadFile, canselDownoload });
            treeView1.ContextMenuStrip = contextMenuStrip1;
            createDirMenuItem.Click += createDirMenuItem_Click;
            renameMenuItem.Click += renameMenuItem_Click;
            deleteMenuItem.Click += deleteMenuItem_Click;
            archiveMenuItem.Click += archiveMenuItem_Click;
            antiArchiveMenuItem.Click += antiArchiveMenuItem_Click;
            moveFromMenuItem.Click += moveFromMenuItem_Click;
            moveMenuItem.Click += moveMenuItem_Click;
            copyMenuItem.Click += copyMenuItem_Click;
            pasteMenuItem.Click += pasteMenuItem_Click;
            openMenuItem.Click += openMenuItem_Click;
            searchRegex.Click += searchRegex_Click;
            downoloadFile.Click += downoloadFile_Click;
            canselDownoload.Click += canselDownoload_Click;
            #endregion
        }

        // Вызов функции при нажатии
        private void searchRegex_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode node = treeView1.SelectedNode;
                string regular = Microsoft.VisualBasic.Interaction.InputBox("Введите регулярное выражение");
                if (regular != "")
                {
                    listBox1.Items.Clear();
                    Regular_Search(node.Name, regular);
                }
            }
            catch (Exception) { }
        }

        // сама функция поиска
        private void Regular_Search(string file, string regular)
        {
            // если файл, да ещё и текстовый, то ищем в файле
            if (File.Exists(file))
            {

                    using (StreamReader Str_Read = new StreamReader(file))
                    {
                        Match[] matches = Regex.Matches(Str_Read.ReadToEnd(), regular)
                                            .Cast<Match>()
                                            .ToArray();
                        this.BeginInvoke((Action)delegate
                        {
                            foreach (Match match in matches)
                                listBox1.Items.Add(file + " - " + match);
                        }
                        );
                    
                }
            }
            // если папка, то рекурсией по элементам в папки
            else if (Directory.Exists(file))
            {
                string[] files = Directory.GetFiles(file);
                string[] directories = Directory.GetDirectories(file);
                Parallel.ForEach(files, currentFile =>
                {
                    Regular_Search(currentFile, regular);
                }
                );

                if (directories != null && directories.Length > 0)
                {
                    Parallel.ForEach(directories, dir =>
                    {
                        Regular_Search(dir, regular);
                    }
                );
                }
            }
        }

        // скачивание файла
        private void downoloadFile_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode node = treeView1.SelectedNode;
                string link = Microsoft.VisualBasic.Interaction.InputBox("Введите ссылку на файл");
                if (link != "")
                {
                    string downFilename = Microsoft.VisualBasic.Interaction.InputBox("Введите название фала с расширением");
                    WebClient client = new WebClient();
                    webClients.Add(client);
                    client.DownloadFileAsync(new Uri(link), $"{node.Name}\\{downFilename}");
                    TreeNode newNode = new TreeNode("");
                    newNode.Text = $"{downFilename}";
                    newNode.Name = $"{node.Name}\\{downFilename}";
                    node.Nodes.Add(newNode);
                }
            }
            catch (Exception) { }
        }

        // Отмена скачивания
        private void canselDownoload_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var client in webClients)
                    client.CancelAsync();
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
            password = custom.password;
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
                while (passwordCheck != password)
                {
                    MessageBox.Show("Неверный пароль");
                    passwordCheck = Microsoft.VisualBasic.Interaction.InputBox("Пароль");
                }
            }
        }
        private void TakeDisk() //Получение всех дисков
        {
            try
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    TreeNode driveNode = new TreeNode(drive.Name);
                    treeView1.Nodes.Add(driveNode);
                    driveNode.Name = driveNode.Text;

                }
            }
            catch (Exception) { }
        }
        private void WriteChild(TreeNode a) //Получение всех папок и файлов и заненсение их в дерево
        {
            try
            {
                string[] dirs = Directory.GetDirectories(a.Text);
                if (true)
                {
                    foreach (string dir in dirs)
                    {
                        TreeNode newNode = new TreeNode(dir);
                        a.Nodes.Add(newNode);
                        newNode.Name = newNode.Text;
                        newNode.Text = dir.Remove(0, dir.LastIndexOf('\\') + 1);
                        int leng = newNode.Name.Length - 4;
                        if (newNode.Name[leng] == '.' && newNode.Name[leng + 1] == 'z' && newNode.Name[leng + 2] == 'i' && newNode.Name[leng + 3] == 'p')
                            newNode.ImageIndex = 0;
                    }
                }
                string[] files = Directory.GetFiles(a.Text);
                if (true)
                {
                    foreach (string s in files)
                    {
                        TreeNode newNode = new TreeNode(s);
                        newNode.Name = s.Substring(s.IndexOf('\\') + 1);
                        newNode.ImageIndex = 1;
                        a.Nodes.Add(newNode);
                        newNode.Name = newNode.Text;
                        int leng = newNode.Name.Length - 4;
                        newNode.Text = s.Remove(0, s.LastIndexOf('\\') + 1);
                        if (newNode.Name[leng] == '.' && newNode.Name[leng + 1] == 'z' && newNode.Name[leng + 2] == 'i' && newNode.Name[leng + 3] == 'p')
                            newNode.ImageIndex = 2;
                    }
                }
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
        void renameMenuItem_Click(object sender, EventArgs e) //Переименование файла или папки
        {
            try
            {
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
        void deleteMenuItem_Click(object sender, EventArgs e) //Удаление
        {
            try
            {
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
        void archiveMenuItem_Click(object sender, EventArgs e) //Архивирование
        {
            try
            {
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
        void antiArchiveMenuItem_Click(object sender, EventArgs e) //Разархивировния, реализовано только для файлов
        {
            try
            {
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
        public static void Compress(string sourceFile, string compressedFile)// Вспомогательный метод для архивирования файлов
        {
            try
            {
                using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
                using (FileStream targetStream = File.Create(compressedFile))
                using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    sourceStream.CopyTo(compressionStream);
            }
            catch (Exception) { }
        }
        void moveFromMenuItem_Click(object sender, EventArgs e)// Обработка нажатия "переместить что"
        {
            nod = treeView1.SelectedNode;
            flagMove = 1;
        }
        void moveMenuItem_Click(object sender, EventArgs e) //Перемещение
        {
            try
            {
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
        void copyMenuItem_Click(object sender, EventArgs e) //Копирование
        {
            try
            {
                nod = treeView1.SelectedNode;
                flag = 1;
                MessageBox.Show("Скопировано");
            }
            catch (Exception) { }
        }

        void treeView1_MouseDown(object sender, MouseEventArgs e)// Обработка двойного клика по файлу
        {
            try
            {
                string url;
                TreeNode node = treeView1.SelectedNode;
                url = node.Text;
                node.Nodes.Clear();
                node.Text = node.Name;
                WriteChild(node);
                node.Text = url;
                treeView1.SelectedNode.Expand();

            }
            catch (Exception) { }
        }
        void treeView1_MouseClick(object sender, MouseEventArgs e)// Заполнение информации по файлу по клику
        {
            try
            {

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
        private void pasteMenuItem_Click(object sender, EventArgs e)// Обработка "Вставить"
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
            password = Microsoft.VisualBasic.Interaction.InputBox("Пароль");

            custom.havePas = havePas;
            custom.login = login;
            custom.password = password;
            custom.Save();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            havePas = 0;

            custom.havePas = havePas;
            custom.Save();
        }

        static void swap(ref string[,] items, int j) // меняет местави элементы
        {
            string[] temp = new string[4];

            for (int k = 0; k < 4; k++)
                temp[k] = items[j, k];

            for (int k = 0; k < 4; k++)
                items[j, k] = items[j + 1, k];

            for (int k = 0; k < 4; k++)
                items[j + 1, k] = temp[k];
        }

        bool Сomparison(string left, string right, int k) // сравнивает строки в зависимости от случая
        {
            if (k < 2) { // сравнивает строки в алфавитном порядке
                if (String.Compare(left, right) < 0)
                    return true;
                else
                    return false;
            }
            else if (k == 2) // сравнивает числа по размеру числа с учётом особо случая
                return right != "<DIR>" && (left == "<DIR>" || (Convert.ToInt64(left) < Convert.ToInt64(right)));
            else
            {
                int t = 0;
                Double left_ = 0; // 30.05.2020 12:34:34
                left_ += Convert.ToDouble(left.Substring(8, 2)) * Math.Pow(10, 10);
                left_ += Convert.ToDouble(left.Substring(3, 2)) * Math.Pow(10, 8);
                left_ += Convert.ToDouble(left.Substring(0, 2)) * Math.Pow(10, 6);
                if (left[12] == ':')
                    t = 1;
                left_ += Convert.ToDouble(left.Substring(11, 2-t)) * Math.Pow(10, 4);
                left_ += Convert.ToDouble(left.Substring(14-t, 2)) * Math.Pow(10, 2);
                left_ += Convert.ToDouble(left.Substring(17-t, 2)) * Math.Pow(10, 0);

                t = 0;
                Double right_ = 0; // 30.05.2020 12:34:34
                right_ += Convert.ToDouble(right.Substring(8, 2)) * Math.Pow(10, 10);
                right_ += Convert.ToDouble(right.Substring(3, 2)) * Math.Pow(10, 8);
                right_ += Convert.ToDouble(right.Substring(0, 2)) * Math.Pow(10, 6);
                if (right[12] == ':')
                    t = 1;
                right_ += Convert.ToDouble(right.Substring(11, 2 - t)) * Math.Pow(10, 4);
                right_ += Convert.ToDouble(right.Substring(14 - t, 2)) * Math.Pow(10, 2);
                right_ += Convert.ToDouble(right.Substring(17 - t, 2)) * Math.Pow(10, 0);

                return left_ < right_;
            } // сравнивает даты

        }

        bool[] state_sort = { false, false, false, false };
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            try
            {
                // переносим информацию в отдельный контейнер items
                int N = listView1.Items.Count;
                string[,] items = new string[N, 4];
                for (int i = 0; i < listView1.Items.Count; i++)
                    for (int j = 0; j < 4; j++)
                        items[i, j] = listView1.Items[i].SubItems[j].Text;
                listView1.Items.Clear();

                // выбранный столбец
                int k = e.Column;
                state_sort[k] = !state_sort[k];

                // сортируем items
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N - 1; j++)
                        if (state_sort[k] == Сomparison(items[j, k], items[j + 1, k], k))
                            swap(ref items, j);

                // добавялем items в listView
                for (int i = 0; i < N; i++)
                    listView1.Items.Add(new ListViewItem(new[] { items[i, 0], items[i, 1], items[i, 2], items[i, 3] }));
            }
            catch (Exception) { }
        }
    }
}
