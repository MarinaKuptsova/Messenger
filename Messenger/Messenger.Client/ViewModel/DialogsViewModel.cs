using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using Messenger.Client.Model;
using Messenger.Client.View;
using Messenger.Client.Commands;
using Messenger.Client.DataAccess;
using Messenger.Model;
using Message = Messenger.Model.Message;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace Messenger.Client.ViewModel
{
    public class DialogsViewModel : BaseModel, IScreen
    {
        public object CurrentDialog { get; set; }
        public MainViewModel Parent { get; set; }
        public User GlobalUser { get; set; }
        protected DialogsControl DialogsControl;
        protected MessagesControl MessagesControl;
        public Group CurrentGroup { get; set; } //отвечает за текущий диалог 
        public ObservableCollection<Group> UsersGroups { get; set; }
        public ObservableCollection<Messenger.Model.Message> UsersMessages { get; set; }
        public CreateMessageParameters MessageParameters { get; set; }
        public CreateMessageWithFileParameters MessageWithFile { get; set; }
        public BitmapImage AvatarUser { get; set; }
        public Byte Status { get; set; }
        public User UpdatedUser { get; set; }
        

        #region Message Parameters

        public string MessageText { get; set; }

        public byte[] array { get; set; }

        #endregion

        public DialogsViewModel(MainViewModel mvm)
        {
            Parent = mvm;
            CurrentGroup = new Group();
            GlobalUser = new User();
            GlobalUser = Parent.MainUser;
            AvatarUser = new BitmapImage();
            AvatarUser = Parent.MainUserAva;
            
        }

        
        #region ToCreateChat

        private RelayCommand _createChatCommand;

        public RelayCommand CreateChatCommand
        {
            get
            {
                return _createChatCommand ?? (_createChatCommand =
                           new RelayCommand(param => ExecuteCreateChatCommand(param),
                               param => CanExecuteCreateChatCommand(param)));
            }
        }

        public void ExecuteCreateChatCommand(object param)
        {
            Parent.CurrentScreenType = ScreenTypes.Group;
            Parent.SetScreen();
        }

        public bool CanExecuteCreateChatCommand(object param)
        {
            return true;
        }

        #endregion

        #region SelectChat

        private RelayCommand _selectChatCommand;

        public RelayCommand SelectChatCommand
        {
            get
            {
                return _selectChatCommand ?? (_selectChatCommand =
                           new RelayCommand(param => ExecuteSelectChatCommand(param),
                               param => CanExecuteSelectChatCommand(param)));
            }
        }

        public void ExecuteSelectChatCommand(object param)
        {
            var result = DataAccess.DataAccess.GetUsersMessagesInGroup(CurrentGroup.Id).Result;
            UsersMessages = new ObservableCollection<Message>(result);
            var user = new User();
            Converter converter = new Converter();
            foreach (var message in UsersMessages)
            {
                user = DataAccess.DataAccess.GetUser(message.MessageFromUserId).Result;
                message.OwnerAva = converter.ConverterByteToImage(user.Ava);
                message.OwnerName = user.FirstName;
                if (user.Id == GlobalUser.Id)
                {
                    message.Orientation = HorizontalAlignment.Right;
                }
                else
                {
                    message.Orientation = HorizontalAlignment.Left;
                }
            }
            CurrentDialog = MessagesControl ?? (MessagesControl = new MessagesControl() { DataContext = this });
        }

        public bool CanExecuteSelectChatCommand(object param)
        {
            return true;
        }

        #endregion

        #region SendMessage

        private RelayCommand _sendMessageCommand;

        public RelayCommand SendMessageCommand
        {
            get
            {
                return _sendMessageCommand ?? (_sendMessageCommand =
                           new RelayCommand(param => ExecuteSendMessageCommand(param),
                               param => CanExecuteSendMessageCommand(param)));
            }
        }

        async Task<Message> ExecuteSendMessageCommand(object param)
        {
            if (CurrentDialog != null && MessageText!= null)
            {
                MessageParameters = new CreateMessageParameters()
                {
                    messageText = MessageText,
                    userFromId = GlobalUser.Id,
                    groupToId = CurrentGroup.Id
                };
                MessageText = null;
                var message = await DataAccess.DataAccess.CreateMessage(MessageParameters);
                message.ButtonVisibility = Visibility.Collapsed;
                message.TextblockFileNameVisibility = Visibility.Collapsed;
                message.OwnerAva = Parent.MainUserAva;
                message.OwnerName = Parent.MainUser.FirstName;
                UsersMessages.Add(message);
                return message;
            }
            else return null;
        }

        public bool CanExecuteSendMessageCommand(object param)
        {
            return true;
        }

        #endregion

        #region Exit

        private RelayCommand _exitCommand;

        public RelayCommand ExitCommand
        {
            get
            {
                return _exitCommand ?? (_exitCommand =
                           new RelayCommand(param => ExecuteExitCommand(param),
                               param => CanExecuteExitCommand(param)));
            }
        }

        public void ExecuteExitCommand(object param)
        {
            Parent.MainUserAva = null;
            AvatarUser = null;
            Parent.CurrentScreenType = ScreenTypes.Login;
            Parent.SetScreen();
        }

        public bool CanExecuteExitCommand(object param)
        {
            return true;
        }

        #endregion

        #region Send File

        private RelayCommand _sendFileCommand;

        public RelayCommand SendFileCommand
        {
            get
            {
                return _sendFileCommand ?? (_sendFileCommand =
                           new RelayCommand(param => ExecuteSendFileCommand(param),
                               param => CanExecuteSendFileCommand(param)));
            }
        }

        async Task<Message> ExecuteSendFileCommand(object param)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            Converter converter = new Converter();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png | Text files (*.txt)|*.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var path = dialog.FileName;
                var fileName = dialog.SafeFileName;
                var array = converter.ConverterImageToArray(path);
                Status = new byte();//получить значение
                MessageWithFile = new CreateMessageWithFileParameters()
                {
                    photo = array,
                    status = 1,
                    userFromId = GlobalUser.Id,
                    groupToId = CurrentGroup.Id,
                    name = fileName.Split('.')[0],
                    type = "." + fileName.Split('.')[1]
                };
                var message = await DataAccess.DataAccess.CreateMessageWithFile(MessageWithFile);
                message.AttachedFileName = fileName;
                UsersMessages.Add(message);
                message.TextblockVisibility = Visibility.Collapsed;
                message.OwnerAva = Parent.MainUserAva;
                message.OwnerName = Parent.MainUser.FirstName;
                return message;
            }
            return null;
        }

        public bool CanExecuteSendFileCommand(object param)
        {
            return true;
        }

        #endregion

        #region Open File

        private RelayCommand _openFileCommand;

        public RelayCommand OpenFileCommand
        {
            get
            {
                return _openFileCommand ?? (_openFileCommand =
                           new RelayCommand(param => ExecuteOpenFileCommand(param),
                               param => CanExecuteOpenFileCommand(param)));
            }
        }

        public void ExecuteOpenFileCommand(object param)
        {
            var result = DataAccess.DataAccess.GetFileFromId((param as Message).AttachedFile).Result;
            Converter converter = new Converter();
            converter.FromBytesToSmth(result);
            
        }

        public bool CanExecuteOpenFileCommand(object param)
        {
            return true;
        }

        #endregion

        #region Update User

        private async void Open(User user)
        {
            UpdateUserView view = new UpdateUserView()
            {
                DataContext = this
            };
            
            await DialogHost.Show(view, "UpdateUserDialog");
        }

        private RelayCommand _updateCommand;

        public RelayCommand UpdateCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand =
                           new RelayCommand(param => ExecuteUpdateCommand(param),
                               param => CanExecuteUpdateCommand(param)));
            }
        }

        async Task<User> ExecuteUpdateCommand(object param)
        {
            var parameters = new UpdateParameters();
            parameters.user = GlobalUser;
            parameters.newUser = UpdatedUser;
            var result = await DataAccess.DataAccess.UpdateUser(parameters);
            Parent.MainUser.FirstName = result.FirstName;
            Parent.MainUser.LastName = result.LastName;
            Parent.MainUser.Password = result.Password;
            DialogHost.CloseDialogCommand.Execute(null, null);
            RaisePropertyChanged(nameof(GlobalUser));
            return GlobalUser;
        }

        public bool CanExecuteUpdateCommand(object param)
        {
            return true;
        }

        private RelayCommand _toUpdateCommand;

        public RelayCommand ToUpdateCommand
        {
            get
            {
                return _toUpdateCommand ?? (_toUpdateCommand =
                           new RelayCommand(param => ExecuteToUpdateCommand(param),
                               param => CanExecuteToUpdateCommand(param)));
            }
        }

        public void ExecuteToUpdateCommand(object param)
        {
            UpdatedUser = new User()
            {
                Id = GlobalUser.Id,
                FirstName = GlobalUser.FirstName,
                LastName = GlobalUser.LastName,
                Password = GlobalUser.Password
            };
            Open(UpdatedUser);
        }

        public bool CanExecuteToUpdateCommand(object param)
        {
            return true;
        }
        #endregion

        public void Initialize()
        {
            var result = DataAccess.DataAccess.GetUsersChats(GlobalUser.Id).Result;
            UsersGroups = new ObservableCollection<Group>(result);
            
        }

        public object View()
        {
            return DialogsControl ?? (DialogsControl = new DialogsControl() { DataContext = this });
        }
    }
}
