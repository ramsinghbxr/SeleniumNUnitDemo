using OpenQA.Selenium;

namespace SeleniumNUnitDemo.Pages;

/// <summary>
/// Inventory Page Object - Contains all elements and methods for Products/Inventory Page
/// </summary>
public class InventoryPage : BasePage
{
    // Web Element Locators
    private By PageTitleLocator => By.ClassName("title");
    private By InventoryListLocator => By.ClassName("inventory_list");
    private By InventoryItemsLocator => By.ClassName("inventory_item");
    private By MenuButtonLocator => By.Id("react-burger-menu-btn");
    private By LogoutButtonLocator => By.Id("logout_sidebar_link");
    private By CartLinkLocator => By.ClassName("shopping_cart_link");
    private By CartBadgeLocator => By.ClassName("shopping_cart_badge");

    public InventoryPage(IWebDriver driver) : base(driver)
    {
    }

    #region Page Actions/Methods

    /// <summary>
    /// Verify inventory page is loaded
    /// </summary>
    public bool IsInventoryPageLoaded()
    {
        return IsElementDisplayed(PageTitleLocator) && GetPageTitle().Contains("Swag Labs");
    }

    /// <summary>
    /// Get inventory page title
    /// </summary>
    public string GetInventoryPageTitle()
    {
        return GetElementText(PageTitleLocator);
    }

    /// <summary>
    /// Get number of inventory items
    /// </summary>
    public int GetInventoryItemCount()
    {
        var items = _driver.FindElements(InventoryItemsLocator);
        return items.Count;
    }

    /// <summary>
    /// Get product name by index
    /// </summary>
    public string GetProductName(int index)
    {
        var items = _driver.FindElements(InventoryItemsLocator);
        if (index < 0 || index >= items.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var productElement = items[index].FindElement(By.ClassName("inventory_item_name"));
        return productElement.Text;
    }

    /// <summary>
    /// Get product price by index
    /// </summary>
    public string GetProductPrice(int index)
    {
        var items = _driver.FindElements(InventoryItemsLocator);
        if (index < 0 || index >= items.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var priceElement = items[index].FindElement(By.ClassName("inventory_item_price"));
        return priceElement.Text;
    }

    /// <summary>
    /// Add product to cart by index
    /// </summary>
    public void AddProductToCart(int index)
    {
        var items = _driver.FindElements(InventoryItemsLocator);
        if (index < 0 || index >= items.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var addButton = items[index].FindElement(By.TagName("button"));
        addButton.Click();
    }

    /// <summary>
    /// Add product to cart by product name
    /// </summary>
    public void AddProductToCartByName(string productName)
    {
        for (int i = 0; i < GetInventoryItemCount(); i++)
        {
            if (GetProductName(i) == productName)
            {
                AddProductToCart(i);
                return;
            }
        }
        throw new NoSuchElementException($"Product '{productName}' not found");
    }

    /// <summary>
    /// Get cart item count
    /// </summary>
    public int GetCartItemCount()
    {
        if (IsElementDisplayed(CartBadgeLocator))
        {
            return int.Parse(GetElementText(CartBadgeLocator));
        }
        return 0;
    }

    /// <summary>
    /// Click on cart link
    /// </summary>
    public CartPage ClickCart()
    {
        WaitAndClick(CartLinkLocator);
        return new CartPage(_driver);
    }

    /// <summary>
    /// Open menu
    /// </summary>
    public void OpenMenu()
    {
        WaitAndClick(MenuButtonLocator);
    }

    /// <summary>
    /// Logout from inventory page
    /// </summary>
    public LoginPage Logout()
    {
        OpenMenu();
        System.Threading.Thread.Sleep(500);
        WaitAndClick(LogoutButtonLocator);
        System.Threading.Thread.Sleep(1000);
        return new LoginPage(_driver);
    }

    #endregion
}
