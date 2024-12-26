# Flowchart Example

```mermaid
graph TD
    A[Start ResizeImage Method] --> B{Image Exists?}
    B -- No --> C[Show Warning: No image found]
    B -- Yes --> D[Retrieve Image and Aspect Ratio]
    D --> E[Open ResizeWindow Dialog]
    E --> F{User Confirms Resize?}
    F -- No --> G[Close Dialog Without Changes]
    F -- Yes --> H[Update Image Width & Height]
    H --> I[End ResizeImage Method]
    G --> I

    subgraph ResizeWindow Logic
        J[Initialize ResizeWindow]
        J --> K[Display Current Size]
        K --> L{Aspect Ratio Locked?}
        L -- Yes --> M[Synchronize Width & Height Inputs]
        L -- No --> N[Allow Independent Input Changes]
        N --> O[User Confirms Resize]
        M --> O
        O --> P[Calculate New Dimensions]
        P --> Q[Update Aspect Ratios Dictionary]
        Q --> R[Return to ResizeImage]
    end
