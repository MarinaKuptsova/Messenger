using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Messenger.Client.Model;
using Messenger.Client.View;
using Messenger.Model;
using Messenger.Client.Commands;

namespace Messenger.Client.ViewModel
{
    public class UsersViewModel : BaseModel, IScreen
    {
        protected UsersControl _usersControl;
        public MainViewModel Parent { get; set; }
        public User OldUser { get; set; }

        public UsersViewModel(MainViewModel mvm)
        {
            OldUser = new User();
            Parent = mvm;
        }

        #region Autentification

        private RelayCommand _loginUserCommand;

        public RelayCommand LoginUserCommand
        {
            get
            {
                return _loginUserCommand ?? (_loginUserCommand =
                           new RelayCommand(param => ExecuteLoginUserCommand(param),
                               param => CanExecuteLoginUserCommand(param)));
            }
        }

        async Task<User> ExecuteLoginUserCommand(object param)
        {
            var user = await DataAccess.DataAccess.LoginUser(OldUser.FirstName, OldUser.LastName, OldUser.Password);
            Parent.CurrentScreenType = ScreenTypes.Dialogs;
            Parent.SetScreen();
            return user;
        }

        public bool CanExecuteLoginUserCommand(object param)
        {
            return true;
        }

        #endregion
        public void Initialize()
        {
        }

        public object View()
        {
            return _usersControl ?? (_usersControl = new UsersControl() {DataContext = this});
        }
    }
}
