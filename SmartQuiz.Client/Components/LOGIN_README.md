# Login Components Documentation

## Overview

This project includes two login components matching the modern design shown in the reference image:

1. **Login.razor** - Full-page login component
2. **LoginDialog.razor** - Modal dialog version

## Features

Both components include:

- ✅ Clean, modern UI with QuizMaster branding
- ✅ Social login buttons (Google, Facebook, TikTok)
- ✅ Email/Username and Password fields
- ✅ Password visibility toggle
- ✅ Forgot password link
- ✅ Sign up link
- ✅ Loading state during authentication
- ✅ Responsive design
- ✅ MudBlazor components with consistent styling

## Usage

### Full-Page Login (/login)

Navigate to `/login` to display the full-page login experience.

```csharp
NavigationManager.NavigateTo("/login");
```

### Login Dialog

To show the login as a modal dialog:

```razor
@inject IDialogService DialogService
@using SmartQuiz.Client.Components

<MudButton OnClick="OpenLoginDialog">Login</MudButton>

@code {
    private async Task OpenLoginDialog()
    {
        var options = new DialogOptions 
        { 
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<LoginDialog>("", options);
        var result = await dialog.Result;

        if (result != null && !result.Canceled)
        {
            // User logged in successfully
            // Handle post-login logic here
        }
    }
}
```

### Example Page

Visit `/login-dialog-example` to see a working example of the login dialog.

## Customization

### Styling

Both components use:

- **MudBlazor** spacing utilities (`pa-`, `mb-`, `gap-`)
- **TailwindCSS** for gradients and advanced effects
- **Consistent colors** from MudBlazor's theme
- **Border radius**: 8px for inputs/buttons, 16px for the container

### Authentication Integration

To integrate with your authentication system, modify the `HandleLogin()` method:

```csharp
private async Task HandleLogin()
{
    _isLoading = true;
    StateHasChanged();

    try
    {
        // Replace with your authentication logic
        // var result = await AuthService.LoginAsync(_email, _password);
        
        // On success:
        NavigationManager.NavigateTo("/");
        
        // For dialog version:
        // MudDialog?.Close(DialogResult.Ok(true));
    }
    catch (Exception ex)
    {
        // Show error message
        Snackbar.Add($"Login failed: {ex.Message}", Severity.Error);
    }
    finally
    {
        _isLoading = false;
    }
}
```

### Social Login

The `HandleExternalLogin` method redirects to OAuth endpoints:

```csharp
private void HandleExternalLogin(string provider)
{
    NavigationManager.NavigateTo(
        $"/authentication/signin-{provider.ToLower()}",
        forceLoad: true);
}
```

Update the URLs to match your authentication provider configuration.

## Design Specifications

### Colors

- **Primary**: MudBlazor theme primary color (blue)
- **Secondary**: Gray for secondary text
- **Background**: White card on light gradient background

### Spacing

- Container padding: `pa-8` (32px)
- Field spacing: `mb-3` or `mb-4` (12-16px)
- Button height: `Size.Large`

### Typography

- Title: `Typo.h5` with `fw-bold`
- Body: `Typo.body2`
- Labels: Default MudTextField labels

### Components Used

- `MudPaper` - Container card
- `MudTextField` - Input fields
- `MudButton` - Action buttons
- `MudIcon` - Icons and logo
- `MudDivider` - Section separator
- `MudLink` - Text links
- `MudProgressCircular` - Loading indicator

## Browser Compatibility

Tested and working on:

- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)

## Accessibility

- Keyboard navigation support
- ARIA labels from MudBlazor components
- Focus management
- Screen reader friendly

## Future Enhancements

Potential improvements:

- [ ] Remember me checkbox
- [ ] Two-factor authentication
- [ ] Biometric login (WebAuthn)
- [ ] Social login callback handling
- [ ] Form validation messages
- [ ] Rate limiting feedback
- [ ] Animated transitions

