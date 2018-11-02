﻿using Com.Danliris.Sales.Test.WebApi.Utils;
using Com.Danliris.Service.Sales.Lib.BusinessLogic.Interface.Garment;
using Com.Danliris.Service.Sales.Lib.Models.CostCalculationGarments;
using Com.Danliris.Service.Sales.Lib.ViewModels.CostCalculationGarment;
using Com.Danliris.Service.Sales.Lib.ViewModels.Garment;
using Com.Danliris.Service.Sales.WebApi.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Sales.Test.WebApi.Controllers
{
    public class RO_Garment_ValidationControllerTest : BaseEmptyControllerTest<RO_Garment_ValidationController, CostCalculationGarment, CostCalculationGarment_RO_Garment_ValidationViewModel, IRO_Garment_Validation>
    {
        protected override CostCalculationGarment_RO_Garment_ValidationViewModel ViewModel
        {
            get {
                var ViewModel = base.ViewModel;
                ViewModel.CostCalculationGarment_Materials = new List<CostCalculationGarment_MaterialViewModel>();

                return ViewModel;
            }
        }

        [Fact]
        public async Task Post_WithoutException_ReturnCreated()
        {
            var mocks = GetMocks();
            mocks.Facade
                .Setup(f => f.ValidateROGarment(It.IsAny<CostCalculationGarment>(), It.IsAny<Dictionary<long, string>>()))
                .ReturnsAsync(1);

            var controller = GetController(mocks);
            var response = await controller.Post(ViewModel);

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Post_ThrowServiceValidationExeption_ReturnBadRequest()
        {
            var mocks = GetMocks();
            mocks.ValidateService
                .Setup(s => s.Validate(It.IsAny<CostCalculationGarment_RO_Garment_ValidationViewModel>()))
                .Throws(GetServiceValidationException());

            var controller = GetController(mocks);
            var response = await controller.Post(ViewModel);

            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Post_ThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.ValidateService
                .Setup(s => s.Validate(It.IsAny<CostCalculationGarment_RO_Garment_ValidationViewModel>()))
                .Verifiable();
            mocks.Facade
                .Setup(s => s.ValidateROGarment(It.IsAny<CostCalculationGarment>(), It.IsAny<Dictionary<long, string>>()))
                .ThrowsAsync(new Exception());

            var controller = GetController(mocks);
            var response = await controller.Post(ViewModel);

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
