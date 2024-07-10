document.addEventListener('DOMContentLoaded', function () {
    console.log('DOMContentLoaded event fired');

    const updateCredentialBadge = async (element) => {
        const credentialId = element.getAttribute('data-credential-id');
        const templateId = element.getAttribute('data-template-id');
        
        const theme = templateId.split('_').pop();
        try {
            console.log('Fetching credential with ID:', credentialId);
            const response = await fetch(`http://localhost:5159/api/GetBadge/${credentialId}/${templateId}/${theme}`);

            if (!response.ok) {
                console.error('Error fetching the credential:', response);
                return;
            }

            const badgeHtml = await response.text();
            console.log('Credential badge HTML:', badgeHtml);

            // Replace the placeholder with the actual badge HTML
            element.outerHTML = badgeHtml;
        } catch (error) {
            console.error('Error fetching the credential:', error);
        }
    };

    // Automatically update all credential badges on the page
    document.querySelectorAll('.credential-badge-placeholder').forEach(async element => {
        console.log('Element:', element, element.getAttribute('data-credential-id'));
        await updateCredentialBadge(element);
    });

    // Expose the function to the global scope
    window.updateCredentialBadge = updateCredentialBadge;
});
