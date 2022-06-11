using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Services;
using System;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
    internal class ForgotPasswordDialogViewModel : ViewModelBase
    {
        private string forgotPasswordEmail;

        public string ForgotPasswordEmail
        {
            get => forgotPasswordEmail;
            set
            {
                forgotPasswordEmail = value;
                OnPropertyChanged();
            }
        }

        public ForgotPasswordDialogViewModel()
        {
        }

        public ICommand SendResetPasswordMailCommand
        {
            get => new CommandBase(SendResetPasswordMail);
        }

        public async void SendResetPasswordMail()
        {
            await Database.SendResetPasswordMail(ForgotPasswordEmail);
            ForgotPasswordEmail = "";
            OnResetPasswordMailSended?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnResetPasswordMailSended;
    }
}