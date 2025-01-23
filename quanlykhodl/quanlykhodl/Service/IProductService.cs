using quanlykhodl.Common;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public interface IProductService
    {
        Task<PayLoad<ProductDTO>> Add(ProductDTO productDTO);
        Task<PayLoad<ProductAddAreas>> AddArea(ProductAddAreas productDTO);
        Task<PayLoad<ProductAddAreas>> UpdateArea(int id, ProductAddAreas productDTO);
        Task<PayLoad<ProductDTO>> Update(int id, ProductDTO productDTO);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<ProductGetAll>> FindOneById(int id);
        Task<PayLoad<bool>> checkLocation(checkLocation data);
        Task<PayLoad<object>> FindOneByCategory(int id, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOneByArea(int id);
        Task<PayLoad<object>> FindOneBySipplier(int id, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOneProductInWarehourse(int id);
        Task<PayLoad<object>> FindAllProductInWarehourse(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindCode(string code);
        Task<PayLoad<object>> FindCodeLocation(string code);


    }
}
