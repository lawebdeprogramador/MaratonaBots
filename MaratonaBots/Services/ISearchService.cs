using MaratonaBots.Models.Search;
using System.Threading.Tasks;

namespace MaratonaBots.Services
{
    public interface ISearchService
    {
        Task<QnaMakerResultsRoot> FindAnswersAsync(string question);
    }
}
