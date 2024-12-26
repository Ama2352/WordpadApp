```mermaid
flowchart TD
    A[Khởi đầu phương thức DateAndTime] --> B[Mở cửa sổ DateAndTimeWindow cho người dùng chọn định dạng ngày giờ]
    B --> C[Hiển thị các định dạng ngày giờ có sẵn cho người dùng]
    C --> D[Người dùng chọn một định dạng ngày giờ]
    D --> E[Người dùng nhấn nút OK]
    E --> F[Xác nhận lựa chọn của người dùng]
    F --> G{Đã chọn định dạng chưa?}
    G -->|Không| H[Hiển thị thông báo cảnh báo: Vui lòng chọn một định dạng.]
    G -->|Có| I[Lưu định dạng ngày giờ đã chọn]
    I --> J[Đóng cửa sổ DateAndTimeWindow]
    J --> K[Trả lại định dạng ngày giờ đã chọn về cửa sổ chính]
    K --> L[Chèn định dạng ngày giờ đã chọn vào trường văn bản trong cửa sổ chính]
    L --> M[Cập nhật văn bản trong RichTextBox]
    M --> N[Kết thúc phương thức DateAndTime]
    E --> O[Người dùng nhấn nút Hủy]
    O --> P[Đóng cửa sổ DateAndTimeWindow mà không lưu lựa chọn]
    P --> N
