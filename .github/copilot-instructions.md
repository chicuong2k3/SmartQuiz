# SmartQuiz Development Guidelines

## Technology Stack
- **Framework**: Blazor WebAssembly + ASP.NET Core
- **State Management**: ActualLab.Fusion (reactive state with automatic invalidation)
- **UI Library**: MudBlazor
- **Database**: PostgreSQL with Entity Framework Core
- **Serialization**: MemoryPack
- **Object Mapping**: Mapster
- **Commands**: ActualLab.CommandR

## Color Scheme (Material Design)

### Primary Color - Vibrant Blue
- **Primary 500 (Main)**: `#2196F3` - Main interactive elements, app bar
- **Primary 700 (Dark)**: `#1976D2` - Status bars, darker elements, text on dark backgrounds
- **Primary 100 (Light)**: `#BBDEFB` - Subtle backgrounds, highlights

### Secondary/Accent Color - Orange
- **Accent 500 (Main)**: `#FF9800` - Important calls to action, floating action buttons, highlights
- **Accent 700 (Dark)**: `#F57C00` - Pressed states, darker accents
- **Accent 100 (Light)**: `#FFE0B2` - Subtle accents, inactive states

### Neutral Colors (Grays & White)
- **Background**: `#F8F9FA` - Main screen backgrounds (use `bg-app-background` class)
- **Surface**: `#FFFFFF` - Card backgrounds, content containers (use `bg-app-surface` class)
- **Text Primary**: `#3C4043` - Primary text, headings (use `text-primary-color` class)
- **Text Secondary**: `#757575` - Secondary text, hints (use `text-secondary-color` class)
- **Text Disabled**: `#BDBDBD` - Disabled text, subtle elements (use `text-disabled-color` class)

### Feedback/Utility Colors
- **Success**: `#4CAF50` - Use `Color.Success` in MudBlazor
- **Warning**: `#FFEB3B` - Use `Color.Warning` in MudBlazor
- **Error**: `#F44336` - Use `Color.Error` in MudBlazor
- **Info**: `#2196F3` - Use `Color.Info` in MudBlazor

### Usage in Components
```razor
<!-- Primary colors -->
<MudButton Color="Color.Primary">Primary Action</MudButton>
<MudButton Color="Color.Secondary">Secondary Action</MudButton>

<!-- Custom background colors -->
<MudPaper Class="bg-app-background pa-4">
    <MudText Class="text-primary-color">Primary text</MudText>
    <MudText Class="text-secondary-color">Secondary text</MudText>
</MudPaper>

<!-- Feedback colors -->
<MudAlert Severity="Severity.Success">Success message</MudAlert>
<MudAlert Severity="Severity.Error">Error message</MudAlert>
```

## Styling Priority Rules

When styling components, follow this priority order:

1. **MudBlazor Built-in Classes (Highest Priority)**
   - Use MudBlazor's built-in utilities first: `Class="pa-4 mb-4 d-flex align-center"`
   - MudBlazor spacing: `pa-{0-16}`, `ma-{0-16}`, `px-{0-16}`, etc.
   - MudBlazor flexbox: `d-flex`, `align-center`, `justify-space-between`, etc.
   - MudBlazor typography: `fw-bold`, `fw-normal`
   - Component props: `Elevation`, `Color`, `Variant`, `Typo`

2. **TailwindCSS Utilities (Second Priority)**
   - Use when MudBlazor doesn't provide the utility
   - Grid layouts: `grid grid-cols-12`, `col-span-6`
   - Advanced responsive: `hidden md:block`, `text-xl md:text-2xl`
   - Transforms: `scale-105`, `rotate-45`, `translate-x-4`
   - Gradients: `bg-gradient-to-r from-blue-500 to-purple-600`
   - Complex effects: `backdrop-blur-xl`, `shadow-2xl`

3. **Blazor Scoped CSS (Third Priority)**
   - Use `.razor.css` files for component-specific styles
   - Keeps styles scoped to a single component
   - Better maintainability and prevents style conflicts
   - Use `::deep` for styling child MudBlazor components

4. **Inline Styles (Last Resort)**
   - Only use `Style` attribute when absolutely necessary
   - Valid use cases: dynamic values, custom colors from data, unique one-off styles
   - Avoid for anything that can be achieved with classes

**Examples:**

```razor
<!-- ✅ GOOD: Use MudBlazor classes -->
<MudPaper Class="pa-6 mb-4">
    <div class="d-flex align-center gap-3">
        <MudText Typo="Typo.h6" Class="fw-bold">Title</MudText>
    </div>
</MudPaper>

<!-- ✅ GOOD: Use TailwindCSS when MudBlazor doesn't have it -->
<div class="grid grid-cols-12 gap-4">
    <div class="col-span-6 md:col-span-4 transform hover:scale-105">
        <MudCard>Content</MudCard>
    </div>
</div>

<!-- ✅ GOOD: Use scoped CSS for component-specific styles -->
<!-- MyComponent.razor -->
<div class="custom-card">
    <MudCard>Content</MudCard>
</div>

<!-- MyComponent.razor.css -->
.custom-card {
    position: relative;
}

.custom-card:hover {
    transform: translateY(-4px);
}

<!-- ✅ ACCEPTABLE: Inline style for dynamic values -->
<MudPaper Style="@($"background-color: {dynamicColor};")">
    Dynamic content
</MudPaper>

<!-- ❌ BAD: Inline styles for static values -->
<MudPaper Style="padding: 24px; margin-bottom: 16px;">
    Content
</MudPaper>

<!-- ❌ BAD: Inline styles for standard utilities -->
<div style="display: flex; align-items: center;">
    Should use: class="d-flex align-center"
</div>
```

