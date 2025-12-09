import { NextRequest, NextResponse } from 'next/server';

const hosts = [
  {
    id: 'h1',
    name: 'host-a',
    signalRHubUrl: 'http://localhost:5001/hostHub',
  },
  {
    id: 'h2',
    name: 'host-b',
    signalRHubUrl: 'http://localhost:5002/hostHub',
  },
];

export async function GET(req: NextRequest) {
  return NextResponse.json(hosts);
}
