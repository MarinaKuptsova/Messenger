using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Controls;
using Messenger.Client.Commands;
using Messenger.Client.Model;
using Messenger.Client.View;
using Messenger.Model;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Messenger.Client.DataAccess;

namespace Messenger.Client.ViewModel
{
    public class UsersRegistrationViewModel : BaseModel, IScreen
    {
        protected UsersRegestrationControl _usersRegistrationView;
        public MainViewModel Parent { get; set; }
        public UserParameters UsersParameters { get; set; }
        public User newUser { get; set; }
        public Files Avatar { get; set; }
        public BitmapImage img { get; set; }
        public byte[] array;
        public string FileName { get; set; } 
        public string Warning { get; set; }

        public UsersRegistrationViewModel(MainViewModel mvm)
        {
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
                var path = dialog.FileName;
                FileName = dialog.SafeFileName;
                Image image1 = Image.FromFile(path);
                
                ImageConverter converter = new ImageConverter();
                array = (byte[]) converter.ConvertTo(image1, typeof(byte[]));
                
                MemoryStream stream = new MemoryStream(array);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();
                img = image;
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
            if (newUser.FirstName == null || newUser.LastName == null || newUser.Password == null || array == null)
            {
                Warning = "*заполните все поля";
                return null;
            }
            else
            {
                UsersParameters = new UserParameters();
                UsersParameters.user = newUser;
                UsersParameters.photo = array;
                UsersParameters.name = FileName.Split('.')[0];
                UsersParameters.type = "." + FileName.Split('.')[1];
                var user = await DataAccess.DataAccess.CreateUser(UsersParameters);
                Warning = null;
                Parent.CurrentScreenType = ScreenTypes.Login;
                Parent.SetScreen();
                return user;
            }
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
            newUser = new User();
            img = null;
        }

        public object View()
        {
            return _usersRegistrationView ??
                   (_usersRegistrationView = new UsersRegestrationControl() {DataContext = this});
        }
    }
}
