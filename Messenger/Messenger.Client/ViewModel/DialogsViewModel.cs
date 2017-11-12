using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Converters;
using Messenger.Client.Model;
using Messenger.Client.View;

namespace Messenger.Client.ViewModel
{
    public class DialogsViewModel : BaseModel, IScreen
    {
        public object CurrentDialog { get; set; }
        protected DialogsControl DialogsControl;

        public void Initialize()
        {
        }

        public object View()
        {
            return DialogsControl ?? (DialogsControl = new DialogsControl() { DataContext = this });
        }
    }
}
