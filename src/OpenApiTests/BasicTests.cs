﻿using Tavis.OpenApi.Model;
using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenApiTests
{
    public class BasicTests
    {
        [Fact]
        public void TestYamlBuilding()
        {
            var doc = new YamlDocument(new YamlMappingNode(
                            new YamlScalarNode("a"), new YamlScalarNode("apple"),
                            new YamlScalarNode("b"), new YamlScalarNode("banana"),
                            new YamlScalarNode("c"), new YamlScalarNode("cherry")
                    ));

            var s = new YamlStream();
            s.Documents.Add(doc);
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            s.Save(writer);
            var output = sb.ToString();
        }

        [Fact]
        public void ParseSimplestOpenApiEver()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream("OpenApiTests.Samples.Simplest.yaml");

            var openApiDoc = OpenApiDocument.Parse(stream);

            Assert.Equal("1.0.0", openApiDoc.Version);
            Assert.Equal(0, openApiDoc.Paths.PathMap.Count());
            Assert.Equal("The Api", openApiDoc.Info.Title);
            Assert.Equal("0.9.1", openApiDoc.Info.Version);
            Assert.Equal(0, openApiDoc.ParseErrors.Count);

        }

        [Fact]
        public void ParseBrokenSimplest()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream("OpenApiTests.Samples.BrokenSimplest.yaml");

            var openApiDoc = OpenApiDocument.Parse(stream);

            Assert.Equal(2, openApiDoc.ParseErrors.Count);
            Assert.NotNull(openApiDoc.ParseErrors.Where(s=> s == "`openapi` property does not match the required format major.minor.patch").FirstOrDefault());
            Assert.NotNull(openApiDoc.ParseErrors.Where(s => s == "`info.title` is a required property").FirstOrDefault());

            
        }

        [Fact]
        public void ParsePetstoreAPI()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream("OpenApiTests.Samples.petstore30.yaml");

            var openApiDoc = OpenApiDocument.Parse(stream);

            Assert.Equal("3.0.0", openApiDoc.Version);
            Assert.Equal(2, openApiDoc.Paths.PathMap.Count());
            Assert.Equal("Swagger Petstore (Simple)", openApiDoc.Info.Title);
            Assert.Equal("1.0.0", openApiDoc.Info.Version);

        }


        [Fact]
        public void ParseCompleteHeaderOpenApi()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream("OpenApiTests.Samples.CompleteHeader.yaml");

            var openApiDoc = OpenApiDocument.Parse(stream);

            Assert.Equal("1.0.0", openApiDoc.Version);
            
            Assert.Equal(0, openApiDoc.Paths.PathMap.Count());
            Assert.Equal("The Api", openApiDoc.Info.Title);
            Assert.Equal("0.9.1", openApiDoc.Info.Version);
            Assert.Equal("This is an api", openApiDoc.Info.Description);
            Assert.Equal("Do what you want!", openApiDoc.Info.TermsOfService);
            Assert.Equal("Darrel Miller", openApiDoc.Info.Contact.Name);
            Assert.Equal("@darrel_miller", openApiDoc.Info.Contact.Extensions["x-twitter"]);

        }
    }
}
