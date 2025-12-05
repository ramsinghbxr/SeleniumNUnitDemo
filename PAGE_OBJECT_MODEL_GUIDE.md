# Page Object Model (POM) Implementation Guide

## Overview

Your Selenium automation project has been refactored to follow the **Page Object Model (POM)** design pattern, which is a best practice for maintaining scalable and maintainable test automation.

## What is Page Object Model?

The Page Object Model is a design pattern that treats each web page or page component as an object. It encapsulates:
- **Web elements** (using Page Factory annotations)
- **User interactions** (methods that interact with elements)
- **Page-specific logic** (validations, assertions)

## Benefits of POM

✅ **Maintainability**: Changes to UI elements are made in one place  
✅ **Reusability**: Page objects can be reused across multiple tests  
✅ **Readability**: Tests become more readable and business-focused  
✅ **Reduced Duplication**: Common actions are centralized  
✅ **Easier Debugging**: Issues are easier to trace and fix  
✅ **Scalability**: Easy to add new pages and tests  

## Project Structure

```
SeleniumNUnitDemo/
├── Pages/                          # Page Objects folder
│   ├── BasePage.cs                # Base class with common methods
│   ├── LoginPage.cs               # Login page object
│   ├── InventoryPage.cs           # Products/Inventory page object
│   ├── CartPage.cs                # Shopping cart page object
│   ├── CheckoutPage.cs            # Checkout information page
│   ├── CheckoutOverviewPage.cs    # Order review page
│   └── CheckoutCompletePage.cs    # Order confirmation page
├── SwagLabsTests_POM.cs           # Test cases using POM
└── SwagLabsTests.cs               # Original test file (old approach)
```

## Key Components

### 1. BasePage.cs - Base Page Class

Contains common methods shared by all pages:

```csharp
public abstract class BasePage
{
    protected IWebDriver _driver;
    protected WebDriverWait _wait;

    // Common methods used by all pages
    protected IWebElement WaitForElement(By locator) { }
    protected void WaitAndClick(By locator) { }
    protected void WaitAndSendKeys(By locator, string text) { }
    protected string GetElementText(By locator) { }
    protected bool IsElementDisplayed(By locator) { }
    public void TakeScreenshot(string fileName) { }
    public string GetPageTitle() { }
    public string GetCurrentUrl() { }
}
```

### 2. Page Factory - @FindBy Annotations

Using `SeleniumExtras.PageObjects` for automatic element initialization:

```csharp
[FindsBy(How = How.Id, Using = "user-name")]
private IWebElement UsernameField { get; set; }

[FindsBy(How = How.Id, Using = "password")]
private IWebElement PasswordField { get; set; }
```

**How to use FindsBy:**
- **How.Id**: Find by ID attribute
- **How.ClassName**: Find by class name
- **How.XPath**: Find by XPath
- **How.CssSelector**: Find by CSS selector
- **How.Name**: Find by name attribute
- **How.TagName**: Find by HTML tag name
- **How.LinkText**: Find by link text

### 3. LoginPage.cs - Example Page Object

```csharp
public class LoginPage : BasePage
{
    public LoginPage(IWebDriver driver) : base(driver)
    {
        PageFactory.InitElements(driver, this);
    }

    // Element definitions
    [FindsBy(How = How.Id, Using = "user-name")]
    private IWebElement UsernameField { get; set; }

    // Page methods
    public void EnterUsername(string username)
    {
        UsernameField.Clear();
        UsernameField.SendKeys(username);
    }

    public InventoryPage Login(string username, string password)
    {
        EnterUsername(username);
        EnterPassword(password);
        ClickLoginButton();
        return new InventoryPage(_driver);
    }
}
```

## Test File - SwagLabsTests_POM.cs

Tests are now much cleaner and business-focused:

```csharp
[Test]
public void Test_SuccessfulLogin()
{
    // Arrange
    var loginPage = new LoginPage(_driver);
    loginPage.NavigateToLoginPage();

    // Act
    var inventoryPage = loginPage.Login("standard_user", "secret_sauce");

    // Assert
    Assert.IsTrue(inventoryPage.IsInventoryPageLoaded());
}
```

### Before (Without POM)
```csharp
// Hard to read and maintain
_driver.FindElement(By.Id("user-name")).SendKeys("user");
_driver.FindElement(By.Id("password")).SendKeys("pass");
_driver.FindElement(By.Id("login-button")).Click();
var title = _driver.FindElement(By.ClassName("title")).Text;
Assert.That(title, Is.EqualTo("Products"));
```

### After (With POM)
```csharp
// Clean and business-focused
var inventoryPage = loginPage.Login("user", "pass");
Assert.IsTrue(inventoryPage.IsInventoryPageLoaded());
```

## Page Objects Included

### 1. LoginPage
- **Elements**: Username, Password, Login Button, Error Message
- **Methods**: 
  - `NavigateToLoginPage()`
  - `Login(username, password)` → returns InventoryPage
  - `IsErrorMessageDisplayed()`

### 2. InventoryPage
- **Elements**: Product List, Cart Link, Logout Button
- **Methods**:
  - `GetInventoryItemCount()`
  - `GetProductName(index)`
  - `AddProductToCart(index)`
  - `ClickCart()` → returns CartPage
  - `Logout()` → returns LoginPage

