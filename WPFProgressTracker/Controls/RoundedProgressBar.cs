using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFProgressTracker.Controls
{
    public class RoundedProgressBar : Control
    {
        static RoundedProgressBar ProgressBarBeingEdited;
        MouseButtonState leftMouseLastStatus = MouseButtonState.Released;
        public RoundedProgressBar() : base()
        {
            this.SizeChanged += onSizeChanged;
        }

        private void onWindowMouseLeave(object sender, MouseEventArgs e)
        {
            onWindowLeftMouseUp(null, null);
        }

        private void onWindowMouseMove(object sender, MouseEventArgs e)
        {
            if (ProgressBarBeingEdited != null && ProgressBarBeingEdited != this)
                return;
            if ((IsMouseOver && e.LeftButton == MouseButtonState.Pressed) || leftMouseLastStatus == MouseButtonState.Pressed)
            {
                leftMouseLastStatus = MouseButtonState.Pressed;
                var progressPercent = e.GetPosition(this).X / ActualWidth; // Calculated progress in percentage
                progressPercent = progressPercent > 0.985 ? 1 : progressPercent; // Skip to the end if very close
                progressPercent = progressPercent < 0.015 ? 0 : progressPercent; // Skip to the start if very close
                var difference = Math.Abs(MinProgress - MaxProgress); // Max difference
                Progress = MinProgress + (difference * progressPercent);
                ProgressBarBeingEdited = this;
            }
        }

        private void onWindowLeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (leftMouseLastStatus == MouseButtonState.Pressed)
            {
                OnProgressBarUpdate(this, null);
                ProgressBarBeingEdited = null;
            }
            leftMouseLastStatus = MouseButtonState.Released;
        }

        private void onSizeChanged(object sender, SizeChangedEventArgs e)
        {
            updateProgressBar(this);
        }

        public int ProgressWidth
        {
            get { return (int)GetValue(ProgressWidthProperty); }
            set { SetValue(ProgressWidthProperty, value); }
        }

        public double MinProgress
        {
            get { return (double)GetValue(MinProgressProperty); }
            set { SetValue(MinProgressProperty, value); }
        }

        public double MaxProgress
        {
            get { return (double)GetValue(MaxProgressProperty); }
            set { SetValue(MaxProgressProperty, value); }
        }

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public bool Editable
        {
            get { return (bool)GetValue(EditableProperty); }
            set { SetValue(EditableProperty, value); }
        }

        static RoundedProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RoundedProgressBar), new FrameworkPropertyMetadata(typeof(RoundedProgressBar)));
        }

        public static readonly DependencyProperty ProgressWidthProperty = DependencyProperty.Register("ProgressWidth", typeof(int), typeof(RoundedProgressBar), new PropertyMetadata(0));
        public static readonly DependencyProperty MinProgressProperty = DependencyProperty.Register("MinProgres", typeof(double), typeof(RoundedProgressBar), new PropertyMetadata(0d, ProgressPropertyChanged));
        public static readonly DependencyProperty MaxProgressProperty = DependencyProperty.Register("MaxProgress", typeof(double), typeof(RoundedProgressBar), new PropertyMetadata(100d, ProgressPropertyChanged));
        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(RoundedProgressBar), new PropertyMetadata(0d, ProgressPropertyChanged));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RoundedProgressBar), new PropertyMetadata(new CornerRadius(), ProgressPropertyChanged));
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("EditableProperty", typeof(bool), typeof(RoundedProgressBar), new PropertyMetadata(false, EditablePropertyChanged));

        static void EditablePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var editable = (bool)e.NewValue;
            var progressBar = (RoundedProgressBar)obj;
            if (editable)
            {
                Application.Current.MainWindow.MouseLeftButtonUp += progressBar.onWindowLeftMouseUp;
                Application.Current.MainWindow.MouseMove += progressBar.onWindowMouseMove;
                Application.Current.MainWindow.MouseLeave += progressBar.onWindowMouseLeave;
                progressBar.Cursor = Cursors.SizeWE;
            }

            else
            {
                Application.Current.MainWindow.MouseLeftButtonUp -= progressBar.onWindowLeftMouseUp;
                Application.Current.MainWindow.MouseMove -= progressBar.onWindowMouseMove;
                Application.Current.MainWindow.MouseLeave += progressBar.onWindowMouseLeave;
                progressBar.Cursor = Cursors.Arrow;
            }
        }

        public event EventHandler OnProgressBarUpdate;

        /// <summary>
        /// If any properties that are required to calculate the width is changed then this is called.
        /// </summary>
        static void ProgressPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            updateProgressBar((RoundedProgressBar)obj);
        }

        static void updateProgressBar(RoundedProgressBar progressBar)
        {
            if (progressBar.MinProgress > progressBar.MaxProgress || progressBar.Progress > progressBar.MaxProgress || progressBar.Progress < progressBar.MinProgress)
                return;

            var normProgress = calculateNormalizedProgress(progressBar.MinProgress, progressBar.MaxProgress, progressBar.Progress);
            progressBar.ProgressWidth = Convert.ToInt32(progressBar.ActualWidth * normProgress);
        }

        /// <returns>A number between 0 and 1</returns>
        static double calculateNormalizedProgress(double min, double max, double progress)
        {
            var normalizedMaxvalue = max - (min); // 0 - maxvalue
            var normalizedProgress = progress - min;
            return  normalizedProgress / normalizedMaxvalue;
        }
    }
}
