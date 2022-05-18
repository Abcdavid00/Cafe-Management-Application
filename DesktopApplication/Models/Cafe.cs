
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using CSWBManagementApplication.Services;

namespace CSWBManagementApplication.Models
{
    [FirestoreData]
    internal class Cafe
    {
        [FirestoreData]
        public class Position
        {

            [FirestoreProperty]
            public int x
            {
                get;
                set;
            }

            [FirestoreProperty]
            public int y
            {
                get;
                set;
            }

            [FirestoreProperty]
            public long SingleValue
            {
                get
                {
                    return y + (x + y + 1) * (x + y) / 2;
                }
                set
                {
                    int degree = (int)Math.Floor((Math.Sqrt(value * 8 + 1) - 1) / 2);
                    long sum = (degree + 1) * degree / 2;
                    this.y = (int)(value - sum);
                    this.x = degree - this.y;

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
                this.x = x;
                this.y = y;
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
            public int Length
            {
                get;
                set;
            }

            public FloorArea()
            {
                Position = new Position();
                Width = 0;
                Length = 0;
            }

            public FloorArea(Position position, int width, int length)
            {
                Position = position;
                Width = width;
                Length = length;
            }

            public bool Contain(Position position)
            {
                return position.x >= Position.x && position.x <= Position.x + Width &&
                       position.y >= Position.y && position.y <= Position.y + Length;
            }
        }
        
        [FirestoreData]
        public class Floor
        {
            [FirestoreDocumentId]
            public string FloorID { get; set; }

            [FirestoreProperty]
            public List<FloorArea> FloorAreas { get; set; }

            [FirestoreProperty]
            public List<Position> Tables { get; set; }
            
            public Floor()
            {
                this.FloorAreas = new List<FloorArea>();
                this.Tables = new List<Position>();
            }

            public Floor(string floorID, List<FloorArea> FloorAreas, List<Position> Table)
            {
                this.FloorID = floorID;
                this.FloorAreas = FloorAreas;
                this.Tables = Table;
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
        
        private string address;
        [FirestoreProperty]
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private Dictionary<string, Floor> floors;
        public Dictionary<string, Floor> Floors
        {
            get { return floors; }
            set { floors = value; }
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
            this.floors = new Dictionary<string, Floor>();
            this.staffs = new Dictionary<string, int>();
        }
        
        public Cafe(string cafeID, string address)
        {
            this.cafeID = cafeID;
            this.address = address;
            this.floors = new Dictionary<string, Floor>();
            this.staffs = new Dictionary<string, int>();            
        }

        public void AddFloor(string floorID, Floor floor)
        {
            this.floors.Add(floorID, floor);
        }
    }
}
