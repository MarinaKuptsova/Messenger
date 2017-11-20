using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Converters;
using Messenger.Client.Model;
using Messenger.Client.View;
using Messenger.Client.Commands;
using Messenger.Model;

namespace Messenger.Client.ViewModel
{
    public class DialogsViewModel : BaseModel, IScreen
    {
        public object CurrentDialog { get; set; }
        public MainViewModel Parent { get; set; }
        protected DialogsControl DialogsControl;
        public Group currentGroup { get; set; } //отвечает за текущий диалог 

        public DialogsViewModel(MainViewModel mvm)
        {
            Parent = mvm;
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

        public void Initialize()
        {
        }

        public object View()
        {
            return DialogsControl ?? (DialogsControl = new DialogsControl() { DataContext = this });
        }
    }
}
