'use client';
import HostCard from "@/components/HostCard";
import { useHostStatus } from "@/providers/SignalRProvider";

export default function HomePage() {
  
  const status = useHostStatus();
  return (
    <div className='min-h-screen bg-gray-50 p-6'>
      <header className='mb-6 flex items-center justify-between'>
        <div className='text-2xl font-bold'>AkkaSync Dashboard</div>
      </header>
      <section className='mb-6'>
        <h2 className='text-xl font-semibold mb-3'>Hosts</h2>
        <HostCard name={'test'} status={status} />
      </section>
    </div>
  );
}