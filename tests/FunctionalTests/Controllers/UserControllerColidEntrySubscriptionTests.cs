using System;
using System.Collections.Generic;
using System.Text;
using COLID.AppDataService.Tests.Functional;
using Xunit;

namespace FunctionalTests.Controllers
{
    [Collection("Sequential")]
    public partial class UserControllerTests : IClassFixture<FunctionTestsFixture>
    {
    }
}
