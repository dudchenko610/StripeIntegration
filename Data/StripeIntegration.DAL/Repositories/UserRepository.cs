using StripeIntegration.DAL.Repositories.Interfaces;
using StripeIntegration.Entities.Entities;

namespace StripeIntegration.DAL.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(DatabaseContext context) : base(context)
    {
    }
}