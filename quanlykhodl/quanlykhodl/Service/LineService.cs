using AutoMapper;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
using System.Diagnostics;

namespace quanlykhodl.Service
{
    public class LineService : ILineService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IUserService _userService;
        public LineService(DBContext context, IOptions<Cloud> cloud, IMapper mapper, IUserService userService)
        {
            _context = context;
            _cloud = cloud.Value;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<PayLoad<LineDTO>> Add(LineDTO lineDTO)
        {
            try
            {
                var checkName = _context.linespage.Where(x => x.name == lineDTO.name && !x.deleted).FirstOrDefault();
                if (checkName != null)
                    return await Task.FromResult(PayLoad<LineDTO>.CreatedFail(Status.DATANULL));

                var checkArea = _context.areas.Where(x => x.id == lineDTO.area && !x.deleted).FirstOrDefault();
                if (checkArea == null)
                    return await Task.FromResult(PayLoad<LineDTO>.CreatedFail(Status.DATANULL));

                var mapData = _mapper.Map<Line>(lineDTO);
                mapData.areasids = checkArea;
                mapData.id_area = checkArea.id;

                _context.linespage.Add(mapData);
                _context.SaveChanges();

                var dataNew = _context.linespage.Where(x => !x.deleted).OrderByDescending(x => x.createdat).FirstOrDefault();
                var checkDataLine = _context.linespage.Where(x => x.id_area == dataNew.id_area && !x.deleted).ToList();
                var checkIndex = checkDataLine.FindIndex(x => x.id == dataNew.id && !x.deleted);

                var indexCode = checkIndex >= 1 && checkIndex <= 9 ? "0" + checkIndex.ToString() : checkIndex.ToString();
                dataNew.code = checkArea.name + indexCode;

                _context.linespage.Update(dataNew);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<LineDTO>.Successfully(lineDTO));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<LineDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkId = _context.linespage.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if(checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.deleted = true;
                _context.linespage.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.linespage.Where(x => !x.deleted).ToList();
                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.name.Contains(name)).ToList();

                var pageList = new PageList<object>(loadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private List<LinegetAll> loadData(List<Line> data)
        {
            var list = new List<LinegetAll>();

            foreach (var item in data)
            {
                list.Add(loadDataFindOne(item));
            }

            return list;
        }

        private LinegetAll loadDataFindOne(Line item)
        {
            var checkArea = _context.areas.Where(x => x.id == item.id_area && !x.deleted).FirstOrDefault();
            var mapData = _mapper.Map<LinegetAll>(item);
            mapData.id_area = checkArea == null ? null : checkArea.id;
            mapData.area_name = checkArea == null ? Status.DATANULL : checkArea.name;
            mapData.area_image = checkArea == null ? Status.DATANULL : checkArea.image;

            return mapData;
        }
        public async Task<PayLoad<object>> FindOne(int id)
        {
            try
            {
                var checkId = _context.linespage.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if(checkId == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(loadDataFindOne(checkId)));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<LineDTO>> Update(int id, LineDTO lineDTO)
        {
            try
            {
                var checkId = _context.linespage.Where(x => x.id == id && !x.deleted).FirstOrDefault();
                if (checkId == null)
                    return await Task.FromResult(PayLoad<LineDTO>.CreatedFail(Status.DATANULL));

                if(!checkQuantityLine(checkId.id, lineDTO.quantityshelf))
                    return await Task.FromResult(PayLoad<LineDTO>.CreatedFail(Status.FULLQUANTITY));

                var checkName = _context.linespage.Where(x => x.name == lineDTO.name && x.name != checkId.name).FirstOrDefault();
                if(checkName != null)
                    return await Task.FromResult(PayLoad<LineDTO>.CreatedFail(Status.DATANULL));

                var checkArea = _context.areas.Where(x => x.id == lineDTO.area && !x.deleted).FirstOrDefault();
                if(checkArea == null)
                    return await Task.FromResult(PayLoad<LineDTO>.CreatedFail(Status.DATANULL));

                checkId.name = lineDTO.name;
                checkId.quantityshelf = lineDTO.quantityshelf;
                checkId.areasids = checkArea;
                checkId.id_area = checkArea.id;

                _context.linespage.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<LineDTO>.Successfully(lineDTO));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<LineDTO>.CreatedFail(ex.Message));
            }
        }

        private bool checkQuantityLine(int id, int total)
        {
            var checkId = _context.shelfs.Where(x => x.line == id && !x.deleted).Count();
            if (checkId > total)
                return false;
            return true;
        }
    }
}
