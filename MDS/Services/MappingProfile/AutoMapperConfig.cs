using AutoMapper;
using MDS.Model.Entity;
using MDS.Services.DTO.Account;
using MDS.Services.DTO.Brand;
using MDS.Services.DTO.Cart;
using MDS.Services.DTO.Category;
using MDS.Services.DTO.Comment;
using MDS.Services.DTO.Discount;
using MDS.Services.DTO.FeedBack;
using MDS.Services.DTO.Inventory;
using MDS.Services.DTO.Order;
using MDS.Services.DTO.Product;

namespace MDS.Services.MappingProfile
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Product, ProductRequest>().ReverseMap();
            CreateMap<Product, ProductResponse>()
                .ForMember(x => x.SoldQuantity, opt => opt.MapFrom(src => src.Inventory.Reservations.Sum(r => r.Quantity)))
                .ForMember(x => x.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(x => x.Stock, opt => opt.MapFrom(src => src.Inventory.Stock))
                .ForMember(x => x.DrugstoreName, opt => opt.MapFrom(src => $"{src.Drugstore.FirstName} {src.Drugstore.LastName}"))
                .ReverseMap();

            CreateMap<Product, MedicineRequest>().ReverseMap();
            CreateMap<Product, MedicineResponse>().ReverseMap();

            CreateMap<Product, NotMedicineRequest>().ReverseMap();
            CreateMap<Product, NotMedicineResponse>().ReverseMap();

            CreateMap<Category, CategoryRequest>().ReverseMap();
            CreateMap<Category, CategoryResponse>().ReverseMap();
            CreateMap<Category, CategoryWithProductsResponse>().ReverseMap();

            CreateMap<Brand, BrandRequest>().ReverseMap();
            CreateMap<Brand, BrandResponse>().ReverseMap();
            CreateMap<Brand, BrandWithProductsResponse>().ReverseMap();

            CreateMap<ApplicationUser, AccountRegisterRequest>().ReverseMap();
            CreateMap<ApplicationUser, AccountRegisterResponse>().ReverseMap();

            CreateMap<ApplicationUser, AccountLoginRequest>().ReverseMap();
            CreateMap<ApplicationUser, AccountLoginResponse>().ReverseMap();

            CreateMap<Address, AddressRequest>().ReverseMap();
            CreateMap<Address, AddressResponse>().ReverseMap();

            CreateMap<Discount, DiscountRequest>().ReverseMap();
            CreateMap<Discount, DiscountResponse>().ReverseMap();

            CreateMap<Cart, CartResponse>().ReverseMap();
            CreateMap<CartItem, CartItemResponse>()
                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(x => x.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(x => x.PictureUrls, opt => opt.MapFrom(src => src.Product.PictureUrls))
                .ForMember(x => x.TotalPrice, opt => opt.MapFrom(src => src.Product.Price * src.Quantity))
                .ForMember(x => x.DrugstoreId, opt => opt.MapFrom(src => src.Product.DrugstoreId))
                .ForMember(x => x.DrugstoreName, opt => opt.MapFrom(src => $"{src.Product.Drugstore.FirstName} {src.Product.Drugstore.LastName}"))
                .ReverseMap();

            CreateMap<Inventory, InventoryResponse>().ReverseMap();

            CreateMap<Order, OrderRequest>().ReverseMap();
            CreateMap<Order, OrderResponse>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailResponse>()
                .ForMember(x => x.DrugstoreName, opt => opt.MapFrom(src => $"{src.Drugstore.FirstName} {src.Drugstore.LastName}"))
                .ReverseMap();

            CreateMap<Comment, CommentRequest>().ReverseMap();
            CreateMap<Comment, CommentResponse>()
                .ForMember(x => x.FullName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ReverseMap();

            CreateMap<FeedBack, FeedBackRequest>().ReverseMap();
            CreateMap<FeedBack, FeedBackResponse>()
                .ForMember(x => x.DrugstoreName, opt => opt.MapFrom(src => $"{src.Drugstore.FirstName} {src.Drugstore.LastName}"))
                .ForMember(x => x.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ReverseMap();

        }
    }
}