## Core ActualLab.Fusion Principles
- Fusion is a reactive state management framework for .NET
- All compute methods are automatically cached and invalidated
- State is always eventually consistent, never stale
- Prefer composition over inheritance when designing services
- All compute methods MUST be virtual and return Task<T>
- Invalidation propagates automatically through dependency graph
- Use `[ComputeMethod]` for read operations, `[CommandHandler]` for write operations
 
## Service Implementation Patterns

### Service Interface (Client-side)
```csharp
using SmartQuiz.Client.Data.Commands;

namespace SmartQuiz.Client.Data.Services;

public interface IFlashcardService : IComputeService 
{
    // Command handlers - write operations
    [CommandHandler]
    Task<FlashcardDto?> CreateFlashcardAsync(CreateFlashcardCommand command, CancellationToken cancellationToken = default);
    
    [CommandHandler]
    Task<FlashcardDto?> UpdateFlashcardAsync(UpdateFlashcardCommand command, CancellationToken cancellationToken = default);
    
    [CommandHandler]
    Task DeleteFlashcardAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Compute methods - read operations (cached)
    [ComputeMethod]
    Task<IEnumerable<FlashcardDto>> GetFlashcardsAsync(CancellationToken cancellationToken = default);
    
    [ComputeMethod]
    Task<FlashcardDto?> GetFlashcardByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
```

### Service Implementation (Server-side)
```csharp
using ActualLab.Collections;
using SmartQuiz.Client.Data.Commands;
using SmartQuiz.Client.Data.Services;
 
namespace SmartQuiz.Application;
 
public class FlashcardService(IServiceProvider services)
    : DbServiceBase<ApplicationDbContext>(services), IFlashcardService
{
    // ============ COMPUTE METHODS - Cached, read-only ============
    
    public virtual async Task<IEnumerable<FlashcardDto>> GetFlashcardsAsync(
        CancellationToken cancellationToken = default)
    {
        // Use CreateDbContext for read operations
        await using var dbContext = await DbHub.CreateDbContext(cancellationToken);
        var items = await dbContext.Flashcards
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        
        // Use Mapster to map entities to DTOs
        return items.Adapt<IEnumerable<FlashcardDto>>();
    }
    
    public virtual async Task<FlashcardDto?> GetFlashcardByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await DbHub.CreateDbContext(cancellationToken);
        var item = await dbContext.Flashcards
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return item?.Adapt<FlashcardDto>();
    }
    
    // ============ COMMAND METHODS - Write operations ============
    
    public async Task<FlashcardDto?> CreateFlashcardAsync(
        CreateFlashcardCommand command,
        CancellationToken cancellationToken = default)
    {
        // Invalidation phase - invalidate related queries
        if (Invalidation.IsActive)
        {
            // Call compute methods with default cancellation token to invalidate their cache
            _ = GetFlashcardsAsync(default);
            return null;
        }
        
        // Use CreateOperationDbContext for write operations
        await using var dbContext = await DbHub.CreateOperationDbContext(cancellationToken);
        
        var flashcard = new Flashcard(
            command.StudySetId,
            command.FrontText,
            command.BackText,
            command.ImageUrl,
            command.AudioUrl
        );
        
        dbContext.Flashcards.Add(flashcard);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return flashcard.Adapt<FlashcardDto>();
    }
    
    public async Task<FlashcardDto?> UpdateFlashcardAsync(
        UpdateFlashcardCommand command, 
        CancellationToken cancellationToken = default)
    {
        var context = CommandContext.GetCurrent();
        
        // Invalidation phase
        if (Invalidation.IsActive)
        {
            // Retrieve DTO from operation items for targeted invalidation
            var dto = context.Operation.Items.Get<FlashcardDto>();
            if (dto != null)
            {
                _ = GetFlashcardByIdAsync(dto.Id, default);
                _ = GetFlashcardsAsync(default);
            }
            return null;
        }
        
        // Execution phase
        await using var dbContext = await DbHub.CreateOperationDbContext(cancellationToken);
        
        var flashcard = await dbContext.Flashcards
            .FindAsync(new object[] { command.Id }, cancellationToken);
        
        if (flashcard == null)
            throw new InvalidOperationException($"Flashcard with ID {command.Id} not found");
        
        // Update entity properties
        flashcard.FrontText = command.FrontText;
        flashcard.BackText = command.BackText;
        flashcard.ImageUrl = command.ImageUrl;
        flashcard.AudioUrl = command.AudioUrl;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        var result = flashcard.Adapt<FlashcardDto>();
        
        // Store in operation items for invalidation phase
        context.Operation.Items.Set(result);
        
        return result;
    }
    
    public async Task DeleteFlashcardAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var context = CommandContext.GetCurrent();
        
        if (Invalidation.IsActive)
        {
            var dto = context.Operation.Items.Get<FlashcardDto>();
            if (dto != null)
            {
                _ = GetFlashcardByIdAsync(dto.Id, default);
                _ = GetFlashcardsAsync(default);
            }
            return;
        }
        
        await using var dbContext = await DbHub.CreateOperationDbContext(cancellationToken);
        
        var flashcard = await dbContext.Flashcards
            .FindAsync(new object[] { id }, cancellationToken);
        
        if (flashcard == null)
            throw new InvalidOperationException($"Flashcard with ID {id} not found");
        
        // Store DTO before deletion for invalidation phase
        var dto = flashcard.Adapt<FlashcardDto>();
        context.Operation.Items.Set(dto);
        
        dbContext.Flashcards.Remove(flashcard);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
```

