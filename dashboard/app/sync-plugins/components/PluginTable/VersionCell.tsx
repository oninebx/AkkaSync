import React from 'react'

interface Props {
  latestVersion?: string;
  installedVersion?: string;
}

const VersionCell = ({ latestVersion, installedVersion}: Props) => {
  if (!latestVersion || installedVersion === latestVersion) {
    return <span>{installedVersion || "-"}</span>;
  }

  const fromVersion = installedVersion || "-";
  
  return (
    <span className="text-blue-600 font-medium">
      {fromVersion} → {latestVersion}
    </span>
  );
}

export default VersionCell;