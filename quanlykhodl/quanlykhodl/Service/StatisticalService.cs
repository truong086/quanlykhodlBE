using CloudinaryDotNet.Core;
using Microsoft.EntityFrameworkCore;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

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
                var data = _dbcontext.productdeliverynotes.Include(d => d.deliverynote_id1)
                    .Include(p => p.product).ThenInclude(pi => pi.imageProducts)
                    .Where(x => x.deliverynote_id1.createdat.Day == DateTimeOffset.UtcNow.Day)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        DayNow = DateTimeOffset.UtcNow,
                        Day = g.createdat.Day,
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
                        image = x.Key.image.Select(i => i.link).ToList()
                    }).OrderBy(x => x.Day).ThenBy(r => r.TotalRevenue).ToList();

                //var data = _dbcontext.deliverynotes.Include(d => d.productdeliverynotes)
                //    .ThenInclude(p => p.product).ThenInclude(pi => pi.imageproducts)
                //    .AsEnumerable()
                //    .GroupBy(g => new
                //    {
                //        Day = g.createdat.Day,
                //        productDeliryNote = g.productdeliverynotes,
                //        data = g.productdeliverynotes.Select(x => x.product)
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
                //var data = _dbcontext.productdeliverynotes.Include(d => d.deliverynote_id1)
                //    .Include(p => p.product).ThenInclude(pi => pi.imageproducts)
                //    .AsEnumerable()
                //    .GroupBy(g => new
                //    {
                //        Hourse = g.deliverynote_id1.createdat.Hour,
                //        idproduct = g.product_map,
                //        productName = g.product.title,
                //        image = g.product.imageproducts
                //    }).Select(x => new ProductSalesByDay
                //    {
                //        hourse = x.Key.Hourse,
                //        ProductId = x.Key.idproduct.Value,
                //        ProductName = x.Key.productName,
                //        TotalQuantitySold = x.Sum(o => o.quantity),
                //        TotalRevenue = x.Sum(od => od.quantity * (int)od.deliverynote_id1.price),
                //        image = x.Key.image.Select(i => i.link).ToList()
                //    }).OrderBy(x => x.Day).ThenBy(r => r.TotalRevenue).ToList();

                var data = _dbcontext.deliverynotes.Include(d => d.productDeliverynotes)
                    .ThenInclude(p => p.product).ThenInclude(pi => pi.imageProducts)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        Hourse = g.createdat.Hour,
                        dataItem = g.productDeliverynotes.Select(pr => pr.product)
                    }).Select(x => new ProductSalesByDay
                    {
                        hourse = x.Key.Hourse,
                        TotalQuantitySold = x.Sum(o => o.quantity),
                        data = x.Key.dataItem.Select(x => new
                        {
                            id = x.id,
                            name = x.title,
                            image = x.imageProducts.Select(x => x.link).ToList(),
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
                //var data = _dbcontext.productdeliverynotes.Include(d => d.deliverynote_id1).Include(p => p.product).ThenInclude(pi => pi.imageproducts)
                //    .AsEnumerable()
                //    .GroupBy(g => new
                //    {
                //        Month = g.deliverynote_id1.createdat.Month,
                //        Year = g.deliverynote_id1.createdat.Year,

                //    }).Select(x => new ProductSalesByMonth
                //    {
                //        Month = x.Key.Month,
                //        Year = x.Key.Year,
                //        TotalQuantitySold = x.Sum(o => o.quantity),
                //        TotalRevenue = x.Sum(od => od.quantity * (int)od.deliverynote_id1.price),
                //        producSalesData = x.Select(pi => pi.product).Select(pi => new producSales
                //        {
                //            ProductName = pi.title,
                //            Quantity = x.Where(d => d.product_map == pi.id).Select(dx => dx.quantity).FirstOrDefault(),
                //            images = pi.imageproducts.Select(i => i.link).ToList()
                //        }).ToList()
                //    }).OrderBy(x => x.Year).ThenBy(r => r.Month).ToList();

                var data = _dbcontext.productdeliverynotes.Include(d => d.deliverynote_id1).Include(p => p.product).ThenInclude(pi => pi.imageProducts)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        Month = g.deliverynote_id1.createdat.Month,
                        Year = g.deliverynote_id1.createdat.Year,
                        g.product // Group By theo product
                    }).Select(x => new 
                    {
                        Month = x.Key.Month,
                        Year = x.Key.Year,
                        ProductName = x.First().product.title, // "x.First()" lấy bản ghi đầu tiên trong nhóm (vì trong mỗi nhóm, tất cả các bản ghi đều có cùng ProductId, nên tên sản phẩm luôn giống nhau).
                        Image = x.First().product.imageProducts,
                        TotalSold = x.Sum(d => d.quantity)
                    }).GroupBy(g => g.Month) // Nhóm lại theo tháng
                    .Select(x => new ProductSalesByMonth
                    {
                        Month = x.Key,
                        TotalQuantitySold = x.Sum(t => t.TotalSold),
                        producSalesData = x.Select(p => new producSales
                        {
                            ProductName = p.ProductName,
                            images = p.Image.Select(i => i.link).ToList(),
                            Quantity = p.TotalSold
                        }).ToList()
                    })
                    .OrderBy(x => x.Year).ThenBy(r => r.Month).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GetTotalProductsSold()
        {
            try
            {
                var data = _dbcontext.productdeliverynotes
                    .Include(d => d.deliverynote_id1).Include(d => d.product).ThenInclude(pi => pi.imageProducts)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        id = g.product.id,
                        productName = g.product.title,
                        image = g.product.imageProducts,
                        code = g.product.code
                    }).Select(x => new TotalProduct
                    {
                        id = x.Key.id,
                        title = x.Key.productName,
                        image = x.Key.image.Select(x => x.link).ToList(),
                        total = x.Sum(o => o.quantity),
                        code = x.Key.code
                    }).OrderByDescending(x => x.total).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GetTotalProductsSoldByCustomer()
        {
            try
            {
                var data = _dbcontext.retailcustomers.Include(a => a.deliverynotes).ThenInclude(s => s.productDeliverynotes)
                    .ThenInclude(d => d.product).ThenInclude(pi => pi.imageProducts).AsEnumerable()
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        id = g.id,
                        customerName = g.name,
                        email = g.email
                    }).Select(x => new LySalesAccountByProductCustomer
                    {
                        id = x.Key.id,
                        customer_name = x.Key.customerName,
                        customer_email = x.Key.email,
                        producSalesData = x.SelectMany(d => d.deliverynotes).SelectMany(ds => ds.productDeliverynotes).GroupBy(ds1 => ds1.product.id).Select(dataItem => new producSales
                        {
                            ProductName = dataItem.First().product.title,
                            Quantity = dataItem.Sum(o => o.quantity),
                            images = dataItem.First().product.imageProducts.Select(x => x.link).ToList()
                        }).ToList(),
                        total = x.SelectMany(o => o.deliverynotes).SelectMany(d => d.productDeliverynotes).Sum(o => o.quantity),
                    }).OrderByDescending(x => x.total).ToList();

                //var data = _dbcontext.deliverynotes.Include(a => a.retailcustomers_id).Include(s => s.productDeliverynotes)
                //    .ThenInclude(d => d.product).ThenInclude(pi => pi.imageProducts).AsEnumerable()
                //    .GroupBy(g => new
                //    {
                //        g.retailcustomers_id
                //    }).Select(x => new 
                //    {
                //        id = x.First().retailcustomers_id.id,
                //        email = x.First().retailcustomers_id.email,
                //        customerName = x.First().retailcustomers_id.name,
                //        product = x.First().productDeliverynotes,
                //        total = x.SelectMany(x => x.productDeliverynotes).Sum(x => x.quantity),
                //        productTotal = x.First().productDeliverynotes.Select(x => x.product),
                //        productTotalQuantity = x.First().productDeliverynotes
                //    }).GroupBy(x => x.id)
                //    .Select(s2 => new
                //    {
                //        productId = s2.First().productTotal.Select(x => x.id),
                //        id = s2.First().id,
                //        email = s2.First().email,
                //        customerName = s2.First().customerName,
                //        product = s2.First().product,
                //        total = s2.First().total,
                //        productTotal = s2.First().productTotalQuantity

                //    })
                //    .GroupBy(x => x.productId)
                //    .Select(s2 => new
                //    {
                //        data = s2.Select(s3 => new LySalesAccountByProductCustomer
                //        {
                //            id = s3.id,
                //            customer_email = s3.email,
                //            customer_name = s3.customerName,
                //            total = s3.total,
                //            producSalesData = s3.product.Select(p => p.product).Select(p2 => new producSales
                //            {
                //                id = p2.id,
                //                ProductName = p2.title,
                //                Quantity = s3.productTotal.Sum(x => x.quantity),
                //                images = p2.imageProducts.Select(x => x.link).ToList()
                //            }).ToList()
                //        }),
                //        total = s2.Sum(x => x.total),
                //    })
                //    .OrderByDescending(x => x.total).ToList();

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
                    Month = g.createdat.Month,
                    Year = g.createdat.Year
                }).Select(x => new MonthlySales
                {
                    Month = x.Key.Month,
                    Year = x.Key.Year,
                    TotalProductsSold = x.SelectMany(d => d.productDeliverynotes).Sum(x => x.quantity)
                }).OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
        public async Task<PayLoad<object>> SetDayAndMonthAnhYearlyProductStatistics()
        {
            try
            {
                //var data = _dbcontext.importforms
                //    .Include(d => d.productimportforms).ThenInclude(d => d.products).ThenInclude(pi => pi.imageproducts)
                //    .AsEnumerable()
                //    .GroupBy(g => new
                //    {
                //        title = g.tite,
                //        code = g.code,
                //        data = g.productimportforms,
                //        Day = g.createdat.Day,
                //        Hourse = g.createdat.Hour,
                //        Month = g.createdat.Month,
                //        Year = g.createdat.Year
                //    }).Select(x => new TotalProductImportFrom
                //    {
                //        Day = x.Key.Day,
                //        Hourse = x.Key.Hourse,
                //        Month = x.Key.Month,
                //        Year = x.Key.Year,
                //        title = x.Key.title,
                //        data = x.Key.data.Select(x => x.products).Select(p => new
                //        {
                //            id = p.id,
                //            title = p.title,
                //            price = p.price,
                //            quantity = p.quantity,
                //            image = p.imageproducts.Select(x => x.link).ToList()
                //        }).ToList(),
                //        Total = x.SelectMany(pri => pri.productimportforms).Sum(x => x.quantity),
                //        code = x.Key.code
                //    }).OrderByDescending(x => x.Total).ToList();


                var data = _dbcontext.productimportforms
                    .Include(d => d.importform_id1).Include(d => d.products).ThenInclude(pi => pi.imageProducts)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {

                        Day = g.importform_id1.createdat.Day,
                        Month = g.importform_id1.createdat.Month,
                        Year = g.importform_id1.createdat.Year,
                        g.products
                    }).Select(x => new
                    {
                        Month = x.Key.Month,
                        Year = x.Key.Year,
                        ProductName = x.First().products.title,
                        images = x.First().products.imageProducts,
                        price = x.First().products.price,
                        quantity = x.First().products.quantity,
                        id = x.First().products.id,
                        total = x.Sum(t => t.quantity)
                    }).GroupBy(g2 => new
                    {
                        month = g2.Month,

                    }).Select(x2 => new TotalProductImportFrom
                    {
                        Month = x2.Key.month,
                        Total = x2.Sum(xt => xt.total),
                        data = x2.Select(xd => new
                        {
                            id = xd.id,
                            title = xd.ProductName,
                            price = xd.price,
                            quantity = xd.quantity,
                            image = xd.images.Select(x => x.link).ToList(),
                            totalProduct = xd.total
                        }).ToList()
                    })
                    .OrderByDescending(x => x.Total).ToList();


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
                        Day = g.createdat.Day,
                        Hourse = g.createdat.Hour,
                        Month = g.createdat.Month,
                        Year = g.createdat.Year,
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
                        image = x.Key.data.Select(x => x.link).ToList(),
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
                            image = p.imageProducts.Select(x => x.link).ToList()
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
                    .Where(x => x.createdat.Day == DateTimeOffset.UtcNow.Day)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        title = g.tite,
                        code = g.code,
                        data = g.productImportforms,
                        Day = g.createdat.Day,
                        Hourse = g.createdat.Hour,
                        Month = g.createdat.Month,
                        Year = g.createdat.Year
                    }).Select(x => new TotalProductImportFrom
                    {
                        title = x.Key.title,
                        data = x.Key.data.Select(x => x.products).Select(p => new
                        {
                            id = p.id,
                            title = p.title,
                            price = p.price,
                            quantity = p.quantity,
                            image = p.imageProducts.Select(x => x.link).ToList()
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

        public async Task<PayLoad<object>> SetTotalProductsSoldBySupplier()
        {
            try
            {
                var data = _dbcontext.suppliers
                    .Include(d => d.products)
                    .ThenInclude(ip => ip.imageProducts)
                    .AsEnumerable()
                    .GroupBy(g => new
                    {
                        id = g.id,
                        name = g.name,
                        address = g.address,
                        image = g.image,
                        data = g.products
                    }).Select(x => new TotalProductSupplier
                    {
                        id = x.Key.id,
                        name = x.Key.name,
                        address = x.Key.address,
                        image = x.Key.image,
                        data = x.Key.data.Select(p => new
                        {
                            id = p.id,
                            title = p.title,
                            price = p.price,
                            quantity = p.quantity,
                            image = p.imageProducts.Select(x => x.link).ToList()
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