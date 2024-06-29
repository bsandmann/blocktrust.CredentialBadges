# OpenBadges: History and Evolution

We spent some time investigating different options on how Credential Badges are displayed on websites. The main other option we identified is the OpenBadge system. OpenBadges have been around since about 2011 when the OpenBadge specification was first released. The goal of OpenBadges was to create a standardized way to represent and verify achievements and skills in the digital world.

## Origin and Early Development

The [OpenBadges project](https://openbadges.org/) was initially conceived by Mozilla. Their aim was to develop a new way of recognizing and validating learning, particularly in informal and non-traditional educational contexts. The idea was to create a system that could acknowledge skills and achievements that might not be captured by traditional academic credentials.

Key [milestones](https://www.1edtech.org/blog/reflecting-on-the-open-badges-journey) in the early development of OpenBadges include:

- 2011: Initial release of the OpenBadges 1.0 specification
- 2013: Mozilla launches the Mozilla Backpack, a platform for learners to collect and display their badges
- 2016: IMS Global Learning Consortium takes over the management of the OpenBadges specification
- 2018: Release of OpenBadges 2.0 specification

## OpenBadges 1.0 and 2.0

Both OpenBadges 1.0 and 2.0 relied on a server-client model for badge issuance and verification. OpenBadges 2.0, released in 2018, built upon the foundation of 1.0 and introduced several improvements, but the core principle remained the same:

1. An issuer creates a badge and hosts its metadata on their server.
2. The badge is awarded to a recipient, who can then store it in a "backpack" or display it on various platforms.
3. When the badge is displayed or shared, the viewer can verify its authenticity by checking with the issuer's server.

While this initially sounds similar to the idea behind this proposal, it is a server-client model at its core and depends on the availability of the company running the services. This system worked well for many use cases but had some limitations, particularly in terms of decentralization and long-term badge validity.

## Transition to OpenBadges 3.0

The development of OpenBadges 3.0 marks a significant shift in the approach to digital credentials. The decision to adopt the W3C Verifiable Credentials (VC) 2.0 standard instead of the previous server-client model was driven by several factors:

1. **Decentralization**: The VC model allows for a more decentralized approach to credential issuance and verification, reducing reliance on centralized servers.

2. **Interoperability**: By aligning with W3C standards, OpenBadges 3.0 aims to improve interoperability with other digital credential systems.

3. **Privacy and Control**: The VC model gives credential holders more control over their data and how it's shared.

4. **Long-term Validity**: Verifiable Credentials can be designed to remain valid and verifiable even if the issuer's systems are no longer available.

5. **Extensibility**: The VC data model is highly flexible and extensible, allowing for a wide range of credential types and use cases.


## Alternatives to OpenBadges

While OpenBadges has emerged as a leading standard in the digital credentials space, there are other projects and approaches, particularly in the web3 world. Some notable alternatives include:
- Noox: A blockchain-based platform for creating and managing digital credentials. [LINK](https://noox.world/)
- Nuri Chain: A solution that leverages blockchain technology for credential issuance and verification. [LINK](https://nurichain.com/en )
- POAP (Proof of Attendance Protocol): A system that uses NFTs to create and distribute proof of attendance for events. [LINK](https://poap.xyz/about-the-protocol )

These alternatives often use NFT-based approaches and are more focused on niche applications, proprietary ecosystem or find it hard to get trackion outside the blockchain community. OpenBadges, on the other hand, has a long history and a broad user base, making it a more established choice for many organizations.

## Why OpenBadges 3.0?
After evaluating different solutions, we decided to focus on achieving compatibility with OpenBadges 3.0 for several reasons:

- Established Ecosystem: OpenBadges is backed by the [1EDTech](https://www.1edtech.org/) consortium (formerly IMS Global), which includes dozens of companies and hundreds of universities involved in learner badges.
- Real-world Adoption: Many well-known education-solution-vendors are already issuing OpenBadges 2.0 badges and are in the process for migrating to OpenBadges 3.0. Some examples include:
  - Moodle (https://moodle.com/)
  - Credly (https://info.credly.com/)
  - Udemy (https://www.udemy.com/)
- OpenBadges 3.0's adoption of the W3C Verifiable Credentials standard aligns well with Self-Sovereign Identity (SSI) principles, which is a key focus of our project.
- Prestigious institutions like MIT, through projects such as the [Digital Credential Consortium (DCC)](https://digitalcredentials.mit.edu/), are actively involved in developing and promoting OpenBadges 3.0.

- Unlike some proprietary solutions, OpenBadges is an open standard that promotes interoperability and widespread adoption, independent of any single vendor or platform or ecosystem (like a specific blockchain).
- OpenBadges 3.0's adoption of the W3C Verifiable Credentials standard aligns well with Self-Sovereign Identity (SSI) principles, which is a key focus of our project.
- Prestigious institutions like MIT, through projects such as the Digital Credential Consortium (DCC), are actively involved in developing and promoting OpenBadges 3.0.

By focusing on compatibility with OpenBadges 3.0, we aim to leverage an existing ecosystem with real-life traction and well-known participants, rather than pushing our own incompatible solution just for the sake of the initial proposal. This approach allows us to contribute to and benefit from a growing standard while still innovating in areas not yet addressed by existing projects in the OpenBadges (3.0) community.

## Compatibility with the Credential-Badges Proposal

The current specification of OpenBadges 3.0 can be found [here](https://www.imsglobal.org/spec/ob/v3p0). In essence, it builds upon the [W3C VC 2.0 specification](https://w3c.github.io/vc-data-model/) and extends the "CredentialSubject" property of the data model, with only minor expected fields outside that property. This makes it generally compatible with any major SSI solution vendor, including Hyperledger Identus (formerly Atala PRISM) that this project (and proposal) is based on.

However, the devil is in the details, and as of now, the current implementation of the Identus agent is not capable of creating credentials fully aligned with the OpenBadges 3.0 specification. These issues are minor and can be addressed in principle:

1. The JSON-LD property of the W3C VCs produced by Identus cannot be modified. We plan to submit a feature request over the coming weeks to address this issue.

2. Type: The Type properties of the VC produced by Identus are currently fixed on "[VerifiableCredential]" but should be configurable through the API. We've already discussed this and will also submit a feature request.

3. Issuer: The issuer property of VCs created by Identus just contains the DID-PRISM of the issuer. The possibility to use an object instead of a simple string with additional fields like "name" or "image" is allowed by the W3C spec and highly recommended by the OpenBadges 3.0 spec, but not required. For specific features, it is highly useful nonetheless.

4. Data Model v1 / v2: The current data model supported by Identus is 1.1. The spec for 2.0 has already been signed off, and the OpenBadges spec is also building on 2.0. While the change is not big and the migration is rather easy for Identus, this is a definite requirement the Identus team has to fulfill. Since this is an obvious change, we expect this to happen in the near future.

5. DID Method: The current DID Method used by Identus is exclusively [DID-PRISM](https://github.com/input-output-hk/prism-did-method-spec/blob/main/w3c-spec/PRISM-method.md). While the Identus team plans to extend the possibility to add other DID methods, this has not happened yet. The OpenBadges specification is agnostic to the used DID Method but favors DID-key and DID-web for ease of use. At the current state, we don't expect other participants of the OpenBadge ecosystem to use DID-PRISM, so to make Credentials fully compatible, we currently have three options:
   a. Wait until Identus has support for DID-key or DID-web
   b. Implement a DID-keys / DID-web issuing workflow ourselves and integrate it into the application
   c. Extend the DID-PRISM support to other projects through pull requests

All of these issues can be addressed. However, we don't expect full compatibility in the next few weeks or even months. It is very likely that we will be able to integrate with the rest of the OpenBadges ecosystem by the end of the proposal and demonstrate our project working flawlessly with Credentials from other ecosystems.

Currently, we are mostly looking at the open-source initiative by the [Digital Credential Consortium (DCC)](https://digitalcredentials.mit.edu/) within MIT, which is leading the work on OpenBadge 3.0 Credentials together with other universities around the world. The team around the DCC is heavily integrated into the SSI community and has already developed example Wallets and an Issuing platform. This means we might be able to leverage some of their codebase to drive our demos and show interoperability. For example, we could issue a W3C Credential with their tools (apart from Identus) and embed such a Credential into a website using our work.

## Architectural Evaluation

While we currently don't think a change request is necessary, we were able to get a much better understanding of the current landscape of similar projects like the one proposed here. This includes, above all, the development of the OpenBadge 3.0 specification and projects created around that over the last few months.

Since OpenBadges 3.0 is – like our project – based on W3C Verifiable Credentials, tight integration is very beneficial, especially because our proposal brings some new aspects to the table which are not available with the existing OpenBadges projects – i.e., the embedding of Credentials into a website. In the OpenBadges ecosystem, the first wallets and issuing systems are being developed – things we might be able to do with Identus and even with our proposal. However, having this infrastructure already provided, it makes sense for us not to build what is already there but instead focus on the new value proposition we bring to the table.

In terms of architecture and deliverables for the proposal, we think of therefore focusing our efforts more on that, rather than the issuer platform we set out to develop in the proposal. We'll still do that (mostly for demo purposes), but will likely focus on getting other parts into a more presentable and feature-rich state which are not yet existing in other projects.
