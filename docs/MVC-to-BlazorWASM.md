# MVC → Blazor WebAssembly Migration

A breakdown of every conceptual change made when converting `RegistrationSample.Web` from ASP.NET Core MVC to Blazor WebAssembly.

---

## 1. Project SDK changed

**Before (MVC):** `Microsoft.NET.Sdk.Web` — builds a server-side ASP.NET Core app.

**After (Blazor WASM):** `Microsoft.NET.Sdk.BlazorWebAssembly` — compiles C# to WebAssembly that runs in the browser.

---

## 2. Entry point changed

**Before:** `Program.cs` was an ASP.NET Core server — it set up middleware, routing, session, and served HTTP requests.

**After:** `Program.cs` is a browser bootstrapper — it creates a `WebAssemblyHostBuilder`, registers services, and calls `RunAsync()` to start the app in the browser.

---

## 3. The HTML shell

**Before:** `_Layout.cshtml` was a Razor view rendered by the server on every request.

**After:** `wwwroot/index.html` is a static HTML file served once. It contains `<div id="app">` where Blazor mounts itself, and loads `blazor.webassembly.js` which bootstraps the .NET runtime in the browser.

---

## 4. Controllers → Pages (components)

**Before:** Each user action hit a Controller action on the server:
```
Browser  POST /account/login  →  Server  →  AccountController.Login()  →  API
```

**After:** Everything runs in the browser. The component handles user input directly:
```
Browser (Blazor component)  →  HttpClient  →  API
```

The three controllers (`AccountController`, `ProfileController`, `HomeController`) were deleted entirely.

---

## 5. Views → Razor Components

**Before:** `.cshtml` files were templates rendered server-side. Every page navigation was a full server round-trip.

**After:** `.razor` files are components that run in the browser. Navigation is instant — only API calls hit the network.

| Old (MVC) | New (Blazor) |
|---|---|
| `Views/Account/Login.cshtml` | `Pages/Login.razor` |
| `Views/Account/Register.cshtml` | `Pages/Register.razor` |
| `Views/Profile/Index.cshtml` | `Pages/Profile/ProfileIndex.razor` |
| `Views/Profile/Edit.cshtml` | `Pages/Profile/ProfileEdit.razor` |
| `Views/Shared/_Layout.cshtml` | `Layout/MainLayout.razor` |

Code-behind files (`.razor.cs`) replaced the controller logic for each page.

---

## 6. Routing changed

**Before:** ASP.NET's routing mapped URLs to controller actions on the server (`MapControllerRoute`).

**After:** Blazor's client-side `<Router>` reads `@page "/account/login"` directives on components and handles navigation entirely in the browser — no server involved.

---

## 7. Authentication/session changed

**Before:** The JWT token was stored in **server-side session** (`HttpContext.Session`), which lives on the server and is keyed by a cookie.

**After:** The JWT token is stored in **browser `localStorage`** via JS Interop. The `TokenService` manages it in memory and persists it across reloads.

---

## 8. HTTP calls

**Before:** The MVC server made HTTP calls to the API using `IHttpClientFactory` (server-to-server).

**After:** The Blazor WASM app makes HTTP calls from the browser to the API (browser-to-server). The `ApiAuthorizationMessageHandler` automatically attaches the Bearer token to every request.

---

## What did NOT change

- The **API project** (`RegistrationSample.API`) — untouched
- The **ViewModels** in `Models/` — same classes, same properties
- The **visual appearance** — same Bootstrap layout and styles
