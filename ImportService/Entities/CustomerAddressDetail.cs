using PointofSaleModels.DatabaseModels;

namespace ImportService.Entities;

public partial class CustomerAddressDetail
{
    public int CustomerAddressId { get; set; }

    public bool IsPrimary { get; set; }

    public int? AddressTypeId { get; set; }

    public int CompanyId { get; set; }

    public int CityId { get; set; }

    public int AreaId { get; set; }

    public string? LandMark { get; set; }

    public string? CompanyName { get; set; }

    public string? Building { get; set; }

    public string? RoomHouse { get; set; }

    public string? BlockFloor { get; set; }

    public string? StreetRowLane { get; set; }

    public int? RoomHouseCaptionId { get; set; }

    public int? BlockFloorCaptionId { get; set; }

    public int? StreetRowLaneCaptionId { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int? PhoneId { get; set; }

    public int? CaptionId { get; set; }

    public string? CompleteAddress { get; set; }
}
