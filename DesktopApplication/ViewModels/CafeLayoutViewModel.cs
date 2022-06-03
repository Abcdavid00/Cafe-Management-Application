using CSWBManagementApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CSWBManagementApplication.Resources;
using System.Windows.Input;
using CSWBManagementApplication.Commands;

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

        private Cafe cafe;
        public Cafe Cafe
        {
            get => cafe;
            private set
            {
                cafe = value;
            }

        }

        private List<Cafe.Floor> floors;
        public List<Cafe.Floor> Floors
        {
            get => floors;
            private set
            {
                floors = value;
                OnPropertyChanged();
            }
        }

        private List<EditableFloorLayoutViewModel> editableFloorLayoutViewModels;
        public List<EditableFloorLayoutViewModel> EditableFloorLayoutViewModels
        {
            get => editableFloorLayoutViewModels;
            private set
            {
                editableFloorLayoutViewModels = value;
                OnPropertyChanged();
            }
        }

        private List<FloorButtonViewModel> floorButtons;
        public List<FloorButtonViewModel> FloorButtons
        {
            get => floorButtons;
            private set
            {
                floorButtons = value;
                OnPropertyChanged(nameof(FloorButtons));
            }
        }

        private void UpdateFloorList()
        {
            FloorButtons?.Clear();
            EditableFloorLayoutViewModels?.Clear();
            //List<FloorButtonViewModel> temp = new List<FloorButtonViewModel>();        
            //foreach (Cafe.Floor floor in Cafe.Floors)
            //{
            //    temp.Add(new FloorButtonViewModel(floor.FloorNumber.ToString(), floor.FloorName, new CommandBase(() =>
            //    {
            //        CurrentFloor = floor.FloorNumber;
            //    })));
            //}            
            FloorButtons = Floors.Select(floor => new FloorButtonViewModel(floor.FloorNumber.ToString(), floor.FloorName, new CommandBase(() =>
            {
                CurrentFloor = floor.FloorNumber;
            }))).ToList();
            EditableFloorLayoutViewModels = Floors.Select(floor => new EditableFloorLayoutViewModel(floor)).ToList();
            foreach (EditableFloorLayoutViewModel floor in EditableFloorLayoutViewModels)
            {
                floor.TileClicked += (object sender, Cafe.Position position) =>
                {
                    floor.ToggleTable(position);
                };
            }

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

                if (currentFloor > 0 && currentFloor <= Floors.Count)
                {
                    FloorButtons[CurrentFloorIndex].IsActive = false;
                }
               
                currentFloor = value;

                if (currentFloor > 0 && currentFloor <= Floors.Count)
                {
                    FloorButtons[CurrentFloorIndex].IsActive = true;
                    OnPropertyChanged(nameof(CurrentFloorText));
                    CurrentEditableFloorLayoutViewModel = EditableFloorLayoutViewModels[CurrentFloorIndex];
                } else
                {
                    CurrentEditableFloorLayoutViewModel = null;
                }
                
                
                
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
            Floors = new List<Cafe.Floor>(cafe.Floors);
            if (Floors.Any())
            {
                CurrentFloor = Floors.First().FloorNumber;
            } else
            {
                CurrentFloor = 0;
            }
        }      

        #region Command
        
        public ICommand AddFloorCommand
        {
            get => new CommandBase(() =>
            {
                Floors.Add(new Cafe.Floor(Floors.Count + 1,""));
                Floors.Sort((f1, f2) => (f2.FloorNumber.CompareTo(f1.FloorNumber)));
                UpdateFloorList();
                CurrentFloor = Floors.First().FloorNumber;
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
                    for (int i = index-1; i >=0; i--)
                    {
                        Floors[i].FloorNumber--;
                    }
                    UpdateFloorList();
                    if (Floors.Any())
                    {
                        CurrentFloor = Floors.First().FloorNumber;
                    }
                    else
                    {
                        CurrentFloor = 0;
                    }
                }
            });
        }

        public ICommand ClearTableCommand
        {
            get => new CommandBase(() =>
            {
                CurrentEditableFloorLayoutViewModel.ClearTables();
            });
        }

        #endregion
    }

}
