using MongoDB.Bson.Serialization.Attributes;
using Offices.Contracts.Enums;

namespace Offices.Domain.Entities;

public class Office
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string OfficeId { get; set; }

    [BsonElement("officePhotoUrl")]
    public string? OfficePhotoUrl { get; set; }

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

    [BsonIgnore]
    public string OfficeAddress => 
        string.Join(" ",new string[] { City, Street, HouseNumber, OfficeNumber }
              .Where(x => x is not null));
}
