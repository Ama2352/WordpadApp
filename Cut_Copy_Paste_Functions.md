# Clipboard Operations Flow

```mermaid
graph TD
    subgraph Paste [Paste Operation]
        A[Start] --> B[Check Clipboard for Image]
        B -->|Yes| C[Create BitmapImage from Clipboard]
        B -->|No| D[Execute ApplicationCommands.Paste]
        C --> E[Create Image Container with Aspect Ratio]
        E --> F[Generate Unique Image ID for Image]
        F --> G[Store Image in Dictionary with ID]
        G --> H[Create InlineUIContainer for Image]
        H --> I[Get Text Selection from RichTextBox]
        I --> J[Check if Selection is Empty]
        J -->|No| K[Delete Existing Image at Position]
        J -->|Yes| L[Create Paragraph for Image]
        K --> M[Insert Image in Paragraph]
        L --> M[Insert Image in Paragraph]
        M --> N[Insert Paragraph at Selection Position]
        N --> O[End]
    end

    subgraph Cut [Cut Operation]
        A1[Cut] --> A2[Execute ApplicationCommands.Cut]
    end

    subgraph Copy [Copy Operation]
        A3[Copy] --> A4[Execute ApplicationCommands.Copy]
    end
