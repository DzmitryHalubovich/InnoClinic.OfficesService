using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using Offices.Contracts.Enums;
using Offices.Domain.Entities;
using Offices.Infrastructure.Repositories;
using Offices.IntegrationTests.Fixture;

namespace Offices.IntegrationTests;

public class OfficesRepositoryTests : IClassFixture<MongoDbFixture>
{
    private readonly MongoDbFixture _fixture;
    private OfficesRepository _sut;

    public OfficesRepositoryTests(MongoDbFixture fixture)
    {
        _fixture = fixture;

        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IMongoClient>();
                    services.AddSingleton<IMongoClient>(
                        (_) => _fixture.Client);
                });
            });
    }

    [Fact]
    public async Task GetllOffices_3OfficesInThedatabase_ReturnsExactly3Offices()
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

        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_GetAllOffices");
        var collection = officesMongoDb.GetCollection<Office>("Offices");
        await collection.InsertManyAsync(preparedListOfOffices);

        _sut = new OfficesRepository(collection);

        //Act
        var result = await _sut.GetAllAsync();

        //Assert
        Assert.True(result.Any());
        Assert.IsType<List<Office>>(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(preparedListOfOffices[0].ToJson(), result[0].ToJson());
        Assert.Equal(preparedListOfOffices[1].ToJson(), result[1].ToJson());
        Assert.Equal(preparedListOfOffices[2].ToJson(), result[2].ToJson());
    }

    [Fact]
    public async Task GetCollectionByIdsAsync_3OfficesInThedatabase_ReturnsExactly3Offices()
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
        var preparedListOfOfficesIds = new string[]
        {
            preparedListOfOffices[0].OfficeId,
            preparedListOfOffices[1].OfficeId,
            preparedListOfOffices[4].OfficeId,
        };

        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_GetCollectionByIdsAsync");
        var collection = officesMongoDb.GetCollection<Office>("Offices");
        await collection.InsertManyAsync(preparedListOfOffices);

        _sut = new OfficesRepository(collection);

        //Act
        var result = await _sut.GetCollectionByIdsAsync(preparedListOfOfficesIds);

        //Assert
        Assert.True(result.Any());
        Assert.IsType<List<Office>>(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(preparedListOfOffices[0].ToJson(), result[0].ToJson());
        Assert.Equal(preparedListOfOffices[1].ToJson(), result[1].ToJson());
        Assert.Equal(preparedListOfOffices[4].ToJson(), result[2].ToJson());
    }

    [Fact]
    public async Task GetByIdAsync_ProvideIdOfficeThatExistsInDatabase_ReturnsOfficeWithProvidedId()
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

        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_GetByIdAsync");
        var collection = officesMongoDb.GetCollection<Office>("Offices");
        await collection.InsertManyAsync(preparedListOfOffices);

        _sut = new OfficesRepository(collection);

        //Act
        var result = await _sut.GetByIdAsync(preparedListOfOffices[2].OfficeId);

        //Assert
        Assert.IsType<Office>(result);
        Assert.Equal(preparedListOfOffices[2].ToJson(), result.ToJson());
    }

    [Fact]
    public async Task GetByIdAsync_ProvideIdOfficeThatDoesntExistInDatabase_ReturnsNull()
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

        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_GetByIdAsync2");
        var collection = officesMongoDb.GetCollection<Office>("Offices");
        await collection.InsertManyAsync(preparedListOfOffices);

        _sut = new OfficesRepository(collection);

        //Act
        var result = await _sut.GetByIdAsync(ObjectId.GenerateNewId().ToString());

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddNewAsync_TryAddNewOfficeToCollection_AddOfficeSuccesfully()
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

        var preparedNewOffice = new Office()
        {
            OfficeId = ObjectId.GenerateNewId().ToString(),
            PhotoId = null,
            City = "Test city 6",
            Street = "Test street 6",
            HouseNumber = "Test house number 6",
            OfficeNumber = "Test 6",
            RegistryPhoneNumber = "+375112223344",
            IsActive = Status.Active
        };

        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_AddNewAsync");
        var collection = officesMongoDb.GetCollection<Office>("Offices");
        await collection.InsertManyAsync(preparedListOfOffices);

        _sut = new OfficesRepository(collection);

        //Act
        await _sut.AddNewAsync(preparedNewOffice);
        var allOfficesFromDatabase = await _sut.GetAllAsync();

        //Assert
        Assert.Equal(6 ,allOfficesFromDatabase.Count);
        Assert.Contains(allOfficesFromDatabase, x => x.OfficeId == preparedNewOffice.OfficeId);
    }

    [Fact]
    public async Task DeleteAsync_TryDeleteOfficeFromDatabase_DeleteOfficeSuccesfully()
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
        var firstPreparedOfficeId = preparedListOfOffices.First().OfficeId;

        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_DeleteAsync");
        var collection = officesMongoDb.GetCollection<Office>("Offices");
        await collection.InsertManyAsync(preparedListOfOffices);

        _sut = new OfficesRepository(collection);

        //Act
        await _sut.DeleteAsync(firstPreparedOfficeId);
        var allOfficesFromDatabase = await _sut.GetAllAsync();

        //Assert
        Assert.Equal(4, allOfficesFromDatabase.Count);
        Assert.DoesNotContain(allOfficesFromDatabase, x => x.OfficeId == firstPreparedOfficeId);
    }

    [Fact]
    public async Task UpdateAsync_TryUpdateOffice_OfficeUpdatedSuccesfully()
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
        var idOfficeThatShouldBeUpdated = preparedListOfOffices.First().OfficeId;
        var preparedOfficeForUpdate = new Office
        {
            OfficeId = idOfficeThatShouldBeUpdated,
            PhotoId = null,
            City = "Updated city 1",
            Street = "Updated street 1",
            HouseNumber = "Updated house number 1",
            OfficeNumber = "Updated 1",
            RegistryPhoneNumber = "+375112223344",
            IsActive = Status.Inactive
        };

        var officesMongoDb = _fixture.Client.GetDatabase("OfficesTEST_UpdateAsync");
        var collection = officesMongoDb.GetCollection<Office>("Offices");
        await collection.InsertManyAsync(preparedListOfOffices);

        _sut = new OfficesRepository(collection);

        //Act
        await _sut.UpdateAsync(idOfficeThatShouldBeUpdated, preparedOfficeForUpdate);
        var updatedOffice = await _sut.GetByIdAsync(idOfficeThatShouldBeUpdated);

        //Assert
        Assert.Equal(preparedOfficeForUpdate.ToJson(), updatedOffice.ToJson());
    }
}
