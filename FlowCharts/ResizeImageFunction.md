# Flowchart Example

```mermaid
graph TD
    A[Khởi đầu phương thức ResizeImage] --> B{Hình ảnh có tồn tại không?}
    B -- Không --> C[Hiển thị cảnh báo: Không tìm thấy hình ảnh]
    B -- Có --> D[Truy xuất hình ảnh và tỷ lệ khung hình]
    D --> E[Mở hộp thoại ResizeWindow]
    E --> F{Người dùng có xác nhận thay đổi kích thước không?}
    F -- Không --> G[Đóng hộp thoại mà không thay đổi]
    F -- Có --> H[Cập nhật chiều rộng và chiều cao của hình ảnh]
    H --> I[Kết thúc phương thức ResizeImage]
    G --> I

    subgraph Logic ResizeWindow
        J[Khởi tạo ResizeWindow]
        J --> K[Hiển thị kích thước hiện tại]
        K --> L{Tỷ lệ khung hình có bị khóa không?}
        L -- Có --> M[Đồng bộ chiều rộng và chiều cao]
        L -- Không --> N[Cho phép thay đổi độc lập]
        N --> O[Người dùng xác nhận thay đổi kích thước]
        M --> O
        O --> P[Tính toán kích thước mới]
        P --> Q[Cập nhật từ điển tỷ lệ khung hình]
        Q --> R[Trở lại ResizeImage]
    end