### 3. CartPage
- **Elements**: Cart Items, Continue Shopping, Checkout Button
- **Methods**:
  - `GetCartItemCount()`
  - `RemoveItemFromCart(index)`
  - `ClickCheckout()` → returns CheckoutPage

### 4. CheckoutPage
- **Elements**: First Name, Last Name, Postal Code, Continue Button
- **Methods**:
  - `FillCheckoutForm(firstName, lastName, postalCode)`
  - `ClickContinue()` → returns CheckoutOverviewPage

### 5. CheckoutOverviewPage
- **Elements**: Subtotal, Tax, Total, Finish Button
- **Methods**:
  - `GetSubtotal()`, `GetTax()`, `GetTotal()`
  - `ClickFinish()` → returns CheckoutCompletePage

### 6. CheckoutCompletePage
- **Elements**: Completion Message, Back to Products
- **Methods**:
  - `GetCompletionMessage()`
  - `ClickBackToProducts()` → returns InventoryPage

## How to Add a New Page

1. Create a new class in the `Pages` folder:

```csharp
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace SeleniumNUnitDemo.Pages;

public class MyNewPage : BasePage
{
    public MyNewPage(IWebDriver driver) : base(driver)
    {
        PageFactory.InitElements(driver, this);
    }

    #region Web Elements
    [FindsBy(How = How.Id, Using = "element-id")]
    private IWebElement MyElement { get; set; }
    #endregion

    #region Page Methods
    public void MyPageMethod()
    {
        WaitAndClick(By.Id("element-id"));
    }

    public AnotherPage GoToAnotherPage()
    {
        MyPageMethod();
        return new AnotherPage(_driver);
    }
    #endregion
}
```

2. Use it in your tests:

```csharp
[Test]
public void MyTest()
{
    var myNewPage = new MyNewPage(_driver);
    myNewPage.MyPageMethod();
    var anotherPage = myNewPage.GoToAnotherPage();
}
```

## Best Practices

1. **One Page, One Class**: Each page/component should have its own class
2. **Encapsulation**: Keep elements private and expose actions as public methods
3. **Meaningful Names**: Use descriptive names for methods and elements
4. **Chain Methods**: Return page objects to allow method chaining
5. **Avoid Hard Waits**: Use WebDriverWait in BasePage
6. **Keep Tests Clean**: Let pages handle all UI interactions
7. **DRY Principle**: Don't repeat locators or actions

## Example: Full E2E Test

```csharp
[Test]
public void Test_CompleteCheckoutFlow()
{
    // Arrange
    var loginPage = new LoginPage(_driver);
    loginPage.NavigateToLoginPage();

    // Act & Assert - Method chaining for fluent API
    var inventoryPage = loginPage
        .Login("standard_user", "secret_sauce");
    
    Assert.IsTrue(inventoryPage.IsInventoryPageLoaded());
    
    inventoryPage.AddProductToCart(0);
    inventoryPage.AddProductToCart(1);
    
    var cartPage = inventoryPage.ClickCart();
    Assert.That(cartPage.GetCartItemCount(), Is.EqualTo(2));
    
    var checkoutPage = cartPage.ClickCheckout();
    checkoutPage.FillCheckoutForm("John", "Doe", "12345");
    
    var overviewPage = checkoutPage.ClickContinue();
    Assert.IsTrue(overviewPage.IsCheckoutOverviewPageLoaded());
    
    var completePage = overviewPage.ClickFinish();
    Assert.That(completePage.GetCompletionMessage(), Contains.Substring("Thank you"));
}
```

## Troubleshooting

### Issue: "The name 'PageFactory' does not exist"
**Solution**: Add `using SeleniumExtras.PageObjects;` to your page class

### Issue: "How.Id does not exist"
**Solution**: Ensure `DotNetSeleniumExtras.PageObjects` NuGet package is installed

### Issue: Element not found during test
**Solution**: 
1. Check the element locator (use browser dev tools F12)
2. Verify waits in `BasePage` are appropriate
3. Check that `PageFactory.InitElements()` is called in constructor

### Issue: Null Reference Exception
**Solution**: Ensure `PageFactory.InitElements(driver, this)` is called in the constructor

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "SwagLabsTests_POM"

# Run specific test method
dotnet test --filter "Test_CompleteCheckoutFlow"

# Run with verbose output
dotnet test -v d
```

## Next Steps

1. ✅ Review the Page Object classes in the `Pages` folder
2. ✅ Study the test cases in `SwagLabsTests_POM.cs`
3. ✅ Create additional page objects for new features
4. ✅ Add more test cases using the POM pattern
5. ✅ Integrate with ReportPortal for test reporting
6. ✅ Set up CI/CD pipeline with POM tests

## References

- [Selenium PageFactory Pattern](https://www.selenium.dev/documentation/test_practices/encouraged_practices/page_object_models/)
- [DotNetSeleniumExtras Documentation](https://github.com/DotNetSeleniumTools/DotNetSeleniumExtras)
- [NUnit Documentation](https://docs.nunit.org/)

---

**Your automation project is now following industry best practices! 🎉**
