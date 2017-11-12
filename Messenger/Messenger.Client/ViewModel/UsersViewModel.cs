using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Messenger.Client.Model;
using Messenger.Client.View;

namespace Messenger.Client.ViewModel
{
    public class UsersViewModel : BaseModel, IScreen
    {
        protected UsersControl _usersControl;
        public void Initialize()
        {
        }

        public object View()
        {
            return _usersControl ?? (_usersControl = new UsersControl() {DataContext = this});
        }
    }
}
