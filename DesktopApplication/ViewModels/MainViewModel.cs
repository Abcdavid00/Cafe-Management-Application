using CSWBManagementApplication.Services;
using Firebase.Auth;

namespace CSWBManagementApplication.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private Models.User currentUser;

        public Models.User CurrentUser
        {
            get => currentUser;
            private set
            {
                currentUser = value;
                if (currentUser != null)
                {
                    switch (currentUser.Role)
                    {
                        case Models.User.Roles.Owner:
                            CurrentWorkingViewModel = new OwnerViewModel(this);
                            break;

                        case Models.User.Roles.Manager:
                            CurrentWorkingViewModel = new ManagerViewModel(this);
                            break;

                        case Models.User.Roles.Staff:
                            CurrentWorkingViewModel = new StaffViewModel(this);
                            break;

                        case Models.User.Roles.None:
                            CurrentWorkingViewModel = null;
                            break;
                    }
                }
                else
                {
                    CurrentWorkingViewModel = null;
                }
                OnPropertyChanged();
            }
        }

        private FirebaseAuthLink userLink;

        public FirebaseAuthLink UserLink
        {
            get { return userLink; }
            set
            {
                userLink = value;
                if (userLink != null)
                {
                    UpdateUser();
                }
                else
                {
                    State = 0;
                    CurrentUser = null;
                }
                OnPropertyChanged();
            }
        }

        private async void UpdateUser()
        {
            CurrentUser = await Database.GetUser(Database.UserDocument(UserLink.User.LocalId));
            if (CurrentWorkingViewModel != null)
            {
                State = 1;
            }
        }

        private int state;

        public int State
        {
            get => state;
            private set
            {
                state = value;
                OnPropertyChanged();
            }
        }

        private ViewModelBase currentLoginViewModel;

        public ViewModelBase CurrentLoginViewModel
        {
            get => currentLoginViewModel;
            private set
            {
                currentLoginViewModel = value;
                OnPropertyChanged();
            }
        }

        private ViewModelBase currentWorkingViewModel;

        public ViewModelBase CurrentWorkingViewModel
        {
            get => currentWorkingViewModel;
            private set
            {
                currentWorkingViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            Initialize();
        }

        public async void Initialize()
        {
            //await Database.ResetDatabaseAsync();

            if ((await Database.GetUserCount()) == 0)
            {
                CurrentLoginViewModel = new CreateOwnerViewModel(this);
            }
            else
            {
                CurrentLoginViewModel = new LoginViewModel(this);
            }
            UserLink = null;
#if DEBUG
            //UserLink = await Database.SignIn("dangkhoabh02@gmail.com", "123456");
#endif
        }
    }
}