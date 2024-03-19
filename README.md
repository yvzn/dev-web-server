# web-server.dev

**👨🏽‍💻 work in progress !**

A development web server with 2 core features:
- serve static files from local folder (example: front end) – or multiple local folders
- reverse proxy calls to local or remote URLs (example: REST API)

Running as a console app, from the command line, with minimal installation and configuration

Motivation:
- some typical scenarios or browser features require HTML / JS / CSS to be hosted on a local HTTP server, even during development (e.g. service workers, clipboard, ...) – and not just opening the files directly from the file system
- when calling local or remote APIs, using a distinct hosting server for HTML / JS / CSS can cause issues (e.g. with CORS, ...)

Having the same frontal server for everything simplifies the developpement setup

Inspiration:
- [Simple web server](https://simplewebserver.org/), but with a reverse proxy
- [Ngnix](https://nginx.org/en/), but without the whole ceremony

## Installation

1. Go to the [Releases page](https://github.com/yvzn/dev-web-server/releases)
2. Download the latest zip file corresponding to your operating system
3. Unzip the downloaded file to your computer

## Configuration / Usage

1. Configuration is done in `settings.json` ; edited according to use case (see examples below)
2. Run `web-server.exe` (Windows) or `web-server` (Linux)
3. Open http://localhost:8080/ in browser

<kbd>CTRL + C</kbd> to stop

## Example use case 1

Create a local server to host static files during development (HTML, JS / ECMAScript, CSS, images...)
- typically required for browser features that are disabled when opening the files directly from file system (e.g. service workers, clipboard, ...)

```jsonc
// settings.json
{
  "routes": {
    "/": "~/my-new-website/"
  }
}
```

Remember to escape the backward slashes and quotes in path.

## Example use case 2

Serve front-end files from local folder and proxy API calls to a back-end system
- removes the need to setup CORS on the back-end, since the browser sees everything as a single web server

```jsonc
// settings.json
{
  "routes": {
    "/": "~/my-new-website/",
    "/api" : "http://swapi.dev/"
  }
}
```

## More complex setups

An arbitrary number of local folders to serve and remote APIs to proxy can be configured:
- Serve front-end files from multiple local folders (for instance a home page and a documentation site)
- Group multiple APIs behind a single access point (for instance public API and private API)
- Front-office app and API and back-office app and API

## Run locally

Requires [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

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
- configurable logs
- https certificate
- proxy headers
