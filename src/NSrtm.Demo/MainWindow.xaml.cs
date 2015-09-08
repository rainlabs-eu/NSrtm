using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using JetBrains.Annotations;
using ReactiveUI;

namespace NSrtm.Demo
{
    internal partial class MainWindow : IViewFor<DemoViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(onActivation);
        }

        [NotNull]
        private IEnumerable<IDisposable> onActivation()
        {
            yield return this.BindCommand(ViewModel, vm => vm.RetrieveElevationsCommand, v => v.ShowElevationsButton);

            yield return this.OneWayBind(ViewModel, vm => vm.AvailableElevationProviders, v => v.ElevationSourceCombo.ItemsSource);
            yield return this.Bind(ViewModel, vm => vm.SelectedElevationProvider, v => v.ElevationSourceCombo.SelectedItem);

            yield return this.Bind(ViewModel, vm => vm.CenterLatitude, v => v.LatitudeSlider.Value);
            yield return this.OneWayBind(ViewModel, vm => vm.CenterLatitude, v => v.LatitudeLabel.Content);

            yield return this.Bind(ViewModel, vm => vm.CenterLongitude, v => v.LongitudeSlider.Value);
            yield return this.OneWayBind(ViewModel, vm => vm.CenterLongitude, v => v.LongitudeLabel.Content);

            yield return this.Bind(ViewModel, vm => vm.RangeDegree, v => v.DegreeRangeSlider.Value);
            yield return this.OneWayBind(ViewModel, vm => vm.RangeDegree, v => v.DegreeRangeLabel.Content);

            yield return this.Bind(ViewModel, vm => vm.MinVisualizedHeight, v => v.VisualizedHeightSlider.LowerValue);
            yield return this.Bind(ViewModel, vm => vm.MaxVisualizedHeight, v => v.VisualizedHeightSlider.UpperValue);

            yield return this.OneWayBind(ViewModel, vm => vm.Progress, v => v.ProgressBar.Value);

            yield return this.OneWayBind(ViewModel, vm => vm.ElevationBitmap, v => v.ElevationImage.Source);
            yield return this.OneWayBind(ViewModel, vm => vm.ElevationValueStats, v => v.StatisticsTextBlock.Text, stats => stats.ToString());

            yield return UserError.RegisterHandler(ue => Observable.Return(ue)
                                                                   .ObserveOn(RxApp.MainThreadScheduler)
                                                                   .Select(askForUserInput));
        }

        private RecoveryOptionResult askForUserInput(UserError ue)
        {
            if (ue.InnerException != null) MessageBox.Show(ue.InnerException.Message, ue.ErrorMessage);
            else
            {
                MessageBox.Show(ue.ErrorMessage);
            }
            return RecoveryOptionResult.FailOperation;
        }

        #region IViewFor<T> implementation

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
                                                                                                  typeof(DemoViewModel),
                                                                                                  typeof(MainWindow));

        [CanBeNull] public DemoViewModel ViewModel { get { return (DemoViewModel)GetValue(ViewModelProperty); } set { SetValue(ViewModelProperty, value); } }

        [CanBeNull] object IViewFor.ViewModel { get { return ViewModel; } set { ViewModel = (DemoViewModel)value; } }

        #endregion
    }
}
