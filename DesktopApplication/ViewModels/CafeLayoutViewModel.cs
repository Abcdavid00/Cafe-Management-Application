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

        private List<FloorButtonViewModel> floors;
        public List<FloorButtonViewModel> Floors
        {
            get => floors;
            private set
            {
                floors = value;
                OnPropertyChanged(nameof(Floors));
            }
        }

        private void UpdateFloorList()
        {
            Floors?.Clear();
            //List<FloorButtonViewModel> temp = new List<FloorButtonViewModel>();        
            //foreach (Cafe.Floor floor in Cafe.Floors)
            //{
            //    temp.Add(new FloorButtonViewModel(floor.FloorNumber.ToString(), floor.FloorName, new CommandBase(() =>
            //    {
            //        CurrentFloor = floor.FloorNumber;
            //    })));
            //}            
            Floors = Cafe.Floors.Select(floor => new FloorButtonViewModel(floor.FloorNumber.ToString(), floor.FloorName, new CommandBase(() =>
            {
                CurrentFloor = floor.FloorNumber;
            }))).ToList();

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
                    Floors[CurrentFloorIndex].IsActive = false;
                }
               
                currentFloor = value;
                Floors[CurrentFloorIndex].IsActive = true;
                
                OnPropertyChanged(nameof(CurrentFloorText));
                CurrentEditableFloorLayoutViewModel = new EditableFloorLayoutViewModel(Cafe.Floors[CurrentFloorIndex]);
                OnPropertyChanged();
                
            }
        }
        public int CurrentFloorIndex
        {
            get => Floors.Count - currentFloor;
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
        }
            
        

        #region Command
        
        public ICommand AddFloorCommand
        {
            get => new CommandBase(() =>
            {
                Cafe.Floors.Add(new Cafe.Floor(Cafe.Floors.Count + 1,""));
                Cafe.Floors.Sort((f1, f2) => (f2.FloorNumber.CompareTo(f1.FloorNumber)));
                UpdateFloorList();
                CurrentFloor = Cafe.Floors.First().FloorNumber;
            });      
            
        }


        #endregion
    }

}
