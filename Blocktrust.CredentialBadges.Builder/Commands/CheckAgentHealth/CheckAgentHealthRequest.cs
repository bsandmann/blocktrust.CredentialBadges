namespace Blocktrust.CredentialBadges.Builder.Commands.CheckAgentHealth;

using FluentResults;
using MediatR;

public class CheckAgentHealthRequest : IRequest<Result<string>>
{
    public CheckAgentHealthRequest(int agentNumber)
    {
        if(agentNumber < 1 || agentNumber > 2)
        {
            throw new ArgumentOutOfRangeException(nameof(agentNumber), "Agent number must be 1 or 2");
        }
        
        AgentNumber = agentNumber; 
    } 
    
    public int AgentNumber { get; set; }
}