import type { Metadata } from "next";
import "./globals.css";
import { SignalRProvider } from "@/providers/SignalRProvider";
import Sidebar from "@/components/Sidebar";


export const metadata: Metadata = {
  title: "AkkaSync Dashboard",
  description: "Dashboard for real-time monitoring and management of multiple ETL pipelines.",
};

const SIGNALR_HUB_URL = process.env.NEXT_PUBLIC_SIGNALR_HUB_URL || 'http://localhost:5000/hub/dashboard';

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  
  return (
    <html lang="en">
      <body className='bg-background flex min-h-screen'>
        <Sidebar />
        <div className="flex-1 p-6 max-w-6xl mx-auto">
          <SignalRProvider url={SIGNALR_HUB_URL} autoReconnect={true}>
            {children}
          </SignalRProvider>
        </div>
      </body>
    </html>
  );
}
