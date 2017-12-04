using System;
using Messenger.Model;

namespace Messenger.DataLayer
{
    public interface IFilesRepository
    {
        Files GetFileFromId(Guid id);
    }
}