### Key Service Implementation Rules
1. **DbContext Usage**:
   - Use `DbHub.CreateDbContext()` for read operations
   - Use `DbHub.CreateOperationDbContext()` for write operations
   
2. **Compute Methods**:
   - Must be `virtual` and decorated with `[ComputeMethod]`
   - Return `Task<T>` or `Task<IEnumerable<T>>`
   - Never modify data - read-only operations
   
3. **Command Handlers**:
   - Must check `Invalidation.IsActive` at the start
   - During invalidation phase, call related compute methods to invalidate cache
   - Use `CommandContext.GetCurrent()` for complex invalidation scenarios
   - Store results in `context.Operation.Items` for invalidation phase access
   
4. **Entity Mapping**:
   - Use Mapster's `.Adapt<>()` to map entities to DTOs
   - DTOs should be immutable with init-only properties

## Data Transfer Objects (DTOs)

DTOs are used for communication between client and server. They must be serializable with MemoryPack.

```csharp
using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Dtos;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial class FlashcardDto
{
    [DataMember, MemoryPackOrder(0)] public Guid Id { get; init; }
    [DataMember, MemoryPackOrder(1)] public Guid StudySetId { get; init; }
    [DataMember, MemoryPackOrder(2)] public string FrontText { get; init; } = string.Empty;
    [DataMember, MemoryPackOrder(3)] public string BackText { get; init; } = string.Empty;
    [DataMember, MemoryPackOrder(4)] public string? ImageUrl { get; init; }
    [DataMember, MemoryPackOrder(5)] public string? AudioUrl { get; init; }
    [DataMember, MemoryPackOrder(6)] public DateTime CreatedAt { get; init; }
}
```

### DTO Rules:
- Must be decorated with `[DataContract, MemoryPackable(GenerateType.VersionTolerant)]`
- Must be `partial` class
- All properties must have `[DataMember, MemoryPackOrder(n)]` attributes
- Use `init` for immutable properties
- Keep MemoryPackOrder sequential and don't skip numbers for better maintainability

## Commands

Commands represent write operations and are passed to CommandHandler methods.

```csharp
using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Commands;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record CreateFlashcardCommand(
    [property: DataMember, MemoryPackOrder(0)] Guid StudySetId,
    [property: DataMember, MemoryPackOrder(1)] string FrontText,
    [property: DataMember, MemoryPackOrder(2)] string BackText,
    [property: DataMember, MemoryPackOrder(3)] string? ImageUrl,
    [property: DataMember, MemoryPackOrder(4)] string? AudioUrl
) : ICommand<FlashcardDto>;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record UpdateFlashcardCommand(
    [property: DataMember, MemoryPackOrder(0)] Guid Id,
    [property: DataMember, MemoryPackOrder(1)] string FrontText,
    [property: DataMember, MemoryPackOrder(2)] string BackText,
    [property: DataMember, MemoryPackOrder(3)] string? ImageUrl,
    [property: DataMember, MemoryPackOrder(4)] string? AudioUrl
) : ICommand<FlashcardDto>;
```

### Command Rules:
- Must be decorated with `[DataContract, MemoryPackable(GenerateType.VersionTolerant)]`
- Use `partial record` for immutability
- Must implement `ICommand<TResult>` where TResult is the return type
- All properties must have `[property: DataMember, MemoryPackOrder(n)]` attributes
- Use primary constructor syntax for concise declaration

## Entity Models

Entity models represent database tables and should follow Domain-Driven Design principles.

```csharp
namespace SmartQuiz.Data.Models;

public class Flashcard : Entity
{
    // Private parameterless constructor for EF Core
    private Flashcard() { }
    
    // Public constructor with required parameters
    public Flashcard(Guid studySetId, string? frontText, string? backText, 
                     string? imageUrl, string? audioUrl)
    {
        StudySetId = studySetId;
        FrontText = frontText;
        BackText = backText;
        ImageUrl = imageUrl;
        AudioUrl = audioUrl;
    }

    public Guid StudySetId { get; set; }
    public string? FrontText { get; set; }
    public string? BackText { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    
    // Navigation properties
    public FlashcardSet? StudySet { get; set; }
}
```

### Entity Rules:
- Inherit from `Entity` base class (provides Id, CreatedAt, UpdatedAt)
- Always include private parameterless constructor for EF Core
- Use public constructor with required parameters for domain logic
- Keep business logic within entity methods when appropriate
- Use nullable reference types appropriately

## Blazor Component Patterns

### 1. ComputedStateComponent - Reactive UI (Recommended)

