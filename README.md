# Blocktrust Credential Badges

## Overview

Blocktrust Credential Badges is a project designed to enhance the display and verification of W3C Verifiable Credentials (VCs) and OpenBadges 3.0 credentials on websites. Our goal is to create a system for embedding these credentials, making them accessible and verifiable for everyone, even without an identity wallet.

## Key Features

1. **Credential Embedding**: A module that enables embedding of W3C VCs and OpenBadges 3.0 credentials into any website.
2. **Verification Infrastructure**: A backend system that supports embedded credentials, ensuring their continued validity and authenticity.
3. **Interactive Verification**: Users can click on embedded credentials to access a detailed verification page.
4. **Credential Issuing Platform**: A platform for issuing Achievement Credentials and managing Endorsement Credential workflows.

## How It Works

The Blocktrust Credential Badges system operates in several steps:

1. **Credential Issuance**: Using our Blazor-based issuing platform, organizations can create and issue verifiable credentials to individuals.

2. **Embedding**: The credential holder receives a JavaScript snippet that can be embedded into their website. This snippet is responsible for displaying the credential badge.

3. **Verification**: When the embedded credential is loaded on a webpage, it makes a call to our backend service to verify its current validity status.

4. **User Interaction**: Visitors to the website can click on the embedded credential to view more details and see a full verification report.


## Project Timeline

The Blocktrust Credential Badges project began in April 2024 and is expected to conclude around December 2024.

## Technology Stack

Our project uses the following technologies:

- Backend: C# with .NET Core
- Issuing Platform Frontend: Blazor
- Embedding Snippets: Native JavaScript and CSS
- Database: PostgreSQL

## Integration with OpenBadges 3.0

We are aligning our project with the OpenBadges 3.0 specification, which is based on the W3C Verifiable Credentials (VC) 2.0 standard. This alignment ensures interoperability with the broader OpenBadges ecosystem.

## Getting Started

(This section will be updated as the project progresses with instructions on how to use the Blocktrust Credential Badges system)


## License

This project is licensed under the Apache 2.0 License - see the [LICENSE.md](link-to-license-file) file for details.

## Contact

For more information about Blocktrust and our projects, visit [https://blocktrust.dev](https://blocktrust.dev).
