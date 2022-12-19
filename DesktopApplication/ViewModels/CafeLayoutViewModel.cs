using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace CSWBManagementApplication.ViewModels
{
    internal class CafeLayoutViewModel : ViewModelBase
    {

        private bool editted;

        public event EventHandler<bool> OnEdittedChange;

        public bool Editted
        {
            get => editted;
            set
            {
                if (editted != value)
                {
                    editted = value;
                    OnEdittedChange?.Invoke(this, editted);
                }
            }
        }

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
                    Editted = true;
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

        private Privilege privilege;

        public CafeLayoutViewModel(Cafe cafe, Privilege privilege)
        {
            this.Cafe = cafe;
            this.privilege = privilege;
            Initialize();
        }

        private async void Initialize()
        {
            await cafe.GetCafeFloorsInfo();
            UpdateFloorsList();

            Editted = false;
        }

        private void ClipboardFloor_OnClick(object sender, List<Position> e)
        {
            PasteLayout(e);
        }

        #region Command

        public ICommand AddFloorCommand
        {
            get => new CommandBase(AddFloor);
        }

        private void AddFloor()
        {
            Floors.Add(new Floor(Floors.Count + 1, ""));
            Editted = true;
            Floors.Sort((f1, f2) => (f2.FloorNumber.CompareTo(f1.FloorNumber)));
            UpdateFloorLayoutsList();
        }

        public ICommand RemoveFloorCommand
        {
            get => new CommandBase(RemoveFloor);
        }

        private void RemoveFloor()
        {
            if (CurrentFloor > 0 && CurrentFloor <= Floors.Count)
            {
                int index = Floors.Count - CurrentFloor;
                Floors.RemoveAt(index);
                for (int i = index - 1; i >= 0; i--)
                {
                    Floors[i].FloorNumber--;
                }
                Editted = true;
                UpdateFloorLayoutsList();
            }
        }

        public ICommand ClearTableCommand
        {
            get => new CommandBase(() =>
            {
                CurrentEditableFloorLayoutViewModel.ClearTables();
                Editted = true;
            });
        }

        public ICommand CopyCommand
        {
            get => new CommandBase(CopyLayout);
        }

        private void CopyLayout()
        {
            if (CurrentFloor > 0 && CurrentFloor <= Floors.Count)
            {
                FloorsClipboard.Add(new ViewOnlyFloorLayoutViewModel(Floors[CurrentFloorIndex].Tables));
                FloorsClipboard.Last().OnClick += ClipboardFloor_OnClick;
                OnPropertyChanged(nameof(FloorsClipboard));
            }
        }

        public ICommand ClearClipboardCommand
        {
            get => new CommandBase(() =>
            {
                FloorsClipboard.Clear();
            });
        }

        public void PasteLayout(List<Position> tables)
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
                Editted = true;
            }
        }

        public ICommand SaveCommand
        {
            get => new CommandBase(() =>
            {
                SaveLayout();
            });
        }

        public void SaveLayout()
        {
            if (!Editted)
            {
                return;
            }
            this.Cafe.Floors = new List<Floor>(Floors);
            this.Cafe.UploadFloors();
            Editted = false;
        }

        public ICommand DiscardCommand
        {
            get => new CommandBase(DiscardChanges);
        }

        private void DiscardChanges()
        {
            UpdateFloorsList();
            Editted = false;
        }

        #endregion Command
    }
}