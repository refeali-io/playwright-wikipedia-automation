# Genpact Automation Framework

A lightweight C# Playwright automation framework demonstrating UI and API testing capabilities against Wikipedia.

## Overview

This project implements a test automation framework using:
- **C#** with **.NET 7.0**
- **Playwright** for browser automation
- **NUnit** as the testing framework
- **Page Object Model (POM)** design pattern
- **Dependency Injection** for clean architecture
- **MediaWiki Parse API** for API testing

## Target Application

All tests target the Wikipedia page: [Playwright (software)](https://en.wikipedia.org/wiki/Playwright_(software))

## Test Cases

### Task 1: Debugging Features Section Comparison
- Extracts the "Debugging features" section via **UI** (using Page Object Model)
- Extracts the same section via **API** (using MediaWiki Parse API)
- Normalizes both texts and counts unique words
- Asserts that both word counts are equal

### Task 2: Microsoft Development Tools Link Validation
- Navigates to the "Microsoft development tools" section
- Validates that all technology names are text links
- Fails the test if any technology name is not a link
- Reports all non-link entries for debugging

### Task 3: Color Theme Change Validation
- Opens the "Color (beta)" section from the appearance sidebar
- Changes the color theme to "Dark"
- Validates that the dark theme was actually applied

## Project Structure

```
GenpactAutomation/
├── Api/                      # API clients
│   ├── IMediaWikiApiClient.cs
│   └── MediaWikiApiClient.cs
├── Config/                   # Configuration classes
│   ├── MediaWikiApiConfig.cs
│   ├── PageNavigatorConfig.cs
│   ├── PlaywrightBrowserConfig.cs
│   ├── PlaywrightBrowserContextConfig.cs
│   ├── WikipediaConfig.cs
│   └── WikipediaOptionsPostConfigure.cs
├── Elements/                 # Custom element abstractions
│   ├── AbstractElement.cs
│   ├── Table.cs
│   └── TextBox.cs
├── Extensions/               # DI and configuration extensions
│   └── ConfigurationExtensions.cs
├── Navigation/               # Navigation utilities
│   └── PageNavigator.cs
├── Pages/                    # Page Objects
│   ├── Page.cs
│   └── WikipediaPlaywrightPage.cs
├── Tests/                    # Test classes
│   ├── PlaywrightBaseTest.cs
│   └── WikipediaPlaywrightTests.cs
├── Utils/                    # Utility classes
│   ├── TestReportHelper.cs
│   └── TextNormalizer.cs
├── appsettings.default.json  # Default configuration
├── SdkPlaywrightBaseTest.cs  # Base test setup
└── TestCompositionRoot.cs    # DI container setup
```

## Architecture Highlights

- **Clean Architecture**: Separation of concerns with distinct layers for pages, API clients, and utilities
- **SOLID Principles**: Single responsibility, dependency injection, and interface segregation
- **Configuration Management**: Centralized configuration with `IOptions` pattern
- **Reusable Components**: Element abstractions and text normalizers for consistent behavior

## Getting Started

### Prerequisites

- .NET 7.0 SDK or later
- Playwright browsers (installed automatically)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/genpact-assignment.git
   cd genpact-assignment
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Install Playwright browsers:
   ```bash
   pwsh GenpactAutomation/bin/Debug/net7.0/playwright.ps1 install
   ```

### Running Tests

Run all tests (Allure.NUnit writes results to `GenpactAutomation/bin/Debug/net7.0/allure-results/` by default):
```bash
dotnet test
```

Run with verbose output:
```bash
dotnet test --logger "console;verbosity=detailed"
```

Run a specific test:
```bash
dotnet test --filter "Task1_DebuggingFeaturesSection_UniqueWordCount_UI_Equals_API"
```

### Viewing the Allure report locally

1. **Prerequisites:** [Node.js](https://nodejs.org/) and Java 8+ (for Allure CLI).

2. **Install Allure commandline** (one time):
   ```bash
   npm install -g allure-commandline
   ```

3. **Run tests**, then generate and open the report:
   ```bash
   dotnet test
   allure serve GenpactAutomation/bin/Debug/net7.0/allure-results
   ```
   This generates the HTML report and opens it in your default browser.

   **Alternative:** generate to a folder and open `allure-report/index.html` in a browser:
   ```bash
   dotnet test
   allure generate GenpactAutomation/bin/Debug/net7.0/allure-results -o allure-report --clean
   # Then open allure-report/index.html
   ```

## Configuration

The framework uses `appsettings.default.json` for configuration:

```json
{
  "Wikipedia": {
    "BaseUrl": "https://en.wikipedia.org/"
  },
  "PlaywrightBrowserConfig": {
    "Headless": true,
    "Args": ["--start-maximized"]
  }
}
```

## CI/CD and Allure report

1. **GitHub Pages:** In the repository **Settings > Pages**, set **Source** to **GitHub Actions**.

2. The workflow (`.github/workflows/dotnet.yml`) does:
   - **Generate report:** Restore, build, install Playwright, run tests (Allure.NUnit writes to `allure-results/`), then run Allure CLI to produce the HTML report in `allure-report/`.
   - **Upload artifact:** `actions/upload-pages-artifact` uploads `allure-report/` (with `index.html` at root).
   - **Deploy:** `actions/deploy-pages` publishes to GitHub Pages.

3. After a successful run, the report is available at your GitHub Pages URL (e.g. `https://<username>.github.io/<repo>/`).

## Technologies Used

| Technology | Purpose |
|------------|---------|
| C# / .NET 7.0 | Programming language and runtime |
| Playwright | Browser automation |
| NUnit | Testing framework |
| Microsoft.Extensions.DI | Dependency injection |
| Microsoft.Extensions.Configuration | Configuration management |
| MediaWiki API | Wikipedia content API |

## Author

Developed as part of the Genpact Automation QA Assessment.

## License

This project is for assessment purposes only.
