type RecordLevel = 'Info' | 'Warn' | 'Error'; 
interface DiagnosisRecord {
  timestamp: string,
  message: string,
  level: RecordLevel
}

interface DiagnosisJournal {
  records: DiagnosisRecord[]
}

export type {
  DiagnosisJournal,
  DiagnosisRecord,
  RecordLevel
}