import Link from "next/link";
import { usePathname } from "next/navigation";
import { Tooltip } from "./Tooltip";

interface SidebarItemProps {
  name: string;
  href: string;
  icon: React.ReactNode;
  collapsed: boolean;
}

export const SidebarItem = ({ name, href, icon, collapsed }: SidebarItemProps) => {
  const pathname = usePathname();
  const active = pathname.startsWith(href);

  const item = (
    <Link
      href={href}
      data-active={active ? "true" : "false"}
      className="
        flex items-center gap-3 px-3 py-2 rounded-md text-sm font-medium transition
        hover:bg-gray-200 hover:text-primary
        data-[active=true]:bg-white
        data-[active=true]:text-primary
        data-[active=true]:shadow-sm
      "
    >
      <span className="w-5 h-5">{icon}</span>

      {!collapsed && <span>{name}</span>}
    </Link>
  );

  return collapsed ? <Tooltip text={name}>{item}</Tooltip> : item;
};