```razor
@inherits ComputedStateComponent<User>
@inject IMyService MyService
 
@if (State.HasValue)
{
    <div>
        <h3>@State.Value.Name</h3>
        <p>@State.Value.Email</p>
    </div>
}
else if (State.HasError)
{
    <div class="alert alert-danger">@State.Error.Message</div>
}
else
{
    <div>Loading...</div>
}
 
@code {
    [Parameter] public string UserId { get; set; } = "";
    
    // Called automatically when component initializes or parameters change
    protected override Task<User> ComputeState(CancellationToken cancellationToken)
        => MyService.GetUser(UserId, cancellationToken);
    
    // Component automatically re-renders when GetUser is invalidated
    // No manual event handlers needed!
}
```

### 2. LiveState - Manual state management

```razor
@inject IStateFactory StateFactory
@inject IMyService MyService
@implements IDisposable
 
@if (_userState?.Value is { } user)
{
    <div>@user.Name</div>
}
 
@code {
    [Parameter] public string UserId { get; set; } = "";
    private ILiveState<User>? _userState;
    
    protected override void OnInitialized()
    {
        _userState = StateFactory.NewLive<User>(
            options => options with {
                InitialValue = User.Loading,
                UpdateDelayer = FixedDelayer.Instant,
            },
            async (_, ct) => await MyService.GetUser(UserId, ct));
    }
    
    protected override void OnParametersSet()
    {
        // Recompute when parameter changes
        _userState?.Recompute();
    }
    
    public void Dispose() => _userState?.Dispose();
}
```

### 3. StatefulComponentBase - With multiple states

```razor
@inherits StatefulComponentBase
@inject IMyService MyService
 
<div>
    <h3>@_userName</h3>
    <p>Items: @_itemCount</p>
</div>
 
@code {
    [Parameter] public string UserId { get; set; } = "";
    
    private string _userName = "";
    private int _itemCount = 0;
    
    protected override async Task OnInitializedAsync()
    {
        // Create multiple live states
        var userState = StateFactory.NewLive<User>(
            async (_, ct) => await MyService.GetUser(UserId, ct));
        
        var itemsState = StateFactory.NewLive<List<Item>>(
            async (_, ct) => await MyService.GetUserItems(UserId, ct));
        
        // Combine states
        await userState.When(x => x.HasValue);
        _userName = userState.Value.Name;
        
        await itemsState.When(x => x.HasValue);
        _itemCount = itemsState.Value.Count;
        
        StateHasChanged();
    }
}
```



## MudBlazor UI Patterns

### Form Handling with Commands

```razor
@page "/flashcard-sets/create"
@using SmartQuiz.Client.Data.Commands
@inject ICommander Commander
@inject NavigationManager Navigation
@inject ISnackbar Snackbar

<MudContainer MaxWidth="MaxWidth.Medium" Class="mt-8">
    <MudPaper Elevation="0" Class="pa-6">
        <MudText Typo="Typo.h4" Class="mb-4">Create New Flashcard Set</MudText>
        
        <MudForm @ref="_form" ValidationDelay="0">
            <MudTextField
                @bind-Value="_command.Title"
                Label="Title"
                Variant="Variant.Outlined" 
                Margin="Margin.Dense" 
                ShrinkLabel
                Required="true"
                RequiredError="Title is required"
                MaxLength="200"
                Immediate="true"
                Validation="@((string value) => ValidateTitle(value))"
                Class="mb-4" />

            <MudTextField
                @bind-Value="_command.Description"
                Label="Description"
                Variant="Variant.Outlined" 
                Lines="4"
                Class="mb-6" />

            <MudStack Row="true" Spacing="3">
                <MudButton
                    OnClick="@HandleSubmit"
                    Variant="Variant.Filled"
                    Color="Color.Primary"
                    Disabled="@_isSubmitting"
                    StartIcon="@Icons.Material.Filled.Add">
                    @(_isSubmitting ? "Creating..." : "Create Set")
                </MudButton>

                <MudButton
                    Variant="Variant.Outlined"
                    Color="Color.Secondary"
                    OnClick="@(() => Navigation.NavigateTo("/"))"
                    Disabled="@_isSubmitting">
                    Cancel
                </MudButton>
            </MudStack>
        </MudForm>
    </MudPaper>
</MudContainer>

@code {
    private bool _isSubmitting;
    private MudForm? _form;
    private readonly CreateFlashcardSetCommand _command = new();
    
    private string? ValidateTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Title is required";
        
        if (value.Length < 3)
            return "Title must be at least 3 characters long";
        
        return null;
    }
    
    private async Task HandleSubmit()
    {
        if (_form == null)
            return;

        await _form.Validate();

        if (!_form.IsValid)
        {
            Snackbar.Add("Please fix validation errors", Severity.Warning);
            return;
        }

        try
        {
            _isSubmitting = true;

            // Execute command using ICommander
            var result = await Commander.Call(_command);

            Snackbar.Add("Flashcard set created successfully!", Severity.Success);
            Navigation.NavigateTo($"/flashcard-sets/{result.Id}");
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isSubmitting = false;
        }
    }
}
```

### MudBlazor Common Patterns

#### Layout Structure
```razor
<MudContainer MaxWidth="MaxWidth.ExtraLarge" Class="py-6">
    <MudGrid>
        <MudItem xs="12" md="6">
            <!-- Content -->
        </MudItem>
    </MudGrid>
</MudContainer>
```

#### Cards
```razor
<MudCard Elevation="2" Style="border-radius: 12px;">
    <MudCardContent>
        <MudText Typo="Typo.h6" Class="fw-bold mb-2">Title</MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary">Description</MudText>
    </MudCardContent>
    <MudCardActions>
        <MudButton Color="Color.Primary">Action</MudButton>
    </MudCardActions>
</MudCard>
```

