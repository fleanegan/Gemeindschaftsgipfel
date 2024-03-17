using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Exceptions;
using Kompetenzgipfel.Models;
using Microsoft.AspNetCore.Identity;

namespace Kompetenzgipfel.Services;

public class TopicService(TopicRepository topicRepository, VoteRepository voteRepository, UserManager<User> userManager)
    : ITopicService
{
    public async Task<Topic> GetTopicById(string topicId)
    {
        var result = await topicRepository.FetchBy(topicId);
        if (result == null) throw new TopicNotFoundException(topicId);
        return result;
    }

    public async Task<Topic> AddTopic(TopicCreationDto toBeAdded, string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        var newTopic = Topic.Create(toBeAdded.Title, toBeAdded.Description ?? "", user!);
        return await topicRepository.Create(newTopic!);
    }

    public async Task<Topic> UpdateTopic(TopicUpdateDto updatedTopicCreationContent,
        string userName)
    {
        var topicToChange = await topicRepository.FetchBy(updatedTopicCreationContent.Id);
        if (topicToChange == null)
            throw new TopicNotFoundException(updatedTopicCreationContent.Id);
        if (userName != topicToChange.User.UserName)
            throw new BatschungaException(topicToChange.Id);
        topicToChange.Title = updatedTopicCreationContent.Title;
        topicToChange.Description = updatedTopicCreationContent.Description ?? "";
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
        if (topicToVote == null) throw new TopicNotFoundException(topicId);
        var voter = await userManager.FindByNameAsync(loggedInUserName);
        var newVote = new Vote(topicToVote, voter!);
        if (topicToVote.Votes.Any(vote => vote.Voter.UserName == voter!.UserName))
            throw new VoteImpossibleException(topicId);
        await voteRepository.Create(newVote);
    }

    public async Task RemoveTopicVote(string topicId, string loggedInUserName)
    {
        var topicToVote = await topicRepository.FetchBy(topicId);
        if (topicToVote == null) throw new TopicNotFoundException(topicId);
        var first = topicToVote.Votes.FirstOrDefault(vote => vote.Voter.UserName == loggedInUserName);
        if (first != null) await voteRepository.Remove(first);
    }

    public async Task RemoveTopic(string topicId, string loggedInUserName)
    {
        var topic = await topicRepository.FetchBy(topicId);
        if (topic == null)
            throw new TopicNotFoundException(topicId);
        if (topic.User.UserName != loggedInUserName)
            throw new Exception("Not your Topic");
        await topicRepository.Remove(topic);
    }
}