using AnonymousCommentSystemViewer.classes;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AnonymousCommentSystemViewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary></summary>
        private const string YYYYMMDD = "yyyy/MM/dd HH:mm:ss.fff";
        private const double MAX_FONT_SIZE = 80;

        /// <summary></summary>
        public string threadID;
        /// <summary></summary>
        public int speed;
        /// <summary></summary>
        public double opacity;
        /// <summary></summary>
        private FontFamily fontFamily;
        /// <summary></summary>
        public double fontSize;
        /// <summary></summary>
        public Color commentForeground;
        /// <summary></summary>
        public Color commentBackground;

        /// <summary></summary>
        private DispatcherTimer timer;
        /// <summary></summary>
        private List<TextBlock> textBockList;
        /// <summary></summary>
        private string last;

        /// <summary>
        /// 
        /// </summary>
        private bool isChangedThreadID = false;

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            fontSize = Properties.Settings.Default.font_size;

            speed = Properties.Settings.Default.speed;
            fontFamily = new FontFamily(Properties.Settings.Default.font_family);

            if (Properties.Settings.Default.since_startup)
            {
                last = DateTime.Now.ToString(YYYYMMDD);
            }
            else
            {
                last = string.Empty;
            }

            this.MouseLeftButtonDown += (sender, e) => this.DragMove();
            this.MouseLeftButtonDown += (sender, e) => this.OnLostFocusTxtThreadID();

            commentForeground = Properties.Settings.Default.foreground;
            commentBackground = Properties.Settings.Default.background;

            opacity = Properties.Settings.Default.opacity;

            Left = Properties.Settings.Default.Window_Left;
            Top = Properties.Settings.Default.Window_Top;

            Width = Properties.Settings.Default.Window_Width;

            threadID = Properties.Settings.Default.thread_id;
            this.txtThreadID.Text = threadID;

            textBockList = new List<TextBlock>();
            RedrawWindow();
            StartEnqueueCommentLoop();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RedrawWindow()
        {
            this.Height = 20 + fontSize * 1.3;
            this.MaxHeight = this.Height;
            this.MinHeight = this.Height;

            this.BaseCanvas.Background = new SolidColorBrush(commentBackground);
            this.BaseCanvas.Background.Opacity = opacity;

            foreach (TextBlock textBlock in textBockList)
            {
                textBlock.FontSize = fontSize;
                textBlock.Foreground = new SolidColorBrush(commentForeground);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartEnqueueCommentLoop()
        {
            bool loop = false;

            // １秒ごとにサーバからコメントを取得する処理を呼び出す
            timer = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(async(object sender, EventArgs e) => {

                if (loop)
                {
                    // 先に取得したコメントリストを処理している最中は次のコメント取得をしない。
                    return;
                }

                List<Comment> commentList = CommentsRequest.getCommentList(threadID, last);
                if (commentList.Count == 0)
                {
                    return;
                }
                last = commentList[commentList.Count - 1].timestamp;

                loop = true;
                foreach (Comment comment in commentList)
                {
                    // 取り出したコメントをティッカーに流す
                    // ひとつのコメントが流れ終わるのを待つ

                    String commentString = comment.comment;
                    Debug("{0}: {1}", comment.threadID, comment.comment);

                    TextBlock textBlock = new TextBlock();
                    textBlock.FontFamily = fontFamily;
                    textBlock.FontSize = fontSize;
                    textBlock.Text = commentString.Trim();
                    textBlock.Foreground = new SolidColorBrush(commentForeground);
                    this.BaseCanvas.Children.Add(textBlock);
                    textBockList.Add(textBlock);

                    TranslateTransform transform = new TranslateTransform(this.Width, 16);

                    double durationTime = (this.Width + commentString.Length * MAX_FONT_SIZE) * speed;

                    textBlock.RenderTransform = transform;
                    Duration duration = new Duration(TimeSpan.FromMilliseconds(durationTime));
                    DoubleAnimation animationX = new DoubleAnimation(-1 * (commentString.Length + 1) * MAX_FONT_SIZE, duration);

                    transform.BeginAnimation(TranslateTransform.XProperty, animationX);

                    await Task.Delay((int)(durationTime - (this.Width / 2 * speed)));

                    if (isChangedThreadID)
                    {
                        isChangedThreadID = false;
                        break;
                    }
                }

                loop = false;
            });

            // タイマーの実行開始
            timer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClosingWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.opacity = this.BaseCanvas.Background.Opacity;
            Properties.Settings.Default.Window_Left = Left;
            Properties.Settings.Default.Window_Top = Top;
            Properties.Settings.Default.Window_Width = Width;
            Properties.Settings.Default.thread_id = this.txtThreadID.Text;
            // ファイルに保存
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickBtnMinimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickBtnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoubleClickTxtThreadID(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.IsReadOnly)
            {
                textBox.IsReadOnly = false;
                textBox.BorderThickness = new Thickness(1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        private void OnLostFocusTxtThreadID()
        {
            TextBox textBox = this.txtThreadID;

            if (textBox.IsReadOnly)
            {
                // do nothing
            }
            else
            {
                textBox.SelectionLength = 0;
                textBox.IsReadOnly = true;
                textBox.BorderThickness = new Thickness(0);

                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = Properties.Settings.Default.default_thread_id;
                }
                if (threadID != textBox.Text)
                {
                    threadID = textBox.Text;
                    last = DateTime.Now.ToString(YYYYMMDD);

                    isChangedThreadID = true;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LostFocusTxtThreadID(object sender, KeyboardFocusChangedEventArgs e)
        {
            OnLostFocusTxtThreadID();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyDownOnTxtThreadID(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnLostFocusTxtThreadID();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenConfigWindow(object sender, RoutedEventArgs e)
        {
            ConfigWindow configWindow = new ConfigWindow(this);
            configWindow.ShowDialog();
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenAboutWindow(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow(this);
            aboutWindow.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        private static void Debug(string message, params object[] parameters)
        {
#if DEBUG
            string msg = string.Format(message, parameters);
            string now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            Console.WriteLine("[Debug]({0}) {1}", now, msg);
#endif
        }

    }
}