#### Dialogs
```razor
<MudDialog>
    <DialogContent>
        <MudText>Are you sure you want to delete this item?</MudText>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="Cancel">Cancel</MudButton>
        <MudButton Color="Color.Error" Variant="Variant.Filled" OnClick="Submit">Delete</MudButton>
    </DialogActions>
</MudDialog>
```

#### Loading States
```razor
@if (State.HasValue)
{
    <!-- Content -->
}
else if (State.HasError)
{
    <MudAlert Severity="Severity.Error">@State.Error.Message</MudAlert>
}
else
{
    <MudProgressCircular Indeterminate="true" />
}
```

## MudBlazor Design Rules

### Core Styling Principles

#### 1. Use MudBlazor's Built-in Spacing System
MudBlazor uses a spacing system based on 4px increments. Use the `Class` attribute with these utilities:

**Padding:**
- `pa-{0-16}`: All sides padding
- `px-{0-16}`: Horizontal (left + right) padding
- `py-{0-16}`: Vertical (top + bottom) padding
- `pt-{0-16}`, `pr-{0-16}`, `pb-{0-16}`, `pl-{0-16}`: Individual sides

**Margin:**
- `ma-{0-16}`: All sides margin
- `mx-{0-16}`: Horizontal margin
- `my-{0-16}`: Vertical margin
- `mt-{0-16}`, `mr-{0-16}`, `mb-{0-16}`, `ml-{0-16}`: Individual sides

```razor
<!-- Good: Using MudBlazor spacing -->
<MudPaper Class="pa-6 mb-4">
    <MudText Class="mb-2">Title</MudText>
</MudPaper>

<!-- Avoid: Inline styles for spacing -->
<MudPaper Style="padding: 24px; margin-bottom: 16px;">
    <MudText Style="margin-bottom: 8px;">Title</MudText>
</MudPaper>
```

#### 2. Flexbox Utilities
Use MudBlazor's flexbox classes for layouts:

```razor
<!-- Flex container -->
<div class="d-flex">
    <!-- Flex items -->
</div>

<!-- Direction -->
<div class="d-flex flex-row">Horizontal</div>
<div class="d-flex flex-column">Vertical</div>

<!-- Alignment -->
<div class="d-flex align-center">Vertically centered</div>
<div class="d-flex justify-center">Horizontally centered</div>
<div class="d-flex justify-space-between">Space between</div>
<div class="d-flex align-center justify-space-between">Both</div>

<!-- Gap spacing -->
<div class="d-flex gap-2">Items with 8px gap</div>
<div class="d-flex gap-4">Items with 16px gap</div>

<!-- Flex grow/shrink -->
<div class="flex-grow-1">Grows to fill space</div>
<div class="flex-grow-0">Doesn't grow</div>
```

#### 3. Typography
Always use `MudText` component with proper `Typo` values:

```razor
<!-- Headings -->
<MudText Typo="Typo.h3">Main Title</MudText>
<MudText Typo="Typo.h4">Section Title</MudText>
<MudText Typo="Typo.h5">Subsection</MudText>
<MudText Typo="Typo.h6">Small Heading</MudText>

<!-- Body text -->
<MudText Typo="Typo.body1">Normal text</MudText>
<MudText Typo="Typo.body2">Smaller text</MudText>

<!-- Special -->
<MudText Typo="Typo.caption" Color="Color.Secondary">Label text</MudText>
<MudText Typo="Typo.subtitle1">Subtitle</MudText>

<!-- Font weight -->
<MudText Class="fw-bold">Bold text</MudText>
<MudText Class="fw-normal">Normal weight</MudText>

<!-- Text alignment -->
<MudText Align="Align.Center">Centered</MudText>
<MudText Align="Align.Right">Right aligned</MudText>
```

#### 4. Color System
Use MudBlazor's semantic color system:

```razor
<!-- Primary colors -->
<MudButton Color="Color.Primary">Primary Action</MudButton>
<MudButton Color="Color.Secondary">Secondary Action</MudButton>

<!-- Semantic colors -->
<MudButton Color="Color.Success">Success</MudButton>
<MudButton Color="Color.Error">Delete</MudButton>
<MudButton Color="Color.Warning">Warning</MudButton>
<MudButton Color="Color.Info">Info</MudButton>

<!-- Text colors -->
<MudText Color="Color.Primary">Primary text</MudText>
<MudText Color="Color.Secondary">Secondary/muted text</MudText>
<MudText Color="Color.Error">Error message</MudText>

<!-- Surface/background -->
<MudPaper Color="Color.Surface">Default surface</MudPaper>
```

#### 5. Elevation and Shadows
Control depth with elevation values (0-25):

```razor
<!-- Cards and surfaces -->
<MudPaper Elevation="0">Flat (no shadow)</MudPaper>
<MudPaper Elevation="1">Subtle shadow</MudPaper>
<MudPaper Elevation="2">Normal card shadow</MudPaper>
<MudPaper Elevation="3">Elevated element</MudPaper>
<MudPaper Elevation="8">Modal/dialog shadow</MudPaper>

<!-- Buttons -->
<MudButton Variant="Variant.Filled" DropShadow="false">No shadow</MudButton>
```

#### 6. Border Radius
Use inline styles or custom CSS for rounded corners:

