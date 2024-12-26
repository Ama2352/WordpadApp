# Flowchart for Insert Image Function

```mermaid
flowchart TD
    A[Khởi đầu] --> B[Mở hộp thoại chọn tệp hình ảnh]
    B --> C{Có chọn tệp không?}
    C -->|Có| D[Tải hình ảnh từ đường dẫn tệp]
    C -->|Không| E[Thoát chức năng]
    D --> F[Tính toán tỷ lệ khung hình của hình ảnh]
    F --> G[Điều chỉnh kích thước hình ảnh theo tỷ lệ khung hình]
    G --> H[Tạo điều khiển hình ảnh với kích thước mới]
    H --> I[Tạo ID duy nhất cho hình ảnh]
    I --> J[Lưu hình ảnh và ID vào từ điển]

    J --> K[Tạo InlineUIContainer để chứa hình ảnh]
    K --> L[Kiểm tra vị trí hiện tại của con trỏ văn bản]

    L -->|Nếu có lựa chọn| M[Xóa hình ảnh hiện có tại vị trí con trỏ]
    L -->|Không có lựa chọn| N[Tạo một đoạn văn mới với hình ảnh]

    M --> N
    N --> O[Chèn đoạn văn mới vào RichTextBox tại vị trí con trỏ]
    O --> P[Kết thúc]

