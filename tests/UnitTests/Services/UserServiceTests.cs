using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Repositories.Interface;
using COLID.AppDataService.Services.Implementation;
using COLID.AppDataService.Services.Interface;
using COLID.AppDataService.Common.Search;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Newtonsoft.Json.Linq;
using COLID.AppDataService.Tests.Unit;
using System.Linq;

namespace UnitTests.Services
{
    [ExcludeFromCodeCoverage]
    public class UserServiceTests
    {
        private readonly Mock<IRemoteSearchService> _mockRemoteSearchService;
        private readonly Mock<IConsumerGroupService> _mockConsumerGroupService;
        private readonly Mock<IMapper> _mockmapper;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly Mock<IColidEntrySubscriptionService> _mockColidEntrySubscriptionService;
        private readonly Mock<IMessageTemplateService> _mockMessageTemplateService;
        private readonly Mock<IGenericRepository> _repo;
        private readonly IUserService _service;

        public UserServiceTests()
        {
            _mockRemoteSearchService = new Mock<IRemoteSearchService>();
            _mockConsumerGroupService = new Mock<IConsumerGroupService>();
            _mockmapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _mockColidEntrySubscriptionService = new Mock<IColidEntrySubscriptionService>();
            _mockMessageTemplateService = new Mock<IMessageTemplateService>();
            _repo = new Mock<IGenericRepository>();
            _service = new UserService(
                _repo.Object,
                _mockConsumerGroupService.Object,
                _mockRemoteSearchService.Object,
                _mockColidEntrySubscriptionService.Object,
                _mockMessageTemplateService.Object,
                _mockmapper.Object,
                _mockLogger.Object
               );
            SetupSearchService(new List<SearchHit>());
        }