```razor
<!-- Standard radius -->
<MudCard Style="border-radius: 12px;">
    <MudCardContent>Rounded card</MudCardContent>
</MudCard>

<!-- Consistent pattern: 12px for cards, 8px for inputs, 24px for pills -->
<MudPaper Style="border-radius: 12px;" Class="pa-4">
    <MudTextField Style="border-radius: 8px;" />
    <MudButton Style="border-radius: 24px;">Pill Button</MudButton>
</MudPaper>
```

#### 7. Icons
Always use Material Icons from `Icons.Material.Filled` or `Icons.Material.Outlined`:

```razor
<!-- In buttons -->
<MudButton StartIcon="@Icons.Material.Filled.Add">Create</MudButton>
<MudButton EndIcon="@Icons.Material.Filled.ArrowForward">Next</MudButton>

<!-- Icon buttons -->
<MudIconButton Icon="@Icons.Material.Filled.Delete" Color="Color.Error" />

<!-- In text -->
<div class="d-flex align-center gap-2">
    <MudIcon Icon="@Icons.Material.Filled.Info" Size="Size.Small" />
    <MudText>Information text</MudText>
</div>

<!-- Icon sizes -->
<MudIcon Icon="@Icons.Material.Filled.Star" Size="Size.Small" />
<MudIcon Icon="@Icons.Material.Filled.Star" Size="Size.Medium" />
<MudIcon Icon="@Icons.Material.Filled.Star" Size="Size.Large" />
```

#### 8. Responsive Design with Grid
Use `MudGrid` and `MudItem` for responsive layouts:

```razor
<MudGrid>
    <!-- Full width on mobile, half on desktop -->
    <MudItem xs="12" md="6">
        <MudCard>Content 1</MudCard>
    </MudItem>
    
    <MudItem xs="12" md="6">
        <MudCard>Content 2</MudCard>
    </MudItem>
    
    <!-- Thirds on desktop -->
    <MudItem xs="12" sm="6" md="4">
        <MudCard>Card 1</MudCard>
    </MudItem>
    <MudItem xs="12" sm="6" md="4">
        <MudCard>Card 2</MudCard>
    </MudItem>
    <MudItem xs="12" sm="6" md="4">
        <MudCard>Card 3</MudCard>
    </MudItem>
</MudGrid>

<!-- Breakpoints: xs (0px), sm (600px), md (960px), lg (1280px), xl (1920px), xxl (2560px) -->
```

#### 9. Form Components Best Practices

```razor
<!-- Consistent form styling -->
<MudForm @ref="_form" ValidationDelay="0">
    <!-- Text fields -->
    <MudTextField
        @bind-Value="_model.Name"
        Label="Name"
        Variant="Variant.Outlined"
        Margin="Margin.Dense"
        ShrinkLabel
        Required="true"
        RequiredError="Name is required"
        Class="mb-4" />
    
    <!-- Select -->
    <MudSelect
        @bind-Value="_model.Category"
        Label="Category"
        Variant="Variant.Outlined"
        Margin="Margin.Dense"
        ShrinkLabel
        Class="mb-4">
        <MudSelectItem Value="@("Option1")">Option 1</MudSelectItem>
        <MudSelectItem Value="@("Option2")">Option 2</MudSelectItem>
    </MudSelect>
    
    <!-- Checkbox -->
    <MudCheckBox
        @bind-Value="_model.IsEnabled"
        Label="Enable this feature"
        Color="Color.Primary"
        Class="mb-4" />
    
    <!-- Actions -->
    <MudStack Row="true" Spacing="2" Class="mt-4">
        <MudButton
            Variant="Variant.Filled"
            Color="Color.Primary"
            OnClick="Submit">
            Save
        </MudButton>
        <MudButton
            Variant="Variant.Outlined"
            OnClick="Cancel">
            Cancel
        </MudButton>
    </MudStack>
</MudForm>
```

#### 10. Menu and Dropdown Patterns

```razor
<!-- User menu example -->
<MudMenu AnchorOrigin="Origin.BottomRight" TransformOrigin="Origin.TopRight">
    <ActivatorContent>
        <MudButton Variant="Variant.Text">
            <div class="d-flex align-center gap-2">
                <MudAvatar Size="Size.Small">A</MudAvatar>
                <MudIcon Icon="@Icons.Material.Filled.KeyboardArrowDown" Size="Size.Small" />
            </div>
        </MudButton>
    </ActivatorContent>
    <ChildContent>
        <!-- Header -->
        <div class="pa-4" style="border-bottom: 1px solid #e0e0e0;">
            <MudText Typo="Typo.body1" Class="fw-bold">User Name</MudText>
            <MudText Typo="Typo.body2" Color="Color.Secondary">user@email.com</MudText>
        </div>
        
        <!-- Menu items -->
        <MudMenuItem OnClick="ViewProfile">
            <div class="d-flex align-center gap-3">
                <MudIcon Icon="@Icons.Material.Filled.Person" Size="Size.Small" />
                <MudText Typo="Typo.body2">Profile</MudText>
            </div>
        </MudMenuItem>
        
        <MudDivider Class="my-2" />
        
        <MudMenuItem OnClick="Logout">
            <div class="d-flex align-center gap-3">
                <MudIcon Icon="@Icons.Material.Filled.Logout" Size="Size.Small" Color="Color.Error" />
                <MudText Typo="Typo.body2" Color="Color.Error">Logout</MudText>
            </div>
        </MudMenuItem>
    </ChildContent>
</MudMenu>
```

