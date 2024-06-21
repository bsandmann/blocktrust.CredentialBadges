// This script fetches the status of a credential from the API and updates the status on the page.
document.addEventListener('DOMContentLoaded', function () {
    const updateCredentialStatus = async (credentialId) => {
        try {
            const response = await fetch(`https://localhost:7277/api/verify/${credentialId}`);
            if (!response.ok) {
                console.error('Error fetching the credential:', response);
            }
            const credential = await response.json();
            const statusElement = document.getElementById(`credential-status-${credentialId}`);
            let statusClass, statusIcon;

            switch (credential.status) {
                case "Verified":
                    statusClass = "text-success";
                    statusIcon = "bi-check-circle-fill";
                    break;
                case "Revoked":
                    statusClass = "text-danger";
                    statusIcon = "bi-x-circle-fill";
                    break;
                case "Expired":
                    statusClass = "text-warning";
                    statusIcon = "bi-exclamation-circle-fill";
                    break;
                case "NotDue":
                    statusClass = "text-primary";
                    statusIcon = "bi-clock-fill";
                    break;
                default:
                    statusClass = "text-muted";
                    statusIcon = "bi-question-circle-fill";
                    break;
            }

            statusElement.innerHTML = `
                <div id="credential-${credentialId}" data-credential-id="${credentialId}" class="card">
                    <div class="card-body">
                        <h5 class="card-title">${credential.name}</h5>
                        <p class="card-text">${credential.description}</p>
                        <p class="${statusClass}">Status: <i class="bi ${statusIcon}"></i> ${credential.status}</p>
                    </div>
                </div>
            `;
        } catch (error) {
            console.error('Error fetching the credential:', error);
        }
    };

    // Automatically update all credential statuses on the page
    document.querySelectorAll('[data-credential-id]').forEach(async element => {
        const credentialId = element.getAttribute('data-credential-id');
       await updateCredentialStatus(credentialId);
    });

    // Expose the function to the global scope
    window.updateCredentialStatus = updateCredentialStatus;
});
