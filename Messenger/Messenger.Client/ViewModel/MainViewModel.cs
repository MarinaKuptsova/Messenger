using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Client.Model;

namespace Messenger.Client.ViewModel
{
    public class MainViewModel : BaseModel
    {
        private MainWindow _window;
        public ScreenTypes CurrentScreenType { get; set; }
        private DialogsViewModel dialogsVM;
        private UsersViewModel usersVM;
        private CreateChatViewModel _createChatViewModel;
        private UsersRegistrationViewModel _usersRegistrationViewModel;

        public MainViewModel()
        {
            CurrentScreenType = ScreenTypes.Register;
        }

        public MainWindow Window
        {
            get
            {
                if (_window == null)
                {
                    _window = new MainWindow() {DataContext = this};
                }
                return _window;
            }
        }

        private object _currentContent;
        public object CurrentContent
        {
            get { return _currentContent; }
            set
            {
                if (_currentContent != value)
                {
                    _currentContent = value;
                    RaisePropertyChanged(nameof(CurrentContent));
                }
            }
        }

        public void SetScreen()
        {
            IScreen screen;
            switch (CurrentScreenType)
            {
                case ScreenTypes.Dialogs:
                    screen = dialogsVM ?? (dialogsVM = new DialogsViewModel(this));
                    break;
                case ScreenTypes.Login:
                    screen = usersVM ?? (usersVM = new UsersViewModel(this));
                    break;
                case ScreenTypes.Register:
                    screen = _usersRegistrationViewModel ?? (_usersRegistrationViewModel = new UsersRegistrationViewModel(this));
                    break;
                case ScreenTypes.Group:
                    screen = _createChatViewModel ?? (_createChatViewModel = new CreateChatViewModel(this));
                    break;
                default:
                    screen = null;
                    break;
            }
            CurrentContent = screen.View();
            screen.Initialize();
        }


    }
}