### MudBlazor Component Styling Rules

**Remember: Follow the Styling Priority Rules (see top of document)**
1. MudBlazor built-in classes
2. TailwindCSS utilities
3. Blazor scoped CSS (`.razor.css`)
4. Inline styles (last resort)

1. **Use Built-in MudBlazor Classes First**
   - Always check if MudBlazor provides a utility before using TailwindCSS or inline styles
   - Use `Class` attribute with MudBlazor utilities: `Class="pa-6 mb-4 d-flex"`
   - Only use `Style` for dynamic values or truly custom styling

2. **Consistent Spacing**
   - Cards/Papers: `Class="pa-6"` (24px padding)
   - Form elements: `Class="mb-4"` (16px bottom margin)
   - Sections: `Class="mb-6"` (24px bottom margin)
   - Container: `Class="py-6"` (24px vertical padding)

3. **Button Guidelines**
   - Primary actions: `Variant="Variant.Filled" Color="Color.Primary"`
   - Secondary actions: `Variant="Variant.Outlined"`
   - Destructive actions: `Color="Color.Error"`
   - Always include icons for better UX: `StartIcon` or `EndIcon`

4. **Card Design**
   - Use `Elevation="2"` for standard cards
   - Use `Elevation="0"` or `Elevation="1"` for subtle surfaces
   - Use TailwindCSS for border-radius: `Class="rounded-xl"` (instead of `Style="border-radius: 12px;"`)
   - Add hover effect with custom class: `Class="hover-card"` (defined in app.css)

5. **Navigation Components**
   - Use `MudNavMenu` for sidebar navigation
   - Use `MudBreadcrumbs` for page hierarchy
   - Use `MudTabs` for content organization within a page

6. **Avoid Inline Styles**
   - ❌ Bad: `Style="border-radius: 12px; padding: 24px;"`
   - ✅ Good: `Class="rounded-xl pa-6"`
   - ❌ Bad: `Style="display: flex; align-items: center;"`
   - ✅ Good: `Class="d-flex align-center"`

## TailwindCSS Integration

### When to Use TailwindCSS vs MudBlazor

**Use MudBlazor Classes for:**
- Spacing (padding, margin)
- Flexbox layouts (`d-flex`, `align-center`, etc.)
- Typography weight (`fw-bold`)
- Component-specific styling

**Use TailwindCSS for:**
- Advanced layouts (Grid)
- Complex responsive design
- Custom gradients and effects
- Utilities not available in MudBlazor

### TailwindCSS Patterns in SmartQuiz

#### 1. Grid Layouts
```razor
<!-- Responsive grid -->
<div class="grid grid-cols-12 gap-4">
    <div class="col-span-12 md:col-span-6 lg:col-span-4">
        <MudCard>Content</MudCard>
    </div>
</div>

<!-- Centered content -->
<div class="grid grid-cols-12 gap-4 h-screen">
    <div class="col-span-10 col-start-2 md:col-span-6 md:col-start-4 self-center">
        <MudPaper>Centered form</MudPaper>
    </div>
</div>
```

#### 2. Glassmorphism Effect
```razor
<MudPaper Class="
    relative w-full p-6 rounded-2xl
    bg-white/20 backdrop-blur-xl
    border border-white/30
    shadow-2xl shadow-blue-900/30
">
    <!-- Content with glass effect -->
</MudPaper>
```

#### 3. Gradient Backgrounds
```razor
<!-- Gradient button -->
<MudButton Class="
    font-bold rounded-full text-white
    bg-gradient-to-r from-[#4285F4] via-[#34A853] to-[#FBBC05]
    hover:from-[#34A853] hover:to-[#EA4335]
    transition-all duration-300
">
    Colorful Button
</MudButton>

<!-- Gradient background -->
<div class="bg-gradient-to-br from-blue-500 to-purple-600">
    <MudContainer>Content</MudContainer>
</div>
```

#### 4. Custom Animations and Transforms
```razor
<!-- Hover effects -->
<MudCard Class="
    transform transition-transform duration-300
    hover:scale-105 hover:shadow-2xl
    cursor-pointer
">
    <MudCardContent>Hover me</MudCardContent>
</MudCard>

<!-- Translate -->
<MudPaper Class="-translate-y-1/2">
    Shifted up by 50%
</MudPaper>
```

#### 5. Responsive Utilities
```razor
<!-- Show/hide based on screen size -->
<div class="hidden md:block">Visible on desktop</div>
<div class="block md:hidden">Visible on mobile</div>

<!-- Responsive spacing -->
<div class="px-4 md:px-8 lg:px-12">
    <MudContainer>Content</MudContainer>
</div>

<!-- Responsive text -->
<MudText Class="text-xl md:text-2xl lg:text-3xl">
    Responsive heading
</MudText>
```

### Hybrid Styling Pattern (MudBlazor + TailwindCSS)

