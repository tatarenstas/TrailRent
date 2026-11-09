using Grpc.Core;
using Services.Catalog.Services;
using TrailRent.Catalog.Grpc;
namespace Services.Catalog.Grpc;
public sealed class CatalogGrpcService(TwoLevelCache cache) : CatalogService.CatalogServiceBase
{
 public static readonly Dictionary<int, ProductReply> Store = new(){[101]=new(){Id=101,Name="Alpine two-person tent",Sku="TENT-ALP-2",DailyRate=420,AvailableStock=5},[205]=new(){Id=205,Name="Carbon trekking poles",Sku="POLE-CARB-01",DailyRate=175,AvailableStock=12}};
 public override async Task<ProductReply> GetProduct(GetProductRequest request,ServerCallContext context){if(request.Id<=0)throw new RpcException(new Status(StatusCode.InvalidArgument,"Id must be positive"));var item=await cache.GetOrCreateAsync($"catalog:product:{request.Id}",()=>Task.FromResult(Store.GetValueOrDefault(request.Id)),context.CancellationToken);return item??throw new RpcException(new Status(StatusCode.NotFound,"Product was not found"));}
 public override async Task<ProductsReply> GetProductsByIds(GetProductsRequest request,ServerCallContext context){var reply=new ProductsReply();foreach(var id in request.Ids){var item=await GetProduct(new(){Id=id},context);reply.Products.Add(item);}return reply;}
 public async Task<ProductReply> UpdateAsync(int id,string name,CancellationToken ct){if(!Store.TryGetValue(id,out var old))throw new RpcException(new Status(StatusCode.NotFound,"Product was not found"));old.Name=name;await cache.InvalidateAsync($"catalog:product:{id}",ct);return old;}
}
