using quanlykhodl.ViewModel;

namespace quanlykhodl.Common
{
    public class MapperData
    {
        public static TModel GanData<TModel, TData>(TModel model, TData dataDTO)
        {
            // Kiểm tra nếu model hoặc data null
            if (model == null || dataDTO == null)
                throw new Exception(Status.DATANULL);

            // Lặp qua các thuộc tính của model
            var modelProperties = model.GetType().GetProperties();
            var dataProperties = dataDTO.GetType().GetProperties();

            foreach (var property in modelProperties)
            {
                // Tìm thuộc tính tương ứng trong data
                var checkDataModel = dataProperties.FirstOrDefault(x => x.Name == property.Name);

                if (checkDataModel != null && property.CanWrite)
                {
                    // Lấy giá trị từ data và gán vào model
                    var value = checkDataModel.GetValue(dataDTO);
                    property.SetValue(model, value);
                }
            }

            return model;
        }
    }
}
