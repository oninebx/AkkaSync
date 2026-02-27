import type { NextConfig } from "next";

const isDev = process.env.NODE_ENV === 'development';
const isMvp = process.env.MVP_BUILD === 'true';

const nextConfig: NextConfig = {
  output: isMvp ? 'export' : 'standalone',
  async rewrites() {
    if(!isDev){
      return [];
    }
    return [
      {
        source: '/hub/:path*',
        destination: 'http://localhost:5000/hub/:path*'
      },
      {
        source: '/api/:path*',
        destination: 'http://localhost:5000/api/:path*'
      }
    ]
  }
};

export default nextConfig;
