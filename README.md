Automation Infrastructure Assignment
Goal: Build a lightweight framework and test cased to some UI and some API.
Target: https://en.wikipedia.org/wiki/Playwright_(software) — section “Debugging features”
The solution should be sent as a link to GitHub repository.
Task 1 
•	Extract the “Debugging features” section:
o	via UI (POM approach)
o	via API (MediaWiki Parse API)
•	Normalize both texts
•	Count unique words
•	Assert that both counts are equal
Task 2 
Via UI:  go to Microsoft development tools section (under “Debugging features”) and validate that all the “technology names” under this section are a text link, if not please fail the test.
Tip:  this is your decision if to do it as a one test case or multiple tests 
Task 3: 
Via UI: Go to the “Color (beta)” section (from the right) and change the color to “Dark” 
validate that the color actually changed  
Requirements
•	Clean architecture
Bonus
•	HTML report
Tech Stack
•	Language: C# + Playwright(preferred)/Selenium


