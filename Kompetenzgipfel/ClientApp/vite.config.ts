import {fileURLToPath, URL} from 'node:url';
import process from 'node:process';
import {defineConfig} from 'vite';
import vue from '@vitejs/plugin-vue';

// Match the URI your web application is being hosted at
const proxyTarget = process.env.ASPNETCORE_HTTPS_PORT
    ? `https://localhost:${process.env.ASPNETCORE_HTTPS_PORT}`
    : process.env.ASPNETCORE_URLS
        ? process.env.ASPNETCORE_URLS.split(';')[0]
        : 'https://localhost:7298';

export default defineConfig({
    plugins: [vue()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url)),
        },
    },
    server: {
        port: 3001,
        proxy: {
            // Proxy requests prefixed with '/api' to your backend server
            '/api': {
                target: 'https://localhost:7298', // Your backend server URL
                changeOrigin: true,
                secure: false,
                rewrite: (path) => path.replace(/^\/api/, '') // Remove '/api' prefix when forwarding requests
            }
        },
    },
    // build: {
    //   outDir: '../wwwroot',
    //   emptyOutDir: true,
    // },
});
