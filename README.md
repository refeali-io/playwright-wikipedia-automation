# Playwright Wikipedia Automation

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
PlaywrightWikipediaAutomation/
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
   git clone https://github.com/your-username/playwright-wikipedia-automation.git
   cd playwright-wikipedia-automation
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
   pwsh PlaywrightWikipediaAutomation/bin/Debug/net7.0/playwright.ps1 install
   ```

### Running Tests

Run all tests (Allure.NUnit writes results to `PlaywrightWikipediaAutomation/bin/Debug/net7.0/allure-results/` by default):
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
   allure serve PlaywrightWikipediaAutomation/bin/Debug/net7.0/allure-results
   ```
   This generates the HTML report and opens it in your default browser.

   **Alternative:** generate to a folder and open `allure-report/index.html` in a browser:
   ```bash
   dotnet test
   allure generate PlaywrightWikipediaAutomation/bin/Debug/net7.0/allure-results -o allure-report --clean
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

1. **GitHub Pages:** In **Settings > Pages**, set **Source** to **Deploy from a branch**, branch **gh-pages** (the workflow publishes to this branch).

2. The workflow (`.github/workflows/dotnet.yml`) does:
   - **Job 1 – Run QA Tests:** Checkout, .NET 7, restore & build, install Playwright, run tests. Allure.NUnit writes to `PlaywrightWikipediaAutomation/bin/Debug/net7.0/allure-results/`. That folder is uploaded as an artifact (`allure-results`), even when the job fails (`if: always()`).
   - **Job 2 – Deploy Allure Report:** Runs `if: always()` after the test job. Downloads the `allure-results` artifact, installs Allure CLI (Allure 2 via wget), restores **history** from `gh-pages` (for report trends), generates the HTML report, deploys to **gh-pages** with `peaceiris/actions-gh-pages`, comments on the PR with the report URL, and adds the URL to the Actions summary.

3. **Triggers:** Push and pull requests to `main` or `master` (ignoring changes to `README.md` only).

4. After a run, the report is at **https://\<username\>.github.io/\<repo>/**. On pull requests, the workflow also posts a comment with that link.

## Technologies Used

| Technology | Purpose |
|------------|---------|
| C# / .NET 7.0 | Programming language and runtime |
| Playwright | Browser automation |
| NUnit | Testing framework |
| Microsoft.Extensions.DI | Dependency injection |
| Microsoft.Extensions.Configuration | Configuration management |
| MediaWiki API | Wikipedia content API |

## License

MIT (or your preferred license).
