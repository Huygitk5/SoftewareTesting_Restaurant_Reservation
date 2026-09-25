# HỆ THỐNG QUẢN LÝ ĐẶT BÀN NHÀ HÀNG
## Spring Boot + Web Customer + WinForms + MySQL

---

## 1. Giới thiệu đề tài

Đây là đồ án cuối kỳ môn Công nghệ Phần mềm Hướng đối tượng, xây dựng hệ thống quản lý đặt bàn nhà hàng theo mô hình client-server, hỗ trợ đồng thời hai luồng nghiệp vụ:

- đặt bàn online cho khách hàng qua website,
- tiếp nhận và vận hành khách walk-in tại quầy qua ứng dụng desktop.

Hệ thống được xây dựng nhằm giải quyết các bài toán thực tế trong vận hành nhà hàng như:

- quản lý trạng thái bàn tập trung,
- tránh trùng lịch đặt bàn,
- hỗ trợ ghép bàn,
- giữ bàn tạm thời trong lúc thanh toán,
- quản lý waitlist,
- check-in, check-out, đổi bàn,
- xử lý no-show, overstay, override,
- báo cáo thống kê và cấu hình vận hành.

Project hiện gồm 3 thành phần chính:

- `backend-api/reservation-api`: backend nghiệp vụ trung tâm,
- `web-customer`: giao diện web dành cho khách hàng,
- `desktop-staff/reservation-winforms`: ứng dụng desktop dành cho lễ tân và quản lý.

---

## 2. Công nghệ sử dụng

- Java 17
- Spring Boot 4.0.4
- Spring Security
- Spring Data JPA / Hibernate
- MySQL
- WebSocket / STOMP
- Spring Mail / Jakarta Mail
- Caffeine Cache
- HTML / CSS / JavaScript
- C# WinForms
- .NET Framework 4.8
- PlantUML
- IntelliJ IDEA
- Visual Studio

---

## 3. Kiến trúc hệ thống

Dự án được tổ chức theo kiến trúc nhiều tầng:

- **Web Customer / Desktop POS**: giao diện người dùng
- **Controller**: tiếp nhận request và điều phối luồng xử lý
- **Service**: xử lý nghiệp vụ chính
- **Repository**: truy cập dữ liệu qua JPA
- **Entity / DTO / Strategy / Event**: mô hình dữ liệu và các thành phần mở rộng nghiệp vụ
- **Database**: lưu trữ dữ liệu hệ thống

Luồng chính:

`Web Customer / Desktop POS -> Controller -> Service -> Repository -> Database`

Riêng ứng dụng desktop còn nhận cập nhật trạng thái bàn theo thời gian thực qua:

`Backend -> WebSocket/STOMP -> Desktop POS`

---

## 4. Chức năng chính

### 4.1. Xác thực và tài khoản khách hàng

- Đăng ký tài khoản khách hàng
- Đăng nhập
- Xác minh email
- Quên mật khẩu / đặt lại mật khẩu
- Cập nhật hồ sơ cá nhân
- Đổi mật khẩu

### 4.2. Đặt bàn online

- Tra cứu ngày và khung giờ còn trống
- Tạo đơn đặt bàn online
- Giữ bàn tạm bằng soft lock trong lúc thanh toán
- Xác nhận thanh toán phí giữ bàn
- Hủy thanh toán
- Xem danh sách đặt bàn của bản thân
- Hủy đặt bàn trong trường hợp hợp lệ

### 4.3. Xử lý khách walk-in và waitlist

- Nhận thông tin khách walk-in
- Gợi ý phương án xếp bàn phù hợp
- Xác nhận phục vụ walk-in
- Hủy gợi ý walk-in
- Thêm khách vào waitlist khi hết bàn
- Hỗ trợ short seating nếu còn khoảng trống ngắn

### 4.4. Vận hành tại chỗ

- Xem sơ đồ bàn theo thời gian thực
- Check-in khách
- Check-out khách
- Đổi bàn
- Override trong tình huống đặc biệt
- Theo dõi cảnh báo bàn qua WebSocket

