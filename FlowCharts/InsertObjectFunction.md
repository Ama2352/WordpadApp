```mermaid
flowchart TD
    A[Start Insert Object Function] --> B[Open InsertObjectWindow]
    B --> C[Display List of Object Types]
    C --> D[User Selects an Object Type]
    D --> E[Show Description and Icon for Selected Object]
    E --> F{Is Create New Selected?}
    F -->|Yes| G[Insert Object as Icon for Selected Type]
    G --> H[Close InsertObjectWindow and Return to Main Window]
    F -->|No| I{Is Create From File Selected?}
    I -->|Yes| J[Browse for File]
    J --> K[Display File Path and File Type]
    K --> L[Insert Object as Icon from File]
    L --> H
    I -->|No| H
    D --> M[User Clicks Cancel Button]
    M --> H
    H --> N[End Insert Object Function]

    %% Event handling for user interactions
    E --> O[User Double Clicks on Inserted Icon]
    O --> P[Open Associated Application for Icon Type]
    P --> N
