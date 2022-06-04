using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace CSWBManagementApplication.ViewModels
{
    internal class CafeLayoutViewModel : ViewModelBase
    {
        internal class FloorButtonViewModel : ViewModelBase
        {
            private bool isActive;

            public bool IsActive
            {
                get => isActive;
                set
                {
                    isActive = value;
                    OnPropertyChanged(nameof(Background));
                    OnPropertyChanged(nameof(Foreground));
                    OnPropertyChanged();
                }
            }

            public Brush Foreground
            {
                get => IsActive ? DarkTheme.SolidDark : DarkTheme.SolidMain;
            }

            public Brush Background
            {
                get => IsActive ? DarkTheme.SolidMain : new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }

            private ICommand command;

            public ICommand Command
            {
                get => command;
                private set
                {
                    command = value;
                    OnPropertyChanged();
                }
            }

            private string content;

            public string Content
            {
                get => content;
                set
                {
                    content = value;
                    OnPropertyChanged();
                }
            }

            private string toolTip;

            public string ToolTip
            {
                get => toolTip;
                set
                {
                    toolTip = value;
                    OnPropertyChanged();
                }
            }

            public FloorButtonViewModel(string content, string toolTip, ICommand command, bool isActive = false)
            {
                this.content = content;
                this.toolTip = toolTip;
                this.command = command;
                this.isActive = isActive;
            }
        }

        private bool changed;

        private Cafe cafe;

        public Cafe Cafe
        {
            get => cafe;
            private set
            {
                cafe = value;
            }
        }

        public ObservableCollection<ViewOnlyFloorLayoutViewModel> FloorsClipboard
        {
            get => App.FloorsClipboard;
        }

        private List<Floor> floors;

        public List<Floor> Floors
        {
            get => floors;
            private set
            {
                floors = value;
                UpdateFloorLayoutsList();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<EditableFloorLayoutViewModel> editableFloorLayoutViewModels;

        public ObservableCollection<EditableFloorLayoutViewModel> EditableFloorLayoutViewModels
        {
            get => editableFloorLayoutViewModels;
            private set
            {
                editableFloorLayoutViewModels = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<FloorButtonViewModel> floorButtons;

        public ObservableCollection<FloorButtonViewModel> FloorButtons
        {
            get => floorButtons;
            private set
            {
                floorButtons = value;
                OnPropertyChanged(nameof(FloorButtons));
            }
        }

        private void UpdateFloorLayoutsList()
        {
            FloorButtons?.Clear();
            CurrentFloor = 0;
            EditableFloorLayoutViewModels?.Clear();
            FloorButtons = new ObservableCollection<FloorButtonViewModel>(Floors.Select(floor => new FloorButtonViewModel(floor.FloorNumber.ToString(), floor.FloorName, new CommandBase(() =>
            {
                CurrentFloor = floor.FloorNumber;
            }))));
            EditableFloorLayoutViewModels = new ObservableCollection<EditableFloorLayoutViewModel>(Floors.Select(floor => new EditableFloorLayoutViewModel(floor)));
            foreach (EditableFloorLayoutViewModel floor in EditableFloorLayoutViewModels)
            {
                floor.TileClicked += (object sender, Position position) =>
                {
                    floor.ToggleTable(position);
                    changed = true;
                };
            }
            if (Floors.Any())
            {
                CurrentFloor = Floors.First().FloorNumber;
            }
            else
            {
                CurrentFloor = 0;
            }
            OnPropertyChanged(nameof(CurrentEditableFloorLayoutViewModel));
        }

        private void UpdateFloorsList()
        {
            Floors?.Clear();
            Floors = (
                from floor in Cafe.Floors
                select new Floor(floor)
            ).ToList();
            foreach (ViewOnlyFloorLayoutViewModel floor in FloorsClipboard)
            {
                floor.OnClick += ClipboardFloor_OnClick;
            }
            UpdateFloorLayoutsList();
        }

        private int currentFloor;

        public int CurrentFloor
        {
            get => currentFloor;
            set
            {
                if (currentFloor == value)
                {
                    return;
                }

                if (currentFloor > 0 && currentFloor <= FloorButtons.Count)
                {
                    FloorButtons[CurrentFloorIndex].IsActive = false;
                }

                currentFloor = value;

                if (currentFloor > 0 && currentFloor <= FloorButtons.Count)
                {
                    FloorButtons[CurrentFloorIndex].IsActive = true;
                    OnPropertyChanged(nameof(CurrentFloorText));
                    CurrentEditableFloorLayoutViewModel = EditableFloorLayoutViewModels[CurrentFloorIndex];
                }
                else
                {
                    CurrentEditableFloorLayoutViewModel = null;
                }
                OnPropertyChanged(nameof(CurrentFloorIndex));
                OnPropertyChanged(nameof(CurrentFloorText));
                OnPropertyChanged();
            }
        }

        public int CurrentFloorIndex
        {
            get => FloorButtons.Count - currentFloor;
        }

        public string CurrentFloorText
        {
            get => currentFloor.ToString();
        }

        private EditableFloorLayoutViewModel currentEditableFloorLayoutViewModel;

        public EditableFloorLayoutViewModel CurrentEditableFloorLayoutViewModel
        {
            get => currentEditableFloorLayoutViewModel;
            private set
            {
                currentEditableFloorLayoutViewModel = value;
                OnPropertyChanged();
            }
        }

        public CafeLayoutViewModel(Cafe cafe)
        {
            this.Cafe = cafe;
            Initialize();
        }

        private async void Initialize()
        {
            await cafe.GetCafeFloorsInfo();
            UpdateFloorsList();

            changed = false;
        }

        private void ClipboardFloor_OnClick(object sender, List<Position> e)
        {
            Paste(e);
        }

        #region Command

        public ICommand AddFloorCommand
        {
            get => new CommandBase(() =>
            {
                Floors.Add(new Floor(Floors.Count + 1, ""));
                changed = true;
                Floors.Sort((f1, f2) => (f2.FloorNumber.CompareTo(f1.FloorNumber)));
                UpdateFloorLayoutsList();
            });
        }

        public ICommand RemoveFloorCommand
        {
            get => new CommandBase(() =>
            {
                if (CurrentFloor > 0 && CurrentFloor <= Floors.Count)
                {
                    int index = Floors.Count - CurrentFloor;
                    Floors.RemoveAt(index);
                    for (int i = index - 1; i >= 0; i--)
                    {
                        Floors[i].FloorNumber--;
                    }
                    changed = true;
                    UpdateFloorLayoutsList();
                }
            });
        }

        public ICommand ClearTableCommand
        {
            get => new CommandBase(() =>
            {
                CurrentEditableFloorLayoutViewModel.ClearTables();
                changed = true;
            });
        }

        public ICommand CopyCommand
        {
            get => new CommandBase(() =>
            {
                if (CurrentFloor > 0 && CurrentFloor <= Floors.Count)
                {
                    FloorsClipboard.Add(new ViewOnlyFloorLayoutViewModel(Floors[CurrentFloorIndex].Tables));
                    FloorsClipboard.Last().OnClick += ClipboardFloor_OnClick;
                    OnPropertyChanged(nameof(FloorsClipboard));
                }
            });
        }

        public ICommand ClearClipboardCommand
        {
            get => new CommandBase(() =>
            {
                FloorsClipboard.Clear();
            });
        }

        public void Paste(List<Position> tables)
        {
            if (CurrentFloor > 0 && CurrentFloor <= Floors.Count)
            {
                Floors[CurrentFloorIndex].Tables = tables;
                EditableFloorLayoutViewModels[CurrentFloorIndex] = new EditableFloorLayoutViewModel(Floors[CurrentFloorIndex]);
                CurrentEditableFloorLayoutViewModel = EditableFloorLayoutViewModels[CurrentFloorIndex];
                CurrentEditableFloorLayoutViewModel.TileClicked += (object sender, Position position) =>
                {
                    CurrentEditableFloorLayoutViewModel.ToggleTable(position);
                };
                changed = true;
            }
        }

        public ICommand SaveCommand
        {
            get => new CommandBase(() =>
            {
                Save();
            });
        }

        public void Save()
        {
            if (!changed)
            {
                return;
            }
            this.Cafe.Floors = new List<Floor>(Floors);
            this.Cafe.UploadFloors();
            changed = false;
        }

        public ICommand DiscardCommand
        {
            get => new CommandBase(() =>
            {
                UpdateFloorsList();
                changed = false;
            });
        }

        #endregion Command
    }
}