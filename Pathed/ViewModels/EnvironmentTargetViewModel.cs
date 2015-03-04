using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Pathed.Models;
using Pathed.Services;

namespace Pathed.ViewModels
{
    public class EnvironmentTargetViewModel : ViewModelBase
    {
        private EnvironmentVariableTarget target = EnvironmentVariableTarget.User;
        public EnvironmentVariableTarget Target
        {
            get { return this.target; }
            set
            {
                if (this.target != value)
                {
                    this.target = value;

                    RaisePropertyChanged();
                    RaisePropertyChanged("HasAccess");
                    RaisePropertyChanged("HaveNoAccess");

                    GetPathsAsync();
                }
            }
        }

        public string PathEnvironmentVariable
        {
            get { return string.Join(";", this.paths.Select(x => x.Value)); }
        }

        private bool isDirty = false;
        public bool IsDirty
        {
            get { return this.isDirty; }
            set
            {
                if (this.isDirty != value)
                {
                    this.isDirty = value;

                    RaisePropertyChanged();

                    if (this.revertChangesCommand != null)
                        this.revertChangesCommand.RaiseCanExecuteChanged();

                    if (this.saveCommand != null)
                        this.saveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool HasAccess
        {
            get { return (Target == EnvironmentVariableTarget.Machine) ? this.securityService.IsAdministrator() : true; }
        }

        public bool HaveNoAccess
        {
            get { return !HasAccess; }
        }

        private ObservableCollection<PathViewModel> paths;
        private Object pathsLock = new Object();
        public ICollectionView Paths { get; protected set; }
        private bool pathsChanged = false;

        private IDialogService dialogService;
        private IEnvironmentService environmentService;
        private ISecurityService securityService;
        private IWatchService watchService;
        private IPathHistoryService pathHistoryService;

        private ObservableCollection<PathViewModel> selectedPaths;

        private ObservableCollection<PathHistoryEntry> pathHistory;
        private Object pathHistoryLock = new Object();
        public ICollectionView PathHistoryView { get; protected set; }

        public RelayCommand<IList> SelectionChangedCommand { get; protected set; }

        public EnvironmentTargetViewModel(IDialogService dialogService, IEnvironmentService environmentService,
            ISecurityService securityService, IWatchService watchService, IPathHistoryService pathHistoryService, EnvironmentVariableTarget target)
        {
            this.dialogService = dialogService;
            this.environmentService = environmentService;
            this.securityService = securityService;
            this.watchService = watchService;
            this.pathHistoryService = pathHistoryService;
            this.target = target;

            this.paths = new ObservableCollection<PathViewModel>();
            this.paths.CollectionChanged += (s, e) =>
            {
                RaisePropertyChanged("PathEnvironmentVariable");

                if (!this.pathsChanged)
                    IsDirty = true;
            };
            BindingOperations.EnableCollectionSynchronization(this.paths, this.pathsLock);
            Paths = CollectionViewSource.GetDefaultView(this.paths);
            Paths.SortDescriptions.Add(new SortDescription("Value", ListSortDirection.Ascending));

            this.selectedPaths = new ObservableCollection<PathViewModel>();
            this.selectedPaths.CollectionChanged += (s, e) => { if (this.removeCommand != null) this.removeCommand.RaiseCanExecuteChanged(); };
            SelectionChangedCommand = new RelayCommand<IList>((items) => this.selectedPaths.Update(items.Cast<PathViewModel>()));

            GetPathsAsync();

            this.watchService.Watch("PATH", this.target, PathChanged);

            this.pathHistory = new ObservableCollection<PathHistoryEntry>(this.pathHistoryService.Entries.Where(x => x.Target.Equals(this.target)));
            BindingOperations.EnableCollectionSynchronization(this.pathHistory, this.pathHistoryLock);
            PathHistoryView = CollectionViewSource.GetDefaultView(this.pathHistory);
        }
        
        private void PathChanged(string path, EnvironmentVariableTarget target)
        {
            Debug.WriteLine("Path Changed (" + target.ToString() + "): " + path);
            var paths = path.Split(';').Select(x => new PathViewModel(x) { IsSet = true });

            IEnumerable<PathViewModel> added;
            IEnumerable<PathViewModel> removed;
            if (this.paths.TryGetDifference(paths, out added, out removed, (x, y) => x.Any(z => z.Value.Equals(y.Value))))
            {
                this.pathsChanged = true;
                this.paths.Remove(removed);
                this.paths.Add(added);
                this.pathsChanged = false;
            }
        }

        private async Task GetPathsAsync()
        {
            this.paths.Update((await this.environmentService.GetPathsAsync(this.target)).Select(x => new PathViewModel(x)));

            await CheckDuplicates();

            IsDirty = false;
        }

        private RelayCommand removeCommand;
        public ICommand RemoveCommand
        {
            get { return this.removeCommand ?? (this.removeCommand = new RelayCommand(Remove, () => this.selectedPaths.Count > 0)); }
        }

        public void Remove()
        {
            this.paths.Remove(this.selectedPaths);
        }

        private RelayCommand removeNotExistingCommand;
        public ICommand RemoveNotExistingCommand
        {
            get { return this.removeNotExistingCommand ?? (this.removeNotExistingCommand = new RelayCommand(RemoveNotExisting, () => this.paths.Where(x => !x.Exists).Count() > 0)); }
        }

        public void RemoveNotExisting()
        {
            this.paths.Remove(this.paths.Where(x => !x.Exists));
        }

        private RelayCommand addCommand;
        public ICommand AddCommand
        {
            get { return this.addCommand ?? (this.addCommand = new RelayCommand(Add)); }
        }

        public void Add()
        {
            string path = this.dialogService.ShowBrowseFolderDialog();

            Add(path);
        }

        public void Add(string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                if (!this.paths.Any(x => x.Value.Equals(path, StringComparison.InvariantCultureIgnoreCase)))
                {
                    this.paths.Add(new PathViewModel(path, false));
                }
                else
                {
                    this.dialogService.ShowErrorMessage("Path already exists.");
                }
            }
        }

        private RelayCommand revertChangesCommand;
        public ICommand RevertChangesCommand
        {
            get
            {
                return this.revertChangesCommand ?? (this.revertChangesCommand = new RelayCommand(async () => { await GetPathsAsync(); }, () => IsDirty));
            }
        }

        private RelayCommand<PathHistoryEntry> revertToCommand;
        public ICommand RevertToCommand
        {
            get
            {
                return this.revertToCommand ?? (this.revertToCommand = new RelayCommand<PathHistoryEntry>(async (x) => { await RevertToAsync(x); }));
            }
        }

        private async Task RevertToAsync(PathHistoryEntry x)
        {
            this.pathHistoryService.ClearEntriesAfter(x);

            await this.environmentService.SetVariableAsync("PATH", x.Value, x.Target);
            await GetPathsAsync();
        }

        private RelayCommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return this.saveCommand ?? (this.saveCommand = new RelayCommand(async () => { await SaveAsync(); }, () => IsDirty));
            }
        }

