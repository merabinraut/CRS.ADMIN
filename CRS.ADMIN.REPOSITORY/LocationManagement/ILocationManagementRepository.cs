using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.LocationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.LocationManagement
{
    public interface ILocationManagementRepository
    {
        List<LocationCommon> GetLocations(string SearchFilter = "");
        LocationCommon GetLocation(LocationCommon locationCommon);
        CommonDbResponse ManageLocation(LocationCommon locationCommon);
        CommonDbResponse EnableDisableLocation(LocationCommon locationCommon);
    }
}