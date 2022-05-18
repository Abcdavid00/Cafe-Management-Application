using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Services;
using Firebase.Auth;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
    internal class LoginViewModel : ViewModelBase
    {
        private string email;
        public string Email
        {
            get { return email; }
            set { email = value; OnPropertyChanged(); }
        }

        private string forgotPasswordEmail;
        public string ForgotPasswordEmail
        {
            get => forgotPasswordEmail;
            set
            { 
                forgotPasswordEmail = value;
                OnPropertyChanged("ForgotPasswordEmailTxtBox");
            }
        }

        private ICommand loginCommand;
        public ICommand LoginCommand
        {
            get => loginCommand;
            private set
            {
                loginCommand = value;
                OnPropertyChanged("LoginCommand");
            }
        }

        private ICommand forgotPasswordCommand;
        public ICommand ForgotPasswordCommand
        {
            get => forgotPasswordCommand;
            private set
            {
                forgotPasswordCommand = value;
                OnPropertyChanged("ForgotPasswordCommand");
            }
        }

        private ICommand sendResetPasswordMailCommand;
        public ICommand SendResetPasswordMailCommand
        {
            get => sendResetPasswordMailCommand;
            private set
            {
                sendResetPasswordMailCommand = value;
                OnPropertyChanged("SendResetPasswordMailCommand");
            }
        }

        public SnackbarMessageQueue LoginViewSnackbarMessageQueue
        {
            get;
            set;
        }

        private bool isForgotPasswordDialogOpen;
        public bool IsForgotPasswordDialogOpen
        {
            get => isForgotPasswordDialogOpen;
            set
            {
                isForgotPasswordDialogOpen = value;
                OnPropertyChanged("IsForgotPasswordDialogOpen");
            }
        }     

        private MainViewModel mainViewModel;

        private Firebase.Auth.FirebaseAuthLink userLink;

        public LoginViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            LoginViewSnackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(5000));
            ForgotPasswordCommand = new CommandBase(() =>
            {
                IsForgotPasswordDialogOpen = true;
            });
            SendResetPasswordMailCommand = new CommandBase(SendResetPasswordMail);
            LoginCommand = new RelayCommand<PasswordBox>(Login);
            userLink = null;

        }
        
        public async void SendResetPasswordMail()
        {
            await Database.SendResetPasswordMail(ForgotPasswordEmail);
            ForgotPasswordEmail = "";
            IsForgotPasswordDialogOpen = false;
        }

        public async void Login(PasswordBox passwordBox)
        {
            if (userLink == null)
            {
                try
                {
                    userLink = await Database.SignIn(Email, passwordBox.Password);

                }
                catch (Exception e)
                {
                    LoginViewSnackbarMessageQueue.Enqueue("Email or password is incorrect");
                    return;
                }
            }
            if (!userLink.User.IsEmailVerified)
            {
                await Database.SendVerifyMail(userLink);
                WaitForVerification();
                LoginViewSnackbarMessageQueue.Enqueue("This account email has been verified");
                LoginViewSnackbarMessageQueue.Enqueue("A verification mail has been sent to your email");
                return;
            }
            mainViewModel.UserLink = userLink;
            Email = "";
            passwordBox.Password = "";
        }

        public async void WaitForVerification()
        {
            while (!userLink.User.IsEmailVerified)
            {
                await userLink.RefreshUserDetails();
                if (userLink.User.IsEmailVerified)
                {
                    mainViewModel.UserLink = userLink;
                }
            }
        }
    }
}
