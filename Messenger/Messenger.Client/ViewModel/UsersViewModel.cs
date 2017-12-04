using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Messenger.Client.Model;
using Messenger.Client.View;
using Messenger.Model;
using Messenger.Client.Commands;
using System.Windows.Media.Imaging;

namespace Messenger.Client.ViewModel
{
    public class UsersViewModel : BaseModel, IScreen
    {
        protected UsersControl _usersControl;
        public MainViewModel Parent { get; set; }
        public User OldUser { get; set; }
        public string WarningText { get; set; } 

        public UsersViewModel(MainViewModel mvm)
        {
            Parent = mvm;
            
        }

        private void ConverterByteToImage(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {

                Parent.MainUserAva.BeginInit();
                Parent.MainUserAva.CacheOption = BitmapCacheOption.OnLoad;
                Parent.MainUserAva.StreamSource = memoryStream;
                Parent.MainUserAva.EndInit();
            }
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
            if (OldUser.FirstName == null || OldUser.LastName == null || OldUser.Password == null)
            {
                WarningText = "*Заполните все поля";
                return null;
            }
            else
            {
                var user = await DataAccess.DataAccess.LoginUser(OldUser.FirstName, OldUser.LastName, OldUser.Password);
                Parent.MainUser = user;
                if (Parent.MainUser == null)
                {
                    WarningText = "*Неверно введены имя, фамилия или пароль";
                    return null;
                }
                else
                {
                    ConverterByteToImage(user.Ava);
                    Parent.CurrentScreenType = ScreenTypes.Dialogs;
                    Parent.SetScreen();
                    return user;
                }
            }
        }

        public bool CanExecuteLoginUserCommand(object param)
        {
            return true;
        }

        #endregion

        #region BackToRegistration

        private RelayCommand _backCommand;
        public RelayCommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand =
                           new RelayCommand(param => ExecuteBackCommand(param),
                               param => CanExecuteBackCommand(param)));
            }
        }

        public void ExecuteBackCommand(object param)
        {
            Parent.CurrentScreenType = ScreenTypes.Register;
            Parent.SetScreen();
        }

        public bool CanExecuteBackCommand(object param)
        {
            return true;
        }

        #endregion
        public void Initialize()
        {
            OldUser = new User();
            WarningText = null;
        }

        public object View()
        {
            return _usersControl ?? (_usersControl = new UsersControl() {DataContext = this});
        }
    }
}
