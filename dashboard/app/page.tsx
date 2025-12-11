'use client';
import HostCard from "@/components/HostCard";
// import { useHostStatus } from "@/providers/SignalRProvider/SignalRProvider";

const kpis = [
  { title: "Running Pipelines", value: "2", color: "#1F2937" },
  { title: "Failed (24h)", value: "0", color: "#EF4444" },
  { title: "Total Pipelines", value: "8", color: "#1F2937" },
  { title: "Queued Jobs", value: "1", color: "#FBBF24" },
];

const pipelines = [
  { name: "UserSync", status: "running", progress: 60, start: "14:00", duration: "2m" },
  { name: "OrderSync", status: "success", progress: 100, start: "13:50", duration: "5m" },
  { name: "PaymentSync", status: "error", progress: 45, start: "13:55", duration: "3m" },
];

export default function HomePage() {

  // const status = useHostStatus();
  const status = 'online';
  return (
    <>
      <div className="min-h-screen px-4 py-6">
        <div className="max-w-7xl mx-auto space-y-6">
          {/* KPI 部分 */}
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-6">
            {kpis.map((kpi) => (
              <div
                key={kpi.title}
                className="bg-white p-4 rounded-lg shadow flex flex-col justify-center h-28"
              >
                <p className="text-gray-500 text-sm">{kpi.title}</p>
                <p className="text-lg font-semibold" style={{ color: kpi.color }}>
                  {kpi.value}
                </p>
              </div>
            ))}
          </div>

          {/* Host 和 Recent Events 部分 */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* Host 卡片 */}
            <div className="bg-white rounded-lg shadow p-4 flex flex-col justify-between h-56">
              <div>
                <p className="text-gray-700 font-semibold">Host: AkkaSync-Primary</p>
                <div className="flex items-center mt-2">
                  <span className="w-3 h-3 bg-success rounded-full mr-2"></span>
                  <p className="text-gray-700 text-sm">Online</p>
                </div>
              </div>
              <p className="text-gray-500 mt-2 text-xs">
                Last Heartbeat: 2025-12-10 14:03:22
              </p>
            </div>

            {/* Recent Events 卡片 */}
            <div className="bg-white rounded-lg shadow p-4 flex flex-col justify-between h-56">
              <div>
                <p className="text-gray-700 font-semibold mb-2">Recent Events</p>
                <ul className="text-sm space-y-1">
                  <li className="text-info">
                    [14:02:01] INFO Pipeline UserSync started
                  </li>
                  <li className="text-gray-500">
                    [14:02:02] DEBUG Task FetchUsers completed (32ms)
                  </li>
                  <li className="text-info">
                    [14:02:03] INFO Task SyncUsers completed (153ms)
                  </li>
                </ul>
              </div>
            </div>
          </div>
           {/* Pipelines 表格 */}
            <div className="bg-white shadow rounded-lg p-4">
              <h2 className="text-lg font-semibold text-gray-700 mb-4">Pipelines Status</h2>
              <div className="overflow-x-auto">
                <table className="min-w-full text-sm">
                  <thead>
                    <tr className="bg-gray-100 text-gray-700">
                      <th className="py-2 px-4 text-left">Pipeline</th>
                      <th className="py-2 px-4 text-left">Status</th>
                      <th className="py-2 px-4 text-left">Progress</th>
                      <th className="py-2 px-4 text-left">Start Time</th>
                      <th className="py-2 px-4 text-left">Duration</th>
                    </tr>
                  </thead>
                  <tbody>
                    {pipelines.map((pipeline) => {
                      let statusColor = "";
                      switch (pipeline.status) {
                        case "success":
                          statusColor = "bg-success text-white";
                          break;
                        case "warning":
                          statusColor = "bg-warning text-white";
                          break;
                        case "error":
                          statusColor = "bg-error text-white";
                          break;
                        case "running":
                          statusColor = "bg-info text-white";
                          break;
                      }
                      return (
                        <tr key={pipeline.name} className="border-b last:border-none">
                          <td className="py-2 px-4">{pipeline.name}</td>
                          <td className="py-2 px-4">
                            <span className={`inline-block px-2 py-1 rounded ${statusColor}`}>
                              {pipeline.status}
                            </span>
                          </td>
                          <td className="py-2 px-4">
                            <div className="w-full bg-gray-200 rounded-full h-2">
                              <div
                                className={`h-2 rounded-full ${pipeline.status === "success"
                                    ? "bg-success"
                                    : pipeline.status === "warning"
                                      ? "bg-warning"
                                      : pipeline.status === "error"
                                        ? "bg-error"
                                        : "bg-info"
                                  }`}
                                style={{ width: `${pipeline.progress}%` }}
                              />
                            </div>
                          </td>
                          <td className="py-2 px-4">{pipeline.start}</td>
                          <td className="py-2 px-4">{pipeline.duration}</td>
                        </tr>
                      );
                    })}
                  </tbody>
                </table>
              </div>
            </div>
        </div>
      </div>
    </>
  );
}