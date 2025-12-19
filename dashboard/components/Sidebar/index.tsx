"use client";

import Image from "next/image";
import { useState } from "react";
import { SidebarItem } from "./SidebarItem";
import {
  Gauge,
  Server,
  Workflow,
  Settings,
  Menu,
  ChevronLeft,
} from "lucide-react";

const items = [
  { name: "Overview", href: "/overview", icon: <Gauge /> },
  { name: "Host", href: "/host-monitor", icon: <Server /> },
  { name: "Pipelines", href: "/pipelines", icon: <Workflow /> },
  { name: "Settings", href: "/settings", icon: <Settings /> },
];

const Sidebar = () => {
  const [collapsed, setCollapsed] = useState(false);

  return (
    <aside
      className={`bg-surface-muted text-gray-700 border-r border-gray-200 transition-all
        ${collapsed ? "w-20" : "w-64"}`}
    >
      <div className="flex items-center justify-between px-4 py-4">
        {!collapsed && (
          <Image
            src="/akkasync-logo.png"
            alt="AkkaSync Logo"
            width={140}
            height={140}
          />
        )}
        <button
          onClick={() => setCollapsed(!collapsed)}
          className="p-1 rounded hover:bg-gray-200"
        >
          {collapsed ? <Menu size={20} /> : <ChevronLeft size={20} />}
        </button>
      </div>

      {/* Nav menu */}
      <nav className="mt-6 flex flex-col gap-1 px-3">
        {items.map((item) => (
          <SidebarItem key={item.name} {...item} collapsed={collapsed} />
        ))}
      </nav>
    </aside>
  );
};

export default Sidebar;
