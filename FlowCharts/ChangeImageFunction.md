# Flowchart for Change Image Function

```mermaid
flowchart TD
    A[Khởi đầu] --> B[Kiểm tra xem có văn bản nào được chọn trong RichTextBox không]
    B --> C{Lựa chọn có trống không?}
    C -->|Có| D[Hiển thị cảnh báo: 'Vui lòng chọn một hình ảnh để thay đổi.']
    C -->|Không| E[Gọi phương thức InsertImage để chọn hình ảnh mới]
    E --> F[Kết thúc]
