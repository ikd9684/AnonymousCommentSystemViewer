﻿using AnonymousCommentSystemViewer.classes;
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
        private DispatcherTimer timer = null;

        /// <summary></summary>
        private static readonly double fontSize;
        /// <summary></summary>
        private static readonly int speed;
        /// <summary></summary>
        private static readonly FontFamily fontFamily;

        /// <summary>
        /// 
        /// </summary>
        static MainWindow()
        {
            fontSize = Properties.Settings.Default.font_size;
            speed = Properties.Settings.Default.speed;
            fontFamily = new FontFamily(Properties.Settings.Default.font_family);

            if (Properties.Settings.Default.since_startup)
            {
                last = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.MouseLeftButtonDown += (sender, e) => this.DragMove();

            Brush BK = Brushes.Black.Clone();
            BK.Opacity = Properties.Settings.Default.opacity;
            this.BaseCanvas.Background = BK;

            Left = Properties.Settings.Default.Window_Left;
            Top = Properties.Settings.Default.Window_Top;

            Width = Properties.Settings.Default.Window_Width;

            this.lblThreadID.Content = Properties.Settings.Default.thread_id;

            StartEnqueueCommentLoop();
        }

        private static string last = string.Empty;
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

                List<Comment> commentList = CommentsRequest.getCommentList(last);
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

                    TextBlock textBlock = new TextBlock();
                    textBlock.FontFamily = fontFamily;
                    textBlock.FontSize = fontSize;
                    textBlock.Text = commentString.Trim();
                    textBlock.Foreground = Brushes.White;
                    this.BaseCanvas.Children.Add(textBlock);

                    TranslateTransform transform = new TranslateTransform(this.Width, 16);

                    int durationTime = ((int)this.Width + commentString.Length * (int)fontSize) * speed;

                    textBlock.RenderTransform = transform;
                    Duration duration = new Duration(TimeSpan.FromMilliseconds(durationTime));
                    DoubleAnimation animationX = new DoubleAnimation(-1 * commentString.Length * fontSize, duration);

                    transform.BeginAnimation(TranslateTransform.XProperty, animationX);

                    await Task.Delay(durationTime - ((int)this.Width / 2 * speed));
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
            Properties.Settings.Default.Window_Left = Left;
            Properties.Settings.Default.Window_Top = Top;
            Properties.Settings.Default.Window_Width = Width;
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

    }
}