# web-server.dev

A development web server with 2 core features:
- serve static files from local folder (example: front end) – or multiple local folders
- reverse proxy calls to local or remote URLs (example: REST API)

Rationale:
- some typical scenarios or browser features require HTML / JS / CSS to be hosted on a local HTTP server, even during development (e.g. service workers, clipboard, ...) – and not just opening the files directly from the file system
- when calling local or remote APIs, using a distinct hosting server for HTML / JS / CSS can cause issues (e.g. with CORS, ...)

Having the same frontal server for everything simplifies the developpement setup

Running as a console app, from the command line, with minimal installation and configuration

Inspiration :
- [Simple web server](https://simplewebserver.org/), but with a reverse proxy
- [Ngnix](https://nginx.org/en/), but without the whole ceremony

## Installation

TODO

## Configuration / Usage

Configuration is in `settings.json`, that needs to be edited according to use case (see examples hereafter)

Then run `web-server.exe` (windows) or `web-server` (linux)

Open http://localhost:8080/ in browser

CTRL + C to stop

## Use case 1

Create a local server to host static files during development (HTML, JS / ECMAScript, CSS, images...)
- typically required for browser features that are disabled when opening the files directly from file system (e.g. service workers, clipboard, ...)

```jsonc
// settings.json
{
  "/": "<<path to directory>>"
}
```

Escape the backward slashes and quotes in path.

## Use case 2

Serve front-end files from local folder and proxy API calls to a back-end system
- removes the need to setup CORS on the back-end, since the browser sees everything as a single web server

```jsonc
// settings.json
{
  "/": "<<path to directory>>",
  "/api" : "http://swapi.dev/"
}
```

## More complex setups

An arbitrary number of local folders to serve and remote APIs to proxy can be configured:
- Serve front-end files from multiple local folders (for instance a home page and a documentation site)
- Group multiple APIs behind a single access point (for instance public API and private API)
- Front-office app and API and back-office app and API

## Run locally

TODO

```bash
cd src
dotnet build
dotnet run
```

## FAQ

TODO

- production usage ?
- route conflicts ?
- trailing slash

## TODO

- proxy query parameters
- https certificate
- proxy headers
