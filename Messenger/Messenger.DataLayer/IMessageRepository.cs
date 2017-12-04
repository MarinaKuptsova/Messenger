using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Model;

namespace Messenger.DataLayer
{
    public interface IMessageRepository
    {
        Message Create(string messageText, Guid userFromId, Guid groupToId, byte status);
        Message CreateWithFile(Guid userFromId, Guid groupToId, byte[] photo, byte status, string name, string type);
        void Delete(Guid messageId);
        
    }
}
