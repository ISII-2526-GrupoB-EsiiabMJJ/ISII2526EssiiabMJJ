using AppForSEII2526.UIT.Shared.CU_Reparaciones;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

public class UCRepairReceipt_UIT : UC_UIT
{
    private readonly SelectRepairsForReceipt_PO selectRepairs;
    private readonly CreateReceipt_PO createReceipt;
    private readonly DetailReceipt_PO detailReceipt;

    public UCRepairReceipt_UIT(ITestOutputHelper output) : base(output)
    {
        selectRepairs = new SelectRepairsForReceipt_PO(_driver, _output);
        createReceipt = new CreateReceipt_PO(_driver, _output);
        detailReceipt = new DetailReceipt_PO(_driver, _output);
    }

    private void InitialStepsForRepairReceipt()
    {
        Perform_login("jaime@uclm.es", "Aa123456789@");

        Thread.Sleep(500);

        _driver.Navigate().GoToUrl(_URI + "repair/selectrepairsforreceipt");

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

        wait.Until(d =>
            d.FindElements(By.Id("repairNameFilter")).Count > 0 ||
            d.FindElements(By.Id("TableOfRepairs")).Count > 0 ||
            d.PageSource.ToLower().Contains("no hay reparaciones"));

        selectRepairs.WaitPageReady();
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC_RepairReceipt_BasicFlow()
    {
        InitialStepsForRepairReceipt();

        selectRepairs.FilterRepairs("", "");

        Assert.True(selectRepairs.IsRepairsTableVisible());

        selectRepairs.AddFirstVisibleRepair();

        Assert.True(selectRepairs.IsCartTableVisible());
        Assert.False(selectRepairs.IsContinueButtonDisabled());
        Assert.Contains("Total:", selectRepairs.GetCartTotalPrice());

        selectRepairs.ContinueReceipt();

        createReceipt.WaitPageReady();

        Assert.True(createReceipt.IsReceiptItemsTableVisible());

        createReceipt.FillFirstRepairModel("iPhone 13");
        createReceipt.FillCustomerData("Jaime Cátedra", "Calle Universidad 1", "PayPal");
        createReceipt.SaveReceipt();

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        wait.Until(d => d.Url.ToLower().Contains("detailreceipt"));

        Assert.Contains("detailreceipt", _driver.Url.ToLower());
        Assert.True(detailReceipt.IsLoaded());
        Assert.True(detailReceipt.CheckReceiptDetail("Jaime Cátedra", "Calle Universidad 1", "PayPal"));
        Assert.True(detailReceipt.IsItemsTableVisible());
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC_RepairReceipt_EmptyCart_DisablesContinueButton()
    {
        InitialStepsForRepairReceipt();

        selectRepairs.FilterRepairs("", "");

        Assert.True(selectRepairs.IsEmptyCartMessageVisible());
        Assert.True(selectRepairs.IsContinueButtonDisabled());
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC_RepairReceipt_AddAndRemoveRepairFromCart()
    {
        InitialStepsForRepairReceipt();

        selectRepairs.FilterRepairs("", "");

        selectRepairs.AddFirstVisibleRepair();

        Assert.True(selectRepairs.IsCartTableVisible());
        Assert.False(selectRepairs.IsContinueButtonDisabled());

        selectRepairs.RemoveFirstRepairFromCart();

        Assert.True(selectRepairs.IsEmptyCartMessageVisible());
        Assert.True(selectRepairs.IsContinueButtonDisabled());
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC_RepairReceipt_FilterRepairs()
    {
        InitialStepsForRepairReceipt();

        selectRepairs.FilterRepairs("pantalla", "");

        Assert.True(
            selectRepairs.IsRepairsTableVisible() ||
            selectRepairs.HasNoRepairsMessage());
    }
}