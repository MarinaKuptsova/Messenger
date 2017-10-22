using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Model;

namespace Messenger.DataLayer
{
    interface IMessageRepository
    {
        void Create(Message message, Guid userFromId, Guid userToId);
        void Delete(Guid id);
        IEnumerable<Message> GetUsersMessages(Guid userId);
    }
}
