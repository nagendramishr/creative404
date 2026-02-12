# Creative 404 Generator

A simple yet creative 404 error page generator that creates unique and engaging error messages.

## Features

- Generate random creative 404 error messages
- Beautiful gradient backgrounds
- Responsive design
- Easy to customize

## Installation

1. Clone the repository:
```bash
git clone https://github.com/nagendramishr/creative404.git
cd creative404
```

2. Install dependencies (optional, for development server):
```bash
npm install
```

## Usage

### Option 1: Direct Browser Open
Simply open `src/index.html` in your web browser.

### Option 2: Development Server
Run the development server:
```bash
npm start
```

Then open your browser to `http://localhost:8080`

## Project Structure

```
creative404/
├── src/                    # Source files
│   ├── index.html         # Main HTML file
│   ├── css/               # Stylesheets
│   │   └── styles.css     # Main styles
│   ├── js/                # JavaScript files
│   │   └── main.js        # Main application logic
│   └── assets/            # Static assets
│       ├── images/        # Image files
│       └── fonts/         # Font files
├── tests/                 # Test files
├── docs/                  # Documentation
├── examples/              # Example implementations
├── config/                # Configuration files
├── package.json           # Node.js package configuration
├── README.md             # This file
└── LICENSE               # License information
```

## Customization

### Adding New Messages

Edit `src/js/main.js` and add new messages to the `messages` array:

```javascript
const messages = [
    "Your custom message here",
    // ... more messages
];
```

### Changing Colors

Modify the `colors` array in `src/js/main.js`:

```javascript
const colors = [
    '#YourColorHex',
    // ... more colors
];
```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Inspired by creative 404 pages across the web
- Built with vanilla HTML, CSS, and JavaScript
