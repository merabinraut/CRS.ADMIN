using CRS.ADMIN.REPOSITORY.LocationManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.LocationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.LocationManagement
{
    public class LocationManagementBusiness : ILocationManagementBusiness
    {
        private readonly ILocationManagementRepository _repository;

        public LocationManagementBusiness(LocationManagementRepository repository) => this._repository = repository;

        public List<LocationCommon> GetLocations(string SearchFilter = "")
        {
            return _repository.GetLocations(SearchFilter);
        }

        public LocationCommon GetLocation(LocationCommon locationCommon)
        {
            return _repository.GetLocation(locationCommon);
        }

        public CommonDbResponse ManageLocation(LocationCommon locationCommon)
        {
            return _repository.ManageLocation(locationCommon);
        }
        public CommonDbResponse EnableDisableLocation(LocationCommon locationCommon)
        {
            return _repository.EnableDisableLocation(locationCommon);
        }
    }
}