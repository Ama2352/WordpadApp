# PasteSpecial Algorithm Flowchart

```mermaid
graph TD
    A[Start] --> B{User selects a paste option}

    B -->|Rich Text RTF selected| C[Check if RTF data exists in Clipboard]
    C -->|Yes| D[Paste RTF content into RichTextBox]
    C -->|No| E[Show warning message]

    B -->|Unformatted Text selected| F[Check if plain text exists in Clipboard]
    F -->|Yes| G[Retrieve plain text from Clipboard]
    G --> H[Insert plain text into RichTextBox]
    H --> I[Reset text formatting to default]
    F -->|No| E

    B -->|Bitmap selected| J[Check if image data exists in Clipboard]
    J -->|Yes| K[Retrieve image from Clipboard]
    K --> L[Insert image into RichTextBox with proper formatting]
    J -->|No| E

    B -->|Invalid option| E

    E[Show warning message] --> Z[End]
    I --> Z
    D --> Z
    L --> Z