### 4.5. Quản lý và điều hành hệ thống

- Thêm, sửa, xóa bàn
- Xem floor map
- Xem báo cáo số lượng reservation
- Xem tỉ lệ no-show
- Theo dõi lịch sử override
- Cập nhật cấu hình hệ thống
- Tạo tài khoản nhân viên
- Chạy các tác vụ scheduler quản trị khi cần

---

## 5. Cấu trúc thư mục dự án

- `backend-api/reservation-api`: mã nguồn backend Spring Boot
- `web-customer`: giao diện web khách hàng
- `desktop-staff/reservation-winforms`: ứng dụng WinForms cho nhân viên/quản lý
- `docs/uml`: các sơ đồ UML PlantUML
- `docs/SOLID-REFACTORING-REQUIREMENTS.md`: tài liệu yêu cầu/refactor
- `README.md`: tài liệu hướng dẫn chạy project

---

## 6. Hướng dẫn tạo cơ sở dữ liệu

### Bước 1: Tạo database MySQL

Chạy lệnh sau:

```sql
CREATE DATABASE restaurant_db;
```

### Bước 2: Cấu hình backend

Mở file:

`backend-api/reservation-api/src/main/resources/application.properties`

Kiểm tra hoặc chỉnh lại các thông số:

- `MYSQL_URL`
- `MYSQL_USER`
- `MYSQL_PASSWORD`
- `SERVER_PORT`
- `APP_BASE_URL`
- `MAIL_HOST`
- `MAIL_PORT`
- `MAIL_USERNAME`
- `MAIL_PASSWORD`

Lưu ý:

- Backend mặc định chạy ở cổng `8081`
- Project hiện dùng `spring.jpa.hibernate.ddl-auto=update`
- Repo hiện không kèm script SQL seed dữ liệu, nên schema sẽ được backend tự tạo/cập nhật khi khởi động
- Các chức năng đăng ký, xác minh email và quên mật khẩu phụ thuộc vào cấu hình SMTP mail
- `APP_BASE_URL` nên trỏ đúng về frontend, mặc định là `http://localhost:8081/web-customer`

---

## 7. Hướng dẫn chạy project

### Cách 1: Chạy backend API bằng Maven Wrapper

Mở terminal tại thư mục:

`backend-api/reservation-api`

và chạy:

```powershell
.\mvnw.cmd spring-boot:run
```

Sau khi chạy thành công:

- API: `http://localhost:8081/api`
- WebSocket native: `ws://localhost:8081/ws-reservation-native`
- Web customer: `http://localhost:8081/web-customer/index.html`

Nếu `mvnw.cmd` không chạy ổn trong PowerShell hiện tại, có thể mở project backend bằng IntelliJ IDEA và chạy class:

`com.hcmute.reservation.ReservationApiApplication`

### Cách 2: Chạy web khách hàng

Web khách hàng được backend ánh xạ trực tiếp như static resource, nên chỉ cần mở:

`http://localhost:8081/web-customer/index.html`

Nếu đổi cổng backend, cần sửa lại:

- `web-customer/js/common/api.js`

### Cách 3: Chạy ứng dụng desktop staff/POS

Mở file:

`desktop-staff/reservation-winforms/reservation-winforms.sln`

bằng Visual Studio, sau đó build và chạy chương trình.

Nếu đổi cổng backend, cần cập nhật:

- `desktop-staff/reservation-winforms/Services/ApiClient.cs`
- `desktop-staff/reservation-winforms/Services/WebSocketService.cs`

---

## 8. Tài khoản sử dụng

### Khách hàng online

- Tài khoản khách hàng được tạo trực tiếp trên giao diện web qua chức năng đăng ký
- Sau khi đăng ký cần xác minh email trước khi sử dụng đầy đủ chức năng

### Nhân viên / quản lý

