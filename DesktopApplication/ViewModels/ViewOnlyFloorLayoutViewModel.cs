using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
    internal class ViewOnlyFloorLayoutViewModel : ViewModelBase
    {
        private ObservableCollection<ViewOnlyFloorTileViewModel> floorTiles;

        public ObservableCollection<ViewOnlyFloorTileViewModel> FloorTiles
        {
            get => floorTiles;
        }

        private Position IndexToPostion(int index)
        {
            int count = App.FLOOR_TILES_PER_FLOOR_LINE;
            int x = index % count;
            int y = index / count;
            return new Position(x, y);
        }

        private int PositionToIndex(Position position)
        {
            return PositionToIndex(position.X, position.Y);
        }

        private int PositionToIndex(int x, int y)
        {
            int count = App.FLOOR_TILES_PER_FLOOR_LINE;
            return y * count + x;
        }

        public event EventHandler<List<Position>> OnClick;

        public ICommand Command
        {
            get => new CommandBase(() => OnClick?.Invoke(this, new List<Position>(Tables)));
        }

        private List<Position> tables;

        public List<Position> Tables
        {
            get => new List<Position>(tables);
        }

        public ViewOnlyFloorLayoutViewModel(List<Position> tables)
        {
            this.tables = new List<Position>(tables);
            int count = App.FLOOR_TILES_PER_FLOOR_LINE * App.FLOOR_TILES_PER_FLOOR_LINE;
            floorTiles = new ObservableCollection<ViewOnlyFloorTileViewModel>();
            for (int i = 0; i < count; i++)
            {
                Position pos = IndexToPostion(i);
                FloorTiles.Add(new ViewOnlyFloorTileViewModel());
            }
            foreach (Position table in this.tables)
            {
                FloorTiles[PositionToIndex(table)].HasTable = true;
            }
        }
    }
}