using AutoMapper;
using TrailRent.Reviews.Application.Dtos;
using TrailRent.Reviews.Domain.Entities;
namespace TrailRent.Reviews.Application.Mapping;
public sealed class ReviewProfile : Profile
{ public ReviewProfile() => CreateMap<Review, ReviewDto>().ForCtorParam("Id", m => m.MapFrom(s => s.Id.ToString())).ForCtorParam("ProductId", m => m.MapFrom(s => s.Product.ProductId)).ForCtorParam("ProductName", m => m.MapFrom(s => s.Product.ProductName)).ForCtorParam("AuthorName", m => m.MapFrom(s => s.Author.DisplayName)).ForCtorParam("CommentsCount", m => m.MapFrom(s => s.Comments.Count)); }
