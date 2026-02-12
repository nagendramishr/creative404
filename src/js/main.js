// Main JavaScript for Creative 404 Generator

const messages = [
    "Oops! This page went on vacation.",
    "404: Page not found. But you found this cool message!",
    "Houston, we have a problem. The page is lost in space.",
    "The page you're looking for is playing hide and seek.",
    "Error 404: Page is out for coffee.",
];

const colors = [
    '#FF6B6B', '#4ECDC4', '#45B7D1', '#FFA07A', '#98D8C8'
];

function generateCreative404() {
    const randomMessage = messages[Math.floor(Math.random() * messages.length)];
    const randomColor = colors[Math.floor(Math.random() * colors.length)];
    
    const preview = document.getElementById('preview');
    preview.innerHTML = `
        <div style="background: ${randomColor}; padding: 2rem; border-radius: 5px; color: white;">
            <h2 style="font-size: 4rem; margin: 0;">404</h2>
            <p style="font-size: 1.5rem; margin-top: 1rem;">${randomMessage}</p>
        </div>
    `;
}

document.addEventListener('DOMContentLoaded', () => {
    const generateBtn = document.getElementById('generateBtn');
    generateBtn.addEventListener('click', generateCreative404);
});
