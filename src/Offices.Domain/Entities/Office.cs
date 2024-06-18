using MongoDB.Bson.Serialization.Attributes;
using Offices.Contracts.Enums;

namespace Offices.Domain.Entities;

public class Office
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string OfficeId { get; set; }

    [BsonElement("photoId")]
    public string? PhotoId { get; set; }

    [BsonElement("city")]
    public string City { get; set; } = null!;

    [BsonElement("street")]
    public string Street { get; set; } = null!;

    [BsonElement("houseNumber")]
    public string HouseNumber { get; set; } = null!;

    [BsonElement("officeNumber")]
    public string? OfficeNumber { get; set; }

    [BsonElement("registryPhoneNumber")]
    public string RegistryPhoneNumber { get; set; } = null!;

    [BsonElement("isActive")]
    public Status IsActive { get; set; }
}
