using Gemeinschaftsgipfel.Controllers.DTOs;
using Gemeinschaftsgipfel.Models;

namespace Gemeinschaftsgipfel.Services;

public interface ISupportTaskService
{
    Task<SupportTask> AddTask(SupportTaskCreationDto userInput, string loggedInUserName);
    Task CommitToSupportTask(string supportTaskId, string loggedInUserName);
    Task ResignFromSupportTask(string supportTaskId, string loggedInUserName);
    Task<IEnumerable<SupportTask>> GetAll();
}