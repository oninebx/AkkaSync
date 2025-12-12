import Card from '@/components/Card'

interface Props {
  title: string;
  value: string;
  color?: string;
}

const KpiCard = ({title, value, color}: Props) => {
  return (
    <Card height='h-28' className="flex flex-col justify-center items-center text-center" padding="p-6">
      <p className="text-gray-500 text-sm">{title}</p>
      <p className="text-lg font-semibold" style={{ color: color || '#000' }}>{value}</p>
    </Card>
  )
}

export default KpiCard;