        private void SetupSearchService(List<SearchHit> SearchHitList)
        {
            var Hits = new HitDTO()
            {
                Total = SearchHitList.Count,
                Hits = SearchHitList
            };
            var searchResult = new SearchResultDTO("testForProcessingStoredQueries",null,Hits);
            _mockRemoteSearchService.Setup(mock => mock.Search(It.IsAny<SearchRequestDto>())).Returns(Task.FromResult(searchResult));
        }
        #region Process Storedqueries
        [Fact]
        public void StoredQueryNeedsToBeEvaluated_Returns_True_IfItNeedsToBeEvaluated()
        {
            var storedQuery_Daily = new StoredQuery() { ExecutionInterval = ExecutionInterval.Daily, LatestExecutionDate = DateTime.Now.AddDays(-1) };
            var storedQuery_weekly = new StoredQuery() { ExecutionInterval = ExecutionInterval.Weekly, LatestExecutionDate = DateTime.Now.AddDays(-7) };
            var storedQuery_monthly = new StoredQuery() { ExecutionInterval = ExecutionInterval.Monthly, LatestExecutionDate = DateTime.Now.AddMonths(-1) };

            var resultSq1 = _service.StoredQueryNeedsToBeEvaluated(storedQuery_Daily);
            var resultSq2 = _service.StoredQueryNeedsToBeEvaluated(storedQuery_weekly);
            var resultSq3 = _service.StoredQueryNeedsToBeEvaluated(storedQuery_monthly);

            Assert.True(resultSq1);
            Assert.True(resultSq2);
            Assert.True(resultSq3);
        }
        [Fact]
        public void StoredQueryNeedsToBeEvaluated_Returns_False_IfItDoesntNeedToBeEvaluated()
        {
            var storedQuery_Daily = new StoredQuery() { ExecutionInterval = ExecutionInterval.Daily, LatestExecutionDate = DateTime.Now };
            var storedQuery_weekly = new StoredQuery() { ExecutionInterval = ExecutionInterval.Weekly, LatestExecutionDate = DateTime.Now.AddDays(-6) };
            var storedQuery_monthly = new StoredQuery() { ExecutionInterval = ExecutionInterval.Monthly, LatestExecutionDate = DateTime.Now.AddDays(-15) };

            var resultSq1 = _service.StoredQueryNeedsToBeEvaluated(storedQuery_Daily);
            var resultSq2 = _service.StoredQueryNeedsToBeEvaluated(storedQuery_weekly);
            var resultSq3 = _service.StoredQueryNeedsToBeEvaluated(storedQuery_monthly);

            Assert.False(resultSq1);
            Assert.False(resultSq2);
            Assert.False(resultSq3);
        }
        [Fact]
        public void StoredQueryNeedsToBeEvaluated_Returns_ArgumentNullException_IfStoredQueryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _service.StoredQueryNeedsToBeEvaluated(null));
        }
        [Fact]
        public void GetUpdatedResources_Returns_UpdatedResoures_IfSearchResultChanged()
        {
            var storedQuery = new StoredQuery() { ExecutionInterval = ExecutionInterval.Daily, LatestExecutionDate = new DateTime(2021, 2, 5)};
            var searchList = new List<SearchHit>();

            SearchHit SearchHit = new SearchHit();
            SearchHit SearchHit2 = new SearchHit();
            SearchHit.Source.Add("http://pid.bayer.com/kos/19014/hasPID", JObject.Parse("{ \"outbound\": [ { \"value\": \"https://dev-pid.bayer.com/kos/test1\", \"uri\": \"https://dev-pid.bayer.com/kos/test1\", \"edge\": \"http://pid.bayer.com/kos/19014/hasPID\" } ], \"inbound\": [] }"));
            SearchHit.Source.Add("https://pid.bayer.com/kos/19050/lastChangeDateTime", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-01T09:07:25.327Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
            SearchHit.Source.Add("https://pid.bayer.com/kos/19050/dateCreated", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-01T09:06:30.452Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
           
            SearchHit2.Source.Add("http://pid.bayer.com/kos/19014/hasPID", JObject.Parse("{ \"outbound\": [ { \"value\": \"https://dev-pid.bayer.com/kos/test2\", \"uri\": \"https://dev-pid.bayer.com/kos/test2\", \"edge\": \"http://pid.bayer.com/kos/19014/hasPID\" } ], \"inbound\": [] }"));
            SearchHit2.Source.Add("https://pid.bayer.com/kos/19050/lastChangeDateTime", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-08T09:07:25.327Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
            SearchHit2.Source.Add("https://pid.bayer.com/kos/19050/dateCreated", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-08T09:06:30.452Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
            searchList.Add(SearchHit);
            searchList.Add(SearchHit2);
            List<string> resultPidUris = _service.GetUpdatedResources(searchList, storedQuery);
            var resultResourcePid = resultPidUris.ToArray()[0];

            Assert.NotNull(resultPidUris);
            Assert.NotEmpty(resultPidUris);
            Assert.Single(resultPidUris);
            Assert.Equal("https://dev-pid.bayer.com/kos/test2", resultResourcePid);

        }
        [Fact]
        public void GetUpdatedResources_Returns_EmptyList_IfSearchResultsDidntChange()
        {
            var storedQuery = new StoredQuery() { ExecutionInterval = ExecutionInterval.Daily, LatestExecutionDate = new DateTime(2021, 2, 5) };
            var searchList = new List<SearchHit>();

            SearchHit SearchHit = new SearchHit();
            SearchHit SearchHit2 = new SearchHit();
            SearchHit.Source.Add("http://pid.bayer.com/kos/19014/hasPID", JObject.Parse("{ \"outbound\": [ { \"value\": \"https://dev-pid.bayer.com/kos/test1\", \"uri\": \"https://dev-pid.bayer.com/kos/test1\", \"edge\": \"http://pid.bayer.com/kos/19014/hasPID\" } ], \"inbound\": [] }"));
            SearchHit.Source.Add("https://pid.bayer.com/kos/19050/lastChangeDateTime", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-01T09:07:25.327Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
            SearchHit.Source.Add("https://pid.bayer.com/kos/19050/dateCreated", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-01T09:06:30.452Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));

            SearchHit2.Source.Add("http://pid.bayer.com/kos/19014/hasPID", JObject.Parse("{ \"outbound\": [ { \"value\": \"https://dev-pid.bayer.com/kos/test2\", \"uri\": \"https://dev-pid.bayer.com/kos/test2\", \"edge\": \"http://pid.bayer.com/kos/19014/hasPID\" } ], \"inbound\": [] }"));
            SearchHit2.Source.Add("https://pid.bayer.com/kos/19050/lastChangeDateTime", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-03T09:07:25.327Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
            SearchHit2.Source.Add("https://pid.bayer.com/kos/19050/dateCreated", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-03T09:06:30.452Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
            searchList.Add(SearchHit);
            searchList.Add(SearchHit2);
            List<string> resultPidUris = _service.GetUpdatedResources(searchList, storedQuery);
            
            Assert.NotNull(resultPidUris);
            Assert.Empty(resultPidUris);
        }
        [Fact]
        public void GetUpdatedResources_Returns_ArgumentNullException_IfSavedSearchOrStoredQueryNull()
        {
            Assert.Throws<ArgumentNullException>(() => _service.GetUpdatedResources(null, null));
        }

        [Fact]
        public void GetElasticSearchResult_Calls_ElasticSearch_When_SearchFilterNotEmpty()
        {
            var user = TestData.GenerateRandomUserWithSavedSearchAndStoredQuery();
            var result = _service.GetElasticSearchResult(user.SearchFiltersDataMarketplace.FirstOrDefault());
            _mockRemoteSearchService.Verify(mock => mock.Search(It.IsAny<SearchRequestDto>()), Times.Once());
        }

        [Fact]
        public void GetElasticSearchResult_Calls_ElasticSearch_When_SearchFilterEmpty()
        {
            var sf = new SearchFilterDataMarketplace();
            var result = _service.GetElasticSearchResult(sf);
            _mockRemoteSearchService.Verify(mock => mock.Search(It.IsAny<SearchRequestDto>()), Times.Once());
        }

        [Fact]
        public async Task GetElasticSearchResult_Throws_ArgumentNullException_When_SearchFilterEmpty()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetElasticSearchResult(null));
        }

        [Fact]
        public void GetHashOfSearchResults_Returns_ArgumentNullException_WhenSearchResultIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _service.GetHashOfSearchResults(null));
        }

        [Fact]
        public void GetHashOfSearchResults_Returns_hash_WhenSearchResultIsEmpty()
        {
            var SearchHitList = new List<SearchHit>();
           
            var Hits = new HitDTO()
            {
                Total = SearchHitList.Count,
                Hits = SearchHitList
            };
            var searchResult = new SearchResultDTO("testForProcessingStoredQueries", null, Hits);

            string computedHash = _service.GetHashOfSearchResults(searchResult);
            Assert.NotEmpty(computedHash);
        }

        [Fact]
        public void GetHashOfSearchResults_Returns_Same_Hash_WhenCallingForSameSearchResult()
        {
            var SearchHitList = new List<SearchHit>();
            var SearchHit = new SearchHit();
            var SearchHit2 = new SearchHit();
            SearchHit.Source.Add("http://pid.bayer.com/kos/19014/hasPID", JObject.Parse("{ \"outbound\": [ { \"value\": \"https://dev-pid.bayer.com/kos/test1\", \"uri\": \"https://dev-pid.bayer.com/kos/test1\", \"edge\": \"http://pid.bayer.com/kos/19014/hasPID\" } ], \"inbound\": [] }"));
            SearchHit.Source.Add("https://pid.bayer.com/kos/19050/lastChangeDateTime", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-01T09:07:25.327Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
            SearchHit.Source.Add("https://pid.bayer.com/kos/19050/dateCreated", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-01T09:06:30.452Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));

            SearchHit2.Source.Add("http://pid.bayer.com/kos/19014/hasPID", JObject.Parse("{ \"outbound\": [ { \"value\": \"https://dev-pid.bayer.com/kos/test2\", \"uri\": \"https://dev-pid.bayer.com/kos/test2\", \"edge\": \"http://pid.bayer.com/kos/19014/hasPID\" } ], \"inbound\": [] }"));
            SearchHit2.Source.Add("https://pid.bayer.com/kos/19050/lastChangeDateTime", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-08T09:07:25.327Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
            SearchHit2.Source.Add("https://pid.bayer.com/kos/19050/dateCreated", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-08T09:06:30.452Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
            SearchHitList.Add(SearchHit);
            SearchHitList.Add(SearchHit2);
            var Hits = new HitDTO()
            {
                Total = SearchHitList.Count,
                Hits = SearchHitList
            };
            var searchResult = new SearchResultDTO("testForProcessingStoredQueries", null, Hits);

            string computedHash = _service.GetHashOfSearchResults(searchResult);
            string computedHash2 = _service.GetHashOfSearchResults(searchResult);
            Assert.NotEmpty(computedHash);
            Assert.NotEmpty(computedHash2);
            Assert.Equal(computedHash, computedHash2);
        }
        [Fact]
        public void GetHashOfSearchResults_Returns_Different_Hashes_WhenCallingForDifferentSearchResults()
        {
            var SearchHitList = new List<SearchHit>();
            var SearchHit = new SearchHit();
            var SearchHit2 = new SearchHit();
            SearchHit.Source.Add("http://pid.bayer.com/kos/19014/hasPID", JObject.Parse("{ \"outbound\": [ { \"value\": \"https://dev-pid.bayer.com/kos/test1\", \"uri\": \"https://dev-pid.bayer.com/kos/test1\", \"edge\": \"http://pid.bayer.com/kos/19014/hasPID\" } ], \"inbound\": [] }"));
            SearchHit.Source.Add("https://pid.bayer.com/kos/19050/lastChangeDateTime", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-01T09:07:25.327Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
            SearchHit.Source.Add("https://pid.bayer.com/kos/19050/dateCreated", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-01T09:06:30.452Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));

            SearchHit2.Source.Add("http://pid.bayer.com/kos/19014/hasPID", JObject.Parse("{ \"outbound\": [ { \"value\": \"https://dev-pid.bayer.com/kos/test2\", \"uri\": \"https://dev-pid.bayer.com/kos/test2\", \"edge\": \"http://pid.bayer.com/kos/19014/hasPID\" } ], \"inbound\": [] }"));
            SearchHit2.Source.Add("https://pid.bayer.com/kos/19050/lastChangeDateTime", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-08T09:07:25.327Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
            SearchHit2.Source.Add("https://pid.bayer.com/kos/19050/dateCreated", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-08T09:06:30.452Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
            SearchHitList.Add(SearchHit);
            SearchHitList.Add(SearchHit2);
            var Hits = new HitDTO()
            {
                Total = SearchHitList.Count,
                Hits = SearchHitList
            };
            var searchResult = new SearchResultDTO("testForProcessingStoredQueries", null, Hits);
            string computedHash = _service.GetHashOfSearchResults(searchResult);

            //Adding a new Hit to the SearchResult List
            var SearchHit3 = new SearchHit();
            SearchHit3.Source.Add("http://pid.bayer.com/kos/19014/hasPID", JObject.Parse("{ \"outbound\": [ { \"value\": \"https://dev-pid.bayer.com/kos/test3\", \"uri\": \"https://dev-pid.bayer.com/kos/test3\", \"edge\": \"http://pid.bayer.com/kos/19014/hasPID\" } ], \"inbound\": [] }"));
            SearchHit3.Source.Add("https://pid.bayer.com/kos/19050/lastChangeDateTime", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-09T09:07:25.327Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
            SearchHit3.Source.Add("https://pid.bayer.com/kos/19050/dateCreated", JObject.Parse("{ \"outbound\": [ { \"value\": \"2021-02-03T09:06:30.452Z\", \"uri\": null, \"edge\": null } ], \"inbound\": [] }"));
            searchResult.Hits.Hits.Add(SearchHit3);

            //Calculate Hash with updated  
            string computedHash2 = _service.GetHashOfSearchResults(searchResult);
            
            
            Assert.NotEmpty(computedHash);
            Assert.NotEmpty(computedHash2);
            Assert.NotEqual(computedHash, computedHash2);
        }
        [Fact]
        public async Task NotifyUserAboutUpdates_Returns_ArgumentNullException_IfSavedSearchOrNewPidListIsNull()
        {
           await Assert.ThrowsAsync<ArgumentNullException>(() => _service.NotifyUserAboutUpdates(null, null));
        }

        #endregion


    }
}
