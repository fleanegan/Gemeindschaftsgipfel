using Gemeinschaftsgipfel.Controllers.DTOs;
using Gemeinschaftsgipfel.Exceptions;
using Gemeinschaftsgipfel.Models;
using Gemeinschaftsgipfel.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Gemeinschaftsgipfel.Services;

public class TopicService(
    TopicRepository topicRepository,
    VoteRepository voteRepository,
    ForumPostRepository forumPostRepository,
    UserManager<User> userManager,
    List<int> allowedPresentationDurations)
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
        PreventForbiddenPresentationDurations(toBeAdded.PresentationTimeInMinutes);
        var user = await userManager.FindByNameAsync(userName);
        var newTopic = Topic.Create(toBeAdded.Title, toBeAdded.PresentationTimeInMinutes, toBeAdded.Description ?? "",
            user!);
        return await topicRepository.Create(newTopic);
    }

    public async Task<Topic> UpdateTopic(TopicUpdateDto updatedTopicCreationContent,
        string userName)
    {
        var topicToChange = await topicRepository.FetchBy(updatedTopicCreationContent.Id);
        if (topicToChange == null)
            throw new TopicNotFoundException(updatedTopicCreationContent.Id);
        if (userName != topicToChange.User.UserName)
            throw new UnauthorizedTopicModificationException(topicToChange.Id);
        PreventForbiddenPresentationDurations(updatedTopicCreationContent.PresentationTimeInMinutes);
        topicToChange.Title = updatedTopicCreationContent.Title;
        topicToChange.PresentationTimeInMinutes = updatedTopicCreationContent.PresentationTimeInMinutes;
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
        if (topicToVote.Votes.Any(vote => vote.Voter.UserName.ToLower() == voter!.UserName.ToLower()))
            throw new VoteImpossibleException(topicId);
        await voteRepository.Create(newVote);
    }

    public async Task RemoveTopicVote(string topicId, string loggedInUserName)
    {
        var topicToVote = await topicRepository.FetchBy(topicId);
        if (topicToVote == null) throw new TopicNotFoundException(topicId);
        var first = topicToVote.Votes.FirstOrDefault(
            vote => vote.Voter.UserName.ToLower() == loggedInUserName.ToLower());
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

    public async Task AddForumPostToTopic(string topicId, string content, string userName)
    {
        var topic = await topicRepository.FetchBy(topicId);
        if (topic == null)
            throw new TopicNotFoundException(topicId);
        var user = await userManager.FindByNameAsync(userName);
        var post = ForumPost.Create(content, user!, topic);
        await forumPostRepository.Create(post);
    }

    public async Task<IEnumerable<ForumPost>> GetForumPostsForTopic(string topicId)
    {
        return await forumPostRepository.GetPostsForTopic(topicId);
    }

    private void PreventForbiddenPresentationDurations(int presentationTimeInMinutes)
    {
        if (!allowedPresentationDurations.Contains(presentationTimeInMinutes) && allowedPresentationDurations.Count > 0)
            throw new ArgumentOutOfRangeException(presentationTimeInMinutes.ToString());
    }

    public List<int> GetLegalPresentationDurations(){
   	return allowedPresentationDurations; 
    }
}
