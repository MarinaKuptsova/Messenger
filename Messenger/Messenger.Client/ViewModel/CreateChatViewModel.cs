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

        public CreateChatViewModel(MainViewModel mvm)
        {
            Parent = mvm;
            CurrentGroup = new Group();
            GroupParameters = new GroupCreateParameters();
            var result = DataAccess.DataAccess.GetAllUsers().Result;
            AllUsers = new ObservableCollection<User>(result);
            SelectedUsers = new ObservableCollection<User>();
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
            if (CurrentUser != null)
            {
                if (!SelectedUsers.Contains(CurrentUser))
                {
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
            ObservableCollection<Guid> UsersIds = new ObservableCollection<Guid>();
            foreach (var user in SelectedUsers)
            {
                UsersIds.Add(user.Id);
            }
            GroupParameters.members = UsersIds;
            GroupParameters.name = CurrentGroup.GroupName;
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

        public void Initialize()
        {
        }

        public object View()
        {
            return _createChatControl ?? (_createChatControl = new CreateChatControl() { DataContext = this });
        }
    }
}
