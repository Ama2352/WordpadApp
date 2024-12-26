```mermaid
flowchart TD
    A[Start] --> B[User selects a function]

    subgraph Function Selection
        B -->|Change Font Family| D1[ChangeFontFamily]
        B -->|Change Font Size| D2[ChangeFontSize]
        B -->|Change Font Color| D3[ChangeFontColor]
        B -->|Change Highlight Color| D4[ChangeHighlightColor]
    end

    D1 --> C[Check if selection is empty]
    D2 --> C
    D3 --> C
    D4 --> C

    C -->|Empty| E[Set selection using SettingForEmptySelectionCase]
    C -->|Not empty| F[Apply the selected property]
    E --> F

    F --> G[Ensure RichTextBox has focus]
    G --> H[End]
