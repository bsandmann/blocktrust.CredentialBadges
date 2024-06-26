namespace Blocktrust.CredentialBadges.Web.Tests;

using Xunit;

[CollectionDefinition("TransactionalTests", DisableParallelization = true)]
public class TransactionalTestsCollection : ICollectionFixture<TransactionalTestDatabaseFixture>
{
}