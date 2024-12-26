```mermaid
flowchart TD
    A[Start DateAndTime Function] --> B[Open DateAndTimeWindow for User to Select DateTime Format]
    B --> C[Display Available DateTime Formats to User]
    C --> D[User Selects a DateTime Format]
    D --> E[User Clicks OK Button]
    E --> F[Validate User Selection]
    F --> G{Is a Format Selected?}
    G -->|No| H[Show Warning Message: Please select a format.]
    G -->|Yes| I[Store Selected DateTime Format]
    I --> J[Close DateAndTimeWindow]
    J --> K[Return Selected DateTime to Main Window]
    K --> L[Insert Selected DateTime into Text Field in Main Window]
    L --> M[Update Text in RichTextBox]
    M --> N[End DateAndTime Function]
    E --> O[User Clicks Cancel Button]
    O --> P[Close DateAndTimeWindow Without Saving Selection]
    P --> N