- Đăng nhập trên ứng dụng desktop bằng tài khoản staff
- Repo hiện không kèm sẵn dữ liệu mẫu tài khoản staff/manager
- Khi demo, cần chuẩn bị sẵn ít nhất một tài khoản `MANAGER` trong database trước lần đăng nhập đầu tiên
- Sau khi có `MANAGER`, có thể dùng chức năng đăng ký staff trong ứng dụng desktop để tạo thêm tài khoản `RECEPTIONIST`

---

## 9. Phân quyền

### CUSTOMER

- Đăng ký, đăng nhập, xác minh email
- Quản lý hồ sơ cá nhân
- Đặt bàn online
- Xem và hủy reservation của bản thân

### RECEPTIONIST

- Xem floor map
- Nhận khách walk-in
- Quản lý waitlist
- Check-in / check-out
- Đổi bàn
- Override

### MANAGER

- Có toàn bộ quyền của `RECEPTIONIST`
- Quản lý bàn
- Xem báo cáo
- Xem override logs
- Quản lý cấu hình hệ thống
- Tạo tài khoản nhân viên
- Truy cập các chức năng quản trị hệ thống

---

## 10. UML

Các sơ đồ UML được lưu trong thư mục `docs/uml`, gồm:

- `activity-booking.puml`
- `use-case-diagram.puml`
- `class-diagram.puml`
- `sequence-diagram.puml`
- `sequence-booking-online.puml`
- `sequence-walk-in.puml`
- `sequence-check-in.puml`
- `sequence-check-out.puml`
- `activity-booking-online.puml`
- `activity-walk-in.puml`
- `activity-check-in.puml`
- `activity-check-out.puml`
- `activity-login.puml`
- `activity-register.puml`
- `activity-override.puml`
- `object-booking-online.puml`
- `object-walk-in.puml`
- `object-check-in.puml`
- `object-check-out.puml`

---

## 11. Thành viên nhóm

| MSSV | Họ và tên | Mức độ đóng góp |
| --- | --- | --- |
| 23110124 | Đoàn Ngọc Mạnh | 100% |
| 23110103 | Đoàn Quốc Huy | 100% |
| 23110125 | Nguyễn Trường Minh | 100% |
| 23110121 | Lương Quang Lâm | 100% |

Giảng viên hướng dẫn: **ThS. Nguyễn Minh Đạo**

---

## 12. Các điểm nổi bật của đồ án

- Áp dụng kiến trúc nhiều tầng, tách biệt rõ UI, service, repository và domain
- Sử dụng Strategy Pattern cho bài toán auto assignment và merge tables
- Có cơ chế soft lock để tránh double booking trong lúc chờ thanh toán/xác nhận
- Có waitlist và short seating để tối ưu sử dụng bàn
- Có optimistic locking và kiểm tra overlap để giảm xung đột đồng thời
- Có scheduler tự động xử lý `EXPIRED`, `NO_SHOW`, `OVERSTAY`, giải phóng bàn
- Có WebSocket/STOMP để cập nhật trạng thái bàn theo thời gian thực
- Áp dụng nguyên lý SOLID trong tổ chức service, interface và dependency

---

## 13. Hướng phát triển thêm

- Tích hợp cổng thanh toán thực tế
- Bổ sung seed data và script database mẫu cho demo nhanh
- Tăng cường kiểm thử tải và kiểm thử đồng thời
- Bổ sung logging, monitoring và audit log
- Mở rộng sang quản lý món ăn, hóa đơn hoặc đa chi nhánh
- Hoàn thiện UX/UI cho cả web và desktop

---

## 14. Kết luận

Dự án minh họa cách xây dựng một hệ thống quản lý đặt bàn nhà hàng tương đối hoàn chỉnh với hai luồng nghiệp vụ online và walk-in, sử dụng Spring Boot, MySQL, WebSocket, web frontend và WinForms desktop. Hệ thống có kiến trúc rõ ràng, bám sát bài toán thực tế của nhà hàng, đồng thời thể hiện được các nội dung trọng tâm của môn OOSE như phân tích yêu cầu, thiết kế hướng đối tượng, UML, SOLID và tổ chức mã nguồn theo hướng dễ mở rộng.
