import {fileURLToPath, URL} from 'node:url';
import process from 'node:process';
import {defineConfig} from 'vite';
import vue from '@vitejs/plugin-vue';
import dotenv from 'dotenv';

dotenv.config({path: '../.env'});

export default defineConfig({
    plugins: [vue()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url)),
        },
    },
    server: {
        port: Number(process.env.CLIENT_PORT),
        proxy: {
            '/api': {
                target: 'https://' + process.env.IP_ADDRESS + ':' + process.env.SERVER_PORT, // Your backend server URL
                changeOrigin: true,
                secure: false,
                rewrite: (path) => path.replace(/^\/api/, '') // Remove '/api' prefix when forwarding requests
            }
        },
    },
    build: {
        outDir: '../wwwroot',
        emptyOutDir: true,
    },
});
