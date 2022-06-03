using CSWBManagementApplication.Services;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace CSWBManagementApplication.Models
{
    [FirestoreData]
    internal class Cafe
    {
        [FirestoreData]
        public class Position
        {
            [FirestoreProperty]
            public int X
            {
                get;
                set;
            }

            [FirestoreProperty]
            public int Y
            {
                get;
                set;
            }

            [FirestoreProperty]
            public long SingleValue
            {
                get
                {
                    return Y + (X + Y + 1) * (X + Y) / 2;
                }
                set
                {
                    int degree = (int)Math.Floor((Math.Sqrt(value * 8 + 1) - 1) / 2);
                    long sum = (degree + 1) * degree / 2;
                    this.Y = (int)(value - sum);
                    this.X = degree - this.Y;
                }
            }

            public Position()
            {
                SingleValue = 0;
            }

            public Position(long singleValue)
            {
                this.SingleValue = singleValue;
            }
            
            public Position(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
            
            public static bool operator==(Position a, Position b)
            {
                return a.X == b.X && a.Y == b.Y;
            }

            public static bool operator !=(Position a, Position b)
            {
                return a.X != b.X || a.Y != b.Y;
            }
        }

        [FirestoreData]
        public class FloorArea
        {
            [FirestoreProperty]
            public Position Position
            {
                get;
                set;
            }

            [FirestoreProperty]
            public int Width
            {
                get;
                set;
            }

            [FirestoreProperty]
            public int Height
            {
                get;
                set;
            }

            public FloorArea()
            {
                Position = new Position();
                Width = 0;
                Height = 0;
            }

            public FloorArea(Position position, int width, int height)
            {
                Position = position;
                Width = width;
                Height = height;
            }

            public bool Contain(Position position)
            {
                return position.X >= Position.X && position.X <= Position.X + Width &&
                       position.Y >= Position.Y && position.Y <= Position.Y + Height;
            }
        }

        [FirestoreData]
        public class Floor
        {
            [FirestoreDocumentId]
            public string FloorID { get; set; }

            [FirestoreProperty]
            public int FloorNumber { get; set; }

            [FirestoreProperty]
            public string FloorName { get; set; }

            [FirestoreProperty]
            public List<FloorArea> FloorAreas { get; set; }

            [FirestoreProperty]
            public List<Position> Tables { get; set; }

            public Floor()
            {
                this.FloorAreas = new List<FloorArea>();
                this.Tables = new List<Position>();
            }

            public Floor(int floorNumber, string floorName)
            {
                this.FloorID = "";
                this.FloorNumber = floorNumber;
                this.FloorName = FloorName;
                this.FloorAreas = new List<FloorArea>();
                this.Tables = new List<Position>();
            }

            public Dictionary<string, object> ToDictionary()
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("FloorID", this.FloorID);
                dict.Add("FloorNumber", this.FloorNumber);
                dict.Add("FloorName", this.FloorName);
                dict.Add("FloorAreas", this.FloorAreas);
                dict.Add("Tables", this.Tables);
                return dict;
            }

            public bool Contain(Position position)
            {
                foreach (FloorArea floorArea in this.FloorAreas)
                {
                    if (floorArea.Contain(position))
                    {
                        return true;
                    }
                }
                return false;
            }

            public void AddTable(Position position)
            {
                this.Tables.Add(position);
            }

            public void RemoveTable(Position position)
            {
                this.Tables.Remove(position);
            }
        }

        private string cafeID;

        [FirestoreDocumentId]
        public string CafeID
        {
            get { return cafeID; }
            set { cafeID = value; }
        }

        public DateTime LastUpdateTime { get; private set; }
        [FirestoreProperty]
        public long BinaryLastUpdateTime
        {
            get => LastUpdateTime.ToBinary();
            set => LastUpdateTime = DateTime.FromBinary(value);
        }

        private string address;

        [FirestoreProperty]
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private List<Floor> floors;
        public List<Floor> Floors {
            get => floors;
            set
            {
                floors = value;
            }
        }

        private Dictionary<string, int> staffs;

        public Dictionary<string, int> Staffs
        {
            get { return staffs; }
            set { staffs = value; }
        }

        public DocumentReference CafeReference
        {
            get => Database.CafeDocument(CafeID);
        }

        public Cafe()
        {
            this.CafeID = "";
            this.address = "";
            this.floors = new List<Floor>();
            this.staffs = new Dictionary<string, int>();
        }

        public Cafe(string cafeID, string address)
        {
            this.cafeID = cafeID;
            this.address = address;
            this.floors = new List<Floor>();
            this.staffs = new Dictionary<string, int>();
        }
        
        
        public async void ChangeAddress(string address)
        {
            this.Address = address;          
            await Database.UpdateCafeAddressAsync(CafeID, Address);
        }
        

    }
}