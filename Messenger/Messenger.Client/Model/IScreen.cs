using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Client.Model
{
    public interface IScreen
    {
        object View();
        void Initialize();
    }
}
