using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Client.Model;
using Messenger.Client.View;

namespace Messenger.Client.ViewModel
{
    public class UsersRegistrationViewModel : BaseModel, IScreen
    {
        protected UsersRegestrationControl _usersRegistrationView;

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public object View()
        {
            return _usersRegistrationView ??
                   (_usersRegistrationView = new UsersRegestrationControl() {DataContext = this});
        }
    }
}
