"use client";

import Image from "next/image";
import React from "react";
import { Search, Bell, Wifi } from "lucide-react";

const TopBar: React.FC = () => {
  return (
    <header className="mb-6">
      <div className="flex items-center justify-between bg-surface p-3 rounded-lg shadow-sm border border-gray-100">
        <div className="flex items-center gap-4">
          <div className="flex items-center gap-3">
            <Image src="/akkasync-logo.png" alt="logo" width={36} height={36} />
            <div>
              <div className="text-sm font-semibold text-title">AkkaSync Dashboard</div>
              <div className="text-xs text-muted">Real-time pipeline monitoring</div>
            </div>
          </div>
        </div>

        <div className="flex-1 px-6">
          <div className="max-w-xl mx-auto">
            <label className="relative block">
              <span className="sr-only">Search</span>
              <span className="absolute inset-y-0 left-0 flex items-center pl-3 text-gray-400">
                <Search size={16} />
              </span>
              <input
                className="placeholder:italic placeholder:text-gray-400 bg-surface-muted border border-gray-200 text-sm rounded-lg w-full pl-9 pr-3 py-2 focus:outline-none"
                placeholder="Search pipelines, hosts, events..."
                type="text"
                name="search"
              />
            </label>
          </div>
        </div>

        <div className="flex items-center gap-4">
          <div className="flex items-center gap-2 text-sm text-muted">
            <Wifi className="text-info" size={16} />
            <span className="text-body">Connected</span>
          </div>

          <button title="Notifications" className="relative p-2 rounded hover:bg-gray-100">
            <Bell />
            <span className="absolute -top-1 -right-1 inline-flex items-center justify-center px-1.5 py-0.5 text-xs font-medium leading-none text-white bg-error rounded-full">3</span>
          </button>

          <div className="flex items-center gap-2">
            <div className="w-8 h-8 rounded-full overflow-hidden bg-gray-200">
              <Image src="/avatar-placeholder.png" alt="user" width={32} height={32} />
            </div>
            <div className="text-sm">
              <div className="text-body font-medium">Admin</div>
              <div className="text-xs text-muted">Operator</div>
            </div>
          </div>
        </div>
      </div>
    </header>
  );
};

export default TopBar;
