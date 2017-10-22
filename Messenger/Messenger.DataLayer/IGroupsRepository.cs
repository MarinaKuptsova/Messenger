using System;
using System.Collections.Generic;
using Messenger.Model;

namespace Messenger.DataLayer
{
    public interface IGroupsRepository
    {
        void Create(IEnumerable<Guid> members, string name);
        IEnumerable<Group> GetUserChats(Guid userId);
        void DeleteChat(Guid groupId);
        void AddMember(Guid memberId, Guid groupId);
        void DeleteMember(Guid memberId, Guid groupId);
    }
}

