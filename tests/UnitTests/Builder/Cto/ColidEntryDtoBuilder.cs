using System;
using COLID.AppDataService.Common.DataModels.TransferObjects;

namespace COLID.AppDataService.Tests.Unit.Builder
{
    public class ColidEntryDtoBuilder
    {
        private ColidEntryDto _dto;

        public ColidEntryDtoBuilder()
        {
            _dto = new ColidEntryDto();
        }

        public ColidEntryDto Build()
        {
            return _dto;
        }

        public ColidEntryDtoBuilder WithColidPidUri(Uri value)
        {
            _dto.ColidPidUri = value;
            return this;
        }

        public ColidEntryDtoBuilder WithColidPidUri(string value)
        {
            _dto.ColidPidUri = new Uri(value);
            return this;
        }

        public ColidEntryDtoBuilder WithLabel(string value)
        {
            _dto.Label = value;
            return this;
        }
    }
}
