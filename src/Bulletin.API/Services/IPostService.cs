using Bulletin.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bulletin.API.Services
{
    public interface IPostService
    {
        Task CreatePost(Post formPost);

        Task<IEnumerable<Post>> GetPersonalFeed();

        Task<IEnumerable<Post>> GetPublicFeed(string username);

        Task LikePost(int postId);

        Task RemoveLike(int postId);

        Task RemoveRePost(int postId);

        Task RePost(int postId);
    }
}