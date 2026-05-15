using CartShop.DAL.Model;

namespace CartShop.DAL.Repositories
{
    public interface IUnitOfWork
    {
        ICartRepository Carts { get; }
        IGenericRepository<Product> Products { get; }
        IOrderRepository Orders { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<Offer> Offers { get; }
        IGenericRepository<ProductOffer> ProductOffers { get; }
        IGenericRepository<Recommendation> Recommendations { get; }
        Task<int> SaveChangesAsync();
    }
}