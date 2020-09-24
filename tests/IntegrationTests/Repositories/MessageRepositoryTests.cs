using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Implementation;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Tests.Unit;
using COLID.AppDataService.Tests.Unit.Builder;
using Xunit;
using Xunit.Abstractions;

namespace COLID.AppDataService.Tests.Integration.Repositories
{
    public class MessageRepositoryTests : IntegrationTestBase
    {
        private readonly AppDataContext _context;
        private readonly IMessageRepository _repo;

        public MessageRepositoryTests(ITestOutputHelper output) : base(output)
        {
            _context = new AppDataContext(GetDbContextOptions());
            _repo = new MessageRepository(_context);

            _context.Database.EnsureCreated();
        }

        [Fact]
        public void GetUnreadMessagesToSend_Should_ReturnUnreadMessages()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var message = new MessageBuilder().WithSubject("yep").WithBody("nope")
                .WithSendOn(DateTime.Now.AddDays(-1))
                .Build();
            user.Messages = new Collection<Message>() { message };
            _seeder.Update(user);

            var actual = _repo.GetUnreadMessagesToSend();
            Assert.NotNull(actual);
        }

        [Fact]
        public void GetUnreadMessagesToSend_Should_Ignore_ReadMessages()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var message = new MessageBuilder().WithSubject("yep").WithBody("nope")
                .WithReadOn(DateTime.Now.AddDays(-2))
                .WithSendOn(DateTime.Now.AddDays(-1))
                .Build();
            user.Messages = new Collection<Message>() { message };
            _seeder.Update(user);

            var actual = _repo.GetUnreadMessagesToSend();
            Assert.Empty(actual);
        }

        [Fact]
        public void GetUnreadMessagesToSend_Should_Ignore_SendDateIsNull()
        {
            var user = _seeder.Add(TestData.GenerateRandomUser());
            var message = new MessageBuilder().WithSubject("yep").WithBody("nope")
                .WithReadOn(DateTime.Now.AddDays(-2))
                .Build();
            user.Messages = new Collection<Message>() { message };
            _seeder.Update(user);

            var actual = _repo.GetUnreadMessagesToSend();
            Assert.Empty(actual);
        }

        [Fact]
        public void GetUnreadMessagesToSend_Should_ReturnNothing_IfNoMessageExist()
        {
            var actual = _repo.GetUnreadMessagesToSend();
            Assert.Empty(actual);
        }
    }
}
