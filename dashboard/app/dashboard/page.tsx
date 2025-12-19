'use client';

import { useEffect, useState } from 'react';
import {getDashboardHub} from '@/lib/signalr';
import { useDashboardStore } from '@/stores/dashboardStore';

export default function DashboardPage() {
  // const { addEvent, events } = useDashboardStore();
  // const [connectionState, setConnectionState] = useState('Disconnected');
  // useEffect(() => {
  //   const conn = getDashboardHub();
  //   conn.on("HostSnapshot", (event) => {
  //     console.log("Event received:", event);
  //     addEvent(event);
  //   });
  //   conn.start()
  //     .then(() => {
  //       console.log("Connected to dashboard hub");
  //       setConnectionState('Connected');
  //     })
  //     .catch((err) => {
  //       console.error("Connection error:", err);
  //       setConnectionState('Error');
  //     });
  //     return () => {
  //       conn.stop();
  //     }
  // }, [addEvent])

  return (
    <div>
      <h1>AkkaSync Dashboard</h1>
      {/* <p>Status: {connectionState}</p>
      <div>
        <button>Start Sync</button>
        <button>Stop Sync</button>
      </div>
      <h2>Events</h2>
      <div>
        {events.map((evt, idx) => (
          <div key={idx}>
            <strong>{evt.type}</strong>
            <pre>{JSON.stringify(evt.data, null, 2)}</pre>
          </div>
        ))}
      </div> */}
    </div>
  );
}