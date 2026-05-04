import { defineConfig } from "@lovable.dev/vite-tanstack-config";
import type { Plugin } from "vite";
import http from "http";

// Custom Vite plugin to proxy /api requests to the .NET backend.
// This must be registered in the pre-phase (before TanStack Start's catch-all
// middleware) because TanStack Start intercepts all requests and never calls next().
const apiProxyPlugin: Plugin = {
  name: "api-proxy",
  configureServer(server) {
    server.middlewares.use((req, res, next) => {
      if (req.url?.startsWith("/api/")) {
        const options: http.RequestOptions = {
          hostname: "localhost",
          port: 5289,
          path: req.url,
          method: req.method,
          headers: req.headers,
        };

        const proxyReq = http.request(options, (proxyRes) => {
          res.writeHead(proxyRes.statusCode!, proxyRes.headers);
          proxyRes.pipe(res, { end: true });
        });

        proxyReq.on("error", (err) => {
          console.error("[api-proxy] Error:", err.message);
          res.writeHead(502, { "Content-Type": "application/json" });
          res.end(JSON.stringify({ message: "Bad Gateway: API server unavailable" }));
        });

        req.pipe(proxyReq, { end: true });
      } else {
        next();
      }
    });
  },
};

export default defineConfig({
  plugins: [apiProxyPlugin],
  cloudflare: false,
  tanstackStart: {
    server: {
      preset: "vercel",
    },
  },
  vite: {
    server: {
      proxy: {
        "/api": {
          target: "http://localhost:5289",
          changeOrigin: true,
        },
      },
    },
  },
});