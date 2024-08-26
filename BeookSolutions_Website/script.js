function downloadFile() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    document.querySelector('.big-text').textContent = "Vielen Dank für's herunterladen!";
    window.location.href = 'https://github.com/marekvonrogall/BeookSolutions/releases/download/1.0.0.2/BeookSolutions.zip';
}