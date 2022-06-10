using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Services;
using Firebase.Auth;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
    internal class CreateAccountDialogViewModel : ViewModelBase
    {
        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();
            }
        }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private SnackbarMessageQueue createAccountDialogSnackbarMessageQueue;
        public SnackbarMessageQueue CreateAccountDialogSnackbarMessageQueue
        {
            get => createAccountDialogSnackbarMessageQueue;
            set
            {
                createAccountDialogSnackbarMessageQueue = value;
                OnPropertyChanged();
            }
        }

        private string sex;
        public string Sex
        {
            get => sex;
            set
            {
                sex = value;
                OnPropertyChanged();
            }
        }

        private DateTime birthdate;
        public DateTime Birthdate
        {
            get => birthdate;
            set
            {
                birthdate = value;
                OnPropertyChanged();
            }
        }

        private string phone;
        public string Phone
        {
            get => phone;
            set
            {
                phone = value;
                OnPropertyChanged();
            }
        }

        public ICommand ButtonCommand
        {
            get => new RelayCommand<PasswordBox>((PasswordBox p) => { });
        }

        public bool IsEnabled
        {
            get { return !Waiting; }
        }

        private bool waiting;

        public bool Waiting
        {
            get { return waiting; }
            set
            {
                waiting = value;
                OnPropertyChanged();
                OnPropertyChanged("IsEnabled");
            }
        }

        private string buttonToolTip;

        public string ButtonToolTip
        {
            get { return buttonToolTip; }
            set
            {
                buttonToolTip = value;
                OnPropertyChanged();
            }
        }

        private PackIconKind buttonIcon;

        public PackIconKind ButtonIcon
        {
            get { return buttonIcon; }
            set
            {
                buttonIcon = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateAccountButtonCommand
        {
            get => new RelayCommand<PasswordBox>(CreateAccount);
        }

        public ICommand ResendVerifyMail
        {
            get => new CommandBase(ResendVerifyMailPressed);
        }

        private Visibility resendVisibility;

        public Visibility ResendVisibility
        {
            get { return resendVisibility; }
            set
            {
                resendVisibility = value;
                OnPropertyChanged();
            }
        }

        private FirebaseAuthLink AuthLink;

        private int state;

        public int State
        {
            get { return state; }
            set
            {
                if (value <= 0)
                {
                    state = 0;
                    ButtonToolTip = "Create account";
                    ButtonIcon = PackIconKind.AccountPlus;
                    ResendVisibility = Visibility.Hidden;
                    Waiting = false;
                }
                else if (value == 1)
                {
                    state = 1;
                    ButtonToolTip = "Waiting for verification";
                    ButtonIcon = PackIconKind.AccountClock;
                    ResendVisibility = Visibility.Visible;
                    Waiting = true;
                }
                else
                {
                    state = 2;
                    ButtonToolTip = "Verified";
                    ButtonIcon = PackIconKind.AccountCheck;
                    ResendVisibility = Visibility.Hidden;
                    Waiting = false;
                }
                OnPropertyChanged();
            }
        }

        public CreateAccountDialogViewModel()
        {
            CreateAccountDialogSnackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000));
            ButtonToolTip = "Press to create account";
            ButtonIcon = PackIconKind.AccountPlus;
            State = 0;
            ResendVisibility = Visibility.Hidden;

        }

        private bool VerifyInput(PasswordBox passwordBox)
        {
            if (string.IsNullOrEmpty(Email))
            {
                CreateAccountDialogSnackbarMessageQueue.Enqueue("Email is required.");
                return false;
            }
            if (passwordBox.Password.Length < 6)
            {
                CreateAccountDialogSnackbarMessageQueue.Enqueue("Password must be at least 6 characters.");
                return false;
            }
            if (string.IsNullOrEmpty(Name))
            {
                CreateAccountDialogSnackbarMessageQueue.Enqueue("Name is required.");
                return false;
            }
            if (string.IsNullOrEmpty(Sex))
            {
                CreateAccountDialogSnackbarMessageQueue.Enqueue("Sex is required.");
                return false;
            }
            if (Birthdate == null)
            {
                CreateAccountDialogSnackbarMessageQueue.Enqueue("Birthday is required.");
                return false;
            }
            if (string.IsNullOrEmpty(Phone))
            {
                CreateAccountDialogSnackbarMessageQueue.Enqueue("Phone is required.");
                return false;
            }
            return true;
        }

        private async void CreateAccount(PasswordBox passwordBox)
        {
            if (!VerifyInput(passwordBox))
            {
                return;
            }
            StaffPlaceholder staffPlaceholder = await Database.GetStaffPlaceholderAsync(email);
            if (staffPlaceholder == null)
            {
                CreateAccountDialogSnackbarMessageQueue.Enqueue("This email is invalid");
                return;
            }
            if (state ==0)
            {
                await Database.DeleteUserByMail(Email);
                AuthLink = await Database.RegisterUser(Email, passwordBox.Password);
                await Database.SendVerifyMail(AuthLink);
                State = 1;
                WaitForVerification(staffPlaceholder);
            }
            else if (state == 1)
            {
                State = 0;
                await Database.DeleteUserByToken(AuthLink.FirebaseToken);
                AuthLink = null;
            }

        }

        private async void WaitForVerification(StaffPlaceholder staffPlaceholder)
        {
            while (Waiting)
            {
                await AuthLink.RefreshUserDetails();
                if (AuthLink.User.IsEmailVerified)
                {
                    State = 2;
                    await Database.CreateUserAsync(AuthLink,staffPlaceholder.CafeID,Name,Phone,Sex.Contains("Male"),Birthdate,false);
                    await Database.RemoveStaffPlaceholderAsync(staffPlaceholder.Email);
                    OnUserSuccessfullyCreated?.Invoke(this, AuthLink);
                }
            }
        }

        private async void ResendVerifyMailPressed()
        {
            if (Waiting && AuthLink != null)
            {
                try
                {
                    await Database.SendVerifyMail(AuthLink);
                }
                catch (Exception e)
                {
                    CreateAccountDialogSnackbarMessageQueue.Enqueue(e.Message);
                }
            }
        }

        public event EventHandler<FirebaseAuthLink> OnUserSuccessfullyCreated;
        
    }
}
