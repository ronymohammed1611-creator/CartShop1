using CartShop.DAL.Data;
using CartShop.DAL.Model;

namespace CartShop.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public ICartRepository Carts { get; private set; }
        public IGenericRepository<Product> Products { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IGenericRepository<OrderItem> OrderItems { get; private set; }
        public IGenericRepository<Offer> Offers { get; private set; }
        public IGenericRepository<ProductOffer> ProductOffers { get; private set; }
        public IGenericRepository<Recommendation> Recommendations { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Carts = new CartRepository(context);
            Products = new GenericRepository<Product>(context);
            Orders = new OrderRepository(context);
            OrderItems = new GenericRepository<OrderItem>(context);
            Offers = new GenericRepository<Offer>(context);
            ProductOffers = new GenericRepository<ProductOffer>(context);
            Recommendations = new GenericRepository<Recommendation>(context);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}