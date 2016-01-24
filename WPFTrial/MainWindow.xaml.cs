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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GamePrototype;

namespace WPFTrial
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Trie trie;
        int pageSize = 16, pageNum = 0;
        public MainWindow()
        {
            InitializeComponent();
            trie = new Trie();
            trie.init("shrink.txt");
            InitDirs();
            InitBlocks();
        }
        private void InitBlocks()
        {
            int blkScale = 4;
            int width = (int)canvas_blks.Width/4, height = width;
            int ofs = 4;
            for (int j = 0; j < blkScale; j++)
            {
                for (int i = 0; i < blkScale; i++)
                {
                    Button lb = new Button();
                    lb.Padding = new Thickness((width/4 + ofs), (width / 4-ofs), (width / 4 + ofs),(width / 4-ofs));
                    lb.Content = " ";
                    lb.Background = (Brush)new BrushConverter().ConvertFromString("#FFFFFFFF");
                    lb.FontFamily = new FontFamily("Consolas");
                    lb.FontSize = 28;
                    Canvas.SetLeft(lb, i * width);
                    Canvas.SetTop(lb, j * height);
                    canvas_blks.Children.Add(lb);
                }
            }
            
        }
        private void InitDirs()
        {
            int[,] dir_ints = { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 },
            {1,1 }, {-1,-1 }, {1,-1 }, {-1,1 } };
            for (int i = 0; i < dir_ints.Length/2; i++)
            {
                dirs[i] = new KeyValuePair<int, int>(dir_ints[i, 0], dir_ints[i, 1]);
            }
        }

        char[,] blocks = new char[4, 4];
        List<string> potientials = new List<string>();
        List<string> results = new List<string>();
        bool[,] visited = new bool[4, 4];
        char[] crt_str = new char[18];

        KeyValuePair<int, int>[] dirs = new KeyValuePair<int, int>[8]; 
        void dfs(int x,int y, int len)
        {
            if (len >= 3)
            {
                potientials.Add(new string(crt_str,0,len));
            }
            if (len == 16)
            {
                return;
            }
            for (int i = 0; i < 8; i++)
            {
                int dx = dirs[i].Key, dy = dirs[i].Value;
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && nx < 4 && ny >= 0 && ny < 4 && visited[nx,ny] == false)
                {
                    visited[nx, ny] = true;
                    crt_str[len] = blocks[nx, ny];
                    dfs(nx, ny, len + 1);
                    visited[nx, ny] = false;
                    crt_str[len] = '\0';
                }
            }
        }
    
        public class LenComp:IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x.Length > y.Length) return -1;
                else if (x.Length == y.Length)
                {
                    return 0;
                }
                else return 1;
            }
        }
        /// <summary>
        /// dfs产生单词序列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            //读取字符串构建方格
            string s_ori = textBox.Text.ToLower();
            if (s_ori.Length != 16)
            {
                return;
            }
            int p = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    blocks[i, j] = s_ori[p];
                    p++;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    visited.Initialize();
                    visited[i, j] = true;
                    crt_str[0] = blocks[i, j];
                    dfs(i, j, 1);
                }
            }
            foreach (var item in potientials)
            {
                if (trie.find(item) != -1)
                {
                    results.Add(item);
                }
            }
            results.Sort(new LenComp());
            showPage();
        }

        private void showPage()
        {
            textBlock.SelectAll();
            TextRange range = new TextRange(textBlock.Selection.Start, textBlock.Selection.End);
            range.Text = "";
            for (int i = pageNum * pageSize; i < pageNum * pageSize + pageSize; i++)
            {
                if (i >= results.Count)
                {
                    break;
                }
                range.Text += results[i]+"\n";
            }
        }

        private void button_prev_Click(object sender, RoutedEventArgs e)
        {
            if (pageNum>0)
            {
                pageNum--;
                showPage();
            }
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            pageNum++;
            showPage();
        }

        private void textBlock_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.textBox.Text.Length > 16)
            {
                return;
            }
            for (int i = 0; i < textBox.Text.Length; i++)
            {
                Button btn = canvas_blks.Children[i] as Button;
                char c = textBox.Text[i];
                if (textBox.Text[i]<='z'&&textBox.Text[i]>='a')
                {
                    c -= (char)('a' - 'A');
                }
                btn.Content = c;
            }
            for (int i = textBox.Text.Length; i < canvas_blks.Children.Count; i++)
            {
                Button btn = canvas_blks.Children[i] as Button;
                btn.Content = " ";
            }
        }
    }
}
