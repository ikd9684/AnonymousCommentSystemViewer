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

namespace AnonymousCommentSystemViewer
{
    /// <summary>
    /// ConfigWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private MainWindow owner;

        public ConfigWindow(MainWindow owner_)
        {
            InitializeComponent();

            base.Owner = owner_;
            this.owner = owner_;

            this.PreviewKeyDown += new KeyEventHandler(OnKeyDown);
            this.MouseLeftButtonDown += (sender, e) => this.DragMove();

            this.TxtThreadID.Text = owner.txtThreadID.Text;
            this.ChkFromBeginning.IsChecked = !Properties.Settings.Default.since_startup;
            this.SliderSpeed.Value = (int)this.SliderSpeed.Maximum + 1 - owner.speed;
            this.SliderOpacity.Value = owner.BaseCanvas.Background.Opacity * 100;

            this.SliderFontSize.Value = owner.fontSize;
            this.ColorPickerForeground.SelectedColor = owner.commentForeground;
            this.ColorPickerBackground.SelectedColor = owner.commentBackground;

            this.BtnApply.IsEnabled = false;
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
        private void ApplySettings()
        {
            // スレッドID
            string threadID = this.TxtThreadID.Text;
            if (string.IsNullOrEmpty(threadID))
            {
                threadID = Properties.Settings.Default.default_thread_id;
            }
            owner.threadID = threadID;
            owner.txtThreadID.Text = threadID;

            // スレッドの最初から表示
            Properties.Settings.Default.since_startup = (this.ChkFromBeginning.IsChecked == false);

            // スクロールスピード
            owner.speed = (int)this.SliderSpeed.Maximum + 1 - (int)this.SliderSpeed.Value;

            // 透明度
            owner.opacity = this.SliderOpacity.Value / 100;

            // フォントサイズ
            owner.fontSize = this.SliderFontSize.Value;

            // 文字色
            owner.commentForeground = this.ColorPickerForeground.SelectedColor;
            // 背景色
            owner.commentBackground = this.ColorPickerBackground.SelectedColor;

            owner.RedrawWindow();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickBtnOK(object sender, RoutedEventArgs e)
        {
            ApplySettings();
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickBtnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickBtnApply(object sender, RoutedEventArgs e)
        {
            ApplySettings();
            this.BtnApply.IsEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hasChanged(object sender, TextChangedEventArgs e)
        {
            if (this.BtnApply != null)
            {
                this.BtnApply.IsEnabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hasChanged(object sender, RoutedEventArgs e)
        {
            if (this.BtnApply != null)
            {
                this.BtnApply.IsEnabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hasChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.BtnApply != null)
            {
                this.BtnApply.IsEnabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hasChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (this.BtnApply != null)
            {
                this.BtnApply.IsEnabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

    }
}
