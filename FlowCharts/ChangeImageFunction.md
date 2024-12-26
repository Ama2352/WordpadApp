flowchart TD
    A[Start] --> B[Check if text is selected in RichTextBox]
    B --> C{Is selection empty?}
    C -->|Yes| D[Show warning: 'Please select an image to change.']
    C -->|No| E[Call InsertImage method to select a new image]
    E --> F[End]
