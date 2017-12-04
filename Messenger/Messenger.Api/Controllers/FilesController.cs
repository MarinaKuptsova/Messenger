using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Messenger.DataLayer;
using Messenger.DataLayer.Sql;
using Messenger.Model;

namespace Messenger.Api.Controllers
{
    public class FilesController : ApiController
    {
        private readonly IFilesRepository _filesRepository;
        private const string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Messenger;MultipleActiveResultSets=True;User Id=Marina; Password=1234Ma";

        public FilesController()
        {
            _filesRepository = new FilesRepository(ConnectionString);
        }

        [HttpGet]
        [Route("api/file/{id}")]
        public Files GetFileFromId(Guid id)
        {
            return _filesRepository.GetFileFromId(id);
        }
    }
}