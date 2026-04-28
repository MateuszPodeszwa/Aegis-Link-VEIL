# UI Assets and Styling

Static assets and styles that shape the look and feel.

## CSS

Main stylesheet:

- `wwwroot/css/app.css` — base styles, layout helpers, and shared UI rules.

Page‑specific styles:

- `Pages/Chat.razor.css` — chat layout, bubbles, and action icons.
- `Pages/Contacts.razor.css` — contacts list and connect form.
- `Pages/Home.razor.css` — landing view and identity card.
- `Pages/Settings.razor.css` — settings panels and toggles.

Layout styles:

- `Layout/MainLayout.razor.css` — sidebar layout.
- `Layout/NavMenu.razor.css` — nav appearance and collapse behaviour.

## Images and icons

- `wwwroot/images/` — app images and logos.
- `wwwroot/favicon.png` — browser tab icon.
- `wwwroot/icon-192.png`, `wwwroot/icon-512.png` — PWA icons.
- Google Material Icons font is loaded in `index.html`.

## Libraries

- `wwwroot/lib/bootstrap/` — Bootstrap CSS.
- `wwwroot/js/nacl-fast.min.js` and `wwwroot/js/nacl-util.min.js` — crypto support.

## See also

- [Client pages](04-client-pages.md)
- [Configuration and hosting assets](10-config-hosting.md)
- [Back to index](README.md)
