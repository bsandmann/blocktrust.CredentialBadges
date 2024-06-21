document.addEventListener('DOMContentLoaded', function () {
    // Function to apply styles
    function applyStyles() {
        const style = document.createElement('style');
        style.textContent = `
            .credential-card {
                text-decoration: none;
                display: block;
                border: 1px solid #ccc;
                border-radius: 5px;
                padding: 10px;
                box-shadow: 0 0 5px rgba(0, 0, 0, 0.1);
                margin-bottom: 10px;
            }
            .credential-card-title {
                font-size: 1.2rem;
                font-weight: bold;
            }
            .credential-card-text {
                margin-top: 10px;
            }
            .credential-text-success {
                color: green;
            }
            .credential-text-danger {
                color: red;
            }
            .credential-text-warning {
                color: orange;
            }
            .credential-text-info {
                color: blue;
            }
            .credential-text-secondary {
                color: gray;
            }
        `;
        document.head.appendChild(style);
    }

    // Apply styles immediately upon loading
    applyStyles();

    // Function to fetch updated credential status and update the UI
    function updateCredentialStatus(credentialId) {
        // Replace with your API endpoint to fetch updated credential status
        const apiEndpoint = `http://localhost:5159/verifycredential/${credentialId}`;

        fetch(apiEndpoint)
            .then(response => response.json())
            .then(data => {
                const statusElement = document.getElementById(`credential-status-${credentialId}`);
                if (statusElement) {
                    const statusColor = getStatusColor(data.status);
                    const statusIcon = getStatusIcon(data.status);

                    statusElement.className = statusColor;
                    statusElement.innerHTML = `Status: <i class="bi ${statusIcon}"></i> ${data.status}`;
                }
            })
            .catch(error => console.error('Error fetching credential status:', error));
    }

    // Function to get status color based on credential status
    function getStatusColor(status) {
        switch (status) {
            case 'Verified':
                return 'credential-text-success';
            case 'Revoked':
                return 'credential-text-danger';
            case 'Expired':
                return 'credential-text-warning';
            case 'NotDue':
                return 'credential-text-info';
            default:
                return 'credential-text-secondary';
        }
    }

    // Function to get status icon based on credential status
    function getStatusIcon(status) {
        switch (status) {
            case 'Verified':
                return 'bi-check-circle-fill';
            case 'Revoked':
                return 'bi-x-circle-fill';
            case 'Expired':
                return 'bi-exclamation-circle-fill';
            case 'NotDue':
                return 'bi-clock-fill';
            default:
                return 'bi-question-circle-fill';
        }
    }

    // Replace with the actual credential ID
    const credentialId = 'replace-with-actual-credential-id';
    updateCredentialStatus(credentialId);
});
