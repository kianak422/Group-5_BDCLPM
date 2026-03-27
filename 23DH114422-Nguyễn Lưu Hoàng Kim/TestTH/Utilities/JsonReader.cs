using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestTH.Utilities
{
    /// <summary>
    /// JsonReader cung cấp các phương thức tiện ích để đọc dữ liệu test từ file JSON.
    /// Hỗ trợ đọc file JSON thành dynamic object hoặc deserialize thành class cụ thể.
    /// </summary>
    public static class JsonReader
    {
        /// <summary>
        /// Đọc toàn bộ nội dung file JSON và trả về dưới dạng JObject.
        /// Sử dụng khi cần truy cập linh hoạt các trường trong JSON.
        /// </summary>
        /// <param name="filePath">Đường dẫn tương đối hoặc tuyệt đối đến file JSON.</param>
        /// <returns>JObject chứa toàn bộ dữ liệu JSON.</returns>
        /// <exception cref="FileNotFoundException">Khi file JSON không tồn tại.</exception>
        public static JObject ReadJsonFile(string filePath)
        {
            // Xây dựng đường dẫn đầy đủ từ thư mục output
            string fullPath = GetFullPath(filePath);

            // Kiểm tra file có tồn tại không
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Không tìm thấy file JSON: {fullPath}");
            }

            // Đọc nội dung file và parse thành JObject
            string jsonContent = File.ReadAllText(fullPath);
            return JObject.Parse(jsonContent);
        }

        /// <summary>
        /// Đọc file JSON và deserialize thành một object kiểu T.
        /// Sử dụng khi muốn map dữ liệu JSON sang một class cụ thể.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu đích để deserialize.</typeparam>
        /// <param name="filePath">Đường dẫn tương đối hoặc tuyệt đối đến file JSON.</param>
        /// <returns>Object kiểu T chứa dữ liệu từ file JSON.</returns>
        /// <exception cref="FileNotFoundException">Khi file JSON không tồn tại.</exception>
        /// <exception cref="JsonException">Khi nội dung JSON không hợp lệ.</exception>
        public static T ReadJsonFile<T>(string filePath)
        {
            string fullPath = GetFullPath(filePath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Không tìm thấy file JSON: {fullPath}");
            }

            string jsonContent = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<T>(jsonContent)
                ?? throw new JsonException($"Không thể deserialize file JSON: {fullPath}");
        }

        /// <summary>
        /// Đọc dữ liệu user đầu tiên từ file users.json.
        /// Đây là phương thức tiện ích giúp lấy nhanh thông tin user test.
        /// </summary>
        /// <returns>JToken chứa thông tin user đầu tiên.</returns>
        public static JToken GetDefaultUser()
        {
            var data = ReadJsonFile("TestData/users.json");
            return data["users"]?[0]
                ?? throw new InvalidOperationException("Không tìm thấy user trong file users.json");
        }

        /// <summary>
        /// Lấy giá trị của một trường cụ thể từ user mặc định.
        /// Ví dụ: GetUserField("username") trả về "john".
        /// </summary>
        /// <param name="fieldName">Tên trường cần lấy (username, password, accountNumber...).</param>
        /// <returns>Giá trị của trường dưới dạng string.</returns>
        public static string GetUserField(string fieldName)
        {
            var user = GetDefaultUser();
            return user[fieldName]?.ToString()
                ?? throw new InvalidOperationException($"Không tìm thấy trường '{fieldName}' trong user data.");
        }

        /// <summary>
        /// Xây dựng đường dẫn đầy đủ từ đường dẫn tương đối.
        /// Nếu đã là đường dẫn tuyệt đối thì giữ nguyên.
        /// </summary>
        /// <param name="relativePath">Đường dẫn tương đối đến file.</param>
        /// <returns>Đường dẫn tuyệt đối.</returns>
        private static string GetFullPath(string relativePath)
        {
            // Nếu đã là đường dẫn tuyệt đối, trả về nguyên bản
            if (Path.IsPathRooted(relativePath))
            {
                return relativePath;
            }

            // Kết hợp với thư mục chứa assembly hiện tại (thư mục bin/Debug)
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, relativePath);
        }
    }
}
