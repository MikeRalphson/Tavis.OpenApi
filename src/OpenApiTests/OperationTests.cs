﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavis.OpenApi;
using Tavis.OpenApi.Model;
using Xunit;

namespace OpenApiTests
{
    public class OperationTests
    {

        private OpenApiDocument _PetStoreDoc;
  //      private Operation _PostOperation;
        public OperationTests()
        {
            var parser = new OpenApiParser();
            var stream = this.GetType().Assembly.GetManifestResourceStream("OpenApiTests.Samples.petstore30.yaml");
            _PetStoreDoc = parser.Parse(stream);
            //_PostOperation = _PetStoreDoc.Paths.PathMap.Where(pm=>pm.Key == "/pets").Value
            //    .Operations.Where()
        }

        [Fact]
        public void CheckPetStoreFirstOperation()
        {
            var firstPath = _PetStoreDoc.Paths.PathItems.First().Value;
            var firstOperation = firstPath.Operations.First();
            Assert.Equal("get", firstOperation.Key);
            Assert.Equal("findPets", firstOperation.Value.OperationId);
            Assert.Equal(2, firstOperation.Value.Parameters.Count);
        }

        [Fact]
        public void CheckPetStoreFirstOperationParameters()
        {

            var firstPath = _PetStoreDoc.Paths.PathItems.First().Value;
            var firstOperation = firstPath.Operations.First().Value;
            var firstParameter = firstOperation.Parameters.First();

            Assert.Equal("tags", firstParameter.Name);
            Assert.Equal(InEnum.query, firstParameter.In);
            Assert.Equal("tags to filter by", firstParameter.Description);
        }

        [Fact]
        public void CheckPetStoreFirstOperationRequest()
        {

            var firstPath = _PetStoreDoc.Paths.PathItems.First().Value;
            var firstOperation = firstPath.Operations.First().Value;
            var requestBody = firstOperation.RequestBody;
            

            Assert.Null(firstOperation.RequestBody);
        }

        [Fact]
        public void GetPostOperation()
        {

            var postOperation = _PetStoreDoc.Paths.PathItems["/pets"].Operations["post"];

            Assert.Equal("addPet", postOperation.OperationId);

            Assert.NotNull(postOperation);

        }

        [Fact]
        public void GetResponses()
        {

            var getOperation = _PetStoreDoc.Paths.PathItems["/pets"].Operations["get"];

            var responses = getOperation.Responses;
            
            Assert.Equal(2, responses["200"].Content.Values.Count());
            var response = getOperation.Responses["200"];
            var content = response.Content["application/json"];

            Assert.NotNull(content.Schema);

        }




        [Fact]
        public void DoesAPathExist()
        {

            Assert.Equal(2, _PetStoreDoc.Paths.PathItems.Count());
            Assert.NotNull(_PetStoreDoc.Paths.GetPath("/pets"));
        }

    }
}
