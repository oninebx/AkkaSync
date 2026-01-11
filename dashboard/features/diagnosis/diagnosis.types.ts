interface DiagnosisRecord {
  timestamp: string,
  message: string,
  level: 'Info' | 'Warn' | 'Error'
}

interface DiagnosisJournal {
  records: DiagnosisRecord[]
}

export type {
  DiagnosisJournal,
  DiagnosisRecord
}