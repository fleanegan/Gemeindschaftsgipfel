using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Models;

namespace Kompetenzgipfel.Controllers.Helpers;

public abstract class ResponseGenerator
{
    public static List<OwnTopicResponseModel> GenerateOwnTopicResponses(IEnumerable<Topic> fetchAllExceptLoggedIn)
    {
        return fetchAllExceptLoggedIn
            .Select(topic => new OwnTopicResponseModel(topic.Id, topic.Title, topic.Description, topic.User.UserName,
                topic.Votes.Count))
            .ToList();
    }

    public static List<ForeignTopicResponseModel> GenerateForeignTopicResponses(
        IEnumerable<Topic> fetchAllExceptLoggedIn, string loggedInUserName)
    {
        return fetchAllExceptLoggedIn
            .Select(topic =>
                new ForeignTopicResponseModel(
                    topic.Id,
                    topic.Title,
                    topic.Description,
                    topic.User.UserName,
                    topic.Votes.Count(vote => vote.Voter.UserName == loggedInUserName) == 1
                )
            )
            .ToList();
    }

    public static List<SupportTaskResponseModel> GenerateSupportTaskResponses(IEnumerable<SupportTask> supportTasks)
    {
        return supportTasks
            .Select(task => new SupportTaskResponseModel(task.Id, task.Title, task.Description, task.Duration,
                task.RequiredSupporters,
                task.SupportPromises.Select(supporter => supporter.Supporter.UserName).ToList()))
            .ToList();
    }
}