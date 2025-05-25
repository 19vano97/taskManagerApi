import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { visualizer } from 'rollup-plugin-visualizer'
import { readFileSync } from 'fs'
import path from 'path'

export default defineConfig({
  plugins: [
    react(),
    visualizer({
      open: true,
      template: "sunburst",
      gzipSize: true,
      brotliSize: true,
      filename: path.resolve(__dirname, 'dist/report.html'),
    }),
  ],
  server: {
    https: {
      key: readFileSync(path.resolve(__dirname, 'cert/localhost-key.pem')),
      cert: readFileSync(path.resolve(__dirname, 'cert/localhost.pem')),
    },
    port: 5173,
  },
  build: {
  rollupOptions: {
    output: {
      manualChunks(id) {
        if (id.includes('node_modules')) {
          if (id.includes('@mantine')) return 'mantine'
          if (id.includes('react')) return 'react'
          if (id.includes('react-oidc-context')) return 'oidc'
          if (id.includes('@tabler')) return 'icons'
          return 'vendor'
        }
        if (id.includes('/src/components/')) {
          return 'components'
        }
        if (id.includes('/src/pages/')) {
          return 'pages'
        }
      },
    },
  },
},

})
