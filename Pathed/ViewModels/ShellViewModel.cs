using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Pathed.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Pathed.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.Shared)]
    public class ShellViewModel : ViewModelBase
    {
        private IDialogService dialogService;
        private IEnvironmentService environmentService;
        private ISecurityService securityService;
        private IApplicationService applicationService;
        private ISettingsService settingsService;
        private IWatchService watchService;
        private IPathHistoryService pathHistoryService;

        private ObservableCollection<EnvironmentTargetViewModel> targets;
        private Object targetsLock = new Object();
        public ICollectionView TargetsView { get; protected set; }

        private string title;
        public string Title
        {
            get { return this.title; }
            set
            {
                if (this.title != value)
                {
                    this.title = value;

                    RaisePropertyChanged();
                }
            }
        }

        public double WindowHeight
        {
            get { return this.settingsService.Get<double>("WindowHeight", 480); }
            set { this.settingsService.Set<double>("WindowHeight", value); }
        }

        public double WindowWidth
        {
            get { return this.settingsService.Get<double>("WindowWidth", 640); }
            set { this.settingsService.Set<double>("WindowWidth", value); }
        }

        public double WindowTop
        {
            get { return this.settingsService.Get<double>("WindowTop", 0); }
            set { this.settingsService.Set<double>("WindowTop", value); }
        }

        public double WindowLeft
        {
            get { return this.settingsService.Get<double>("WindowLeft", 0); }
            set { this.settingsService.Set<double>("WindowLeft", value); }
        }
        
        public WindowState WindowState
        {
            get { return this.settingsService.Get<WindowState>("WindowState", WindowState.Normal); }
            set { this.settingsService.Set<WindowState>("WindowState", value); }
        }

        [ImportingConstructor]
        public ShellViewModel(IDialogService dialogService, IEnvironmentService environmentService,
            ISecurityService securityService, IApplicationService applicationService, IWatchService watchService,
            ISettingsService settingsService, IPathHistoryService pathHistoryService)
        {
            this.dialogService = dialogService;
            this.environmentService = environmentService;
            this.securityService = securityService;
            this.applicationService = applicationService;
            this.settingsService = settingsService;
            this.watchService = watchService;
            this.pathHistoryService = pathHistoryService;

            Title = String.Format("{0} v{1}", this.applicationService.GetTitle(), this.applicationService.GetVersion().ToString(2));

            this.targets = new ObservableCollection<EnvironmentTargetViewModel>(CreateEnvironments());
            BindingOperations.EnableCollectionSynchronization(this.targets, this.targetsLock);
            TargetsView = CollectionViewSource.GetDefaultView(this.targets);

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                EnvironmentVariableTarget target;
                if (Enum.TryParse<EnvironmentVariableTarget>(args[1], out target))
                {
                    TargetsView.MoveCurrentTo(this.targets.Single(x => x.Target == target));
                }
            }
        }

        private IEnumerable<EnvironmentTargetViewModel> CreateEnvironments()
        {
            foreach (var target in Enum.GetValues(typeof(EnvironmentVariableTarget)))
            {
                yield return new EnvironmentTargetViewModel(this.dialogService,
                    this.environmentService,
                    this.securityService,
                    this.watchService,
                    this.pathHistoryService,
                    (EnvironmentVariableTarget)target);
            }
        }

        private RelayCommand<CancelEventArgs> closingEvent;
        public ICommand ClosingEvent
        {
            get { return this.closingEvent ?? (this.closingEvent = new RelayCommand<CancelEventArgs>(async (e) => await HandleClosingEvent(e))); }
        }
        
        private async Task HandleClosingEvent(CancelEventArgs args)
        {
            if (this.targets.Any(x => x.IsDirty))
            {
                var result = this.dialogService.ShowDialog("Pending Changes", "You have unsaved changes, do you want to save before exiting?", DialogButton.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    foreach (var target in this.targets.Where(x => x.IsDirty))
                    {
                        await target.SaveAsync();
                    }
                } else if (result == DialogResult.Cancel)
                {
                    args.Cancel = true;
                }
            }
        }
    }
}
