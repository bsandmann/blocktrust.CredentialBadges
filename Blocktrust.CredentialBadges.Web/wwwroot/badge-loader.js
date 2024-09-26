(function() {
    function loadStyles(domain) {
        // Load custom CSS
        const link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = `${domain}/credential-badge-style.css`;
        document.head.appendChild(link);

        // Load Material Icons font
        const materialIconsLink = document.createElement('link');
        materialIconsLink.rel = 'stylesheet';
        materialIconsLink.href = 'https://fonts.googleapis.com/icon?family=Material+Icons';
        document.head.appendChild(materialIconsLink);

        // Load Material Symbols Outlined font
        const materialSymbolsLink = document.createElement('link');
        materialSymbolsLink.rel = 'stylesheet';
        materialSymbolsLink.href = 'https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@24,300,0,0';
        document.head.appendChild(materialSymbolsLink);
    }

    async function fetchAndRenderBadge(element, domain) {
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

    function getDomain() {
        const scripts = document.getElementsByTagName('script');
        for (let i = scripts.length - 1; i >= 0; i--) {
            const src = scripts[i].src;
            if (src && src.includes('badge-loader.js')) {
                return new URL(src).origin;
            }
        }
        console.error('Unable to determine script domain');
        return '';
    }

    function init() {
        const domain = getDomain();
        if (!domain) return;

        loadStyles(domain);
        document.querySelectorAll('.blocktrust-badge').forEach(element => fetchAndRenderBadge(element, domain));
    }

    // Ensure the script only executes after the page has fully loaded
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();