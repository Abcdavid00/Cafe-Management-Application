using CSWBManagementApplication.Services;
using Google.Cloud.Firestore;
using System;

namespace CSWBManagementApplication.Models
{
    [FirestoreData]
    internal class Staff : User
    {
        private string cafeID;

        [FirestoreProperty]
        public string CafeID
        {
            get { return cafeID; }
            set { cafeID = value; }
        }

        private string name;

        [FirestoreProperty]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string phone;

        [FirestoreProperty]
        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        private bool isMale;

        [FirestoreProperty]
        public bool IsMale
        {
            get { return isMale; }
            set { isMale = value; }
        }

        private DateTime birthdate;

        public DateTime Birthdate
        {
            get { return birthdate; }
            set { birthdate = value; }
        }

        private string profilePicturePath;

        [FirestoreProperty]
        public string ProfilePicturePath
        {
            get => profilePicturePath;
            set
            {
                profilePicturePath = value;
            }
        }

        [FirestoreProperty]
        public long BinaryBirthDate
        {
            get { return birthdate.ToBinary(); }
            set { birthdate = DateTime.FromBinary(value); }
        }

        public Staff()
        {
            isOwner = false;
            cafeID = "";
            name = "";
            phone = "";
            isMale = false;
            birthdate = new DateTime();
        }

        public Staff(string uid, string email, string cafeID, string name, string phone, bool isMale, DateTime birthdate) : base(uid, email)
        {
            isOwner = false;
            this.cafeID = cafeID;
            this.name = name;
            this.phone = phone;
            this.isMale = isMale;
            this.birthdate = birthdate;
        }

        public ulong Salary(int month, int year)
        {
            throw new NotImplementedException();
        }

        public async void UpdateInfo()
        {
            await Database.UpdateStaffInfoAsync(this);
        }
    }
}