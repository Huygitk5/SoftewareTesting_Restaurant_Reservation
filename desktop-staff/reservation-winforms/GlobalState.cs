namespace reservation_winforms
{
    // Chữ 'static' giúp class này tồn tại duy nhất và gọi được từ mọi nơi 
    // mà không cần phải dùng từ khóa 'new'
    public static class GlobalState
    {
        // Biến lưu trữ Token để gọi API
        public static string StaffToken { get; set; } = string.Empty;

        // Biến lưu tên tài khoản đang đăng nhập để hiển thị lên góc màn hình
        public static string CurrentUsername { get; set; } = string.Empty;

        // Biến lưu chức vụ để phân quyền (Lễ tân thì ẩn bớt nút của Quản lý)
        public static string Role { get; set; } = string.Empty;
    }
}
