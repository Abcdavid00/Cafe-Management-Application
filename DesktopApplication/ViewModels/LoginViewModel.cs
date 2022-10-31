using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Services;
using Firebase.Auth;
using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
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

        public ICommand LoginCommand
        {
            get => new RelayCommand<PasswordBox>(OnLoginButtonClicked);
        }

        public ICommand CreateAccountCommand
        {
            get => new CommandBase(() =>
            {
                IsCreateAccountDialog = true;
                IsDialogOpen = true;
            });
        }

        public ICommand ForgotPasswordCommand
        {
            get => new CommandBase(() =>
            {
                IsCreateAccountDialog = false;
                IsDialogOpen = true;
            });
        }

        private ForgotPasswordDialogViewModel forgotPasswordDialogViewModel;
        private CreateAccountDialogViewModel createAccountDialogViewModel;

        private bool isCreateAccountDialog;

        private bool IsCreateAccountDialog
        {
            get => isCreateAccountDialog;
            set
            {
                isCreateAccountDialog = value;
                OnPropertyChanged(nameof(DialogViewModel));
                OnPropertyChanged();
            }
        }

        public ViewModelBase DialogViewModel
        {
            get
            {
                if (IsCreateAccountDialog)
                {
                    return createAccountDialogViewModel;
                }
                return forgotPasswordDialogViewModel;
            }
        }

        public SnackbarMessageQueue LoginViewSnackbarMessageQueue
        {
            get;
            set;
        }

        private bool isDialogOpen;

        public bool IsDialogOpen
        {
            get => isDialogOpen;
            private set
            {
                isDialogOpen = value;
                OnPropertyChanged();
            }
        }

        private MainViewModel mainViewModel;

        private FirebaseAuthLink userLink;

        public LoginViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            this.forgotPasswordDialogViewModel = new ForgotPasswordDialogViewModel();
            this.forgotPasswordDialogViewModel.OnResetPasswordMailSended += ForgotPasswordDialogViewModel_OnResetPasswordMailSended;
            this.createAccountDialogViewModel = new CreateAccountDialogViewModel();
            this.createAccountDialogViewModel.OnUserSuccessfullyCreated += NewAccoutCreated;
            isCreateAccountDialog = true;
            LoginViewSnackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(5000));
            userLink = null;
        }

        private void NewAccoutCreated(object o, FirebaseAuthLink AuthLink)
        {
            IsDialogOpen = false;
            mainViewModel.UserLink = AuthLink;
        }

        private void ForgotPasswordDialogViewModel_OnResetPasswordMailSended(object sender, EventArgs e)
        {
            IsDialogOpen = false;
            LoginViewSnackbarMessageQueue.Enqueue("Password reset email has just been sent");
        }

        private async void OnLoginButtonClicked(PasswordBox passwordBox)
        {
            string result = await Login(passwordBox);
            Console.WriteLine(result);
        }

        public async Task<string> Login(PasswordBox passwordBox)
        {
            if (userLink == null)
            {
                try
                {
                    userLink = await Database.SignIn(Email, passwordBox.Password);
                }
                catch (Exception e)
                {
                    string error = "Email or password is incorrect";
                    LoginViewSnackbarMessageQueue.Enqueue(error);
                    return error;
                }
            }
            if (!userLink.User.IsEmailVerified)
            {
                await Database.SendVerifyMail(userLink);
                WaitForVerification();
                LoginViewSnackbarMessageQueue.Enqueue("This account email has been verified");
                LoginViewSnackbarMessageQueue.Enqueue("A verification mail has been sent to your email");
                return "This account email has been verified";
            }
            mainViewModel.UserLink = userLink;
            Email = "";
            passwordBox.Password = "";
            return "User with email \"" + userLink.User.Email + "\" has just logged in!";
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