using OpenQA.Selenium;

namespace SeleniumNUnitDemo.Pages;

/// <summary>
/// Cart Page Object - Contains all elements and methods for Shopping Cart Page
/// Uses Page Object Model pattern with By locators
/// </summary>
public class CartPage : BasePage
{
    public CartPage(IWebDriver driver) : base(driver)
    {
        // Page elements initialized via locators
    }

    #region Web Element Locators

    private By PageTitleLocator => By.ClassName("title");
    private By CartListLocator => By.ClassName("cart_list");
    private By CartItemsLocator => By.ClassName("cart_item");
    private By ContinueShoppingButtonLocator => By.Id("continue-shopping");
    private By CheckoutButtonLocator => By.Id("checkout");
    private By ItemQuantitiesLocator => By.ClassName("cart_quantity");

    #endregion

    #region Page Actions/Methods

    /// <summary>
    /// Verify cart page is loaded
    /// </summary>
    public bool IsCartPageLoaded()
    {
        return IsElementDisplayed(By.ClassName("title")) && GetPageTitle().Contains("Swag Labs");
    }

    /// <summary>
    /// Get cart page title
    /// </summary>
    public string GetCartPageTitle()
    {
        return GetElementText(By.ClassName("title"));
    }

    /// <summary>
    /// Get number of items in cart
    /// </summary>
    public int GetCartItemCount()
    {
        return _driver.FindElements(CartItemsLocator).Count;
    }

    /// <summary>
    /// Get product name in cart by index
    /// </summary>
    public string GetCartItemName(int index)
    {
        var cartItems = _driver.FindElements(CartItemsLocator);
        if (index < 0 || index >= cartItems.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var nameElement = cartItems[index].FindElement(By.ClassName("inventory_item_name"));
        return nameElement.Text;
    }

    /// <summary>
    /// Get product price in cart by index
    /// </summary>
    public string GetCartItemPrice(int index)
    {
        var cartItems = _driver.FindElements(CartItemsLocator);
        if (index < 0 || index >= cartItems.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var priceElement = cartItems[index].FindElement(By.ClassName("inventory_item_price"));
        return priceElement.Text;
    }

    /// <summary>
    /// Remove item from cart by index
    /// </summary>
    public void RemoveItemFromCart(int index)
    {
        var cartItems = _driver.FindElements(CartItemsLocator);
        if (index < 0 || index >= cartItems.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var removeButton = cartItems[index].FindElement(By.TagName("button"));
        removeButton.Click();
    }

    /// <summary>
    /// Remove item from cart by product name
    /// </summary>
    public void RemoveItemFromCartByName(string productName)
    {
        var cartItems = _driver.FindElements(CartItemsLocator);
        for (int i = 0; i < cartItems.Count; i++)
        {
            if (GetCartItemName(i) == productName)
            {
                RemoveItemFromCart(i);
                return;
            }
        }
        throw new NoSuchElementException($"Product '{productName}' not found in cart");
    }

    /// <summary>
    /// Click continue shopping button
    /// </summary>
    public InventoryPage ClickContinueShopping()
    {
        WaitAndClick(ContinueShoppingButtonLocator);
        return new InventoryPage(_driver);
    }

    /// <summary>
    /// Click checkout button
    /// </summary>
    public CheckoutPage ClickCheckout()
    {
        WaitAndClick(CheckoutButtonLocator);
        return new CheckoutPage(_driver);
    }

    #endregion
}
