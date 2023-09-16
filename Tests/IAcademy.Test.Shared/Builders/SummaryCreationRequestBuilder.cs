using Domain.DTO.Summary;

public class SummaryCreationRequestBuilder
{
    private SummaryCreationRequest request;

    public SummaryCreationRequestBuilder() => request = CreateDefault();

    private static SummaryCreationRequest CreateDefault() => new SummaryCreationRequest
    {
        Theme = "DefaultTheme",
        Icon = "DefaultIcon",
        Category = "DefaultCategory",
        Subcategory = "DefaultSubcategory",
        ConfigurationId = "DefaultConfigurationId",
        OwnerId = "DefaultOwnerId"
    };

    public SummaryCreationRequestBuilder WithTheme(string theme)
    {
        request.Theme = theme;
        return this;
    }

    public SummaryCreationRequestBuilder WithCategory(string category)
    {
        request.Category = category;
        return this;
    }

    public SummaryCreationRequestBuilder WithSubcategory(string subcategory)
    {
        request.Subcategory = subcategory;
        return this;
    }

    public SummaryCreationRequestBuilder WithConfigurationId(string configurationId)
    {
        request.ConfigurationId = configurationId;
        return this;
    }

    public SummaryCreationRequestBuilder WithOwnerId(string ownerId)
    {
        request.OwnerId = ownerId;
        return this;
    }

    public SummaryCreationRequest Build() => request;
}
