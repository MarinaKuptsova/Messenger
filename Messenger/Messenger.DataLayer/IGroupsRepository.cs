using System;
using System.Collections.Generic;
using Messenger.Model;

namespace Messenger.DataLayer
{
    public interface IGroupsRepository
    {
        Group Create(IEnumerable<Guid> members, string name);
        List<User> GetChatMembers(Guid groupId);
        void DeleteChat(Guid groupId);
        void AddMember(Guid memberId, Guid groupId);
        void DeleteMember(Guid memberId, Guid groupId);
        List<Message> GetUsersMessagesInGroup(Guid groupId);
        //List<Files> GetGroupsFiles(Guid groupId);
    }
}

