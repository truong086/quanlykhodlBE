using Microsoft.EntityFrameworkCore;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public class StatisticalService : IStatisticalService
    {
        private readonly DBContext _dbcontext;
        public StatisticalService(DBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<PayLoad<object>> GetDaylyProductStatistics()
        {
            try
            {
                var data = _dbcontext.productDeliverynotes.Include(d => d.deliverynote_id1)
                    .Include(p => p.product).ThenInclude(pi => pi.imageProducts)
                    .Where(x => x.deliverynote_id1.CreatedAt.Day == DateTimeOffset.UtcNow.Day)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        DayNow = DateTimeOffset.UtcNow,
                        Day = g.CreatedAt.Day,
                        idproduct = g.product_map,
                        productName = g.product.title,
                        image = g.product.imageProducts
                    }).Select(x => new ProductSalesByDay
                    {
                        hourse = null,
                        dateNow = x.Key.DayNow,
                        Day = x.Key.Day,
                        ProductId = x.Key.idproduct.Value,
                        ProductName = x.Key.productName,
                        TotalQuantitySold = x.Sum(o => o.quantity),
                        TotalRevenue = x.Sum(od => od.quantity * (int)od.deliverynote_id1.price),
                        image = x.Key.image.Select(i => i.Link).ToList()
                    }).OrderBy(x => x.Day).ThenBy(r => r.TotalRevenue).ToList();

                //var data = _dbcontext.deliverynotes.Include(d => d.productDeliverynotes)
                //    .ThenInclude(p => p.product).ThenInclude(pi => pi.imageProducts)
                //    .AsEnumerable()
                //    .GroupBy(g => new
                //    {
                //        Day = g.CreatedAt.Day,
                //        productDeliryNote = g.productDeliverynotes,
                //        data = g.productDeliverynotes.Select(x => x.product)
                //    }).Select(x => new ProductSalesByDay
                //    {
                //        hourse = null,
                //        Day = x.Key.Day,
                //        TotalQuantitySold = x.Key.productDeliryNote.Sum(x => x.quantity),
                //        data = x.Key.data

                //    }).OrderBy(x => x.Day).ThenBy(r => r.TotalRevenue).ToList();
                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GetHourselyProductStatistics()
        {
            try
            {
                //var data = _dbcontext.productDeliverynotes.Include(d => d.deliverynote_id1)
                //    .Include(p => p.product).ThenInclude(pi => pi.imageProducts)
                //    .AsEnumerable()
                //    .GroupBy(g => new
                //    {
                //        Hourse = g.deliverynote_id1.CreatedAt.Hour,
                //        idproduct = g.product_map,
                //        productName = g.product.title,
                //        image = g.product.imageProducts
                //    }).Select(x => new ProductSalesByDay
                //    {
                //        hourse = x.Key.Hourse,
                //        ProductId = x.Key.idproduct.Value,
                //        ProductName = x.Key.productName,
                //        TotalQuantitySold = x.Sum(o => o.quantity),
                //        TotalRevenue = x.Sum(od => od.quantity * (int)od.deliverynote_id1.price),
                //        image = x.Key.image.Select(i => i.Link).ToList()
                //    }).OrderBy(x => x.Day).ThenBy(r => r.TotalRevenue).ToList();

                var data = _dbcontext.deliverynotes.Include(d => d.productDeliverynotes)
                    .ThenInclude(p => p.product).ThenInclude(pi => pi.imageProducts)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        Hourse = g.CreatedAt.Hour,
                        dataItem = g.productDeliverynotes.Select(pr => pr.product)
                    }).Select(x => new ProductSalesByDay
                    {
                        hourse = x.Key.Hourse,
                        TotalQuantitySold = x.Sum(o => o.quantity),
                        data = x.Key.dataItem.Select(x => new
                        {
                            id = x.id,
                            name = x.title,
                            image = x.imageProducts.Select(x => x.Link).ToList(),
                            price = x.price
                        }).ToList()
                    }).OrderBy(x => x.Day).ThenBy(r => r.TotalRevenue).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GetMonthlyProductStatistics()
        {
            try
            {
                var data = await _dbcontext.productDeliverynotes.Include(d => d.deliverynote_id1).Include(p => p.product).ThenInclude(pi => pi.imageProducts).GroupBy(g => new
                {
                    Month = g.deliverynote_id1.CreatedAt.Month,
                    Year = g.deliverynote_id1.CreatedAt.Year,
                    idproduct = g.product_map,
                    productName = g.product.title,
                    image = g.product.imageProducts
                }).Select(x => new ProductSalesByMonth
                {
                    Month = x.Key.Month,
                    Year = x.Key.Year,
                    ProductId = x.Key.idproduct.Value,
                    ProductName = x.Key.productName,
                    TotalQuantitySold = x.Sum(o => o.quantity),
                    TotalRevenue = x.Sum(od => od.quantity * (int)od.deliverynote_id1.price),
                    image = x.Key.image.Select(i => i.Link).ToList()
                }).OrderBy(x => x.Year).ThenBy(r => r.Month).ToListAsync();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GetTotalProductsSold()
        {
            try
            {
                var data = _dbcontext.productDeliverynotes
                    .Include(d => d.deliverynote_id1).Include(d => d.product).ThenInclude(pi => pi.imageProducts)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        productName = g.product.title,
                        image = g.product.imageProducts,
                        code = g.product.code
                    }).Select(x => new TotalProduct
                    {
                        title = x.Key.productName,
                        image = x.Key.image.Select(x => x.Link).ToList(),
                        total = x.Sum(o => o.quantity),
                        code = x.Key.code
                    }).OrderByDescending(x => x.total).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GetTotalProductsSoldByAccount()
        {
            try
            {
                var data = _dbcontext.deliverynotes.Include(a => a.account).Include(d => d.productDeliverynotes)
                    .GroupBy(g => new
                    {
                        accountName = g.account.username,
                        image = g.account.image,
                        email = g.account.email,
                    }).Select(x => new MonthlySalesAccountByProduct
                    {
                        username = x.Key.accountName,
                        image = x.Key.image,
                        email = x.Key.email,
                        total = x.SelectMany(o => o.productDeliverynotes).Sum(o => o.quantity),
                    }).OrderByDescending(x => x.total).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GetTotalProductsSoldByCustomer()
        {
            try
            {
                var data = _dbcontext.deliverynotes.Include(a => a.account).Include(s => s.retailcustomers_id).Include(d => d.productDeliverynotes)
                    .GroupBy(g => new
                    {
                        accountName = g.account.username,
                        image = g.account.image,
                        email = g.account.email,
                        customer = g.retailcustomers_id.name,
                        customer_address = g.retailcustomers_id.address,
                        customer_email = g.retailcustomers_id.email
                    }).Select(x => new LySalesAccountByProductCustomer
                    {
                        username = x.Key.accountName,
                        image = x.Key.image,
                        email = x.Key.email,
                        customer_name = x.Key.customer,
                        customer_address =x.Key.customer_address,
                        customer_email = x.Key.customer_email,
                        total = x.SelectMany(o => o.productDeliverynotes).Sum(o => o.quantity),
                    }).OrderByDescending(x => x.total).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GetTotalProductsSoldByMonth()
        {
            try
            {
                var data = await _dbcontext.deliverynotes.Include(pd => pd.productDeliverynotes).GroupBy(g => new
                {
                    Month = g.CreatedAt.Month,
                    Year = g.CreatedAt.Year
                }).Select(x => new MonthlySales
                {
                    Month = x.Key.Month,
                    Year = x.Key.Year,
                    TotalProductsSold = x.SelectMany(d => d.productDeliverynotes).Sum(x => x.quantity)
                }).OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> SetDayAndMonthAnhYearlyProductStatistics()
        {
            try
            {
                var data = _dbcontext.importforms
                    .Include(d => d.productImportforms).ThenInclude(d => d.products).ThenInclude(pi => pi.imageProducts)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        title = g.tite,
                        code = g.code,
                        data = g.productImportforms,
                        Day = g.CreatedAt.Day,
                        Hourse = g.CreatedAt.Hour,
                        Month = g.CreatedAt.Month,
                        Year = g.CreatedAt.Year
                    }).Select(x => new TotalProductImportFrom
                    {
                        title = x.Key.title,
                        data = x.Key.data.Select(x => x.products).Select(p => new
                        {
                            id = p.id,
                            title = p.title,
                            price = p.price,
                            quantity = p.quantity,
                            image = p.imageProducts.Select(x => x.Link).ToList()
                        }).ToList(),
                        Total = x.SelectMany(pri => pri.productImportforms).Sum(x => x.quantity),
                        code = x.Key.code
                    }).OrderByDescending(x => x.Total).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
        public async Task<PayLoad<object>> SetTotalProductsSold()
        {
            try
            {
                var data = _dbcontext.products1
                    .Include(a => a.account)
                    .Include(ip => ip.imageProducts)
                    .Include(d => d.productImportforms)
                    .ThenInclude(i => i.importform_id1)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        title = g.title,
                        code = g.code,
                        data = g.imageProducts,
                        Day = g.CreatedAt.Day,
                        Hourse = g.CreatedAt.Hour,
                        Month = g.CreatedAt.Month,
                        Year = g.CreatedAt.Year,
                        total = g.productImportforms,
                        description = g.description,
                        Id = g.id,
                        account = g.account,
                    }).Select(x => new TotalProductQuantity
                    {
                        Day = x.Key.Day, 
                        Hourse = x.Key.Hourse, 
                        Month = x.Key.Month, 
                        Year = x.Key.Year,
                        Id = x.Key.Id,
                        title = x.Key.title,
                        description = x.Key.description,
                        image = x.Key.data.Select(x => x.Link).ToList(),
                        Total = x.Key.total.Sum(x => x.quantity),
                        code = x.Key.code,
                        account_image = x.Key.account.image,
                        account_name = x.Key.account.username
                    }).OrderByDescending(x => x.Total).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> SetTotalImportFromProductsSoldBySupplier()
        {
            try
            {
                var data = _dbcontext.suppliers
                    .Include(d => d.productimportforms).ThenInclude(d => d.products).ThenInclude(pi => pi.imageProducts)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        name = g.name,
                        address = g.address,
                        image = g.image,
                        data = g.productimportforms
                    }).Select(x => new TotalProductSupplier
                    {
                        name = x.Key.name,
                        address = x.Key.address,
                        image = x.Key.image,
                        data = x.Key.data.Select(x => x.products).Select(p => new
                        {
                            id = p.id,
                            title = p.title,
                            price = p.price,
                            quantity = p.quantity,
                            image = p.imageProducts.Select(x => x.Link).ToList()
                        }).ToList(),
                        Total = x.SelectMany(pri => pri.productimportforms).Sum(x => x.quantity),
                    }).OrderByDescending(x => x.Total).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> SetTotalProductsSoldToDay()
        {
            try
            {
                var data = _dbcontext.importforms
                    .Include(d => d.productImportforms).ThenInclude(d => d.products).ThenInclude(pi => pi.imageProducts)
                    .Where(x => x.CreatedAt.Day == DateTimeOffset.UtcNow.Day)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        title = g.tite,
                        code = g.code,
                        data = g.productImportforms,
                        Day = g.CreatedAt.Day,
                        Hourse = g.CreatedAt.Hour,
                        Month = g.CreatedAt.Month,
                        Year = g.CreatedAt.Year
                    }).Select(x => new TotalProductImportFrom
                    {
                        title = x.Key.title,
                        data = x.Key.data.Select(x => x.products).Select(p => new
                        {
                            id = p.id,
                            title = p.title,
                            price = p.price,
                            quantity = p.quantity,
                            image = p.imageProducts.Select(x => x.Link).ToList()
                        }).ToList(),
                        Total = x.SelectMany(pri => pri.productImportforms).Sum(x => x.quantity),
                        code = x.Key.code
                    }).OrderByDescending(x => x.Total).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> SetTotalProductsSoldBySupplier()
        {
            try
            {
                var data = _dbcontext.suppliers
                    .Include(d => d.products)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        name = g.name,
                        address = g.address,
                        image = g.image,
                        data = g.products
                    }).Select(x => new TotalProductSupplier
                    {
                        name = x.Key.name,
                        address = x.Key.address,
                        image = x.Key.image,
                        data = x.Key.data.Select(p => new
                        {
                            id = p.id,
                            title = p.title,
                            price = p.price,
                            quantity = p.quantity,
                            image = p.imageProducts.Select(x => x.Link).ToList()
                        }).ToList(),
                        Total = x.Key.data.Sum(s => s.quantity)
                    }).OrderByDescending(x => x.Total).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}
