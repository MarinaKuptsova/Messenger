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
        Message Create(string messageText, Guid userFromId, Guid groupToId);
        Message CreateWithFile(string messageText, Guid userFromId, Guid groupToId, Files file);
        void Delete(Guid messageId);
        //Message GetMessage(Guid messageId);
        
    }
}
