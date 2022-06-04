using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

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
        public List<Floor> Floors { get; set; }

        public CafeLayoutUpdate(string CafeID, List<Floor> floors)
        {
            this.CafeID = CafeID;
            this.Floors = floors;
        }
    }
}