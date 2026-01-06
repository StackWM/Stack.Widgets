namespace LostTech.Stack.Widgets.DataSources
{
    using System.Windows;
    using System.Windows.Threading;

    public sealed class DataSource
    {
        public static Duration GetRefreshInterval(DependencyObject obj) =>
            (Duration)obj.GetValue(RefreshIntervalProperty);
        public static void SetRefreshInterval(DependencyObject obj, Duration value) =>
            obj.SetValue(RefreshIntervalProperty, value);

        public static readonly DependencyProperty RefreshIntervalProperty =
            DependencyProperty.RegisterAttached("RefreshInterval",
                typeof(Duration), typeof(DependencyObject),
                new PropertyMetadata(Duration.Forever, propertyChangedCallback: OnRefreshIntervalChanged));

        static readonly DependencyPropertyKey RefresherPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("__Refresher__",
                typeof(Refresher), typeof(DependencyObject), new PropertyMetadata(null));

        static void OnRefreshIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs change) {
            if (d is not IRefreshable refreshable)
                return;

            var refresher = (Refresher?)d.GetValue(RefresherPropertyKey.DependencyProperty);
            Duration value = change.NewValue is Duration duration ? duration : Duration.Forever;

            if (value.HasTimeSpan) {
                if (refresher == null) {
                    refresher = new Refresher(refreshable);
                    refresher.Timer.Tick += RefreshTimerOnTick;
                    d.SetValue(RefresherPropertyKey, refresher);
                }

                refresher.Timer.Interval = value.TimeSpan;
                refresher.Timer.IsEnabled = true;
            } else if (refresher != null) {
                refresher.Dispose();
                d.SetValue(RefresherPropertyKey.DependencyProperty, null);
            }
        }

        static void RefreshTimerOnTick(object? sender, EventArgs e) {
            var timer = (DispatcherTimer)sender!;
            var self = (WeakReference)timer.Tag;
            if (self.Target is not Refresher refresher) {
                timer.Stop();
                return;
            }
            var refreshable = refresher.Refreshable;
            if (refreshable.RefreshCommand.CanExecute(null)) {
                Trace.WriteLine($"refreshing {refreshable}");
                refreshable.RefreshCommand.Execute(null);
            }
        }
    }
}
