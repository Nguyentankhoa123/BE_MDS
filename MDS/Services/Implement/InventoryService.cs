using AutoMapper;
using MDS.Model.Entity;
using MDS.Services.DTO.Inventory;
using MDS.Shared.Core.Enums;
using MDS.Shared.Database.DbContext;
using Microsoft.EntityFrameworkCore;

namespace MDS.Services.Implement
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public InventoryService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<InventoryObjectResponse> InsertInventory(int productId, int stock, string drugstoreId)
        {
            InventoryObjectResponse response = new();

            var inventory = new Inventory
            {
                ProductId = productId,
                Stock = stock,
                DrugstoreId = drugstoreId
            };

            _context.Inventories.Add(inventory);

            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Inventory created!";
            response.Data = _mapper.Map<InventoryResponse>(inventory);

            return response;
        }

        public async Task<int> ReservationInventory(int productId, int quantity, int cartId)
        {
            var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.ProductId == productId && x.Stock >= quantity);

            if (inventory == null)
            {
                return 0;
            }

            inventory.Stock -= quantity;

            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);

            if (product != null)
            {
                product.Quantity -= quantity;
            }

            var reservation = new Reservation
            {
                Quantity = quantity,
                CartId = cartId,
                CreateOn = DateTime.Now,
            };

            inventory.Reservations.Add(reservation);

            return await _context.SaveChangesAsync();
        }
    }
}
