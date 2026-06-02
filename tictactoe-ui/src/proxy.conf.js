const { env } = require('process');

let target = 'http://localhost:5000';

if (env.ASPNETCORE_URLS) {
  const urls = env.ASPNETCORE_URLS.split(';').map(u => u.trim()).filter(Boolean);
  target = urls.find(u => u.startsWith('https://')) || urls[0] || target;
} else if (env.ASPNETCORE_HTTPS_PORT) {
  target = `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`;
}

console.log('API proxy target:', target);

module.exports = {
  '/api': {
    target,
    secure: false,
    changeOrigin: true
  }
};
