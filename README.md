# SnapIt
Snap It! is a window manager for Windows 10/11 that organizes your windows to improve productivity when working with wide and multiple screens. It's designed to be quick and simple.

## Screenshots
<img src="/documents/00.png" alt="Usage" width="500">
<img src="/documents/1.png" alt="Layouts" width="500">
<img src="/documents/0.png" alt="Designer" width="500">

## Features
- Divide screens into snapping areas
- Move windows between snapping areas by dragging
- Run applications and move to a specified area with one click
- Snap windows using keyboard and mouse
- Choose different layouts for each screen
- Create custom layouts using the design tool
- Set your theme for overlay and highlighting of areas
- Customizable options for dragging windows
- Predefined layouts for different use cases (vertical or horizontal screens)
- Supports different DPI and taskbar positions for each screen
- Dark and Light theme

## Build Status
[![SnapIt CI](https://github.com/enginkirmaci/SnapIt/actions/workflows/SnapIt-CI.yml/badge.svg)](https://github.com/enginkirmaci/SnapIt/actions/workflows/SnapIt-CI.yml)
[![SnapIt Release](https://github.com/enginkirmaci/SnapIt/actions/workflows/snapit-release.yml/badge.svg)](https://github.com/enginkirmaci/SnapIt/actions/workflows/snapit-release.yml)

## Usage
To use SnapIt, follow these steps:
1. Download and install the latest version of SnapIt from the [releases page](https://github.com/enginkirmaci/SnapIt/releases).
2. Run SnapIt from the Start menu or by double-clicking the desktop shortcut.
3. Choose a layout for each screen in the layout window.
4. Drag windows to snap them to the snapping areas on your screen.
5. Use the designer to create/edit custom layouts in the layout window.

For more detailed instructions, please refer to the [user manual](https://github.com/enginkirmaci/SnapIt/wiki/User-Manual). *UNDERCONSTRUCTION

## Settings Storage
SnapIt now uses SQLite to store settings and layouts instead of JSON files. The database file (`SnapItSettings.db`) is stored in `%LocalApplicationData%\SnapIt\`.

### Automatic Migration
If you're upgrading from a previous version that used JSON files:
- Your existing settings will be automatically migrated to the SQLite database on first run
- The original JSON files will remain untouched for backward compatibility
- No manual intervention is required

### Benefits of SQLite Storage
- Improved performance for read/write operations
- Better data integrity with ACID transactions
- Reduced file I/O overhead
- Easier backup (single database file instead of multiple JSON files)

## Contributing
We welcome contributions to SnapIt! If you would like to contribute, please follow these steps:
1. Fork the repository.
2. Create a new branch for your changes.
3. Make your changes and commit them to your branch.
4. Open a pull request to merge your changes into the main branch.

Please make sure to follow the code style and conventions used in the project. If you have any questions or need help, feel free to open an issue.

Thank you for considering contributing to SnapIt!

## License
SnapIt is licensed under the [GNU GPLv3](LICENSE).