        public async Task SaveAsync()
        {
            this.watchService.SetIsSaving("PATH", this.target);

            this.pathHistory.Add(this.pathHistoryService.Add(await this.environmentService.GetVariableAsync("PATH", this.target), this.target));

            await this.environmentService.SetPathsAsync(this.paths.Select(x => x.Value).ToArray(), this.target);

            foreach (var path in this.paths.Where(x => !x.IsSet))
            {
                path.IsSet = true;
            }

            IsDirty = false;
        }

        private RelayCommand elevateCommand;
        public ICommand ElevateCommand
        {
            get
            {
                return this.elevateCommand ?? (this.elevateCommand = new RelayCommand(Elevate, () => !this.securityService.IsAdministrator()));
            }
        }

        public void Elevate()
        {
            this.securityService.ElevateToAdministrator(this.target.ToString());
        }

        private RelayCommand<PathViewModel> openCommand;
        public ICommand OpenCommand
        {
            get
            {
                return this.openCommand ?? (this.openCommand = new RelayCommand<PathViewModel>(Open, x => x != null && x.Exists));
            }
        }

        public void Open(PathViewModel path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            this.dialogService.OpenFolder(path.Value);
        }

        private async Task CheckDuplicates()
        {
            foreach (var path in this.paths)
            {
                if (paths.Any(x => x != path && x.Value.Equals(path.Value)))
                    path.IsDuplicate = true;
            }
        }

        private RelayCommand removeDuplicatesCommand;
        public ICommand RemoveDuplicatesCommand
        {
            get
            {
                return this.removeDuplicatesCommand ?? (this.removeDuplicatesCommand = new RelayCommand(async () => await RemoveDuplicates(), () => this.paths.Any(x => x.IsDuplicate)));
            }
        }

        public async Task RemoveDuplicates()
        {
            var toRemove = new List<PathViewModel>();
            foreach (var path in this.paths)
            {
                if (toRemove.Contains(path))
                    continue;

                var duplicates = paths.Where(x => x != path && x.Value.Equals(path.Value)).ToArray();
                if (duplicates.Length > 0)
                {
                    toRemove.AddRange(duplicates);
                    path.IsDuplicate = false;
                }
            }

            this.paths.Remove(toRemove);
        }

        private RelayCommand<DragEventArgs> dropCommand;
        public ICommand DropCommand
        {
            get
            {
                return this.dropCommand ?? (this.dropCommand = new RelayCommand<DragEventArgs>(Drop));
            }
        }

        private void Drop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var directories = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var directory in directories)
                {
                    if (Directory.Exists(directory))
                        Add(directory);
                }

                e.Handled = true;
            }
        }

        private RelayCommand<DragEventArgs> dragOverCommand;
        public ICommand DragOverCommand
        {
            get
            {
                return this.dragOverCommand ?? (this.dragOverCommand = new RelayCommand<DragEventArgs>(DragOver));
            }
        }

        private void DragOver(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
    }
}
