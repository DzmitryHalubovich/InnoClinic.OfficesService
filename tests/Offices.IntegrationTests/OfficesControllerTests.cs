using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using Offices.Contracts.DTOs;
using Offices.Contracts.Enums;
using Offices.Domain.Entities;
using Offices.IntegrationTests.Fixture;
using System.Net;
using System.Net.Http.Json;

namespace Offices.IntegrationTests;

public class OfficesControllerTests : IClassFixture<MongoDbFixture>
{
    private readonly MongoDbFixture _fixture;
    private HttpClient _client;

    public OfficesControllerTests(MongoDbFixture fixture) => 
        _fixture = fixture;

    [Fact]
    public async Task GetAllOffices_TryToGetAllOfficesWhen3OfficesInDatabase_OkResult()
    {
        //Arrange
        var preparedListOfOffices = new List<Office>()
        {
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 1",
                Street = "Test street 1",
                HouseNumber = "Test house number 1",
                OfficeNumber = "Test 1",
                RegistryPhoneNumber = "+375112223344",
                IsActive = Status.Active
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 2",
                Street = "Test street 2",
                HouseNumber = "Test house number 2",
                OfficeNumber = "Test 2",
                RegistryPhoneNumber = "+375223334455",
                IsActive = Status.Active
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 3",
                Street = "Test street 3",
                HouseNumber = "Test house number 3",
                OfficeNumber = "Test 3",
                RegistryPhoneNumber = "+375445556677",
                IsActive = Status.Inactive
            }
        };
        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_GetAllOffices_Ok");
        var collection = officesMongoDb.GetCollection<Office>("OfficesTEST");
        collection.InsertMany(preparedListOfOffices);

        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IMongoClient>();
                    services.RemoveAll<IMongoCollection<Office>>();
                    services.AddSingleton<IMongoClient>(_fixture.Client);
                    services.AddSingleton(collection);
                });
            });

        _client = appFactory.CreateClient();

        //Act
        var result = await _client.GetAsync("/api/offices");
        var collectionOffices = await result.Content.ReadFromJsonAsync<List<OfficeShortInfoDTO>>();

        //Assert
        Assert.True(result.IsSuccessStatusCode);
        Assert.True(collectionOffices.Any());
        Assert.Equal(3, collectionOffices.Count);
    }

    [Fact]
    public async Task GetAllOffices_TryToGetAllOfficesWhenDatabaseEmpty_NotFoundResult()
    {
        //Arrange
        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_GetAllOffices_NotFound");
        var collection = officesMongoDb.GetCollection<Office>("OfficesTEST");

        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IMongoClient>();
                    services.RemoveAll<IMongoCollection<Office>>();
                    services.AddSingleton<IMongoClient>(_fixture.Client);
                    services.AddSingleton(collection);
                });
            });

        _client = appFactory.CreateClient();

        //Act
        var result = await _client.GetAsync("/api/offices");

        //Assert
        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task GetOfficesByIds_TryToGetOfficesByCollectionIds_OkResult()
    {
        //Arrange
        var preparedListOfOffices = new List<Office>()
        {
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 1",
                Street = "Test street 1",
                HouseNumber = "Test house number 1",
                OfficeNumber = "Test 1",
                RegistryPhoneNumber = "+375112223344",
                IsActive = Status.Active
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 2",
                Street = "Test street 2",
                HouseNumber = "Test house number 2",
                OfficeNumber = "Test 2",
                RegistryPhoneNumber = "+375223334455",
                IsActive = Status.Active
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 3",
                Street = "Test street 3",
                HouseNumber = "Test house number 3",
                OfficeNumber = "Test 3",
                RegistryPhoneNumber = "+375445556677",
                IsActive = Status.Inactive
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 4",
                Street = "Test street 4",
                HouseNumber = "Test house number 4",
                OfficeNumber = "Test 4",
                RegistryPhoneNumber = "+375445556677",
                IsActive = Status.Inactive
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 5",
                Street = "Test street 5",
                HouseNumber = "Test house number 5",
                OfficeNumber = "Test 5",
                RegistryPhoneNumber = "+375445556677",
                IsActive = Status.Active
            }
        };
        var preparedOfficesIds = string.Join(",", preparedListOfOffices[0].OfficeId, preparedListOfOffices[3].OfficeId);
        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_GetOfficesByIds_Ok");
        var collection = officesMongoDb.GetCollection<Office>("OfficesTEST");
        collection.InsertMany(preparedListOfOffices);

        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IMongoClient>();
                    services.RemoveAll<IMongoCollection<Office>>();
                    services.AddSingleton<IMongoClient>(_fixture.Client);
                    services.AddSingleton(collection);
                });
            });

        _client = appFactory.CreateClient();

        //Act
        var result = await _client.GetAsync($"/api/offices/collection/({preparedOfficesIds})");
        var officeFromResponse = await result.Content.ReadFromJsonAsync<List<OfficeDetailsDTO>>();

        //Assert
        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(2, officeFromResponse.Count);
        Assert.Equal(officeFromResponse[0].OfficeId, preparedListOfOffices[0].OfficeId);
        Assert.Equal(officeFromResponse[1].OfficeId, preparedListOfOffices[3].OfficeId);
    }

    [Fact]
    public async Task GetOfficeById_TryGetOfficeByIdWhenItExistsInDatabase_OkResultWithOffice()
    {
        //Arrange
        var preparedListOfOffices = new List<Office>()
        {
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 1",
                Street = "Test street 1",
                HouseNumber = "Test house number 1",
                OfficeNumber = "Test 1",
                RegistryPhoneNumber = "+375112223344",
                IsActive = Status.Active
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 2",
                Street = "Test street 2",
                HouseNumber = "Test house number 2",
                OfficeNumber = "Test 2",
                RegistryPhoneNumber = "+375223334455",
                IsActive = Status.Active
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 3",
                Street = "Test street 3",
                HouseNumber = "Test house number 3",
                OfficeNumber = "Test 3",
                RegistryPhoneNumber = "+375445556677",
                IsActive = Status.Inactive
            }
        };
        var secondOfficeId = preparedListOfOffices[1].OfficeId;
        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_GetOfficeById_Ok");
        var collection = officesMongoDb.GetCollection<Office>("OfficesTEST");
        collection.InsertMany(preparedListOfOffices);

        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IMongoClient>();
                    services.RemoveAll<IMongoCollection<Office>>();
                    services.AddSingleton<IMongoClient>(_fixture.Client);
                    services.AddSingleton(collection);
                });
            });

        _client = appFactory.CreateClient();

        //Act
        var result = await _client.GetAsync($"/api/offices/{secondOfficeId}");
        var officeFromResponse = await result.Content.ReadFromJsonAsync<OfficeDetailsDTO>();

        //Assert
        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(preparedListOfOffices[1].OfficeId, officeFromResponse.OfficeId);
        Assert.Equal(preparedListOfOffices[1].City, officeFromResponse.City);
        Assert.Equal(preparedListOfOffices[1].Street, officeFromResponse.Street);
        Assert.Equal(preparedListOfOffices[1].OfficeNumber, officeFromResponse.OfficeNumber);
        Assert.Equal(preparedListOfOffices[1].HouseNumber, officeFromResponse.HouseNumber);
        Assert.Equal(preparedListOfOffices[1].RegistryPhoneNumber, officeFromResponse.RegistryPhoneNumber);
        Assert.Equal(preparedListOfOffices[1].IsActive, officeFromResponse.IsActive);
    }

    [Fact]
    public async Task GetOfficeById_TryGetOfficeByIdWhenItDoesntExitsInDatabase_NotFoundResult()
    {
        //Arrange
        var preparedListOfOffices = new List<Office>()
        {
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 1",
                Street = "Test street 1",
                HouseNumber = "Test house number 1",
                OfficeNumber = "Test 1",
                RegistryPhoneNumber = "+375112223344",
                IsActive = Status.Active
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 2",
                Street = "Test street 2",
                HouseNumber = "Test house number 2",
                OfficeNumber = "Test 2",
                RegistryPhoneNumber = "+375223334455",
                IsActive = Status.Active
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 3",
                Street = "Test street 3",
                HouseNumber = "Test house number 3",
                OfficeNumber = "Test 3",
                RegistryPhoneNumber = "+375445556677",
                IsActive = Status.Inactive
            }
        };
        var fakeOfficeId = ObjectId.GenerateNewId();
        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_GetOfficeById_NotFound");
        var collection = officesMongoDb.GetCollection<Office>("OfficesTEST");
        collection.InsertMany(preparedListOfOffices);

        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IMongoClient>();
                    services.RemoveAll<IMongoCollection<Office>>();
                    services.AddSingleton<IMongoClient>(_fixture.Client);
                    services.AddSingleton(collection);
                });
            });

        _client = appFactory.CreateClient();

        //Act
        var result = await _client.GetAsync($"/api/offices/{fakeOfficeId}");

        //Assert
        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task AddOffice_TryAddNewOffice_CreatedResult()
    {
        //Arrange
        var preparedListOfOffices = new List<Office>()
        {
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 1",
                Street = "Test street 1",
                HouseNumber = "Test house number 1",
                OfficeNumber = "Test 1",
                RegistryPhoneNumber = "+375112223344",
                IsActive = Status.Active
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 2",
                Street = "Test street 2",
                HouseNumber = "Test house number 2",
                OfficeNumber = "Test 2",
                RegistryPhoneNumber = "+375223334455",
                IsActive = Status.Active
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 3",
                Street = "Test street 3",
                HouseNumber = "Test house number 3",
                OfficeNumber = "Test 3",
                RegistryPhoneNumber = "+375445556677",
                IsActive = Status.Inactive
            }
        };
        var newOffice = new OfficeCreateDTO(
            PhotoId: null,
            City: "Test new city",
            Street: "Test new street",
            HouseNumber: "Test house",
            OfficeNumber: "Test office",
            RegistryPhoneNumber: "+375112223344",
            IsActive: Status.Active);

        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_AddOffice");
        var collection = officesMongoDb.GetCollection<Office>("OfficesTEST");
        collection.InsertMany(preparedListOfOffices);

        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IMongoClient>();
                    services.RemoveAll<IMongoCollection<Office>>();
                    services.AddSingleton<IMongoClient>(_fixture.Client);
                    services.AddSingleton(collection);
                });
            });

        _client = appFactory.CreateClient();

        //Act
        var result = await _client.PostAsJsonAsync($"/api/offices", newOffice);

        var officeFromDatabase = await collection.FindAsync(x => x.City == newOffice.City && x.Street == newOffice.Street);

        //Assert
        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(officeFromDatabase);
    }

    [Fact]
    public async Task DeleteOffice_TryDeleteOffice_DeleteResult()
    {
        //Arrange
        var preparedListOfOffices = new List<Office>()
        {
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 1",
                Street = "Test street 1",
                HouseNumber = "Test house number 1",
                OfficeNumber = "Test 1",
                RegistryPhoneNumber = "+375112223344",
                IsActive = Status.Active
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 2",
                Street = "Test street 2",
                HouseNumber = "Test house number 2",
                OfficeNumber = "Test 2",
                RegistryPhoneNumber = "+375223334455",
                IsActive = Status.Active
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 3",
                Street = "Test street 3",
                HouseNumber = "Test house number 3",
                OfficeNumber = "Test 3",
                RegistryPhoneNumber = "+375445556677",
                IsActive = Status.Inactive
            }
        };

        var preparedId = preparedListOfOffices[1].OfficeId;

        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_DeleteOffice");
        var collection = officesMongoDb.GetCollection<Office>("OfficesTEST");
        collection.InsertMany(preparedListOfOffices);

        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IMongoClient>();
                    services.RemoveAll<IMongoCollection<Office>>();
                    services.AddSingleton<IMongoClient>(_fixture.Client);
                    services.AddSingleton(collection);
                });
            });

        _client = appFactory.CreateClient();

        //Act
        var result = await _client.DeleteAsync($"/api/offices/{preparedId}");

        var officeFromDatabase = await collection.Find(o => o.OfficeId == preparedId).FirstOrDefaultAsync();
        var officesCollection = await collection.Find(_ => true).ToListAsync();

        //Assert
        Assert.True(result.IsSuccessStatusCode);
        Assert.Null(officeFromDatabase);
        Assert.Equal(2, officesCollection.Count);
    }

    [Fact]
    public async Task UpdateOfficeById_TryUpdateOffice_NoContentResult()
    {
        //Arrange
        var preparedListOfOffices = new List<Office>()
        {
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 1",
                Street = "Test street 1",
                HouseNumber = "Test house number 1",
                OfficeNumber = "Test 1",
                RegistryPhoneNumber = "+375112223344",
                IsActive = Status.Active
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 2",
                Street = "Test street 2",
                HouseNumber = "Test house number 2",
                OfficeNumber = "Test 2",
                RegistryPhoneNumber = "+375223334455",
                IsActive = Status.Active
            },
            new Office()
            {
                OfficeId = ObjectId.GenerateNewId().ToString(),
                PhotoId = null,
                City = "Test city 3",
                Street = "Test street 3",
                HouseNumber = "Test house number 3",
                OfficeNumber = "Test 3",
                RegistryPhoneNumber = "+375445556677",
                IsActive = Status.Inactive
            }
        };
        var preparedId = preparedListOfOffices[1].OfficeId;
        var updatedOfficeModel = new OfficeUpdateDTO(
            PhotoId: null,
            City: "Test updated city",
            Street: "Test updated street",
            HouseNumber: "Test house",
            OfficeNumber: "Test office",
            RegistryPhoneNumber: "+375112223344",
            IsActive: Status.Active);

        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_UpdateOfficeById");
        var collection = officesMongoDb.GetCollection<Office>("OfficesTEST");
        collection.InsertMany(preparedListOfOffices);

        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IMongoClient>();
                    services.RemoveAll<IMongoCollection<Office>>();
                    services.AddSingleton<IMongoClient>(_fixture.Client);
                    services.AddSingleton(collection);
                });
            });

        _client = appFactory.CreateClient();

        //Act
        var result = await _client.PutAsJsonAsync($"/api/offices/{preparedId}", updatedOfficeModel);

        var updatedOfficeFromDatabase = await collection.FindAsync(x => x.City == updatedOfficeModel.City && x.Street == updatedOfficeModel.Street);

        //Assert
        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        Assert.NotNull(updatedOfficeFromDatabase);
    }
}
