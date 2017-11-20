using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Client.Commands;
using Messenger.Client.Model;
using Messenger.Client.View;
using Messenger.Model;
using System.Windows.Forms;
using Messenger.Client.DataAccess;

namespace Messenger.Client.ViewModel
{
    public class UsersRegistrationViewModel : BaseModel, IScreen
    {
        protected UsersRegestrationControl _usersRegistrationView;
        public MainViewModel Parent { get; set; }
        public User newUser { get; set; }
        public Files Avatar { get; set; }

        public UsersRegistrationViewModel(MainViewModel mvm)
        {
            newUser = new User();
            Avatar = new Files();
            Parent = mvm;
        }
        

        #region GetAvatar
        private RelayCommand _openFileDialogCommand;

        public RelayCommand OpenFileDialogCommand
        {
            get
            {
                return _openFileDialogCommand ?? (_openFileDialogCommand =
                           new RelayCommand(param => ExecuteOpenFileDialogCommand(param),
                               param => CanExecuteOpenFileDialogCommand(param)));
            }
        }

        public void ExecuteOpenFileDialogCommand(object param)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Avatar.Path = dialog.FileName;
            }
        }

        public bool CanExecuteOpenFileDialogCommand(object param)
        {
            return true;
        }

        #endregion

        #region Registration

        private RelayCommand _createUserCommand;

        public RelayCommand CreateUserCommand
        {
            get
            {
                return _createUserCommand ?? (_createUserCommand =
                           new RelayCommand(param => ExecuteCreateUserCommand(param),
                               param => CanExecuteCreateUserCommand(param)));
            }
        }

        async Task<User> ExecuteCreateUserCommand(object param)
        {
            var user = await DataAccess.DataAccess.CreateUser(newUser);
            Parent.CurrentScreenType = ScreenTypes.Login;
            Parent.SetScreen();
            return user;
        }

        public bool CanExecuteCreateUserCommand(object param)
        {
            return true;
        }

        #endregion

        
        #region ToAutorisation

        private RelayCommand _toUserLoginCommand;

        public RelayCommand ToUserLoginCommand
        {
            get
            {
                return _toUserLoginCommand ?? (_toUserLoginCommand =
                           new RelayCommand(param => ExecuteToUserLoginCommand(param),
                               param => CanExecuteToUserLoginCommand(param)));
            }
        }

        public void ExecuteToUserLoginCommand(object param)
        {
            Parent.CurrentScreenType = ScreenTypes.Login;
            Parent.SetScreen();
        }

        public bool CanExecuteToUserLoginCommand(object param)
        {
            return true;
        }

        #endregion

        public void Initialize()
        {
        }

        public object View()
        {
            return _usersRegistrationView ??
                   (_usersRegistrationView = new UsersRegestrationControl() {DataContext = this});
        }
    }
}
