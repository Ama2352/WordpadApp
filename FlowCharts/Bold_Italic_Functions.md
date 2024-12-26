```mermaid
flowchart TD
    A[Start] --> B[User selects ToggleBold or ToggleItalic]

    subgraph Text Style Toggle
        B -->|Toggle Bold| D1[ToggleBold]
        B -->|Toggle Italic| D2[ToggleItalic]
    end

    D1 --> C[Check if selection is empty]
    D2 --> C

    C -->|Empty| E[Set selection using SettingForEmptySelectionCase]
    C -->|Not empty| F[Retrieve current font style]
    E --> F

    F --> G[Check if font style is unset]
    G -->|Unset| H[Apply selected style]
    G -->|Set| I[Toggle between Normal and selected style]
    H --> J[Ensure RichTextBox has focus]
    I --> J

    J --> K[End]