```razor
<!-- Recommended approach: Combine both -->
<MudContainer MaxWidth="MaxWidth.Large" Class="py-6">
    <MudGrid>
        <MudItem xs="12" md="6">
            <MudCard 
                Elevation="2"
                Class="pa-6 cursor-pointer hover-card transform transition-transform hover:scale-105"
                Style="border-radius: 12px;">
                
                <!-- MudBlazor for layout -->
                <div class="d-flex align-center gap-3 mb-4">
                    <MudAvatar Color="Color.Primary">A</MudAvatar>
                    <div>
                        <MudText Typo="Typo.h6" Class="fw-bold">Title</MudText>
                        <MudText Typo="Typo.body2" Color="Color.Secondary">Subtitle</MudText>
                    </div>
                </div>
                
                <!-- TailwindCSS for custom gradient -->
                <div class="bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg p-4">
                    <MudText Class="text-white">Gradient section</MudText>
                </div>
            </MudCard>
        </MudItem>
    </MudGrid>
</MudContainer>
```

### CSS Scoping with Razor Component Styles

For component-specific styles, use `.razor.css` files:

```razor
<!-- Component.razor -->
<div class="custom-component">
    <MudCard Class="styled-card">
        <MudCardContent>Content</MudCardContent>
    </MudCard>
</div>

<!-- Component.razor.css -->
```

```css
/* These styles are scoped to the component */
.custom-component {
    position: relative;
}

.styled-card {
    transition: transform 0.3s ease;
}

.styled-card:hover {
    transform: translateY(-4px);
}

/* Use ::deep for styling child components */
.custom-component ::deep .mud-card-content {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}
```

### TailwindCSS Configuration

The project uses TailwindCSS via CDN import:

```css
/* tailwind.css */
@import "tailwindcss";
```

**Available Utility Classes:**
- Layout: `flex`, `grid`, `block`, `inline`, `hidden`
- Sizing: `w-full`, `h-screen`, `min-h-screen`
- Spacing: `p-4`, `m-auto`, `gap-4`, `space-x-4`
- Colors: `bg-blue-500`, `text-white`, `border-gray-300`
- Effects: `shadow-lg`, `backdrop-blur-xl`, `opacity-50`
- Transitions: `transition-all`, `duration-300`, `ease-in-out`
- Transforms: `scale-105`, `rotate-45`, `-translate-y-1/2`
- Gradients: `bg-gradient-to-r`, `from-blue-500`, `to-purple-600`

## Command Execution Patterns

### Using ICommander in Components

```csharp
@inject ICommander Commander

private async Task ExecuteCommand()
{
    try
    {
        var command = new CreateFlashcardCommand(
            StudySetId: _studySetId,
            FrontText: _frontText,
            BackText: _backText,
            ImageUrl: null,
            AudioUrl: null
        );
        
        var result = await Commander.Call(command);
        
        // Handle success
        Snackbar.Add("Success!", Severity.Success);
    }
    catch (Exception ex)
    {
        // Handle error
        Snackbar.Add($"Error: {ex.Message}", Severity.Error);
    }
}
```

### Command with CancellationToken

```csharp
private CancellationTokenSource? _cts;

private async Task ExecuteCommandWithCancellation()
{
    _cts = new CancellationTokenSource();
    
    try
    {
        var command = new UpdateFlashcardCommand(...);
        var result = await Commander.Call(command, _cts.Token);
    }
    catch (OperationCanceledException)
    {
        // Handle cancellation
    }
    finally
    {
        _cts?.Dispose();
        _cts = null;
    }
}

public void Dispose()
{
    _cts?.Cancel();
    _cts?.Dispose();
}
```

## Best Practices

### 1. Naming Conventions
- **Services**: `IFlashcardService`, `FlashcardService`
- **DTOs**: `FlashcardDto`, `FlashcardSetDto`
- **Commands**: `CreateFlashcardCommand`, `UpdateFlashcardCommand`
- **Entities**: `Flashcard`, `FlashcardSet`
- **Pages**: `HomePage.razor`, `CreateFlashcardSetPage.razor`

### 2. File Organization
```
SmartQuiz.Client/
├── Data/
│   ├── Commands/           # All command definitions
│   ├── Dtos/              # All DTO definitions
│   └── Services/          # Service interfaces
├── Pages/                 # Routable pages
├── Components/            # Reusable components
└── Layout/               # Layout components

SmartQuiz/
├── Application/          # Service implementations
├── Data/
│   ├── Models/          # Entity models
│   ├── Configurations/  # EF Core configurations
│   └── Migrations/      # EF Core migrations
```

### 3. State Management Guidelines
- Use `ComputedStateComponent<T>` for simple reactive state (recommended)
- Use `ILiveState` for complex scenarios requiring manual control
- Always dispose of `ILiveState` instances
- Avoid mixing reactive and imperative state management

### 4. Error Handling
- Always wrap command execution in try-catch blocks
- Use `ISnackbar` for user-friendly error messages
- Log errors appropriately on the server side
- Return appropriate HTTP status codes from APIs

### 5. Performance Tips
- Use `IEnumerable<T>` for simple collections in DTOs
- Use `ImmutableList<T>` when immutability is crucial
- Always include proper indexes on frequently queried columns
- Use `.AsNoTracking()` for read-only queries when not using Fusion
- Let Fusion handle caching - don't add manual caching layers

### 6. Security Considerations
- Always validate commands on the server side
- Use `[Authorize]` attributes appropriately
- Never trust client-side validation alone
- Sanitize user inputs before storing in database
- Use parameterized queries (EF Core handles this automatically)

### 7. Testing Guidelines
- Test compute methods with different cache states
- Test command invalidation logic
- Test concurrent command execution scenarios
- Mock `ICommander` in component tests
- Use in-memory database for integration tests
