﻿using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.ViewModels
{
    internal class OrderFloorLayoutViewModel : ViewModelBase
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

        public OrderFloorLayoutViewModel(Floor floor, int startTableIndex)
        {
            floor.Tables.Sort((p1, p2) => p1.CompareTo(p2));
            int count = App.FLOOR_TILES_PER_FLOOR_LINE * App.FLOOR_TILES_PER_FLOOR_LINE;
            FloorTiles = new ObservableCollection<FloorTileViewModel>();
            for (int i = 0; i < count; i++)
            {
                Position pos = IndexToPostion(i);
                FloorTiles.Add(new FloorTileViewModel());
            }
            foreach (Position table in floor.Tables)
            {
                FloorTiles[PositionToIndex(table)].HasTable = true;
                FloorTiles[PositionToIndex(table)].Content = startTableIndex.ToString();
                startTableIndex++;
                FloorTiles[PositionToIndex(table)].Command = new CommandBase(() => OnFloorTileClicked(table));
            }
        }
    }
}
