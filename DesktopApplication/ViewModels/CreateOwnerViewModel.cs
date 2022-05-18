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
    internal class CreateOwnerViewModel : ViewModelBase
    {
        private string mail;
        public string Mail
        {
            get { return mail; }
            set
            {
                mail = value;
                OnPropertyChanged();
            }
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

        private ICommand buttonCommand;
        public ICommand ButtonCommand
        {
            get { return buttonCommand; }
            set
            {
                buttonCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand resendVerifyMail;
        public ICommand ResendVerifyMail
        {
            get { return resendVerifyMail; }
            set
            {
                resendVerifyMail = value;
                OnPropertyChanged();
            }
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
                    ButtonToolTip = "Create Owner";
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
                } else
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

        public SnackbarMessageQueue CreateOwnerViewSnackbarMessageQueue
        {
            get;
            set;
        }

        private MainViewModel mainViewModel;

        public CreateOwnerViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            CreateOwnerViewSnackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(5000));
            ButtonToolTip = "Press to create account";
            ButtonIcon = PackIconKind.AccountPlus;
            Mail = "";
            State = 0;
            ResendVisibility = Visibility.Hidden;
            ButtonCommand = new RelayCommand<PasswordBox>(ButtonPressed);
            ResendVerifyMail = new CommandBase(ResendVerifyMailPressed);
        }

        private async void ButtonPressed(PasswordBox passwordBox)
        {
            {
                if (state == 0)
                {
                    if (passwordBox.Password.Length < 6)
                    {
                        CreateOwnerViewSnackbarMessageQueue.Enqueue("Password must be at least 6 characters long");
                        return;
                    }
                    State = 1;
                    AuthLink = await Database.RegisterUser(Mail, passwordBox.Password);
                    await Database.SendVerifyMail(AuthLink);
                    WaitForVerification();
                }
                else if (state == 1)
                {
                    State = 0;
                    await Database.DeleteUser(AuthLink.FirebaseToken);
                    AuthLink = null;
                }
            }
        }

        private async void WaitForVerification()
        {
            while (Waiting)
            {
                await AuthLink.RefreshUserDetails();
                if (AuthLink.User.IsEmailVerified)
                {
                    State = 2;
                    await Database.CreateUserAsync(AuthLink);
                    NavigateToOwnerView();
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
                    CreateOwnerViewSnackbarMessageQueue.Enqueue(e.Message);
                }
            }
        }

        public void NavigateToOwnerView()
        {
            mainViewModel.UserLink = AuthLink;
        }
    }
}
