using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.Offers.CreateOffer;

public class CreateOfferRequest : IRequest<Result<string>>
{
    public Claims Claims { get; set; }
    public string CredentialFormat { get; set; }
    public string IssuingDID { get; set; }
    public string ConnectionId { get; set; }
    
    public string ApiKey { get; set; }
    
    
}

public class Claims
{
    public List<string> Type { get; set; }
    public Achievement Achievement { get; set; }
}

public class Achievement
{
    public string Id { get; set; }
    public List<string> Type { get; set; }
    public string AchievementType { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Criteria Criteria { get; set; }
    public Image Image { get; set; }
}

public class Criteria
{
    public string Type { get; set; }
    public string Narrative { get; set; }
}

public class Image
{
    public string Id { get; set; }
    public string Type { get; set; }
}