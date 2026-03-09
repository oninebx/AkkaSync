import React from 'react';
import KpiCard from './KpiCard';
import { KpiVM } from '@/app/overview/types';

type Props = {
  data: KpiVM[];
}

const KpiBanner = ({ data }: Props) => {
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-6">
      { data.map((kpi) => 
          <KpiCard key={kpi.title} data={kpi} />)
      }
    </div>
  )
}

export default KpiBanner