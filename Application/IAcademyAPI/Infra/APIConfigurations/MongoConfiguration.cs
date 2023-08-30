using MongoDB.Bson.Serialization.Conventions;

namespace IAcademyAPI.Infra.APIConfigurations;

public static class MongoConfiguration
{
    public static void RegisterConfigurations()
    {
        var pack = new ConventionPack()
        {
            new IgnoreExtraElementsConvention(true),
            new CamelCaseElementNameConvention(),
            new EnumRepresentationConvention(MongoDB.Bson.BsonType.String)
        };

        ConventionRegistry.Register("My Solution Conventions", pack, t => true);
    }
}