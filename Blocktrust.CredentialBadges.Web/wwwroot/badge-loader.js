(function() {
    const domain =   "http://localhost:5159";


    function loadStyles() {
        const link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = `${domain}/credential-badge-style.css`;
        document.head.appendChild(link);

        const fontLink = document.createElement('link');
        fontLink.rel = 'stylesheet';
        fontLink.href = 'https://fonts.googleapis.com/icon?family=Material+Icons';
        document.head.appendChild(fontLink);
    }

    async function fetchAndRenderBadge(element) {
        const credentialId = element.getAttribute('data-id');
        const templateId = element.getAttribute('data-template');
        const theme = templateId.split('_').pop();

        try {
            const response = await fetch(`${domain}/api/GetBadge/${credentialId}/${templateId}/${theme}`);
            if (!response.ok) {
                console.error('Error fetching the credential:', response);
                return;
            }
            const badgeHtml = await response.text();
            element.innerHTML = badgeHtml;
        } catch (error) {
            console.error('Error fetching the credential:', error);
        }
    }

    function init() {
        loadStyles();
        document.querySelectorAll('#blocktrust-badge').forEach(fetchAndRenderBadge);
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();