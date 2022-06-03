using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;


namespace CSWBManagementApplication.ViewModels
{
    internal class EditableFloorLayoutViewModel : ViewModelBase
    {
        

        private List<FloorTileViewModel> floorTiles;
        public List<FloorTileViewModel> FloorTiles
        {
            get => floorTiles;
            set
            {
                floorTiles = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler<Cafe.Position> TileClicked;
        
        private void OnFloorTileClicked(Cafe.Position pos)
        {
            TileClicked?.Invoke(this, pos);
        }

        private Cafe.Position IndexToPostion(int index)
        {
            int count = App.FLOOR_TILES_PER_FLOOR_LINE;
            int x = index % count;
            int y = index / count;
            return new Cafe.Position(x, y);
        }

        private int PositionToIndex(Cafe.Position position)
        {
            return PositionToIndex(position.X, position.Y);
        }

        private int PositionToIndex(int x, int y)
        {
            int count = App.FLOOR_TILES_PER_FLOOR_LINE;
            return y * count + x;
        }

        private Cafe.Floor floor;
        
        public EditableFloorLayoutViewModel(Cafe.Floor floor)
        {
            this.floor = floor;
            int count = App.FLOOR_TILES_PER_FLOOR_LINE * App.FLOOR_TILES_PER_FLOOR_LINE;
            FloorTiles = new List<FloorTileViewModel>();
            for (int i = 0; i < count; i++)
            {
                Cafe.Position pos = IndexToPostion(i);
                FloorTiles.Add(new FloorTileViewModel(new CommandBase(() =>
                {                    
                    OnFloorTileClicked(pos);
                })));
            }
            foreach (Cafe.Position table in floor.Tables)
            {
                FloorTiles[PositionToIndex(table)].HasTable = true;
            }
        }

        public void ToggleTable(Cafe.Position position)
        {
            if (!floor.Tables.Where<Cafe.Position>(p => p == position).Any())
            {
                floor.Tables.Add(position);
                FloorTiles[PositionToIndex(position)].HasTable = true;
            } else
            {
                floor.Tables.Remove(floor.Tables.Where<Cafe.Position>(p => p == position).Single());
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
