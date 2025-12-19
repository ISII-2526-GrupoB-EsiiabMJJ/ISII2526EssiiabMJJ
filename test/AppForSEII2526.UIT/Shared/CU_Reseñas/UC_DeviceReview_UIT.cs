using AppForMovies.UIT.CU_Reseñas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.CU_Reseñas
{

    public class UC_DeviceReview_UIT: UC_UIT
    {
        private ListDeviceForReview_PO selectDeviceForReview_PO;
        private CreateReview_PO createReview_PO;
        private ReviewDetails_PO reviewDetails_PO;
        private List<string[]> reviewedDeviceList = null;
        private List<string> chosedDevices = null;
        private const string deviceCounterId = "devicesCounter";
        private const string createReviewButton = "createReviewButton";
        private const string endReviewButton = "endReview";
        private const string modifyDevicesButton = "ModifyDevices";

        public UC_DeviceReview_UIT(ITestOutputHelper output) : base(output)
        {
            selectDeviceForReview_PO = new ListDeviceForReview_PO(_driver, _output);
            createReview_PO = new CreateReview_PO(_driver, _output);
            reviewDetails_PO = new ReviewDetails_PO(_driver, _output);

            chosedDevices = new List<string> {
                "RTX 5090 Founders Edition",
                "RTX 4080 Ti Dual Fan",
                "RTX 4090 Dual Ultimate",
                "RTX 5080 Gaming Pro",
                "RTX 5090 Founders Edition",
                "RTX 5090 OC Edition"
            };



            reviewedDeviceList = new List<string[]>{
            new string[] {
                "RTX 5090 Founders Edition",
                "NVIDIA GeForce RTX 5090",
                "2025","5","Random comment for test"},
            new string[] {
                "RTX 4080 Ti Dual Fan",
                "NVIDIA GeForce RTX 4080 Ti",
                "2024","4","Random comment for test"},
            new string[] {
                "RTX 4090 Dual Ultimate",
                "NVIDIA GeForce RTX 4090",
                "2024","5","Random comment for test"}, //modified mark 4 by 5 to pass sprint 3 functional test change
            new string[] {
                "RTX 5080 Gaming Pro",
                "NVIDIA GeForce RTX 5080",
                "2025","5","Random comment for test"},
            new string[] {
                "RTX 5090 OC Edition",
                "NVIDIA GeForce RTX 5090",
                "2025","5","Random comment for test"},
        };


        }
        public void Precondition_perform_login() {
            Perform_login("Pasatpetruvlad@gmail.com", "Pepe_0114");
            
        }
        public void InitialStepsForDeviceReview()
        {
            // Paso 1: login y esperar ver opcion de crear reseña en el navbar
            Precondition_perform_login();
            selectDeviceForReview_PO.WaitForBeingVisible(By.Id("createReviewNavbarButton"));
            // Paso 2: navegar directamente a la ruta de listado
            _driver.Navigate().GoToUrl(_URI + "review/listdevicesforreview");

            // Paso 3: esperar a que la tabla de dispositivos esté visible
            selectDeviceForReview_PO.WaitForBeingVisible(By.Id("devicesTable"));
            selectDeviceForReview_PO.WaitForBeingClickable(By.Id("devicesTable"));
        }
        //ReviwedDevices
        [Fact]
        [Trait("Level Testing", "Function Testing")]
        public void UC3_CAMBIO_EXAMEN_BF_AF0_AF1()
        {
            string? customerName = "Petru Vlad";
            string customerCountry = "Rumania";
            string reviewTitle = "Perfect GPU for Blender design";
            string deviceRating = "5";
            string deviceComment = "Random comment for test";
            List<string> devicesList = new List<string> {
                chosedDevices[0]
            };
            InitialStepsForDeviceReview();


            //search all devices
            selectDeviceForReview_PO.SearchDevices("", "");
            //select chosen devices without filter use
            foreach (var deviceName in devicesList)
            {
                selectDeviceForReview_PO.clickButton(By.Id("device_to_add_" + deviceName));
            }
            /////////////////////////////////////////////////
            /////////////////////////////////////////////////
            ///////////////////   Change 1   //////////////////
            //generate new list of selected device alfter filter apply.
            selectDeviceForReview_PO.SearchDevices("MSI", "");
            List<string> deviceFiltered = new List<string> {
                //chosedDevices[0]//before change
                chosedDevices[2]// here we changed the index information of the review done to show the data of the filtered device chosen, not the first device
                //chosen without filler apply as defult.
            };
            //select chosen devices after filter use
            foreach (var deviceName in deviceFiltered)
            {
                selectDeviceForReview_PO.clickButton(By.Id("device_to_add_" + deviceName));
            }
            /////////////////////////////////////////////////
            /////////////////////////////////////////////////
            /////////////////////////////////////////////////
            /////////////////////////////////////////////////
            ///////////////////   Change 2   //////////////////
            ///
            selectDeviceForReview_PO.SearchDevices("", "");// we filtered by brand MSI, but the firts device chosen without filter is an NVIDIA
            //so....to remove the first added device to cart we must apply a new filter as NVIDIA as brand and null year or directly with empty imputs
            //remove first chosen device
            foreach (var deviceName in devicesList) {
                selectDeviceForReview_PO.WaitForBeingClickable(By.Id("device_to_remove_" + deviceName));
                selectDeviceForReview_PO.clickButton(By.Id("device_to_remove_" + deviceName));
            }
            /////////////////////////////////////////////////
            /////////////////////////////////////////////////

            //click creare review to redirect to next route
            selectDeviceForReview_PO.clickButton(By.Id(createReviewButton));

            createReview_PO.FillInReviewInfo(customerName, customerCountry, reviewTitle);
            //createReview.FillDeviceRating(deviceRating);
            foreach (var deviceName in deviceFiltered) // we changed 
            {
                //wait the rating input to be visible
                selectDeviceForReview_PO.WaitForBeingVisible(By.Id("rating_" + deviceName));
                //add 5 as mark for each device
                createReview_PO.FillDeviceRating(deviceRating, "rating_" + deviceName);
                //wait the comment input to be visible
                selectDeviceForReview_PO.WaitForBeingVisible(By.Id("comment_" + deviceName));
                //add comment for each device
                createReview_PO.FillDeviceComment(deviceComment, "comment_" + deviceName);
            }
            //pulsar crear reseña
            selectDeviceForReview_PO.clickButton(By.Id(endReviewButton));



            // eeperamos mensaje de si estamos seguros de continuar
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            // Wait for clicable
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".modal-dialog")));
            string confirmMessage = _driver.FindElement(By.CssSelector(".modal-dialog .modal-body")).Text;
            // Click continue
            _driver.FindElement(By.Id("Button_DialogOK")).Click();


            bool areEquals = reviewDetails_PO.CheckReviewDetail(
                customerName,
                customerCountry,
                reviewTitle,
                new List<string[]> { reviewedDeviceList[2] },//modified reviewedDeviceList[0] by reviewedDeviceList[2]
                By.Id("ReviwedDevices"));

            // Assert
            Assert.True(areEquals);

        }

        //ReviwedDevices
        [Fact]
        [Trait("Level Testing", "Function Testing")]
        public void UC3_1_full_basic_flow()
        {
            string? customerName = "Petru Vlad";
            string customerCountry = "Rumania";
            string reviewTitle = "Perfect GPU for Blender design";
            string deviceRating = "5";
            string deviceComment = "Random comment for test";
            List<string> devicesList = new List<string> {
                chosedDevices[0]
            };
            InitialStepsForDeviceReview();


            //search all devices
            selectDeviceForReview_PO.SearchDevices("", "");
            //select chosen devices
            foreach (var deviceName in devicesList)
            {
                selectDeviceForReview_PO.clickButton(By.Id("device_to_add_" + deviceName));
            }
            //click creare review to redirect to next route
            selectDeviceForReview_PO.clickButton(By.Id(createReviewButton));

            createReview_PO.FillInReviewInfo(customerName, customerCountry, reviewTitle);
            //createReview.FillDeviceRating(deviceRating);
            foreach (var deviceName in devicesList)
            {
                //wait the rating input to be visible
                selectDeviceForReview_PO.WaitForBeingVisible(By.Id("rating_" + deviceName));
                //add 5 as mark for each device
                createReview_PO.FillDeviceRating(deviceRating, "rating_" + deviceName);
                //wait the comment input to be visible
                selectDeviceForReview_PO.WaitForBeingVisible(By.Id("comment_" + deviceName));
                //add comment for each device
                createReview_PO.FillDeviceComment(deviceComment, "comment_" + deviceName);
            }
            //pulsar crear reseña
            selectDeviceForReview_PO.clickButton(By.Id(endReviewButton));



            // eeperamos mensaje de si estamos seguros de continuar
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            // Wait for clicable
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".modal-dialog")));
            string confirmMessage = _driver.FindElement(By.CssSelector(".modal-dialog .modal-body")).Text;
            // Click continue
            _driver.FindElement(By.Id("Button_DialogOK")).Click();


            bool areEquals = reviewDetails_PO.CheckReviewDetail(
                customerName,
                customerCountry,
                reviewTitle,
                new List<string[]> { reviewedDeviceList[0] },
                By.Id("ReviwedDevices"));

            // Assert
            Assert.True(areEquals);

        }



        [Fact]
        [Trait("Level Testing", "Function Testing")]
        public void UC3_2_filter_by_brand()
        {
            InitialStepsForDeviceReview();

            var expectedDevices = new List<string[]> {
                new string[] {
                    "RTX 5090 Founders Edition",
                    "NVIDIA",
                    "Negro",
                    "2025",
                    "NVIDIA GeForce RTX 5090"
                },
            };

            // Act
            selectDeviceForReview_PO.SearchDevices("NVIDIA", "");

            // Arrange
            Assert.True(selectDeviceForReview_PO.CheckListOfDevices(expectedDevices));
        }
        
        [Fact]
        [Trait("Level Testing", "Function Testing")]
        public void UC3_3_filter_by_year()
        {
            InitialStepsForDeviceReview();

            var expectedDevices = new List<string[]> {
                new string[] {
                    "RTX 4080 Ti Dual Fan",
                    "Gigabyte",
                    "Blanco",
                    "2024",
                    "NVIDIA GeForce RTX 4080 Ti"
                },
                new string[] {
                    "RTX 4090 Dual Ultimate",
                    "MSI",
                    "Negro",
                    "2024",
                    "NVIDIA GeForce RTX 4090"
                },
            };
            //Act
            selectDeviceForReview_PO.SearchDevices("", "2024");

            //Agange
            Assert.True(selectDeviceForReview_PO.CheckListOfDevices(expectedDevices));

        }
        
        [Fact]
        [Trait("Level Testing", "Function Testing")]
        public void UC3_4_add_2_and_remove_1_from_cart()
        {
            InitialStepsForDeviceReview();
            //Act
            selectDeviceForReview_PO.SearchDevices("", "");
            //add devices
            selectDeviceForReview_PO.WaitForBeingClickable(By.Id("device_to_add_RTX 4080 Ti Dual Fan"));
            selectDeviceForReview_PO.clickButton(By.Id("device_to_add_RTX 4080 Ti Dual Fan"));
            //add devices
            selectDeviceForReview_PO.WaitForBeingClickable(By.Id("device_to_add_RTX 4090 Dual Ultimate"));
            selectDeviceForReview_PO.clickButton(By.Id("device_to_add_RTX 4090 Dual Ultimate"));
            //remove one divice
            selectDeviceForReview_PO.WaitForBeingClickable(By.Id("device_to_remove_RTX 4080 Ti Dual Fan"));
            selectDeviceForReview_PO.clickButton(By.Id("device_to_remove_RTX 4080 Ti Dual Fan"));
            //check if the counter decices show text
            selectDeviceForReview_PO.WaitForTextToBePresentInElement(By.Id("devicesCounter"), "1");

            //check actual cart counter
            //Agange
            string  cuantity = selectDeviceForReview_PO.GetTextById(By.Id("devicesCounter"));
            Assert.Equal(1, int.Parse(cuantity));
        }
        
        [Fact]
        [Trait("Level Testing", "Function Testing")]
        public void UC3_5_no_clicable_button_for_create_review()
        {
            InitialStepsForDeviceReview();
            //Act
            selectDeviceForReview_PO.SearchDevices("", "");
            //add devices
            selectDeviceForReview_PO.WaitForBeingClickable(By.Id("device_to_add_RTX 4080 Ti Dual Fan"));
            selectDeviceForReview_PO.clickButton(By.Id("device_to_add_RTX 4080 Ti Dual Fan"));

            //remove device
            selectDeviceForReview_PO.WaitForBeingClickable(By.Id("device_to_remove_RTX 4080 Ti Dual Fan"));
            selectDeviceForReview_PO.clickButton(By.Id("device_to_remove_RTX 4080 Ti Dual Fan"));

            //wait and check is is displayed and enabled the create review button
            bool isClicable = selectDeviceForReview_PO.checkClicableButton(By.Id("createReviewButton"));
            //Agange
            Assert.True(!isClicable);
        }



        [Theory]
        //UC3_6_country_not_provided
        [InlineData("Petru Vlad", "", "Current most powerful GPU", "5", "Perfect GPU for Blender design",
            "El nombre del país es obligatorio.")]

        //UC3_7_country_shorter_than_4
        [InlineData("Petru Vlad", "Rum", "Current most powerful GPU", "5", "Perfect GPU for Blender design",
            "El país debe tener entre 4 y 40 caracteres.")]

        //UC3_8_country_larger_than_40
        [InlineData("Petru Vlad", "Rumania_________________________________________",
            "Current most powerful GPU", "5", "Perfect GPU for Blender design",
            "El país debe tener entre 4 y 40 caracteres.")]

        //UC3_9_title_no_provided
        [InlineData("Petru Vlad", "Rumania", "", "5", "Perfect GPU for Blender design",
            "El título de la reseña es obligatorio.")]

        //UC3_10_title_shorter_than_10
        [InlineData("Petru Vlad", "Rumania", "Current", "5", "Perfect GPU for Blender design",
            "El título debe tener entre 10 y 100 caracteres.")]

        //UC3_11_title_larger_than_100
        [InlineData("Petru Vlad", "Rumania",
            "...............................................................................................................",
            "5", "Perfect GPU for Blender design",
            "El título debe tener entre 10 y 100 caracteres.")]

        //UC3_12_rating_mark_small_than_1
        [InlineData("Petru Vlad", "Rumania", "Current most powerful GPU", "0", "Perfect GPU for Blender design",
            "La calificación debe pertenecer al rango [1,5].")]



        //UC3_13_rating_mark_bigger_than_5
        [InlineData("Petru Vlad", "Rumania", "Current most powerful GPU", "6", "Perfect GPU for Blender design",
            "La calificación debe pertenecer al rango [1,5].")]

        //UC3_14_device_comment_not_provided
        [InlineData("Petru Vlad", "Rumania", "Current most powerful GPU", "5", "",
            "Es obligatorio un comentario por cada dispositivo.")]

        //UC3_15_device_comment_shorter_than_20
        [InlineData("Petru Vlad", "Rumania", "Current most powerful GPU", "5", "Perfect",
            "Todos los comentarios deben tener entre 20 y 100 caracteres.")]

        //UC3_16_device_comment_larger_than_100
        [InlineData("Petru Vlad", "Rumania", "Current most powerful GPU", "5",
            "Perfect GPU for Blender design______________________________________________________________________________________________________________________________________________",
            "Todos los comentarios deben tener entre 20 y 100 caracteres.")]
        [Trait("Level Testing", "Function Testing")]
        public void UC3_6_until_16_AF_testErrorsMandatoryData(
            string? customerName,
            string customerCountry,
            string reviewTitle,
            string deviceRating,
            string deviceComment,
            string expectedErrorMessage)
        {
            var devicesSelected = new List<string>() {
                chosedDevices[0]
            };
            InitialStepsForDeviceReview();
            //search all devices
            selectDeviceForReview_PO.SearchDevices("","");
            //select chosen devices
            foreach (var deviceName in devicesSelected) {
                selectDeviceForReview_PO.clickButton(By.Id("device_to_add_" + deviceName));
            }
            //click creare review to redirect to next route
            selectDeviceForReview_PO.clickButton(By.Id(createReviewButton));

            createReview_PO.FillInReviewInfo(customerName, customerCountry, reviewTitle);
            //createReview.FillDeviceRating(deviceRating);
            foreach (var deviceName in devicesSelected)
            {
                //wait the rating input to be visible
                selectDeviceForReview_PO.WaitForBeingVisible(By.Id("rating_" + deviceName));
                //add 5 as mark for each device
                createReview_PO.FillDeviceRating(deviceRating,"rating_" + deviceName);
                //wait the comment input to be visible
                selectDeviceForReview_PO.WaitForBeingVisible(By.Id("comment_" + deviceName));
                //add comment for each device
                createReview_PO.FillDeviceComment(deviceComment, "comment_" + deviceName);
            }
            //pulsar crear reseña
            selectDeviceForReview_PO.clickButton(By.Id(endReviewButton));



            // eeperamos mensaje de si estamos seguros de continuar
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            // Wait for clicable
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".modal-dialog")));
            string confirmMessage = _driver.FindElement(By.CssSelector(".modal-dialog .modal-body")).Text;
            // Click continue
            _driver.FindElement(By.Id("Button_DialogOK")).Click();




            // Wait for the error dialog
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".modal-dialog")));
            //Copy text information from dialog
            string errorMessage = _driver.FindElement(By.CssSelector(".modal-dialog .modal-body")).Text;
            // Close error dialog
            _driver.FindElement(By.Id("Button_DialogOK")).Click();



            // Assert
            Assert.Equal(expectedErrorMessage, errorMessage);
        }

        [Fact]
        [Trait("Level Testing", "Function Testing")]
        public void UC3_17_modify_selected_devices()
        {
            string? customerName = "Petru Vlad";
            string customerCountry = "Rumania";
            string reviewTitle = "Perfect GPU for Blender design";
            string deviceRating = "5";
            string deviceComment = "Random comment for test";
            List<string>  devicesList = new List<string> {
                chosedDevices[0], chosedDevices[1]
            };
            InitialStepsForDeviceReview();

            //search all devices
            selectDeviceForReview_PO.SearchDevices("", "");
            //select chosen devices
            foreach (var deviceName in devicesList)
            {
                selectDeviceForReview_PO.clickButton(By.Id("device_to_add_" + deviceName));
            }
            //click creare review to redirect to next route
            selectDeviceForReview_PO.clickButton(By.Id(createReviewButton));

            createReview_PO.FillInReviewInfo(customerName, customerCountry, reviewTitle);
            //createReview.FillDeviceRating(deviceRating);
            foreach (var deviceName in devicesList)
            {
                //wait the rating input to be visible
                selectDeviceForReview_PO.WaitForBeingVisible(By.Id("rating_" + deviceName));
                //add 5 as mark for each device
                createReview_PO.FillDeviceRating(deviceRating, "rating_" + deviceName);
                //wait the comment input to be visible
                selectDeviceForReview_PO.WaitForBeingVisible(By.Id("comment_" + deviceName));
                //add comment for each device
                createReview_PO.FillDeviceComment(deviceComment, "comment_" + deviceName);
            }
            //return to device list section
            selectDeviceForReview_PO.clickButton(By.Id(modifyDevicesButton));
            //remove device from cart
            selectDeviceForReview_PO.WaitForBeingClickable(By.Id("device_to_remove_" + devicesList[1]));
            selectDeviceForReview_PO.clickButton(By.Id("device_to_remove_" + devicesList[1]));

            selectDeviceForReview_PO.clickButton(By.Id(createReviewButton));

            List<string[]> expectedDevices = new List<string[]>()
            {
                new string[]{
                    "RTX 5090 Founders Edition",
                    "2025",
                    "NVIDIA GeForce RTX 5090",
                    deviceRating,
                    deviceComment
                },
            };
            bool momentaniusEquals = createReview_PO.CheckCreateReviewPersistance(customerName, customerCountry, reviewTitle);

            //equals = equals && createReview_PO.CheckBodyTable(devicechecker, By.Id("TableOfReviewItems"));  <======== useless if you dond use the sane blazor files

            Assert.True(momentaniusEquals);
            //verify saved information
            List<string[]> data = createReview_PO.reviewItemsInfo();
            // Assert
            Assert.Equal(expectedDevices, data);
        }
        
        //noDevicesAvailable
        [Fact]
        [Trait("Level Testing", "Function Testing")]
        public void UC3_18_no_devices_available()
        {
            InitialStepsForDeviceReview();

            // Act
            selectDeviceForReview_PO.SearchDevices("", "2023");

            selectDeviceForReview_PO.WaitForTextToBePresentInElement(By.Id("noDevicesAvailable"),
                "There are no devices available to create a review");
            // Esperar y obtener el texto directamente
            string text = selectDeviceForReview_PO.GetTextById(By.Id("noDevicesAvailable"));

            // Assert: primero esperado, luego actual
            Assert.Equal("There are no devices available to create a review", text);
        }

    }
}
