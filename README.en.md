# UINavigator

UINavigator is a powerful and flexible UI navigation system for Unity, providing a structured way to manage UI screens, pages, and modals with smooth transitions and animations.

## Features

- **Multiple UI Container Types**
  - `SheetContainer`: Manages individual UI sheets with single-view display
  - `PageContainer`: Handles UI pages with navigation history support
  - `ModalContainer`: Controls modal dialogs with backdrop support

- **Animation Support**
  - Built-in show/hide animations
  - Custom animation configuration per container or view
  - Smooth transitions between UI states

- **(Optional) Dependency Injection Support**
  - VContainer integration

- **(Optional) Addressable Asset Support**
  - Load UI prefabs through Unity's Addressable Asset System
  - Dynamic UI loading and instantiation

- **Event System**
  - Rich lifecycle events (PreInitialize, PostInitialize, Appear, Appeared, etc.)
  - Observable pattern using R3

## Requirements

- Unity 2021.3 or higher
- Dependencies:
  - UniTask
  - DOTween
  - R3
  - VContainer (optional)
  - Addressables (optional)

## Installation

1. Ensure all required dependencies are installed in your project
2. Import the UINavigator package
3. Add the necessary define symbols to your project:
   - `UNITASK_SUPPORT`
   - `DOTWEEN_SUPPORT`
   - `UNITASK_DOTWEEN_SUPPORT`
   - `R3_SUPPORT`
   - `ADDRESSABLE_SUPPORT` (if using Addressables)
   - `VCONTAINER_SUPPORT` (if using VContainer)

### Via Package Manager

For Unity 2019.3.4f1 or higher, you can install the package directly through the Package Manager using a Git URL.

1. Open Package Manager (Window > Package Manager)
2. Click '+' button and select "Add package from git URL"
3. Enter the following URL:
```
https://github.com/jinhosung96/UINavigatorForUnity.git
```

Alternatively, you can add it directly to your `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.jhs-library.auto-path-generator": "https://github.com/jinhosung96/UINavigatorForUnity.git"
  }
}
```

To install a specific version, add the #{version} tag to the URL:
```
https://github.com/jinhosung96/UINavigatorForUnity.git#1.0.0
```

## Basic Usage

### Sheet Container

```csharp
// Get sheet container instance
var container = SheetContainer.Main;

// Navigate to a new sheet
await container.NextAsync<MainMenuSheet>();

// Navigate with initialization
await container.NextAsync<GameSheet>(sheet => {
    sheet.Initialize(gameData);
});
```

### Page Container

```csharp
// Navigate to a new page
await PageContainer.Main.NextAsync<HomePageView>();

// Go back to previous page
await PageContainer.Main.PrevAsync();

// Reset to initial page
await PageContainer.Main.ResetAsync();
```

### Modal Container

```csharp
// Show modal dialog
await ModalContainer.Main.NextAsync<ConfirmationDialog>();

// Close current modal
await ModalContainer.Main.PrevAsync();
```

## Key Concepts

### UI Containers

Containers manage different types of UI views:
- **Sheet**: Single-view displays where only one sheet is visible at a time
- **Page**: Stack-based navigation with history support
- **Modal**: Overlay dialogs with optional backdrop

### View Lifecycle

Each UI view goes through the following states:
1. PreInitialize
2. PostInitialize
3. Appearing
4. Appeared
5. Disappearing
6. Disappeared

### Animation Settings

Two types of animation settings:
- **Container**: Uses container-level animation settings
- **Custom**: Uses view-specific animation settings

## Advanced Features

### Container Naming

```csharp
// Find container by name
var namedContainer = PageContainer.Find("MainPageContainer");
```

### DontDestroyOnLoad Support

Containers can be configured to persist between scene loads:

```csharp
// Set in inspector or via code
container.isDontDestroyOnLoad = true;
```

### Back Navigation

```csharp
// Handle back navigation globally
if (await UIContainer.BackAsync())
{
    // Back navigation handled
}
```

## Best Practices

1. **Container Organization**
   - Use separate containers for different sections of your UI
   - Name containers appropriately for easy reference

2. **View Lifecycle**
   - Initialize view data in PreInitialize
   - Setup UI elements in PostInitialize
   - Cleanup resources in WhenPostDisappearAsync

3. **Animation**
   - Use container animations for consistent transitions
   - Implement custom animations only when necessary

4. **Memory Management**
   - Set appropriate recycle settings for frequently used views
   - Properly dispose of resources in cleanup methods

## License

This project is licensed under the MIT License - see the LICENSE file for details.