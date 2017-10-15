using System;

public interface IGroupsRepository
{
    Chat Create(IEnumerable<Guid> members, string name);
    IEnumerable<Chat> GetUserChats(Guid userId);
    void DeleteChat(Guid chatId);
}
