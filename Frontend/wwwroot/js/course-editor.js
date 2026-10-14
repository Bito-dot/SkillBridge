(() => {
    const input = document.getElementById('czThumbnailInput');
    const preview = document.getElementById('czThumbnailPreview');
    const placeholder = document.getElementById('czThumbnailPlaceholder');
    const error = document.getElementById('czThumbnailClientError');
    if (!input || !preview || !placeholder || !error) return;
    let previewReader = null;
    const originalSrc = preview.getAttribute('src');
    const hadOriginal = !preview.hidden;
    input.addEventListener('change', () => {
        if (previewReader?.readyState === FileReader.LOADING) previewReader.abort();
        error.textContent = '';
        const file = input.files?.[0];
        if (!file) {
            preview.hidden = !hadOriginal;
            placeholder.hidden = hadOriginal;
            preview.src = originalSrc || '';
            return;
        }
        if (!['image/jpeg', 'image/png', 'image/webp'].includes(file.type) || file.size > 2 * 1024 * 1024) {
            error.textContent = 'Choose a JPG, PNG or WebP image no larger than 2 MB.';
            input.value = '';
            preview.hidden = !hadOriginal;
            placeholder.hidden = hadOriginal;
            preview.src = originalSrc || '';
            return;
        }
        previewReader = new FileReader();
        previewReader.addEventListener('load', () => {
            preview.src = previewReader.result;
            preview.hidden = false;
            placeholder.hidden = true;
        });
        previewReader.readAsDataURL(file);
    });
    window.addEventListener('pagehide', () => { if (previewReader?.readyState === FileReader.LOADING) previewReader.abort(); });
})();
