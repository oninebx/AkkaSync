import Card from '@/components/Card'

export interface Kpi {
  id: string;
  title: string;
  value: string;
  color: string;
}

interface Props {
  data: Kpi;
}

const KpiCard = ({data: {id, title, value, color}}: Props) => {
  return (
    <Card key={id} height='h-28' className="flex flex-col justify-center items-center text-center" padding="p-6">
      <p className="text-gray-500 text-sm">{title}</p>
      <p className="text-lg font-semibold" style={{ color: color || '#000' }}>{value}</p>
    </Card>
  )
}

export default KpiCard;