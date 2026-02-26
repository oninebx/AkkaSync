import type { NextConfig } from "next";

const isDev = process.env.NODE_ENV === 'development';

const nextConfig: NextConfig = {
  output: 'standalone',
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
