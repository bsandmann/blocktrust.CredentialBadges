document.addEventListener('DOMContentLoaded', function () {
    console.log('DOMContentLoaded event fired');

    // Add card styling to the DOM
    const style = document.createElement('style');
    style.textContent = `
        .credential-card {
            display: block;
            margin: 1em 0;
            border: 1px solid #dee2e6;
            border-radius: 0.25rem;
            background-color: #fff;
            box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
            text-decoration: none;
            color: inherit;
        }
        .credential-card-body {
            padding: 1.25rem;
        }
        .credential-card-title {
            margin-bottom: 0.75rem;
            font-size: 1.25rem;
        }
        .credential-card-text {
            margin-bottom: 1rem;
            color: #6c757d;
        }
        .credential-container{
        img{
        max-width: 100px;
        }
        }
    `;
    document.head.appendChild(style);
    console.log('Style added to head');

    const updateCredentialStatus = async (credentialId) => {
        try {
            console.log('Fetching credential with ID:', credentialId);
            const response = await fetch(`https://credentialbadges.azurewebsites.net/api/VerifyCredential/${credentialId}`);

            if (!response.ok) {
                console.error('Error fetching the credential:', response);
                return;
            }

            const credential = await response.json();
            console.log('Credential data:', credential);

            const statusElement = document.getElementById(`credential-${credentialId}`);
            let statusClass, statusIcon, statusText;

            // Verified,
            //     Revoked,
            //     Expired,
            //     NotDue,
            //     Invalid

            switch (credential.status) {
                case 0:
                    statusClass = "text-success";
                    statusIcon = "bi-check-circle-fill";
                    statusText = "Verified";
                    break;
                case 1:
                    statusClass = "text-danger";
                    statusIcon = "bi-x-circle-fill";
                    statusText = "Revoked";
                    break;
                case 2:
                    statusClass = "text-warning";
                    statusIcon = "bi-exclamation-circle-fill";
                    statusText = "Expired";
                    break;
                case 3:
                    statusClass = "text-primary";
                    statusIcon = "bi-clock-fill";
                    statusText = "NotDue";
                    break;
                case 4:
                    statusClass = "text-danger";
                    statusIcon = "bi-x-circle-fill";
                    statusText = "Invalid";
                    break;
                default:
                    statusClass = "text-muted";
                    statusIcon = "bi-question-circle-fill";
                    statusText = "Invalid";
                    break;
            }

            statusElement.innerHTML = `
                <div id="credential-${credentialId}" data-credential-id="${credentialId}" class="card">
                    <div class="card-body">
                        <h5 class="card-title">${credential.name}</h5>
                        <p class="card-text">${credential.description}</p>
                        ${credential.image ? `<img src="data:image/png;base64,${credential.image}" alt="Credential Image" />` : ''}
                        <p class="${statusClass}">Status: <i class="bi ${statusIcon}"></i> ${statusText}</p>
                    </div>
                </div>
            `;

            console.log('Credential status updated:', statusElement);
        } catch (error) {
            console.error('Error fetching the credential:', error);
        }
    };

    // Automatically update all credential statuses on the page
    document.querySelectorAll('[data-credential-id]').forEach(async element => {
        console.log('Element:', element, element.getAttribute('data-credential-id'));
        const credentialId = element.getAttribute('data-credential-id');
        await updateCredentialStatus(credentialId);
    });

    // Expose the function to the global scope
    window.updateCredentialStatus = updateCredentialStatus;
});
