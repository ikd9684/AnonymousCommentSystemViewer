﻿using System;
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

            this.owner = owner_;

            this.MouseLeftButtonDown += (sender, e) => this.DragMove();

            this.TxtThreadID.Text = owner.txtThreadID.Text;
            this.ChkFromBeginning.IsChecked = !Properties.Settings.Default.since_startup;
            this.SliderSpeed.Value = (int)this.SliderSpeed.Maximum + 1 - owner.speed;
            this.SliderOpacity.Value = owner.BaseCanvas.Background.Opacity * 100;
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
        private void SaveSettings()
        {
            // スレッドID
            string threadID = this.TxtThreadID.Text;
            if (string.IsNullOrEmpty(threadID))
            {
                threadID = Properties.Settings.Default.default_thread_id;
            }
            owner.threadID = threadID;
            owner.txtThreadID.Text = threadID;
            Properties.Settings.Default.thread_id = threadID;

            // スレッドの最初から表示
            Properties.Settings.Default.since_startup = (this.ChkFromBeginning.IsChecked == false);

            // スクロールスピード
            owner.speed = (int)this.SliderSpeed.Maximum + 1 - (int)this.SliderSpeed.Value;
            Properties.Settings.Default.speed = owner.speed;

            // 透明度
            owner.BaseCanvas.Background.Opacity = this.SliderOpacity.Value / 100;
            Properties.Settings.Default.opacity = owner.BaseCanvas.Background.Opacity;

            // 保存
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickBtnOK(object sender, RoutedEventArgs e)
        {
            SaveSettings();
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
            SaveSettings();
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

    }
}
