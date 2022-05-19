using CSWBManagementApplication.Models;
using FireSharp;
using FireSharp.Config;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Storage;
using System.IO;
using FirebaseConfig = FireSharp.Config.FirebaseConfig;
using FirebaseAdmin.Auth;

namespace CSWBManagementApplication.Services
{
    internal class Database
    {
        #region Initialize

        private const string API_KEY = "AIzaSyDQ8hGV0cMQezXSUHvl4viVh7_8uBd3gH0";

        private static string AdminSDKPath
        {
            get => AppDomain.CurrentDomain.BaseDirectory + @"cafe-shop-kkk-firebase-adminsdk.json";
        }

        #region RealtimeDatabase

        private static FirebaseClient firebaseClient;
        public static FirebaseClient RealtimeDatabase
        {
            get
            {
                if (firebaseClient == null)
                {
                    firebaseClient = new FirebaseClient(new FirebaseConfig
                    {
                        AuthSecret = "3ZhynlyCO7xyMqtpDNOugg3BNGaViIxeS5bCrNMX",
                        BasePath = @"https://cafe-shop-kkk-default-rtdb.asia-southeast1.firebasedatabase.app/"
                    });
                }
                return firebaseClient;
            }
        }

        #endregion

        #region Firestore

        const int BATCH_SIZE = 10;

        const string CAFE_COLLECTION = "Cafes";
        const string CAFE_STAFF_COLLECTION = "Staffs";
        const string CAFE_FLOOR_COLLECTION = "Floors";

        const string USER_COLLECTION = "Users";

        static private FirestoreDb db;
        static public FirestoreDb FirestoreDatabase
        {
            get
            {
                if (db == null)
                {
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", AdminSDKPath);
                    db = FirestoreDb.Create("cafe-shop-kkk");
                }
                return db;
            }
        }
        static public CollectionReference CafeCollection = FirestoreDatabase.Collection(CAFE_COLLECTION);
        static public CollectionReference UserCollection = FirestoreDatabase.Collection(USER_COLLECTION);

        #endregion

        #region Storage

        private const string STORAGE_BUCKET = "cafe-shop-kkk.appspot.com";

        static private FirebaseStorage storage;
        static public FirebaseStorage Storage
        {
            get
            {
                if (storage == null)
                {
                    storage = new FirebaseStorage(STORAGE_BUCKET);
                }
                return storage;
            }
        }



        #endregion

        #region Authentication 

        private static FirebaseAuthProvider authProvider;
        public static FirebaseAuthProvider AuthProvider
        {
            get
            {
                if (authProvider == null)
                {
                    authProvider = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(API_KEY));
                }
                return authProvider;
            }
        }

        private static FirebaseAdmin.FirebaseApp fbAdmin;
        public static FirebaseAdmin.FirebaseApp FBAdmin
        {
            get
            {
                if (fbAdmin == null)
                {
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", AdminSDKPath);
                    fbAdmin = FirebaseAdmin.FirebaseApp.Create();
                }
                return fbAdmin;
            }
        }

        public static FirebaseAdmin.Auth.FirebaseAuth AdminAuth
        {
            get => FirebaseAdmin.Auth.FirebaseAuth.GetAuth(FBAdmin);
        }

        #endregion

        #endregion

        /// <summary>
        /// Clear database and reinitialize it asynchronously.
        /// </summary>
        /// <returns></returns>
        public static async Task ResetDatabaseAsync()
        {
            List<Task> tasks = new List<Task>();
            List<DocumentSnapshot> documents = new List<DocumentSnapshot>();

            tasks.Add(RealtimeDatabase.DeleteAsync("/"));
            List<CollectionReference> rootCollectionReferences = await FirestoreDatabase.ListRootCollectionsAsync().ToListAsync();

            foreach (CollectionReference collectionReference in rootCollectionReferences)
            {
                if (collectionReference != StorageItemCollection)
                {
                    tasks.Add(DeleteCollection(collectionReference, BATCH_SIZE));
                }
            }

            tasks.Add(ForEachUserAsync((user) =>
            {
                tasks.Add(AdminAuth.DeleteUserAsync(user.Uid));
            }));

            tasks.Add(ClearStorage(BATCH_SIZE));

            await Task.WhenAll(tasks);
        }

