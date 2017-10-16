using System;

public interface IUsersRepository
{
    User Create(User user);
    void Delete(Guid id);
    User Get(Guid id);
    void Update(Guid id);
}
