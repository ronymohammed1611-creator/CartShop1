using CartShop.BLL.Dtos;
using CartShop.BLL.Interfaces;
using CartShop.DAL.Model;
using CartShop.DAL.Model.CartShop.DAL.Model;
using CartShop.DAL.Model.Enums;
using CartShop.DAL.Repositories;

namespace CartShop.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ── Add To Cart ──
        public async Task<CartResponseDto> AddToCartAsync(string userId, AddToCartDto dto)
        {
            // 1. جيب أو إنشئ الـ Cart
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Status = CartStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }

            // 2. احسب السعر
            decimal unitPrice = 0;
            decimal totalPrice = 0;

            if (dto.UnitType == UnitType.Piece)
            {
                // نعتمد على السعر المرسل لو موجود، ولو مش موجود نحاول نجيبه من الـ DB كـ fallback
                if (dto.BaseUnitPrice.HasValue && dto.BaseUnitPrice.Value > 0)
                {
                    unitPrice = dto.BaseUnitPrice.Value;
                    totalPrice = dto.BaseUnitPrice.Value;
                }
                else
                {
                    var product = await _unitOfWork.Products.GetByNameAsync(dto.ProductName);
                    if (product == null)
                        return new CartResponseDto
                        {
                            Success = false,
                            Message = "المنتج مش موجود في الـ DB ولا تم إرسال سعر له"
                        };

                    unitPrice = product.Price;
                    totalPrice = product.Price;
                }
            }
            else // Weight
            {
                if (dto.WeightPrice == null || dto.BaseUnitPrice == null)
                    return new CartResponseDto
                    {
                        Success = false,
                        Message = "لازم تبعت السعر مع المنتج بالوزن"
                    };

                unitPrice = dto.BaseUnitPrice.Value;
                totalPrice = dto.WeightPrice.Value;
            }

            // 3. هل المنتج موجود في الـ Cart قبل كده؟
            var existingItem = cart.CartItems?
                .FirstOrDefault(ci => ci.
                
                
                ProductName == dto.ProductName
                                   && ci.UnitType == UnitType.Piece);

            if (existingItem != null && dto.UnitType == UnitType.Piece)
            {
                // زود الكمية
                existingItem.Quantity++;
                existingItem.TotalPrice = existingItem.UnitPrice * existingItem.Quantity;
                _unitOfWork.Carts.Update(cart);
            }
            else
            {
                // ضيف Item جديد
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductName = dto.ProductName,
                    ImageUrl = dto.ImageUrl,
                    WeightInGrams = dto.WeightInGrams,
                    Quantity = 1,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice,
                    UnitType = dto.UnitType,
                    AddedBy = AddedBy.AI,
                    AddedAt = DateTime.UtcNow
                };
                if (cart.CartItems == null)
                    cart.CartItems = new List<CartItem>();

                ((ICollection<CartItem>)cart.CartItems).Add(cartItem);
                _unitOfWork.Carts.Update(cart);
            }

            await _unitOfWork.SaveChangesAsync();

            return new CartResponseDto
            {
                Success = true,
                Message = "تم الإضافة للـ Cart",
                Cart = MapToCartDto(cart)
            };
        }

        // ── Get Cart ──
        public async Task<CartResponseDto> GetCartAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
                return new CartResponseDto
                {
                    Success = false,
                    Message = "مفيش Cart"
                };

            return new CartResponseDto
            {
                Success = true,
                Cart = MapToCartDto(cart)
            };
        }

        // ── Update Quantity ──
        public async Task<CartResponseDto> UpdateQuantityAsync(
            string userId, int cartItemId, int quantity)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
                return new CartResponseDto
                {
                    Success = false,
                    Message = "مفيش Cart"
                };

            var item = cart.CartItems?.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item == null)
                return new CartResponseDto
                {
                    Success = false,
                    Message = "المنتج مش موجود في الـ Cart"
                };

            if (item.UnitType == UnitType.Weight)
                return new CartResponseDto
                {
                    Success = false,
                    Message = "منتجات الوزن بتتحدث من الميزان مش يدوي"
                };

            if (quantity <= 0)
            {
                cart.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
                item.TotalPrice = item.UnitPrice * quantity;
            }

            _unitOfWork.Carts.Update(cart);
            await _unitOfWork.SaveChangesAsync();

            return new CartResponseDto
            {
                Success = true,
                Message = "تم التحديث",
                Cart = MapToCartDto(cart)
            };
        }

        // ── Remove Item ──
        public async Task<CartResponseDto> RemoveItemAsync(string userId, int cartItemId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
                return new CartResponseDto
                {
                    Success = false,
                    Message = "مفيش Cart"
                };

            var item = cart.CartItems?.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item == null)
                return new CartResponseDto
                {
                    Success = false,
                    Message = "المنتج مش موجود في الـ Cart"
                };

            cart.CartItems.Remove(item);
            _unitOfWork.Carts.Update(cart);
            await _unitOfWork.SaveChangesAsync();

            return new CartResponseDto
            {
                Success = true,
                Message = "تم الحذف",
                Cart = MapToCartDto(cart)
            };
        }

        // ── Clear Cart ──
        public async Task<CartResponseDto> ClearCartAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
                return new CartResponseDto
                {
                    Success = false,
                    Message = "مفيش Cart"
                };

            cart.CartItems?.Clear();
            _unitOfWork.Carts.Update(cart);
            await _unitOfWork.SaveChangesAsync();

            return new CartResponseDto
            {
                Success = true,
                Message = "تم مسح الـ Cart"
            };
        }

        // ── Helper ──
        private CartDto MapToCartDto(Cart cart)
        {
            var items = cart.CartItems?.Select(ci => new CartItemDto
            {
                CartItemId = ci.Id,
                ProductName = ci.ProductName,
                ImageUrl = ci.ImageUrl,
                WeightInGrams = ci.WeightInGrams,
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice,
                TotalPrice = ci.TotalPrice,
                UnitType = ci.UnitType.ToString()
            }).ToList() ?? new List<CartItemDto>();

            return new CartDto
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                Items = items,
                TotalPrice = items.Sum(i => i.TotalPrice)
            };
        }
    }
}