using System;
using System.Collections.Generic;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Tests.Unit.Builder;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Tests.Unit
{
    public static class TestData
    {
        private static readonly Random _random = new Random();

        public static User GenerateRandomUser()
        {
            var id = Guid.NewGuid();
            var email = id.ToString("n").Substring(0, 10) + "@bayer.com";
            return new UserBuilder()
                .WithId(id)
                .WithEmailAddress(email)
                .WithMessageConfig(GenerateSampleMessageConfig())
                .WithSearchFilterDataMarketplace(new List<SearchFilterDataMarketplace>())
                .Build();
        }

        public static User GenerateRandomUserWithSavedSearchAndStoredQuery()
        {
            var user = TestData.GenerateRandomUser();
            user.MessageConfig = new MessageConfig() { DeleteInterval = DeleteInterval.Monthly , SendInterval = SendInterval.Weekly };
            var searchFilter = TestData.GenerateRandomSearchFilterDataMarketplace();
            searchFilter.User = user;
            var storedQuery = TestData.GenerateRandomStoredQuery();
            user.SearchFiltersDataMarketplace = new List<SearchFilterDataMarketplace>();
            searchFilter.StoredQuery = storedQuery;
            user.SearchFiltersDataMarketplace.Add(searchFilter);
            return user;
        }

        public static ConsumerGroup GenerateRandomConsumerGroup()
        {
            return new ConsumerGroupBuilder()
                .WithUri(new Uri($"https://pid.bayer.com/kos/19050#{Guid.NewGuid()}"))
                .Build();
        }

        public static ConsumerGroupDto GenerateRandomConsumerGroupDto()
        {
            return new ConsumerGroupBuilder()
                .WithUri(new Uri($"https://pid.bayer.com/kos/19050#{Guid.NewGuid()}"))
                .BuildDto();
        }

        public static SearchFilterEditor GenerateRandomSearchFilterEditor()
        {
            var randomString = Guid.NewGuid().ToString("n");
            var name = "SearchFilter_" + randomString.Substring(0, 10);
            var json = JObject.Parse("{\"editorValue\":\"" + randomString.Substring(0, 10) + "\"}");
            return new SearchFilterEditorBuilder()
                .WithFilterJson(json)
                .Build();
        }

        public static SearchFilterDataMarketplace GenerateRandomSearchFilterDataMarketplace()
        {
            var randomString = Guid.NewGuid().ToString("n");
            var name = "SearchFilter_" + randomString.Substring(0, 10);
            var json = JObject.Parse("{ \"aggregations\": { \"http://www.w3.org/1999/02/22-rdf-syntax-ns#type\": [ \"Ontologie\" ] }, \"ranges\": { } }");
            return new SearchFilterDataMarketplaceBuilder()
                .WithName(name)
                .WithFilterJson(json)
                .Build();
        }

        public static StoredQuery GenerateRandomStoredQuery()
        {
            var random = new Random(DateTime.Now.Second);
            var randomExecInterval = (ExecutionInterval) random.Next(0, 2);
            return new StoredQueryBuilder()
                .WithExecutionInterval(randomExecInterval)
                .WithSearchNumberResult(random.Next(0,20))
                .WithlatestExecutionDate(DateTime.Now.AddMonths(-1))
                .WithSearchResultHash("testStringHash"+ random.Next(0, 200))
                .Build();
        }

        public static ColidEntrySubscription GenerateRandomColidEntrySubscription()
        {
            return new ColidEntrySubscriptionBuilder()
                .WithColidEntry(new Uri($"https://pid.bayer.com/kos/19050/{Guid.NewGuid()}"))
                .Build();
        }

        public static ColidEntrySubscriptionDto GenerateRandomColidEntrySubscriptionDto()
        {
            return new ColidEntrySubscriptionBuilder()
                .WithColidEntry(new Uri($"https://pid.bayer.com/kos/19050/{Guid.NewGuid()}"))
                .BuildDto();
        }

        #region MessageTemplates - ColidEntrySubscriptions

        public static MessageTemplate GenerateSampleMessageTemplateForColidEntrySubscriptionUpdate()
        {
            return GenerateSampleMessageTemplateForColidEntrySubscription(MessageType.ColidEntrySubscriptionUpdate)
                .Build();
        }

        public static MessageTemplateDto GenerateSampleMessageTemplateForColidEntrySubscriptionUpdateDto()
        {
            return GenerateSampleMessageTemplateForColidEntrySubscription(MessageType.ColidEntrySubscriptionUpdate)
                .BuildDto();
        }

        public static MessageTemplate GenerateSampleMessageTemplateForColidEntrySubscriptionDelete()
        {
            return GenerateSampleMessageTemplateForColidEntrySubscription(MessageType.ColidEntrySubscriptionDelete)
                .Build();
        }

        public static MessageTemplateDto GenerateSampleMessageTemplateForColidEntrySubscriptionDeleteDto()
        {
            return GenerateSampleMessageTemplateForColidEntrySubscription(MessageType.ColidEntrySubscriptionDelete)
                .BuildDto();
        }

        #endregion MessageTemplates - ColidEntrySubscriptions

        private static MessageTemplateBuilder GenerateSampleMessageTemplateForColidEntrySubscription(MessageType messageType)
        {
            var msgTplBuilder = new MessageTemplateBuilder()
                .WithType(messageType);

            if (MessageType.ColidEntrySubscriptionUpdate.Equals(messageType))
            {
                return msgTplBuilder
                    .WithSubject($"Updated: %COLID_LABEL%")
                    .WithBody(
                        "The resource with title %COLID_LABEL% with the pid uri %COLID_PID_URI% has been updated in colid. Take a direct look by clicking on the uri.");
            }
            // else delete
            return msgTplBuilder
                .WithSubject($"Deleted: %COLID_LABEL%")
                .WithBody(
                    "The resource with title %COLID_LABEL% with the pid uri %COLID_PID_URI% has been deleted in colid.");
        }

        public static MessageConfig GenerateSampleMessageConfig()
        {
            return new MessageConfigBuilder()
                .WithSendInterval(SendInterval.Weekly)
                .WithDeleteInterval(DeleteInterval.Monthly)
                .Build();
        }

        public static Message GenerateSampleMessage()
        {
            var sendOn = DateTime.Now.CalculateByInterval(SendInterval.Weekly);
            var deleteOn = sendOn.Value.CalculateByInterval(DeleteInterval.Monthly);

            return new MessageBuilder()
                .WithSubject("A wild `COLID resource label` appeared!")
                .WithBody("Better throw the master ball to catch this precious pid uri: http://psyduck.pokemon")
                .WithSendOn(sendOn.Value)
                .WithDeleteOn(deleteOn.Value)
                .Build();
        }

        public static MessageDto GenerateSampleMessageDto()
        {
            var sendOnDate = (DateTime)DateTime.Now.CalculateByInterval(SendInterval.Weekly);
            var deleteOnDate = (DateTime)DateTime.Now.CalculateByInterval(DeleteInterval.Monthly);

            return new MessageBuilder()
                .WithSubject("A wild `COLID resource label` appeared!")
                .WithBody("Better throw the master ball to catch this precious pid uri: http://psyduck.pokemon")
                .WithSendOn(sendOnDate)
                .WithDeleteOn(deleteOnDate)
                .BuildDto();
        }

        public static IEnumerable<User> GetPreconfiguredUsers()
        {
            return new List<User>()
            {
                new UserBuilder()
                    .WithId(new Guid("11111111-1111-1111-1111-111111111111"))
                    .WithEmailAddress("joe.schreibvogel@tiger.king")
                    .Build(),

                new UserBuilder()
                    .WithId(new Guid("22222222-2222-2222-2222-222222222222"))
                    .WithEmailAddress("carole.baskin@tiger.king")
                    .Build(),

                new UserBuilder()
                    .WithId(new Guid("33333333-3333-3333-3333-333333333333"))
                    .WithEmailAddress("travis.maldonado@tiger.king")
                    .Build(),

                new UserBuilder()
                    .WithId(new Guid("44444444-4444-4444-4444-444444444444"))
                    .WithEmailAddress("jeff.lowe@tiger.king")
                    .Build(),

                new UserBuilder()
                    .WithId(new Guid("55555555-5555-5555-5555-555555555555"))
                    .WithEmailAddress("bhagavan.antle@tiger.king")
                    .Build()
            };
        }

        public static IEnumerable<ConsumerGroup> GetPreconfiguredConsumerGroups()
        {
            return new List<ConsumerGroup>()
            {
                new ConsumerGroupBuilder()
                    .WithUri(new Uri("https://pid.bayer.com/kos/19050#bf2f8eeb-fdb9-4ee1-ad88-e8932fa8753c"))
                    .Build(),

                new ConsumerGroupBuilder()
                    .WithUri(new Uri("https://pid.bayer.com/kos/19050#3bb018e4-b006-4c9d-a85c-cd409fec89e5"))
                    .Build(),

                new ConsumerGroupBuilder()
                    .WithUri(new Uri("https://pid.bayer.com/kos/19050#2b3f0380-dd22-4666-a28b-7f1eeb82a5ff"))
                    .Build(),

                new ConsumerGroupBuilder()
                    .WithUri(new Uri("https://pid.bayer.com/kos/19050#82fc2870-ca4e-407f-a197-bf3766ad785f"))
                    .Build()
            };
        }

        public static IEnumerable<SearchFilterEditor> GetPreconfiguredSearchFilterEditor()
        {
            var jsonString1 =
                "{\"aggregations\":[{\"key\":\"http://www.w3.org/1999/02/22-rdf-syntax-ns#type\",\"label\":\"Resource Type\",\"order\":0,\"buckets\":[{\"key\":\"Ontologie\",\"doc_count\":303},{\"key\":\"Generic Dataset\",\"doc_count\":171},{\"key\":\"Mapping\",\"doc_count\":1}]},{\"key\":\"https://pid.bayer.com/kos/19050/containsLicensedData\",\"label\":\"Licensed Data\",\"order\":2,\"buckets\":[{\"key\":\"false\",\"doc_count\":165},{\"key\":\"true\",\"doc_count\":6}]},{\"key\":\"https://pid.bayer.com/kos/19050#hasConsumerGroup\",\"label\":\"Consumer Group\",\"order\":3,\"buckets\":[{\"key\":\"Data Services\",\"doc_count\":245},{\"key\":\"Location 360\",\"doc_count\":162},{\"key\":\"Data Integration\",\"doc_count\":35},{\"key\":\"Customer 360\",\"doc_count\":33}]},{\"key\":\"https://pid.bayer.com/kos/19050/isPersonalData\",\"label\":\"Personal Data\",\"order\":2,\"buckets\":[{\"key\":\"false\",\"doc_count\":108},{\"key\":\"true\",\"doc_count\":63}]},{\"key\":\"https://pid.bayer.com/kos/19050/hasLifecycleStatus\",\"label\":\"Lifecycle Status\",\"order\":5,\"buckets\":[{\"key\":\"Released\",\"doc_count\":321},{\"key\":\"Under Development\",\"doc_count\":119},{\"key\":\"Deprecated\",\"doc_count\":35}]},{\"key\":\"https://pid.bayer.com/kos/19050/hasInformationClassification\",\"label\":\"Information Classification\",\"order\":4,\"buckets\":[{\"key\":\"Restricted\",\"doc_count\":296},{\"key\":\"Internal\",\"doc_count\":158},{\"key\":\"Open\",\"doc_count\":21}]}],\"rangeFilters\":[{\"key\":\"https://pid.bayer.com/kos/19050/lastChangeDateTime\",\"label\":\"Last Change Date and Time\",\"from\":\"2019-01-31T13:16:24.360Z\",\"to\":\"2020-04-03T09:01:54.845Z\"},{\"key\":\"https://pid.bayer.com/kos/19050/dateCreated\",\"label\":\"Date Created\",\"from\":\"2016-12-05T00:00:00.000Z\",\"to\":\"2020-04-02T10:27:56.214Z\"}]}";

            var jsonString2 =
                "{\"aggregations\":[{\"key\":\"http://www.w3.org/1999/02/22-rdf-syntax-ns#type\",\"label\":\"Distribution Endpoint Type\",\"order\":0,\"buckets\":[{\"key\":\"Test\",\"doc_count\":1},{\"key\":\"Generic Dataset\",\"doc_count\":171},{\"key\":\"Mapping\",\"doc_count\":1}]},{\"key\":\"https://pid.bayer.com/kos/19050/containsLicensedData\",\"label\":\"Licensed Data\",\"order\":2,\"buckets\":[{\"key\":\"false\",\"doc_count\":165},{\"key\":\"true\",\"doc_count\":6}]},{\"key\":\"https://pid.bayer.com/kos/19050#hasConsumerGroup\",\"label\":\"Consumer Group\",\"order\":3,\"buckets\":[{\"key\":\"Data Services\",\"doc_count\":245},{\"key\":\"Location 360\",\"doc_count\":162},{\"key\":\"Data Integration\",\"doc_count\":35},{\"key\":\"Customer 360\",\"doc_count\":33}]},{\"key\":\"https://pid.bayer.com/kos/19050/isPersonalData\",\"label\":\"Personal Data\",\"order\":2,\"buckets\":[{\"key\":\"false\",\"doc_count\":108},{\"key\":\"true\",\"doc_count\":63}]},{\"key\":\"https://pid.bayer.com/kos/19050/hasLifecycleStatus\",\"label\":\"Lifecycle Status\",\"order\":5,\"buckets\":[{\"key\":\"Released\",\"doc_count\":321},{\"key\":\"Under Development\",\"doc_count\":119},{\"key\":\"Deprecated\",\"doc_count\":35}]},{\"key\":\"https://pid.bayer.com/kos/19050/hasInformationClassification\",\"label\":\"Information Classification\",\"order\":4,\"buckets\":[{\"key\":\"Restricted\",\"doc_count\":296},{\"key\":\"Internal\",\"doc_count\":158},{\"key\":\"Open\",\"doc_count\":21}]}],\"rangeFilters\":[{\"key\":\"https://pid.bayer.com/kos/19050/lastChangeDateTime\",\"label\":\"Last Change Date and Time\",\"from\":\"2019-01-31T13:16:24.360Z\",\"to\":\"2020-04-03T09:01:54.845Z\"},{\"key\":\"https://pid.bayer.com/kos/19050/dateCreated\",\"label\":\"Date Created\",\"from\":\"2016-12-05T00:00:00.000Z\",\"to\":\"2020-04-02T10:27:56.214Z\"}]}";

            var jsonString3 = "{\"aggregations\": \"none\"}";

            return new List<SearchFilterEditor>()
            {
                 new SearchFilterEditorBuilder()
                    .WithFilterJson(jsonString1)
                    .Build(),

                 new SearchFilterEditorBuilder()
                    .WithFilterJson(jsonString2)
                    .Build(),

                 new SearchFilterEditorBuilder()
                    .WithFilterJson(jsonString3)
                    .Build()
            };
        }

        public static IEnumerable<SearchFilterDataMarketplace> GetPreconfiguredSearchFiltersDataMarketplace()
        {
            var jsonString =
                "{\"aggregations\":[{\"key\":\"http://www.w3.org/1999/02/22-rdf-syntax-ns#type\",\"label\":\"Resource Type\",\"order\":0,\"buckets\":[{\"key\":\"Ontologie\",\"doc_count\":303},{\"key\":\"Generic Dataset\",\"doc_count\":171},{\"key\":\"Mapping\",\"doc_count\":1}]},{\"key\":\"https://pid.bayer.com/kos/19050/containsLicensedData\",\"label\":\"Licensed Data\",\"order\":2,\"buckets\":[{\"key\":\"false\",\"doc_count\":165},{\"key\":\"true\",\"doc_count\":6}]},{\"key\":\"https://pid.bayer.com/kos/19050#hasConsumerGroup\",\"label\":\"Consumer Group\",\"order\":3,\"buckets\":[{\"key\":\"Data Services\",\"doc_count\":245},{\"key\":\"Location 360\",\"doc_count\":162},{\"key\":\"Data Integration\",\"doc_count\":35},{\"key\":\"Customer 360\",\"doc_count\":33}]},{\"key\":\"https://pid.bayer.com/kos/19050/isPersonalData\",\"label\":\"Personal Data\",\"order\":2,\"buckets\":[{\"key\":\"false\",\"doc_count\":108},{\"key\":\"true\",\"doc_count\":63}]},{\"key\":\"https://pid.bayer.com/kos/19050/hasLifecycleStatus\",\"label\":\"Lifecycle Status\",\"order\":5,\"buckets\":[{\"key\":\"Released\",\"doc_count\":321},{\"key\":\"Under Development\",\"doc_count\":119},{\"key\":\"Deprecated\",\"doc_count\":35}]},{\"key\":\"https://pid.bayer.com/kos/19050/hasInformationClassification\",\"label\":\"Information Classification\",\"order\":4,\"buckets\":[{\"key\":\"Restricted\",\"doc_count\":296},{\"key\":\"Internal\",\"doc_count\":158},{\"key\":\"Open\",\"doc_count\":21}]}],\"rangeFilters\":[{\"key\":\"https://pid.bayer.com/kos/19050/lastChangeDateTime\",\"label\":\"Last Change Date and Time\",\"from\":\"2019-01-31T13:16:24.360Z\",\"to\":\"2020-04-03T09:01:54.845Z\"},{\"key\":\"https://pid.bayer.com/kos/19050/dateCreated\",\"label\":\"Date Created\",\"from\":\"2016-12-05T00:00:00.000Z\",\"to\":\"2020-04-02T10:27:56.214Z\"}]}";

            return new List<SearchFilterDataMarketplace>()
            {
                new SearchFilterDataMarketplaceBuilder()
                    .WithName("First Custom DMP filter")
                    .WithFilterJson(jsonString)
                    .Build(),

                new SearchFilterDataMarketplaceBuilder()
                    .WithName("Second custom filter for data-marketplace")
                    .WithFilterJson(jsonString)
                    .Build(),

                new SearchFilterDataMarketplaceBuilder()
                    .WithName("Third custom filter for data-marketplace")
                    .WithFilterJson(jsonString)
                    .Build()
            };
        }

        public static IEnumerable<StoredQuery> GetPreconfiguredStoredQueries()
        {
 
            return new List<StoredQuery>() {
                new StoredQueryBuilder()
                    .WithSearchNumberResult(5)
                    .WithExecutionInterval(ExecutionInterval.Daily)
                    .WithSearchResultHash("testHash")
                    .WithlatestExecutionDate(DateTime.Now)
                    .Build(),

                new StoredQueryBuilder()
                    .WithSearchNumberResult(2)
                    .WithExecutionInterval(ExecutionInterval.Daily)
                    .WithSearchResultHash("testHash2")
                    .WithlatestExecutionDate(DateTime.Now)
                    .Build(),

                new StoredQueryBuilder()
                    .WithSearchNumberResult(1)
                    .WithExecutionInterval(ExecutionInterval.Daily)
                    .WithSearchResultHash("testHash3")
                    .WithlatestExecutionDate(DateTime.Now)
                    .Build(),
            };
        }

        public static IEnumerable<ColidEntrySubscription> GetPreconfiguredColidEntrySubscriptions()
        {
            return new List<ColidEntrySubscription>()
            {
                new ColidEntrySubscriptionBuilder()
                .WithColidEntry(new Uri($"https://pid.bayer.com/kos/19050#{Guid.NewGuid()}"))
                .Build(),

                new ColidEntrySubscriptionBuilder()
                .WithColidEntry(new Uri($"https://pid.bayer.com/kos/19050#{Guid.NewGuid()}"))
                .Build(),

                new ColidEntrySubscriptionBuilder()
                .WithColidEntry(new Uri($"https://pid.bayer.com/kos/19050#{Guid.NewGuid()}"))
                .Build(),

                new ColidEntrySubscriptionBuilder()
                .WithColidEntry(new Uri($"https://pid.bayer.com/kos/19050#{Guid.NewGuid()}"))
                .Build()
            };
        }

        public static IEnumerable<MessageTemplate> GetPreconfiguredMessageTemplates()
        {
            return new List<MessageTemplate>()
            {
                GenerateSampleMessageTemplateForColidEntrySubscription(MessageType.ColidEntrySubscriptionUpdate).WithId(1).Build(),

                GenerateSampleMessageTemplateForColidEntrySubscription(MessageType.ColidEntrySubscriptionDelete).WithId(2).Build(),

                new MessageTemplateBuilder()
                    .WithId(3)
                    .WithType(MessageType.StoredQueryResult)
                    .WithSubject("New results for your stored query `%STORED_QUERY_NAME%` found")
                    .WithBody("The query you stored by the name %STORED_QUERY_NAME% has found new results! Take a look at %STORED_QUERY_LINK%")
                    .Build(),
            };
        }

        public static IEnumerable<MessageConfig> GetPreconfiguredUserMessageConfigs()
        {
            return new List<MessageConfig>()
            {
                new MessageConfigBuilder()
                    .WithSendInterval(SendInterval.Daily)
                    .WithDeleteInterval(DeleteInterval.Monthly)
                    .Build(),

                new MessageConfigBuilder()
                    .WithSendInterval(SendInterval.Daily)
                    .WithDeleteInterval(DeleteInterval.Weekly)
                    .Build(),

                new MessageConfigBuilder()
                    .WithSendInterval(SendInterval.Monthly)
                    .WithDeleteInterval(DeleteInterval.Quarterly)
                    .Build(),

                new MessageConfigBuilder()
                    .WithSendInterval(SendInterval.Immediately)
                    .WithDeleteInterval(DeleteInterval.Quarterly)
                    .Build()
            };
        }

        public static IEnumerable<WelcomeMessage> GetPreconfiguredWelcomeMessages()
        {
            return new List<WelcomeMessage>()
            {
                new WelcomeMessageBuilder()
                    .WithType(ColidType.Editor)
                    .WithContent("<h1>EDITOR POWER!</h1>\n")
                    .Build(),

                new WelcomeMessageBuilder()
                    .WithType(ColidType.DataMarketplace)
                    .WithContent("<h3>Just a data marketplace message</h3>\n")
                    .Build(),
            };
        }

        private static T GetRandomEnum<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(_random.Next(values.Length));
        }

        private static string GetRandomString()
        {
            return Guid.NewGuid().ToString("n").Substring(0, 8);
        }

        private static bool GetRandomBoolean()
        {
            return _random.Next(0, 2) == 0;
        }
    }
}
