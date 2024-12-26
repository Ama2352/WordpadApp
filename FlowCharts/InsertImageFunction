# Flowchart for Insert Image Function

```mermaid
flowchart TD
    A[Start] --> B[Open file dialog to choose image]
    B --> C{Is a file selected?}
    C -->|Yes| D[Load image from file path]
    C -->|No| E[Exit function]
    D --> F[Calculate aspect ratio of the image]
    F --> G[Adjust image size based on aspect ratio]
    G --> H[Create image control with new size]
    H --> I[Generate unique ID for the image]
    I --> J[Save the image and its ID to dictionary]

    J --> K[Create InlineUIContainer to hold the image]
    K --> L[Check the current position of the text cursor]

    L -->|If selection exists| M[Delete existing image at the cursor position]
    L -->|No selection| N[Create a new paragraph with the image]

    M --> N
    N --> O[Insert the new paragraph into RichTextBox at cursor position]
    O --> P[End]
