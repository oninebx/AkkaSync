import '@/infrastructure/signalr/connection.handlers';
import '@/features/pipeline/pipeline.handlers';
import '@/features/host/host.handlers';
import '@/features/scheduler/scheduler.handlers';
import '@/features/diagnosis/dianosis.handlers';
import '@/features/plugin-artifact/plugin.handlers';
import '@/features/worker/worker.handlers';


export { envelopeHandlerMap as envelopeHandlers } from '@/shared/events/envelopeHandlerMap';