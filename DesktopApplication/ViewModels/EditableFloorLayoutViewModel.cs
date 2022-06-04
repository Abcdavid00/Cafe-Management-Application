using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CSWBManagementApplication.ViewModels
{
    internal class EditableFloorLayoutViewModel : ViewModelBase
    {
        private ObservableCollection<FloorTileViewModel> floorTiles;

        public ObservableCollection<FloorTileViewModel> FloorTiles
        {
            get => floorTiles;
            set
            {
                floorTiles = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler<Position> TileClicked;

        private void OnFloorTileClicked(Position pos)
        {
            TileClicked?.Invoke(this, pos);
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

        private Floor floor;

        public EditableFloorLayoutViewModel(Floor floor)
        {
            this.floor = floor;
            int count = App.FLOOR_TILES_PER_FLOOR_LINE * App.FLOOR_TILES_PER_FLOOR_LINE;
            FloorTiles = new ObservableCollection<FloorTileViewModel>();
            for (int i = 0; i < count; i++)
            {
                Position pos = IndexToPostion(i);
                FloorTiles.Add(new FloorTileViewModel(new CommandBase(() =>
                {
                    OnFloorTileClicked(pos);
                })));
            }
            foreach (Position table in floor.Tables)
            {
                FloorTiles[PositionToIndex(table)].HasTable = true;
            }
        }

        public void ToggleTable(Position position)
        {
            if (!floor.Tables.Where<Position>(p => p == position).Any())
            {
                floor.Tables.Add(position);
                FloorTiles[PositionToIndex(position)].HasTable = true;
            }
            else
            {
                floor.Tables.Remove(floor.Tables.Where<Position>(p => p == position).Single());
                FloorTiles[PositionToIndex(position)].HasTable = false;
            }
        }

        public void ClearTables()
        {
            floor.Tables.Clear();
            foreach (FloorTileViewModel tile in FloorTiles.Where<FloorTileViewModel>(f => f.HasTable))
            {
                tile.HasTable = false;
            }
        }
    }
}