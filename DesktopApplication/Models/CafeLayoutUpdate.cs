using CSWBManagementApplication.Services;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.Models
{
    [FirestoreData]
    internal class CafeLayoutUpdate
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string CafeID { get; set; }
        
        public DateTime Time { get; set; }
        [FirestoreProperty]
        public long BinaryTime
        {
            get => Time.ToBinary();
            set => Time = DateTime.FromBinary(value);
        }

        [FirestoreProperty]
        public List<string> RemoveFloor { get; set; }
        
        [FirestoreProperty]
        public List<Cafe.Floor> AddFloor { get; set; }

        [FirestoreProperty]
        public List<Cafe.Floor> UpdateFloor { get; set; }

        public CafeLayoutUpdate(string cafeID)
        {
            this.CafeID = cafeID;
            RemoveFloor = new List<string>();
            AddFloor = new List<Cafe.Floor>();
            UpdateFloor = new List<Cafe.Floor>();
            Time = DateTime.Now;
        }

        public static Cafe operator +(Cafe cafe, CafeLayoutUpdate update)
        {
            foreach (string floor in update.RemoveFloor)
            {
                int i = cafe.Floors.FindIndex(x => x.FloorID == floor);
                cafe.Floors.RemoveAt(i);
            }
            foreach (Cafe.Floor floor in update.AddFloor)
            {
                cafe.Floors.Add(floor);
            }

            foreach (Cafe.Floor floor in update.UpdateFloor)
            {
                int i = cafe.Floors.FindIndex(x => x.FloorID == floor.FloorID);
                cafe.Floors[i] = floor;
            }
            return cafe;
        }

        public async void UpdateDatabase()
        {
            List<Task> tasks = new List<Task>();
            foreach (string floor in RemoveFloor)
            {
                tasks.Add(Database.RemoveFloorFromCafeAsync(CafeID, floor));
            }
            await Task.WhenAll(tasks);
            tasks.Clear();
            foreach (Cafe.Floor floor in AddFloor)
            {
                tasks.Add(Database.AddFloorToCafeAsync(CafeID, floor));
            }
            await Task.WhenAll(tasks);
            tasks.Clear();
            foreach (Cafe.Floor floor in UpdateFloor)
            {
                tasks.Add(Database.UpdateFloorAsync(CafeID, floor));
            }
            await Task.WhenAll(tasks);
        }
    }
}
