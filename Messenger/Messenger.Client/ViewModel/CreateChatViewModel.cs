using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Client.Model;
using Messenger.Client.View;
using Messenger.Model;
using Messenger.Client.Commands;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MaterialDesignThemes.Wpf;
using Messenger.Client.DataAccess;

namespace Messenger.Client.ViewModel
{
    class CreateChatViewModel : BaseModel, IScreen
    {
        protected CreateChatControl _createChatControl;
        public MainViewModel Parent { get; set; }
        public GroupCreateParameters GroupParameters { get; set; }
        public Group CurrentGroup { get; set; }
        public User CurrentUser { get; set; }
        public ObservableCollection<User> AllUsers { get; set; }
        public ObservableCollection<User> SelectedUsers { get; set; }
        //public BitmapImage UserImage { get; set; }

        public CreateChatViewModel(MainViewModel mvm)
        {
            Parent = mvm;
            GroupParameters = new GroupCreateParameters();
            var result = DataAccess.DataAccess.GetAllUsers().Result;
            AllUsers = new ObservableCollection<User>(result);
        }
        

        #region SelectUser

        private RelayCommand _selectUserCommand;

        public RelayCommand SelectUserCommand
        {
            get
            {
                return _selectUserCommand ?? (_selectUserCommand =
                           new RelayCommand(param => ExecuteSelectUserCommand(param),
                               param => CanExecuteSelectUserCommand(param)));
            }
        }

        public void ExecuteSelectUserCommand(object param)
        {
            var converter = new Converter();
            if (CurrentUser != null)
            {
                if (!SelectedUsers.Contains(CurrentUser))
                {
                    CurrentUser.UserBitmapImage = converter.ConverterByteToImage(CurrentUser.Ava); 
                    SelectedUsers.Add(CurrentUser);
                }
                if (AllUsers.Contains(CurrentUser))
                {
                    AllUsers.Remove(CurrentUser);
                }
            }
        }

        public bool CanExecuteSelectUserCommand(object param)
        {
            return true;
        }

        #endregion

        #region ButtonCreateGroup

        private RelayCommand _createGroupCommand;

        public RelayCommand CreateGroupCommand
        {
            get
            {
                return _createGroupCommand ?? (_createGroupCommand =
                           new RelayCommand(param => ExecuteCreateGroupCommand(param),
                               param => CanExecuteCreateGroupCommand(param)));
            }
        }

        async Task<Group> ExecuteCreateGroupCommand(object param)
        {
            SelectedUsers.Add(Parent.MainUser);
            ObservableCollection<Guid> UsersIds = new ObservableCollection<Guid>();
            foreach (var user in SelectedUsers)
            {
                UsersIds.Add(user.Id);
            }
            GroupParameters.members = UsersIds;
            if (CurrentGroup.GroupName == null)
            {
                foreach (var name in SelectedUsers)
                {
                    CurrentGroup.GroupName += name.FirstName + ", ";
                }
                CurrentGroup.GroupName = CurrentGroup.GroupName.TrimEnd(',', ' ');
                GroupParameters.name = CurrentGroup.GroupName;
            }
            else
            {
                GroupParameters.name = CurrentGroup.GroupName;
            }
            var group = await DataAccess.DataAccess.CreateChat(GroupParameters);
            Parent.CurrentScreenType = ScreenTypes.Dialogs;
            Parent.SetScreen();
            return group;
        }



        public bool CanExecuteCreateGroupCommand(object param)
        {
            return true;
        }

        #endregion

        #region DeleteChip

        private RelayCommand _deleteChipCommand;

        public RelayCommand DeleteChipCommand
        {
            get
            {
                return _deleteChipCommand ?? (_deleteChipCommand =
                           new RelayCommand(param => ExecuteDeleteChipCommand(param),
                               param => CanExecuteDeleteChipCommand(param)));
            }
        }

        public void ExecuteDeleteChipCommand(object param)
        {
            SelectedUsers.Remove(param as User);
        }

        public bool CanExecuteDeleteChipCommand(object param)
        {
            return true;
        }

        #endregion

        #region CancelButton

        private RelayCommand _cancelCommand;

        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand =
                           new RelayCommand(param => ExecuteCancelCommand(param),
                               param => CanExecuteCancelCommand(param)));
            }
        }

        public void ExecuteCancelCommand(object param)
        {
            Parent.CurrentScreenType = ScreenTypes.Dialogs;
            Parent.SetScreen();
        }

        public bool CanExecuteCancelCommand(object param)
        {
            return true;
        }

        #endregion

        public void Initialize()
        {
            CurrentGroup = new Group();
            SelectedUsers = new ObservableCollection<User>();
            //SelectedUsers.Add(Parent.MainUser);
        }

        public object View()
        {
            return _createChatControl ?? (_createChatControl = new CreateChatControl() { DataContext = this });
        }
    }
}
