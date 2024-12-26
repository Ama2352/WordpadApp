```mermaid
flowchart TD
    A[Khởi đầu phương thức Insert Object] --> B[Mở cửa sổ InsertObjectWindow]
    B --> C[Hiển thị danh sách các loại đối tượng]
    C --> D[Người dùng chọn một loại đối tượng]
    D --> E[Hiển thị mô tả và biểu tượng cho đối tượng đã chọn]
    E --> F{Có chọn Tạo Mới?}
    F -->|Có| G[Chèn đối tượng dưới dạng biểu tượng cho loại đã chọn]
    G --> H[Đóng cửa sổ InsertObjectWindow và quay lại cửa sổ chính]
    F -->|Không| I{Có chọn Tạo từ Tệp?}
    I -->|Có| J[Chọn tệp]
    J --> K[Hiển thị đường dẫn và loại tệp]
    K --> L[Chèn đối tượng dưới dạng biểu tượng từ tệp]
    L --> H
    I -->|Không| H
    D --> M[Người dùng nhấn nút Hủy]
    M --> H
    H --> N[Kết thúc phương thức Insert Object]

    %% Xử lý sự kiện cho các tương tác của người dùng
    E --> O[Người dùng nhấp đúp vào biểu tượng đã chèn]
    O --> P[Mở ứng dụng liên kết với loại biểu tượng]
    P --> N
