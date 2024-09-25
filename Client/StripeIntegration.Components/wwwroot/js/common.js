function isMobileDevice() {
    return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
}

function getScreenBounds() {
    const width = window.innerWidth ? document.documentElement.clientWidth : document.body.clientWidth;
    const height = window.innerHeight ? document.documentElement.clientHeight : document.body.clientHeight;

    return {
        x: width,
        y: height
    }
}

function goBack() {
    window.history.back();
}