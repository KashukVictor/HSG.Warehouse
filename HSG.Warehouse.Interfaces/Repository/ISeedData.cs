using System.Threading.Tasks;

namespace HSG.Warehouse.Interfaces.Repository
{
    public interface ISeedData
    {
        Task<string?> Seed();
    }
}
