window.clipboardCopy = {
    copyText: function(text) {
        navigator.clipboard.writeText(text).then(function () {
        })
            .catch(function (error) {
                alert(error);
            });
    }
};

window.focusElement = (element) => {
    var elementToFocus=document.getElementById(element);
    elementToFocus.focus();
}



window.localStorageFunctions = {
    setItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    getItem: function (key) {
        return localStorage.getItem(key);
    }
};

window.resizeImage = async (buffer, width, height) => {
    let blob = new Blob([buffer]);
    let img = document.createElement('img');
    img.src = URL.createObjectURL(blob);

    await new Promise(resolve => img.onload = resolve);

    let canvas = document.createElement('canvas');
    let ctx = canvas.getContext('2d');

    canvas.width = width;
    canvas.height = height;
    ctx.drawImage(img, 0, 0, width, height);

    return new Uint8Array(await canvas.toBlob(blob => blob.arrayBuffer()));
};
