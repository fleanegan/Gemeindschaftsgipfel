using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Models;

namespace Kompetenzgipfel.Services;

public interface ISupportTaskService
{
    Task<SupportTask> AddTask(SupportTaskCreationDto userInput, string loggedInUserName);
    Task CommitToSupportTask(string supportTaskId, string loggedInUserName);
    Task ResignFromSupportTask(string supportTaskId, string loggedInUserName);
    Task<IEnumerable<SupportTask>> GetAll();
}