        #region Firestore

        #region Misc

        static public DocumentReference CafeDocument(string cafeID)
        {
            return CafeCollection.Document(cafeID);
        }

        static public CollectionReference CafeStaffCollection(string cafeID)
        {
            return CafeDocument(cafeID).Collection(CAFE_STAFF_COLLECTION);
        }

        static public DocumentReference StaffDocument(string cafeID, string staffUserID)
        {
            return CafeStaffCollection(cafeID).Document(staffUserID);
        }

        static public CollectionReference CafeFloorCollection(string cafeID)
        {
            return CafeDocument(cafeID).Collection(CAFE_FLOOR_COLLECTION);
        }

        static public DocumentReference FloorDocument(string cafeID, string floorID)
        {
            return CafeFloorCollection(cafeID).Document(floorID);
        }

        static public DocumentReference UserDocument(string userID)
        {
            return UserCollection.Document(userID);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete a Collection and it SubCollection
        /// </summary>
        /// <param name="collectionReference"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public static async Task DeleteCollection(CollectionReference collectionReference, int batchSize)
        {
            List<Task> tasks = new List<Task>();
            List<DocumentSnapshot> documents = new List<DocumentSnapshot>();
            QuerySnapshot snapshot = await collectionReference.Limit(batchSize).GetSnapshotAsync();
            documents.AddRange(snapshot.Documents);
            while (snapshot.Documents.Count > 0)
            {
                foreach (DocumentSnapshot document in documents)
                {
                    List<CollectionReference> subCollections = await document.Reference.ListCollectionsAsync().ToListAsync();
                    foreach (CollectionReference subCollection in subCollections)
                    {
                        await DeleteCollection(subCollection, batchSize);
                    }
                    tasks.Add(collectionReference.Document(document.Id).DeleteAsync());
                }
                await Task.WhenAll(tasks);
                tasks.Clear();
                snapshot = await collectionReference.Limit(batchSize).GetSnapshotAsync();
                documents.Clear();
                documents.AddRange(snapshot.Documents);
            }

        }

        #endregion

        #region Cafe

        public static async Task<Cafe> CreateCafeAsync(string address)
        {
            if (FindCafeAsync(address) != null)
            {
                throw new Exception("Cafe already exist!");
            }
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "Address", address }
            };
            DocumentReference cafeReference = await CafeCollection.AddAsync(data);
            return (await cafeReference.GetSnapshotAsync()).ConvertTo<Cafe>();
        }

        static public async Task<DocumentReference> FindCafeAsync(string address)
        {

            QuerySnapshot cafeSnapshot = await CafeCollection.WhereEqualTo("Address", address).Limit(1).GetSnapshotAsync();
            if (cafeSnapshot.Count == 0)
            {
                return null;
            }
            return CafeDocument(cafeSnapshot.Documents[0].Id);
        }

        static public async Task<Cafe> GetCafe(DocumentReference cafeReference)
        {
            DocumentSnapshot cafeSnapshot = await cafeReference.GetSnapshotAsync();
            if (!cafeSnapshot.Exists)
            {
                return null;
            }
            Cafe result = cafeSnapshot.ConvertTo<Cafe>();

            QuerySnapshot staffSnapshots = await cafeReference.Collection(CAFE_STAFF_COLLECTION).GetSnapshotAsync();
            QuerySnapshot floorSnapshots = await cafeReference.Collection(CAFE_FLOOR_COLLECTION).GetSnapshotAsync();

            foreach (DocumentSnapshot staffSnapshot in staffSnapshots)
            {
                result.Staffs.Add(staffSnapshot.Id, staffSnapshot.GetValue<int>("Level"));
            }

            foreach (DocumentSnapshot floorSnapshot in floorSnapshots)
            {
                result.Floors.Add(floorSnapshot.Id, floorSnapshot.ConvertTo<Cafe.Floor>());
            }

            return result;
        }

        static public async Task<IEnumerable<Cafe>> GetAllCafes() 
        {
            QuerySnapshot cafesSnapshot = await CafeCollection.GetSnapshotAsync();
            List<Cafe> cafes = new List<Cafe>();
            foreach (DocumentSnapshot cafeSnapshot in cafesSnapshot)
            {
                cafes.Add(cafeSnapshot.ConvertTo<Cafe>());
            }
            return cafes.AsEnumerable();
        }

        public static async Task AddFloorToCafeAsync(string cafeID, Cafe.Floor floor)
        {
            DocumentReference floorDocument = FloorDocument(cafeID, floor.FloorID);
#if DEBUG
            DocumentSnapshot floorSnapshot = await floorDocument.GetSnapshotAsync();
            if (floorSnapshot.Exists)
            {
                throw new Exception("Floor already exists");
            }
#endif
            await floorDocument.SetAsync(floor);
        }

        public static async Task UpdateFloorAsync(string cafeID, Cafe.Floor floor)
        {
            DocumentReference floorDocument = FloorDocument(cafeID, floor.FloorID);

            DocumentSnapshot floorSnapshot = await floorDocument.GetSnapshotAsync();
            if (!floorSnapshot.Exists)
            {
                await AddFloorToCafeAsync(cafeID, floor);
                return;
            }

            await floorDocument.SetAsync(floor);
        }

        public static async Task RemoveFloorFromCafeAsync(string cafeID, string floorID)
        {
            DocumentReference floorDocument = FloorDocument(cafeID, floorID);
            await floorDocument.DeleteAsync();
        }

        public static async Task<IEnumerable<DocumentReference>> GetStaffsReference(string cafeID)
        {
            List<DocumentReference> staffs = new List<DocumentReference>();
            QuerySnapshot staffsSnapshot = await CafeStaffCollection(cafeID).GetSnapshotAsync();
            foreach (DocumentSnapshot staffSnapshot in staffsSnapshot)
            {
                
            }
            return staffs.AsEnumerable();
        }

        #endregion

        #region User

        public static async Task<Models.User> CreateUserAsync(FirebaseAuthLink userLink)
        {
            DocumentReference userReference = UserDocument(userLink.User.LocalId);
            Owner user = new Owner(userLink.User.LocalId, userLink.User.Email);
            await userReference.SetAsync(user);
            return user;

        }

        public static async Task<Models.User> CreateUserAsync(FirebaseAuthLink userLink, string cafeID, string name, string phone, bool isMale, DateTime birthdate, bool isManager = false)
        {
            Staff user = new Staff(userLink.User.LocalId, userLink.User.Email, cafeID, name, phone, isMale, birthdate);
            if (!string.IsNullOrWhiteSpace(cafeID))
            {
                DocumentReference cafeReference = CafeDocument(cafeID);
                DocumentSnapshot cafeSnapshot = await cafeReference.GetSnapshotAsync();
                if (cafeSnapshot.Exists)
                {
                    await StaffDocument(cafeID, user.UID).SetAsync(new Dictionary<string, object> {
                        { "Level", (isManager?1:0) }
                    });
                }
                else
                {
                    user.CafeID = "";
                }
            }
            await UserDocument(user.UID).SetAsync(user);
            return user;
        }

        public static async Task<DocumentReference> FindUserAsync(string mail)
        {
            QuerySnapshot userSnapshot = await UserCollection.WhereEqualTo("Mail", mail).Limit(1).GetSnapshotAsync();
            if (userSnapshot.Count == 0)
            {
                return null;
            }
            return UserDocument(userSnapshot.Documents[0].Id);
        }

        public static async Task<Models.User> GetUser(DocumentReference userReference)
        {
            DocumentSnapshot userSnapshot = await userReference.GetSnapshotAsync();

            if (!userSnapshot.Exists)
            {
                return null;
            }

            if (userSnapshot.GetValue<bool>("IsOwner"))
            {
                return userSnapshot.ConvertTo<Owner>();
            }

            return userSnapshot.ConvertTo<Staff>();
        }

        public static async Task<Models.User.Roles> UserRole(DocumentReference userReference)
        {
            DocumentSnapshot userSnapshot = await userReference.GetSnapshotAsync();

            if (!userSnapshot.Exists)
            {
                throw new Exception("User does not exist!");
            }

            if (userSnapshot.GetValue<bool>("IsOwner"))
            {
                return Models.User.Roles.Owner;
            }

            Staff staff = userSnapshot.ConvertTo<Staff>();
            DocumentSnapshot staffSnapshot = await StaffDocument(staff.CafeID, staff.UID).GetSnapshotAsync();

            if (!staffSnapshot.Exists)
            {
                return Models.User.Roles.None;
            }

            int level = staffSnapshot.GetValue<int>("Level");

            return (level == 2 ? Models.User.Roles.Manager : Models.User.Roles.Staff);
        }

        public static async Task RemoveStaffFromCafeAsync(string staffID)
        {
            DocumentReference userReference = UserDocument(staffID);
            DocumentSnapshot userSnapshot = await userReference.GetSnapshotAsync();

#if DEBUG
            if (!userSnapshot.Exists || userSnapshot.GetValue<bool>("IsOwner"))
            {
                throw new Exception("Staff not exists");
            }
#endif
            string cafeID = userSnapshot.GetValue<string>("CafeID");

            if (!string.IsNullOrWhiteSpace(cafeID))
            {
                DocumentReference staffReference = StaffDocument(cafeID, staffID);
                await staffReference.DeleteAsync();
            }

            await userReference.UpdateAsync("CafeID", "");
        }

        public static async Task AddStaffToCafeAsync(string cafeID, string staffID, bool isManager = false)
        {
            DocumentReference userReference = UserDocument(staffID);
            DocumentSnapshot userSnapshot = await userReference.GetSnapshotAsync();

#if DEBUG
            if (!userSnapshot.Exists || userSnapshot.GetValue<bool>("IsOwner"))
            {
                throw new Exception("Staff not exists");
            }
#endif
            string currentCafeID = userSnapshot.GetValue<string>("CafeID");

            DocumentReference staffReference;

            if (!string.IsNullOrWhiteSpace(currentCafeID))
            {
                staffReference = StaffDocument(currentCafeID, staffID);
                await staffReference.DeleteAsync();
            }

            await userReference.UpdateAsync("CafeID", cafeID);

            staffReference = StaffDocument(cafeID, staffID);

            await staffReference.SetAsync(new Dictionary<string, object> { { "Level", (isManager ? 1 : 0) } });
        }

        #endregion    

        #endregion

        #region Authentication

        public static async Task<FirebaseAuthLink> RegisterUser(string mail, string password)
        {
            UserRecord existUserRecord = null;
            try
            {
                existUserRecord = await AdminAuth.GetUserByEmailAsync(mail);
            }
            catch (Exception)
            {
                // Do nothing
            }

            if (existUserRecord != null)
            {
                throw new Exception("Email has already been used");
            }

            QuerySnapshot existUser = await UserCollection.WhereEqualTo("Email", mail).Limit(1).GetSnapshotAsync();
            if (existUser.Count > 0)
            {
                DocumentSnapshot userSnapshot = existUser.Documents[0];
                UserRecordArgs userRecordArgs = new UserRecordArgs
                {
                    Uid = userSnapshot.Id,
                    Email = mail,
                    Password = password
                };
                UserRecord newUser = await AdminAuth.CreateUserAsync(userRecordArgs);
                return await SignIn(mail, password);
            }
            return await AuthProvider.CreateUserWithEmailAndPasswordAsync(mail, password);

        }

        public static async Task SendVerifyMail(FirebaseAuthLink authLink)
        {
            await AuthProvider.SendEmailVerificationAsync(authLink.FirebaseToken);
        }

        public static async Task SendResetPasswordMail(string mail)
        {
            await AuthProvider.SendPasswordResetEmailAsync(mail);
        }

        public static async Task DeleteUser(string firebaseToken)
        {
            await AuthProvider.DeleteUserAsync(firebaseToken);
        }

        public static async Task<FirebaseAuthLink> SignIn(string mail, string password)
        {
            try
            {
                return await AuthProvider.SignInWithEmailAndPasswordAsync(mail, password);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static async Task ForEachUserAsync(Action<ExportedUserRecord> action)
        {
            IAsyncEnumerator<ExportedUserRecords> recordsList = Database.AdminAuth.ListUsersAsync(null).AsRawResponses().GetAsyncEnumerator();
            while (await recordsList.MoveNextAsync())
            {
                ExportedUserRecords records = recordsList.Current;
                if (records.Users != null)
                {
                    foreach (ExportedUserRecord record in records.Users)
                    {
                        action(record);
                    }
                }
            }
        }

        public static async Task<int> GetUserCount()
        {
            int count = 0;
            await ForEachUserAsync(user => count++);
            return count;
        }

        #endregion

        #region FirebaseStorage

        [FirestoreData]
        public class StorageItem
        {
            private string name;
            [FirestoreProperty]
            public string Name
            {
                get => name;
                set
                {
                    name = value;
                }
            }

            private string folder;
            [FirestoreProperty]
            public string Folder
            {
                get => folder;
                set
                {
                    folder = value;
                }
            }

            [FirestoreDocumentId]
            public string ID
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(Folder) || string.IsNullOrWhiteSpace(Name))
                    {
                        return "";
                    }
                    return $"{Folder}*{name}";
                }
                set
                {
                    int deviderPosition = value.IndexOf('*');
                    if (deviderPosition != -1)
                    {
                        Folder = value.Substring(0, deviderPosition);
                        Name = value.Substring(deviderPosition + 1);
                    }
                }
            }

            public string Path
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(Folder) || string.IsNullOrWhiteSpace(Name))
                    {
                        return "";
                    }
                    return $"{Folder}/{name}";
                }
                set
                {
                    int deviderPosition = value.IndexOf('/');
                    if (deviderPosition != -1)
                    {
                        Folder = value.Substring(0, deviderPosition);
                        Name = value.Substring(deviderPosition + 1);
                    }
                }
            }

            public StorageItem()
            {
                Folder = "";
                Name = "";
            }

            public StorageItem(string folder, string name)
            {
                Folder = folder;
                Name = name;
            }

            public StorageItem(string path)
            {
                Path = path;
            }
        }

        public const string USER_PROFILE_PICTURE_FOLDER = "UserProfilePicture";

        public const string PRODUCT_PICTURE_FOLDER = "ProductPicture";

        const string STORAGE_ITEM_COLLECTION = "StorageItem";

        public static CollectionReference StorageItemCollection = FirestoreDatabase.Collection(STORAGE_ITEM_COLLECTION);

        public static DocumentReference StorageItemReference(string path)
        {
            return StorageItemCollection.Document(path);
        }

        public static async Task<string> AddStorageItemAsync(string folder, string name, FileStream fileStream)
        {
            StorageItem item = new StorageItem(folder, name);
            DocumentReference itemReference = StorageItemReference(item.ID);
            DocumentSnapshot existedItem = await itemReference.GetSnapshotAsync();
            if (existedItem.Exists)
            {
                throw new Exception("Item already exists");
            }
            await itemReference.SetAsync(item);
            return await Storage.Child(item.Path).PutAsync(fileStream);
        }

        public static async Task<string> GetStorageItemLink(StorageItem item)
        {
            try
            {
                return await Storage.Child(item.Path).GetDownloadUrlAsync();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task DeleteStorageItemAsync(StorageItem item)
        {
            try
            {
                await Storage.Child(item.Path).DeleteAsync();
            }
            catch (Exception e)
            {

            }

            try
            {
                await StorageItemReference(item.ID).DeleteAsync();
            }
            catch (Exception e)
            {

            }
        }

        public static async Task ClearStorage(int batchSize = 1)
        {
            List<Task> tasks = new List<Task>();
            List<DocumentSnapshot> documents = new List<DocumentSnapshot>();
            QuerySnapshot snapshot = await StorageItemCollection.Limit(batchSize).GetSnapshotAsync();
            documents.AddRange(snapshot.Documents);
            while (snapshot.Documents.Count > 0)
            {
                foreach (DocumentSnapshot document in documents)
                {
                    tasks.Add(DeleteStorageItemAsync(document.ConvertTo<StorageItem>()));
                }
                await Task.WhenAll(tasks);
                tasks.Clear();
                snapshot = await StorageItemCollection.Limit(batchSize).GetSnapshotAsync();
                documents.Clear();
                documents.AddRange(snapshot.Documents);
            }
        }

        #endregion
    }
}
