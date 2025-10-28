# Copilot Instructions for unity-360

## Project Overview
This is a Unity-based project focused on interactive 360° environments, with quiz/questionnaire functionality and UI panels. The main code is in `Assets/Scripts`, organized by feature (e.g., `Questionnaries`, `Minimap`, `Panels`).

## Architecture & Key Components
- **Questionnaire System**: Core logic in `Assets/Scripts/Questionnaries/`:
  - `QuestionManagerJSON.cs`: Handles answer validation and question navigation.
  - `QuestionViewerJSON.cs`: Loads and displays questions, options, and correct answers. Integrates with Excel via NPOI for data import.
  - `UiControllerJSON.cs`: Manages UI buttons and connects user actions to the questionnaire logic.
- **UI Panels**: Controllers for different UI panels (e.g., `PanelManager.cs`, `NormalPanelController.cs`) manage visibility and state.
- **Minimap & Navigation**: Scripts like `MinimapController.cs` and `TeleportDestination.cs` support spatial navigation.

## Developer Workflows
- **Build**: Use Unity Editor (no custom build scripts detected). Open the project in Unity and build via the Editor menu.
- **Test/Debug**: Play mode in Unity Editor is the main workflow. No automated test scripts found.
- **External Data**: Question data can be loaded from Excel files using NPOI (see `QuestionViewerJSON.cs`). Excel path is set via `PlayerPrefs`.

## Conventions & Patterns
- **Namespace**: All questionnaire scripts use `HMStudio.EasyQuiz`.
- **UI Binding**: UI elements are bound via `[SerializeField]` and connected in the Unity Inspector.
- **Button Actions**: Button listeners are set in `Awake()` methods, with logic separated into handler functions.
- **Data Flow**: `UiController` triggers actions in `QuestionManager`, which updates `QuestionViewer`.
- **Meta Files**: Unity `.meta` files are present; do not edit manually.

## Integration Points
- **TextMeshPro**: Used for all text UI elements.
- **NPOI**: Used for Excel file reading (see `using NPOI.SS.UserModel` in `QuestionViewerJSON.cs`).
- **Unity UI**: Standard Unity UI components (Button, TextMeshProUGUI).

## Examples
- To add a new question type, extend `QuestionViewer` and update `QuestionManager` logic.
- To add a new UI panel, create a new controller script in `Assets/Scripts` and bind it in the Unity Editor.

## Key Files & Directories
- `Assets/Scripts/Questionnaries/`: Questionnaire logic
- `Assets/Scripts/PanelManager.cs`: UI panel management
- `Assets/Scripts/MinimapController.cs`: Minimap logic
- `Packages/manifest.json`: Unity package dependencies

---
For more details, inspect the scripts in `Assets/Scripts/` and use the Unity Editor for scene/component setup.
