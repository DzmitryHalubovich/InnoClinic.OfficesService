﻿using Mongo2Go;
using MongoDB.Driver;

namespace Offices.IntegrationTests.Fixture;

public class MongoDbFixture : IDisposable
{
    public MongoDbRunner Runner { get; private set; }
    public MongoClient Client { get; private set; }

    public MongoDbFixture()
    {
        Runner = MongoDbRunner.Start();
        Client = new MongoClient(Runner.ConnectionString);
    }

    public void Dispose()
    {
        Runner.Dispose();
    }
}
