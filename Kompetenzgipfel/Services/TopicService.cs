using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Models;
using Microsoft.AspNetCore.Identity;

namespace Kompetenzgipfel.Services;

public class TopicService(TopicRepository topicRepository, VoteRepository voteRepository, UserManager<User> userManager)
    : ITopicService
{
    public async Task<string> GetTopicsByPresenterId()
    {
        return await Task.FromResult("original implementation");
    }

    public async Task<Topic> AddTopic(TopicCreationDto toBeAdded, string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
        }

        var newTopic = Topic.Create(toBeAdded.Title, toBeAdded.Description ?? "", user);
        return await topicRepository.Create(newTopic);
    }

    public async Task<Topic> UpdateTopic(TopicUpdateDto updatedTopicCreationContent,
        string userName)
    {
        var topicToChange = await topicRepository.FetchBy(updatedTopicCreationContent.Id);
        if (topicToChange == null)
            throw new Exception("Invalid Topic Id");
        if (userName != topicToChange.User.UserName)
            throw new Exception("This Topic does not belong to you. Not allowed to update");
        topicToChange.Title = updatedTopicCreationContent.Title;
        topicToChange.Description = updatedTopicCreationContent.Description;
        return await topicRepository.Update(topicToChange);
    }

    public async Task<IEnumerable<Topic>> FetchAllExceptLoggedIn(string loggedInUserName)
    {
        return await topicRepository.GetAllExceptForUser(loggedInUserName);
    }

    public Task<IEnumerable<Topic>> FetchAllOfLoggedIn(string loggedInUserName)
    {
        return topicRepository.GetAllForUser(loggedInUserName);
    }

    public async Task AddTopicVote(string topicId, string loggedInUserName)
    {
        var topicToVote = await topicRepository.FetchBy(topicId);
        if (topicToVote == null) throw new Exception("Invalid Topic Id");
        var voter = await userManager.FindByNameAsync(loggedInUserName);
        var newVote = new Vote(topicToVote, voter!);
        if (topicToVote.Votes.Any(vote => vote.Voter.UserName == voter!.UserName))
            return;
        await voteRepository.Create(newVote);
    }

    public async Task RemoveTopicVote(string topicId, string loggedInUserName)
    {
        var topicToVote = await topicRepository.FetchBy(topicId);
        if (topicToVote == null) throw new Exception("Invalid Topic Id");
        var first = topicToVote.Votes.FirstOrDefault(vote => vote.Voter.UserName == loggedInUserName);
        if (first != null) await voteRepository.Remove(first);
    }
}