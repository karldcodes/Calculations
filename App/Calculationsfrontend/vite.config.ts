import { defineConfig, type UserConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [tailwindcss(), plugin()],
    server: {
        port: 53434,
    },
    test: {
        environment: "jsdom",
        globals: true,
        setupFiles: "./tests/setup.ts",
    }
} as UserConfig)
