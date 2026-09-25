
---

# Tài liệu Thiết kế Kiến trúc: Hệ thống Quản lý & Đặt bàn Nhà hàng (Reservation System)

Hệ thống được thiết kế dựa trên các nguyên lý **OOSE** hiện đại và tuân thủ chặt chẽ 5 nguyên lý **SOLID**. Điểm nhấn của kiến trúc là sự phân chia rõ ràng giữa các tầng (Layered Architecture), áp dụng **Strategy Pattern** để dễ mở rộng, xử lý triệt để bài toán Đa luồng (Concurrency) và tuân thủ tuyệt đối **DIP (Dependency Inversion Principle)** thông qua cấu trúc `Interface -> Implementation`.

## I. KIẾN TRÚC TỔNG THỂ (UML)

Hệ thống chia làm 3 nhóm chính: **Core Domain** (Logic nghiệp vụ lõi), **Application Services** (Luồng Use Case ứng dụng) và **Infrastructure / Notification** (Hạ tầng và Giao tiếp ngoài). Tất cả tương tác giữa các module đều thông qua **Interface (Bản hợp đồng)**.

```plantuml
@startuml
skinparam packageStyle rectangle
skinparam classAttributeIconSize 0

package "Core Domain Services (Dịch vụ lõi)" {
  interface TableAvailabilityService <<Interface>>
  class TableAvailabilityServiceImpl
  
  interface TableAllocationStrategy <<Strategy>>
  class SingleTableStrategy {Order 1}
  class OptimalCapacityMergeStrategy {Order 2}
}

package "Application Services (Dịch vụ ứng dụng)" {
  interface AuthService <<Interface>>
  class AuthServiceImpl
  
  interface StaffAuthService <<Interface>>
  class StaffAuthServiceImpl

  interface TableService <<Interface>>
  class TableServiceImpl

  interface AssignmentService <<Interface>>
  class AssignmentServiceImpl

  interface AvailabilityApiService <<Interface>>
  class AvailabilityApiServiceImpl
  
  interface OverrideService <<Interface>>
  class OverrideServiceImpl
  
  interface ReportService <<Interface>>
  class ReportServiceImpl
  
  interface SystemSchedulerService <<Interface>>
  class SystemSchedulerServiceImpl
}

package "Infrastructure & Notification (Hạ tầng)" {
  interface ReservationMappingService <<Interface>>
  class ReservationMappingServiceImpl
  
  interface EmailService <<Interface>>
  class EmailServiceImpl
}

' Thực thi (Realization)
TableAvailabilityService <|.. TableAvailabilityServiceImpl
TableAllocationStrategy <|.. SingleTableStrategy
TableAllocationStrategy <|.. OptimalCapacityMergeStrategy

AuthService <|.. AuthServiceImpl
StaffAuthService <|.. StaffAuthServiceImpl
TableService <|.. TableServiceImpl
AssignmentService <|.. AssignmentServiceImpl
AvailabilityApiService <|.. AvailabilityApiServiceImpl
OverrideService <|.. OverrideServiceImpl
ReportService <|.. ReportServiceImpl
SystemSchedulerService <|.. SystemSchedulerServiceImpl

ReservationMappingService <|.. ReservationMappingServiceImpl
EmailService <|.. EmailServiceImpl

' Phụ thuộc (Dependency)
AssignmentServiceImpl --> TableAvailabilityService
AssignmentServiceImpl --> TableAllocationStrategy
AssignmentServiceImpl --> ReservationMappingService

AvailabilityApiServiceImpl --> TableAvailabilityService
AvailabilityApiServiceImpl --> TableAllocationStrategy

AuthServiceImpl --> EmailService
AuthServiceImpl --> JwtUtil

SystemSchedulerServiceImpl --> TransactionTemplate
@enduml
```

---

## II. CHI TIẾT CÁC THÀNH PHẦN

### 1. Nhóm Core Domain (Nền tảng logic)

#### **`TableAvailabilityService` (Interface & Impl)**
* **Mục đích:** Là nguồn sự thật duy nhất (Single Source of Truth) về trạng thái trống/bận thực tế của bàn.
* **Đặc điểm SOLID:** Tuân thủ **SRP**. Ngăn chặn lỗi N+1 Query bằng cách cung cấp các API lấy dữ liệu thô tối ưu (`getCurrentlyAvailableTables`, `getNextBookingTime`), hỗ trợ xử lý cả `bufferMinutes` (thời gian dọn dẹp).

#### **`TableAllocationStrategy` (Interface & Impls)**
* **Mục đích:** Hệ thống thuật toán lõi giúp tìm bàn phù hợp cho khách.
* **Cài đặt:**
  * **`SingleTableStrategy` (`@Order(1)`):** Quét nhanh bàn đơn.
  * **`OptimalCapacityMergeStrategy` (`@Order(2)`):** Áp dụng **Quy hoạch động (Dynamic Programming)** để tìm tổ hợp ghép bàn ít dư thừa chỗ nhất. Tuân thủ **OCP**, dễ dàng thay đổi thuật toán.

---

