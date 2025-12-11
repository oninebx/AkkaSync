import Link from 'next/link';
import Image from 'next/image';

interface SidebarItem {
  name: string;
  href: string;
}

const sidebarItems: SidebarItem[] = [
  { name: "Overview", href: "/overview" },
  { name: "Host", href: "/host-monitor" },
  { name: "Pipelines", href: "/pipelines" },
  { name: "Settings", href: "/settings" },
];

const Sidebar = () => (
  <div className="min-w-72 bg-surface-muted flex flex-col items-center py-6 text-gray-700">
    <Image
      src="/akkasync-logo.png"
      alt="AkkaSync Logo"
      width={256}
      height={256}
      className="mb-10"
    />
    {sidebarItems.map((item) => (
      <Link key={item.name} href={item.href} className="mb-6 text-sm hover:text-primary">
        {item.name}
      </Link>
    ))}
  </div>
);

export default Sidebar;