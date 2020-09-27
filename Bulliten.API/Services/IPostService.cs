using Bulliten.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bulliten.API.Services
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetPersonalFeed();

        Task<IEnumerable<Post>> GetPublicFeed(string username);
    }
}