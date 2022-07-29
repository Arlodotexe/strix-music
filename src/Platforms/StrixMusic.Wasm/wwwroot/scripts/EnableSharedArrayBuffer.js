// From https://dev.to/stefnotch/enabling-coop-coep-without-touching-the-server-2d3n

if ("serviceWorker" in navigator) {
    // Register service worker
    navigator.serviceWorker.register(new URL("SharedArrayBufferServiceWorker.js", window.location.href)).then(
        function (registration) {
            console.log("COOP/COEP Service Worker registered", registration.scope);
            // If the registration is active, but it's not controlling the page
            if (registration.active && !navigator.serviceWorker.controller) {
                window.location.reload();
            }
        },
        function (err) {
            console.log("COOP/COEP Service Worker failed to register", err);
        }
    );
} else {
    console.warn("Cannot register a service worker");
}