### 2. Nhóm Application Services (Luồng nghiệp vụ)
*Toàn bộ tầng này giao tiếp với Controller thông qua Interface, giấu kín chi tiết cài đặt ở class Impl.*

#### **`AuthService` & `StaffAuthService` (Bảo mật & Phân quyền)**
* **Mục đích:** Quản lý luồng đăng nhập, đăng ký, xác thực JWT và quên mật khẩu cho Khách hàng và Nhân viên.
* **Điểm sáng Kiến trúc:** Áp dụng **Clean Code** và Functional Programming (`.map().orElseGet()`) để loại bỏ hoàn toàn cấu trúc `if-else` lồng nhau. Tách biệt logic kiểm tra trạng thái xác minh thành các hàm private (Tuân thủ **SRP**).

#### **`TableService` (Quản lý Bàn vật lý)**
* **Mục đích:** Cung cấp các API CRUD cho bàn (tạo, sửa, xóa, lấy sơ đồ Floor Map).
* **Điểm sáng Kiến trúc:**
  * **Thread-Safety (An toàn Đa luồng):** Đảm bảo an toàn khi nhiều nhân viên cùng truy cập Sơ đồ bàn cùng lúc bằng cách sử dụng Local Variable thay vì Class/Instance Variable.
  * **Optimistic Locking:** Sử dụng cơ chế kiểm tra `version` để chống ghi đè dữ liệu đồng thời.

#### **`AssignmentService` & `AvailabilityApiService` (Xếp bàn & Tra cứu)**
* **Mục đích:** Xử lý logic gán bàn thực tế và tra cứu sơ đồ bàn/gợi ý giờ.
* **Đặc điểm:** Áp dụng mẫu kiến trúc **CQRS** (Command Query Responsibility Segregation). Tách biệt rõ ràng luồng Ghi (`Assignment`) và luồng Đọc (`Availability`).

#### **`SystemSchedulerService` (Tiến trình chạy ngầm)**
* **Mục đích:** Tự động hủy đơn hết hạn, đánh dấu No-Show, cảnh báo Overstay và giải phóng bàn.
* **Điểm sáng Kiến trúc:** * Xử lý Transaction độc lập cho từng đơn hàng (bằng `TransactionTemplate`) giúp ngăn chặn lỗi "chết chùm" (Rollback toàn bộ).
  * Áp dụng **Higher-Order Function** (Hàm bậc cao) để đóng gói boilerplate code (Try-catch, Transaction mở/đóng, Re-fetch Entity), tuân thủ tuyệt đối nguyên lý **DRY (Don't Repeat Yourself)**.

#### **`OverrideService` & `ReportService` (Ngoại lệ & Thống kê)**
* **Mục đích:** Xử lý các tình huống Checkout khẩn cấp (Override) và xuất dữ liệu báo cáo (No-Show rate, Đơn theo ngày).
* **Đặc điểm:** Tách biệt rõ ràng các khâu (Validate, Giải phóng bàn, Ghi Log) thành các hàm nhỏ rành mạch. Tận dụng sức mạnh của Java Stream API để gộp nhóm dữ liệu (GroupingBy).

---

### 3. Nhóm Infrastructure & Notification (Hạ tầng)

#### **`EmailService` (Dịch vụ Thông báo)**
* **Mục đích:** Quản lý toàn bộ việc định dạng template và gửi Email qua giao thức SMTP.
* **Điểm sáng Kiến trúc:**
  * **SRP Tuyệt đối:** Chuyển toàn bộ trách nhiệm "nặn" nội dung email (lời chào, format ngày giờ) từ Event Listener về cho `EmailServiceImpl`. Listener chỉ đóng vai trò "Điều hướng" (Router).

#### **`ReservationMappingService` (Lưu trữ quan hệ DB)**
* **Mục đích:** Quản lý việc ghi xuống Database các mối quan hệ N-N. Sử dụng `@Transactional(propagation = Propagation.MANDATORY)` để đảm bảo tính toàn vẹn (Atomic).

---

## III. TỔNG KẾT TƯ DUY KIẾN TRÚC (DESIGN MINDSET)

Kiến trúc hiện tại đã giải quyết được các bài toán khó nhất của một hệ thống Enterprise:
1.  **Dependency Inversion (DIP):** Không có Service nào giao tiếp trực tiếp với Concrete Class của Service khác. Mọi thứ đều là Hợp đồng (Interface), giúp cho việc Unit Test và Mock dữ liệu trở nên dễ dàng.
2.  **Đa luồng & Concurrency:** Từ việc xử lý biến cục bộ trong `TableService`, áp dụng Optimistic Locking (`version`), cho đến xử lý Transaction cô lập trong Scheduler đều được thiết kế chặt chẽ.
3.  **Clean Architecture & Clean Code:** Loại bỏ hoàn toàn sự lặp lại code (DRY), loại bỏ các khối if-else lồng nhau (Arrow Anti-Pattern), và tách bạch rõ ràng logic kết nối Database, thuật toán toán học, và logic tạo Template UI.

---
*Tài liệu được cập nhật dựa trên quy trình Tái cấu trúc (Refactoring) theo tiêu chuẩn OOSE & SOLID mới nhất.*

---